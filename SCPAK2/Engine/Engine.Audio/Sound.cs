using System;

namespace Engine.Audio
{
	public sealed class Sound : BaseSound
	{
		public bool m_audioTrackCreateAttempted;

		public SoundBuffer m_soundBuffer;

		public SoundBuffer SoundBuffer => m_soundBuffer;

		public Sound(SoundBuffer soundBuffer, float volume = 1f, float pitch = 1f, float pan = 0f, bool isLooped = false, bool disposeOnStop = false)
		{
			if (soundBuffer == null)
			{
				throw new ArgumentNullException("soundBuffer");
			}
			Initialize(soundBuffer);
			base.ChannelsCount = soundBuffer.ChannelsCount;
			base.SamplingFrequency = soundBuffer.SamplingFrequency;
			base.Volume = volume;
			base.Pitch = pitch;
			base.Pan = pan;
			base.IsLooped = isLooped;
			base.DisposeOnStop = disposeOnStop;
		}

		internal override void InternalPlay()
		{
			if (m_audioTrack == null)
			{
				if (!m_audioTrackCreateAttempted)
				{
					m_audioTrackCreateAttempted = true;
					m_audioTrack = AudioTrackCache.GetAudioTrack(SoundBuffer, m_isLooped);
					if (m_audioTrack != null)
					{
						m_stopPosition = (m_isLooped ? (-1) : (SoundBuffer.SamplesCount - 1));
						InternalSetVolume(base.Volume);
						InternalSetPitch(base.Pitch);
						InternalSetPan(base.Pan);
						m_audioTrack.Play();
					}
					else if (!m_isLooped)
					{
						Stop();
					}
				}
			}
			else
			{
				m_audioTrack.SetPlaybackHeadPosition(m_audioTrack.PlaybackHeadPosition);
				m_audioTrack.Play();
			}
		}

		internal override void InternalPause()
		{
			if (m_audioTrack != null)
			{
				m_audioTrack.Pause();
			}
		}

		internal override void InternalStop()
		{
			if (m_audioTrack != null)
			{
				AudioTrackCache.ReturnAudioTrack(m_audioTrack);
				m_audioTrack = null;
			}
			m_audioTrackCreateAttempted = false;
		}

		internal override void InternalDispose()
		{
			if (m_audioTrack != null)
			{
				AudioTrackCache.ReturnAudioTrack(m_audioTrack);
				m_audioTrack = null;
			}
			base.InternalDispose();
		}

		public override void Dispose()
		{
			base.Dispose();
			if (m_soundBuffer != null)
			{
				int num = --m_soundBuffer.UseCount;
				m_soundBuffer = null;
			}
		}

		internal void Initialize(SoundBuffer soundBuffer)
		{
			if (soundBuffer == null)
			{
				throw new ArgumentNullException("soundBuffer");
			}
			m_soundBuffer = soundBuffer;
			int num = ++m_soundBuffer.UseCount;
		}
	}
}
