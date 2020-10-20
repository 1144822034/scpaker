using Android.Media;
using Android.OS;
using System;
using System.Collections.Generic;

namespace Engine.Audio
{
	public static class Mixer
	{
		internal static int m_audioTracksCreated;

		internal static int m_audioTracksDestroyed;

		internal static HashSet<BaseSound> m_sounds = new HashSet<BaseSound>();

		public static List<BaseSound> m_soundsToStop = new List<BaseSound>();

		public static List<BaseSound> m_pausedSounds = new List<BaseSound>();

		public static float m_masterVolume = 1f;

		public static bool EnableAudioTrackCaching
		{
			get;
			set;
		}

		public static float MasterVolume
		{
			get
			{
				return m_masterVolume;
			}
			set
			{
				value = MathUtils.Saturate(value);
				if (value != m_masterVolume)
				{
					m_masterVolume = value;
					InternalSetMasterVolume(value);
				}
			}
		}

		internal static void Initialize()
		{
			Window.Activity.VolumeControlStream = Stream.Music;
			EnableAudioTrackCaching = (Build.VERSION.SdkInt < (BuildVersionCodes)19);
		}

		internal static void Dispose()
		{
		}

		internal static void Activate()
		{
			foreach (BaseSound pausedSound in m_pausedSounds)
			{
				pausedSound.Play();
			}
		}

		internal static void Deactivate()
		{
			foreach (BaseSound sound in m_sounds)
			{
				if (sound.State == SoundState.Playing)
				{
					sound.Pause();
					m_pausedSounds.Add(sound);
				}
			}
		}

		internal static void BeforeFrame()
		{
			foreach (BaseSound sound in m_sounds)
			{
				if (sound.m_audioTrack != null && sound.State == SoundState.Playing && sound.m_stopPosition >= 0 && sound.m_audioTrack.PlaybackHeadPosition >= sound.m_stopPosition)
				{
					m_soundsToStop.Add(sound);
				}
			}
			foreach (BaseSound item in m_soundsToStop)
			{
				item.Stop();
			}
			m_soundsToStop.Clear();
		}

		internal static void AfterFrame()
		{
		}

		internal static void InternalSetMasterVolume(float volume)
		{
			foreach (BaseSound sound in m_sounds)
			{
				sound.InternalSetVolume(sound.Volume);
			}
		}

		internal static void CheckTrackStatus(TrackStatus status)
		{
			if (status != 0)
			{
				throw new InvalidOperationException("AudioTrack error " + status.ToString() + ".");
			}
		}
	}
}
