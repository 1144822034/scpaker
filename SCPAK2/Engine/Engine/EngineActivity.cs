using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;
using System.Threading;

namespace Engine
{

	public class EngineActivity : Activity
	{
		internal static EngineActivity m_activity;

		public event Action Paused;

		public event Action Resumed;

		public event Action Destroyed;

		public event Action<Intent> NewIntent;
		
		public EngineActivity()
		{
			m_activity = this;
		}
		public HashSet<string> ListAppAssemblies()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ZipArchiveEntry entry in ZipFile.OpenRead(ApplicationInfo.SourceDir).Entries)
			{
				string text = entry.Name.ToLower();
				if (entry.FullName.StartsWith("assemblies/") && text.EndsWith(".dll"))
				{
					hashSet.Add(entry.Name);
				}
				else if (text.StartsWith("libaot-") && text.EndsWith(".so"))
				{
					hashSet.Add(entry.Name.Substring(7, entry.Name.Length - 7 - 3));
				}
			}
			return hashSet;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			while (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
			{
				RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 0);
			}
			RequestWindowFeature(WindowFeatures.NoTitle);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			base.VolumeControlStream = Android.Media.Stream.Music;
			RequestedOrientation = ScreenOrientation.UserLandscape;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.GetName().Name == "mscorlib" || assembly.GetName().Name == "Mono.Android")
				{
					continue;
				}
				Type[] types = assembly.GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					MethodInfo method = types[j].GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (!(method != null))
					{
						continue;
					}
					List<object> list = new List<object>();
					ParameterInfo[] parameters = method.GetParameters();
					if (parameters.Length == 1)
					{
						if (parameters[0].ParameterType != typeof(string[]))
						{
							continue;
						}
						list.Add(new string[0]);
					}
					else if (parameters.Length > 1)
					{
						continue;
					}
					method.Invoke(null, list.ToArray());
					return;
				}
			}
			throw new Exception("Cannot find static Main method.");
		}
		protected override void OnPause()
		{
			base.OnPause();
			this.Paused?.Invoke();
		}

		protected override void OnResume()
		{
			base.OnResume();
			this.Resumed?.Invoke();
		}

		protected override void OnDestroy()
		{
			try
			{
				base.OnDestroy();
				this.Destroyed?.Invoke();
			}
			finally
			{
				Thread.Sleep(250);
				System.Environment.Exit(0);
			}
		}
	}
}
