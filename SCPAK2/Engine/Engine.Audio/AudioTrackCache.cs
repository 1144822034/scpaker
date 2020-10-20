using Android.Media;
using System;
using System.Collections.Generic;

namespace Engine.Audio
{
	internal static class AudioTrackCache
	{
		public class AudioTrackData
		{
			public AudioTrack AudioTrack;

			public SoundBuffer SoundBuffer;

			public int BytesCount;

			public double ReloadStaticDataTime;

			public bool IsAvailable;
		}

		public const int MaxCacheSize = 16;

		public static List<AudioTrackData> m_audioTracks;

		public static byte[] m_buffer;

		public static int m_cacheHits;

		public static int m_cacheHitsWithWrite;

		public static int m_cacheMisses;

		public static int m_cacheFulls;

		static AudioTrackCache()
		{
			m_audioTracks = new List<AudioTrackData>();
			Window.Frame += Frame;
			Window.Deactivated += Deactivated;
		}

		public static int GetAudioTrackBytesCount(AudioTrack audioTrack)
		{
			foreach (AudioTrackData audioTrack2 in m_audioTracks)
			{
				if (audioTrack2.AudioTrack == audioTrack)
				{
					return audioTrack2.BytesCount;
				}
			}
			return 0;
		}

		public static AudioTrack GetAudioTrack(SoundBuffer soundBuffer, bool isLooped)
		{
			if (!isLooped && Mixer.EnableAudioTrackCaching && m_audioTracks.Count >= 16)
			{
				foreach (AudioTrackData audioTrack2 in m_audioTracks)
				{
					if (audioTrack2.IsAvailable && audioTrack2.SoundBuffer == soundBuffer)
					{
						audioTrack2.IsAvailable = false;
						m_cacheHits++;
						LogCacheStats();
						return audioTrack2.AudioTrack;
					}
				}
				AudioTrackData audioTrackData = null;
				foreach (AudioTrackData audioTrack3 in m_audioTracks)
				{
					if (audioTrack3.IsAvailable && audioTrack3.SoundBuffer.ChannelsCount == soundBuffer.ChannelsCount && audioTrack3.SoundBuffer.SamplingFrequency == soundBuffer.SamplingFrequency && audioTrack3.BytesCount >= soundBuffer.m_data.Length && (audioTrackData == null || audioTrack3.BytesCount <= audioTrackData.BytesCount))
					{
						audioTrackData = audioTrack3;
					}
				}
				if (audioTrackData != null)
				{
					if (m_buffer == null || m_buffer.Length < audioTrackData.BytesCount)
					{
						m_buffer = new byte[audioTrackData.BytesCount];
					}
					Array.Copy(soundBuffer.m_data, 0, m_buffer, 0, soundBuffer.m_data.Length);
					Array.Clear(m_buffer, soundBuffer.m_data.Length, audioTrackData.BytesCount - soundBuffer.m_data.Length);
					audioTrackData.AudioTrack.Write(m_buffer, 0, audioTrackData.BytesCount);
					audioTrackData.SoundBuffer = soundBuffer;
					audioTrackData.IsAvailable = false;
					m_cacheHitsWithWrite++;
					LogCacheStats();
					return audioTrackData.AudioTrack;
				}
				bool flag = true;
				foreach (AudioTrackData audioTrack4 in m_audioTracks)
				{
					if (audioTrack4.BytesCount < soundBuffer.m_data.Length)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					m_cacheFulls++;
					Log.Warning("AudioTrackCache full, no audio tracks available.");
					LogCacheStats();
					return null;
				}
			}
			AudioTrack audioTrack = new AudioTrack(Stream.Music, soundBuffer.SamplingFrequency, (soundBuffer.ChannelsCount == 1) ? ChannelOut.FrontLeft : ChannelOut.Stereo, Encoding.Pcm16bit, soundBuffer.m_data.Length, AudioTrackMode.Static);
//			AudioTrack audioTrack = new AudioTrack(new AudioAttributes.Builder().SetUsage(AudioUsageKind.Media).SetContentType(AudioContentType.Music).Build(),new AudioFormat(), soundBuffer.m_data.Length, AudioTrackMode.Static,0);

			if (audioTrack.State != 0)
			{
				audioTrack.Write(soundBuffer.m_data, 0, soundBuffer.m_data.Length);
				if (!isLooped)
				{
					m_audioTracks.Add(new AudioTrackData
					{
						AudioTrack = audioTrack,
						SoundBuffer = soundBuffer,
						BytesCount = soundBuffer.m_data.Length,
						IsAvailable = false
					});
				}
				else
				{
					Mixer.CheckTrackStatus(audioTrack.SetLoopPoints(0, soundBuffer.SamplesCount, -1));
				}
			}
			else
			{
				audioTrack.Release();
				audioTrack = null;
				Log.Warning("Failed to create Cache AudioTrack.");
			}
			m_cacheMisses++;
			if (Mixer.EnableAudioTrackCaching && m_cacheMisses > 200 && m_cacheMisses % 100 == 0)
			{
				Log.Warning("Over {0} AudioTrack objects created.", m_cacheMisses);
			}
			LogCacheStats();
			return audioTrack;
		}

		public static void ReturnAudioTrack(AudioTrack audioTrack)
		{
			AudioTrackData audioTrackData = null;
			for (int i = 0; i < m_audioTracks.Count; i++)
			{
				if (m_audioTracks[i].AudioTrack == audioTrack)
				{
					audioTrackData = m_audioTracks[i];
					break;
				}
			}
			if (!Mixer.EnableAudioTrackCaching)
			{
				if (audioTrackData != null)
				{
					m_audioTracks.Remove(audioTrackData);
				}
				audioTrack.Pause();
				audioTrack.Release();
				return;
			}
			if (audioTrackData == null)
			{
				audioTrack.Pause();
				audioTrack.Release();
				return;
			}
			bool flag = false;
			if (m_audioTracks.Count > 16)
			{
				flag = true;
				for (int j = 0; j < m_audioTracks.Count; j++)
				{
					if (m_audioTracks[j].BytesCount < audioTrackData.BytesCount)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				audioTrack.Pause();
				audioTrack.Release();
				m_audioTracks.Remove(audioTrackData);
			}
			else
			{
				audioTrack.Stop();
				audioTrack.SetPlaybackHeadPosition(audioTrackData.BytesCount / audioTrackData.SoundBuffer.ChannelsCount / 2);
				audioTrackData.ReloadStaticDataTime = Time.FrameStartTime + 0.75;
			}
			LogCacheStats();
		}

		public static void Frame()
		{
			if (!Mixer.EnableAudioTrackCaching)
			{
				Clear();
			}
			foreach (AudioTrackData audioTrack in m_audioTracks)
			{
				if (audioTrack.ReloadStaticDataTime > 0.0 && Time.FrameStartTime >= audioTrack.ReloadStaticDataTime)
				{
					audioTrack.ReloadStaticDataTime = 0.0;
					audioTrack.AudioTrack.ReloadStaticData();
					audioTrack.IsAvailable = true;
				}
			}
		}

		public static void Deactivated()
		{
			Clear();
		}

		public static void Clear()
		{
			int num = 0;
			while (num < m_audioTracks.Count)
			{
				if (m_audioTracks[num].IsAvailable || m_audioTracks[num].ReloadStaticDataTime > 0.0)
				{
					m_audioTracks[num].AudioTrack.Release();
					m_audioTracks.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}

		public static void LogCacheStats()
		{
		}
	}
}
