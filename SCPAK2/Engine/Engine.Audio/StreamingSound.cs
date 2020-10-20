using Android.Media;
using Engine.Media;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Engine.Audio
{
	public sealed class StreamingSound : BaseSound
	{
		public enum Command
		{
			Play,
			Pause,
			Stop,
			Exit
		}

		public Task m_task;

		public BlockingCollection<Command> m_queue = new BlockingCollection<Command>(100);

		public float m_bufferDuration;

		public StreamingSource StreamingSource
		{
			get;
			set;
		}

		public StreamingSound(StreamingSource streamingSource, float volume = 1f, float pitch = 1f, float pan = 0f, bool isLooped = false, bool disposeOnStop = false, float bufferDuration = 0.3f)
		{
			VerifyStreamingSource(streamingSource);
			m_bufferDuration = MathUtils.Clamp(bufferDuration, 0f, 10f);
			ChannelOut channelConfig = (streamingSource.ChannelsCount == 1) ? ChannelOut.FrontLeft : ChannelOut.Stereo;
			int minBufferSize = AudioTrack.GetMinBufferSize(streamingSource.SamplingFrequency, channelConfig, Encoding.Pcm16bit);
			int bufferSizeInBytes = MathUtils.Max(CalculateBufferSize(m_bufferDuration), minBufferSize);
			m_audioTrack = new AudioTrack(Stream.Music, streamingSource.SamplingFrequency, channelConfig, Encoding.Pcm16bit, bufferSizeInBytes, AudioTrackMode.Stream);
			//m_audioTrack = new AudioTrack(new AudioAttributes.Builder().SetUsage(AudioUsageKind.Media).SetContentType(AudioContentType.Music).Build(), new AudioFormat(), bufferSizeInBytes, AudioTrackMode.Static, 0);

			Mixer.m_audioTracksCreated++;
			if (m_audioTrack.State == AudioTrackState.Uninitialized)
			{
				m_audioTrack.Release();
				m_audioTrack = null;
				Mixer.m_audioTracksDestroyed++;
				Log.Warning("Failed to create StreamingSound AudioTrack. Created={0}, Destroyed={1}", Mixer.m_audioTracksCreated, Mixer.m_audioTracksDestroyed);
			}
			StreamingSource = streamingSource;
			base.ChannelsCount = streamingSource.ChannelsCount;
			base.SamplingFrequency = streamingSource.SamplingFrequency;
			base.Volume = volume;
			base.Pitch = pitch;
			base.Pan = pan;
			base.IsLooped = isLooped;
			base.DisposeOnStop = disposeOnStop;
			if (m_audioTrack != null)
			{
				m_task = Task.Run(delegate
				{
					try
					{
						StreamingThreadFunction();
					}
					catch (Exception message)
					{
						Log.Error(message);
					}
				});
			}
		}

		internal override void InternalDispose()
		{
			if (m_task != null)
			{
				m_queue.Add(Command.Exit);
				m_task.Wait();
				m_task = null;
			}
			if (StreamingSource != null)
			{
				StreamingSource.Dispose();
				StreamingSource = null;
			}
			m_queue.Dispose();
			base.InternalDispose();
		}

		internal override void InternalPlay()
		{
			if (m_audioTrack == null)
			{
				Stop();
			}
			else
			{
				m_queue.Add(Command.Play);
			}
		}

		internal override void InternalPause()
		{
			m_queue.Add(Command.Pause);
		}

		internal override void InternalStop()
		{
			m_queue.Add(Command.Stop);
		}

		public void StreamingThreadFunction()
		{
			long num = 0L;
			int num2 = 0;
			byte[] array = new byte[256];
			bool flag = false;
			while (true)
			{
				if (m_queue.TryTake(out Command item, (!flag) ? 100 : 0))
				{
					switch (item)
					{
					case Command.Exit:
						return;
					case Command.Play:
						m_audioTrack.Play();
						flag = true;
						break;
					case Command.Pause:
						m_audioTrack.Pause();
						flag = false;
						break;
					case Command.Stop:
						m_audioTrack.Pause();
						m_audioTrack.Flush();
						StreamingSource.Position = 0L;
						num2 = 0;
						num = 0L;
						flag = false;
						break;
					}
				}
				else
				{
					if (!flag)
					{
						continue;
					}
					if (num2 == 0)
					{
						num2 = ReadStreamingSource(array, array.Length);
						if (num2 == 0 && m_audioTrack.PlaybackHeadPosition >= num / 2 / base.ChannelsCount - 1)
						{
							flag = false;
							Dispatcher.Dispatch(base.Stop);
						}
					}
					if (num2 > 0)
					{
						int num3 = m_audioTrack.Write(array, array.Length - num2, num2);
						if (num3 > 0)
						{
							num2 -= num3;
							num += num3;
						}
					}
				}
			}
		}

		public int CalculateBufferSize(float duration)
		{
			return 2 * base.ChannelsCount * (int)((float)base.SamplingFrequency * duration);
		}

		public int ReadStreamingSource(byte[] buffer, int count)
		{
			int num = 0;
			if (StreamingSource.BytesCount > 0)
			{
				while (count > 0)
				{
					int num2 = StreamingSource.Read(buffer, num, count);
					if (num2 > 0)
					{
						num += num2;
						count -= num2;
						continue;
					}
					if (!m_isLooped)
					{
						break;
					}
					StreamingSource.Position = 0L;
				}
			}
			return num;
		}

		public void VerifyStreamingSource(StreamingSource streamingSource)
		{
			if (streamingSource == null)
			{
				throw new ArgumentNullException("streamingSource");
			}
			if (streamingSource.ChannelsCount < 1 || streamingSource.ChannelsCount > 2)
			{
				throw new InvalidOperationException("Unsupported channels count.");
			}
			if (streamingSource.SamplingFrequency < 8000 || streamingSource.SamplingFrequency > 48000)
			{
				throw new InvalidOperationException("Unsupported frequency.");
			}
		}
	}
}
