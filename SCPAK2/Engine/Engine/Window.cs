using Android.Content;
using Android.OS;
using Engine.Audio;
using Engine.Graphics;
using Engine.Input;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
	public static class Window
	{
		public enum State
		{
			Uncreated,
			Inactive,
			Active
		}

		public static State m_state;

		public static bool m_contextLost;

		internal static bool m_focusRegained;

		public static int m_presentationInterval = 1;

		public static double m_frameStartTime;

		public static bool IsCreated => m_state != State.Uncreated;

		public static bool IsActive => m_state == State.Active;

		public static EngineActivity Activity => EngineActivity.m_activity;

		public static EngineView View
		{
			get;
			set;
		}

		public static Point2 ScreenSize => new Point2(EngineActivity.m_activity.Resources.DisplayMetrics.WidthPixels, EngineActivity.m_activity.Resources.DisplayMetrics.HeightPixels);

		public static WindowMode WindowMode
		{
			get
			{
				VerifyWindowOpened();
				return WindowMode.Fullscreen;
			}
			set
			{
				VerifyWindowOpened();
			}
		}

		public static Point2 Position
		{
			get
			{
				VerifyWindowOpened();
				return Point2.Zero;
			}
			set
			{
				VerifyWindowOpened();
			}
		}

		public static Point2 Size
		{
			get
			{
				VerifyWindowOpened();
				return new Point2(View.Size.Width, View.Size.Height);
			}
			set
			{
				VerifyWindowOpened();
			}
		}

		public static string Title
		{
			get
			{
				VerifyWindowOpened();
				return string.Empty;
			}
			set
			{
				VerifyWindowOpened();
			}
		}

		public static int PresentationInterval
		{
			get
			{
				VerifyWindowOpened();
				return m_presentationInterval;
			}
			set
			{
				VerifyWindowOpened();
				m_presentationInterval = MathUtils.Clamp(value, 1, 4);
			}
		}

		public static event Action Created;

		public static event Action Resized;

		public static event Action Activated;

		public static event Action Deactivated;

		public static event Action Closed;

		public static event Action Frame;

		public static event Action<UnhandledExceptionInfo> UnhandledException;

		public static event Action<Uri> HandleUri;

		public static void InitializeAll()
		{
			Dispatcher.Initialize();
			Display.Initialize();
			Keyboard.Initialize();
			Mouse.Initialize();
			Touch.Initialize();
			GamePad.Initialize();
			Mixer.Initialize();
		}

		public static void DisposeAll()
		{
			Dispatcher.Dispose();
			Display.Dispose();
			Keyboard.Dispose();
			Mouse.Dispose();
			Touch.Dispose();
			GamePad.Dispose();
			Mixer.Dispose();
		}

		public static void BeforeFrameAll()
		{
			Time.BeforeFrame();
			Dispatcher.BeforeFrame();
			Display.BeforeFrame();
			Keyboard.BeforeFrame();
			Mouse.BeforeFrame();
			Touch.BeforeFrame();
			GamePad.BeforeFrame();
			Mixer.BeforeFrame();
		}

		public static void AfterFrameAll()
		{
			Time.AfterFrame();
			Dispatcher.AfterFrame();
			Display.AfterFrame();
			Keyboard.AfterFrame();
			Mouse.AfterFrame();
			Touch.AfterFrame();
			GamePad.AfterFrame();
			Mixer.AfterFrame();
		}

		public static void Run(int width = 0, int height = 0, WindowMode windowMode = WindowMode.Fullscreen, string title = "")
		{
			if (View != null)
			{
				throw new InvalidOperationException("Window is already opened.");
			}
			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args)
			{
				if (Window.UnhandledException != null)
				{
					Exception ex = args.ExceptionObject as Exception;
					if (ex == null)
					{
						ex = new Exception($"Unknown exception. Additional information: {args.ExceptionObject}");
					}
					Window.UnhandledException(new UnhandledExceptionInfo(ex));
				}
			};
			Log.Information("Android.OS.Build.Display: " + Build.Display);
			Log.Information("Android.OS.Build.Device: " + Build.Device);
			Log.Information("Android.OS.Build.Hardware: " + Build.Hardware);
			Log.Information("Android.OS.Build.Manufacturer: " + Build.Manufacturer);
			Log.Information("Android.OS.Build.Model: " + Build.Model);
			Log.Information("Android.OS.Build.Product: " + Build.Product);
			Log.Information("Android.OS.Build.Brand: " + Build.Brand);
			Log.Information("Android.OS.Build.VERSION.SdkInt: " + ((int)Build.VERSION.SdkInt).ToString());
			View = new EngineView(Activity);
			View.ContextSet += ContextSetHandler;
			View.Resize += ResizeHandler;
			View.ContextLost += ContextLostHandler;
			View.RenderFrame += RenderFrameHandler;
			Activity.Paused += PausedHandler;
			Activity.Resumed += ResumedHandler;
			Activity.Destroyed += DestroyedHandler;
			Activity.NewIntent += NewIntentHandler;
			Activity.SetContentView(View);
			View.RequestFocus();
			View.Run();
		}

		public static void Close()
		{
			VerifyWindowOpened();
			Activity.Finish();
		}

		public static void VerifyWindowOpened()
		{
			if (View == null)
			{
				throw new InvalidOperationException("Window is not opened.");
			}
		}

		public static void PausedHandler()
		{
			if (m_state == State.Active)
			{
				m_state = State.Inactive;
				Keyboard.Clear();
				Mixer.Deactivate();
				if (Window.Deactivated != null)
				{
					Window.Deactivated();
				}
			}
		}

		public static void ResumedHandler()
		{
			if (m_state == State.Inactive)
			{
				m_state = State.Active;
				Mixer.Activate();
				View.EnableImmersiveMode();
				Window.Activated?.Invoke();
			}
		}

		public static void DestroyedHandler()
		{
			if (m_state == State.Active)
			{
				m_state = State.Inactive;
				Window.Deactivated?.Invoke();
			}
			m_state = State.Uncreated;
			Window.Closed?.Invoke();
			DisposeAll();
		}

		public static void NewIntentHandler(Intent intent)
		{
			if (Window.HandleUri != null && intent != null)
			{
				Uri uriFromIntent = GetUriFromIntent(intent);
				if (uriFromIntent != null)
				{
					Window.HandleUri(uriFromIntent);
				}
			}
		}

		public static void ResizeHandler(object sender, EventArgs args)
		{
			if (m_state != 0)
			{
				Display.Resize();
				Window.Resized?.Invoke();
			}
		}

		public static void ContextSetHandler(object sender, EventArgs args)
		{
			if (m_contextLost)
			{
				m_contextLost = false;
				Display.HandleDeviceReset();
			}
		}

		public static void ContextLostHandler(object sender, EventArgs args)
		{
			m_contextLost = true;
			Display.HandleDeviceLost();
		}

		public static void RenderFrameHandler(object sender, EventArgs args)
		{
			if (m_state == State.Uncreated)
			{
				InitializeAll();
				m_state = State.Inactive;
				Window.Created?.Invoke();
				m_state = State.Active;
				Window.Activated?.Invoke();
				NewIntentHandler(Activity.Intent);
			}
			if (m_state != State.Active)
			{
				return;
			}
			BeforeFrameAll();
			Window.Frame?.Invoke();
			AfterFrameAll();
			View.GraphicsContext.SwapBuffers();
			if (m_presentationInterval >= 2)
			{
				double num = Time.RealTime - m_frameStartTime;
				int num2 = (int)(1000.0 * ((double)((float)m_presentationInterval / 60f) - num));
				if (num2 > 0)
				{
					Task.Delay(num2).Wait();
				}
			}
			m_frameStartTime = Time.RealTime;
			if (m_focusRegained)
			{
				m_focusRegained = false;
				View.EnableImmersiveMode();
			}
		}

		public static Uri GetUriFromIntent(Intent intent)
		{
			Uri result = null;
			if (!string.IsNullOrEmpty(intent.DataString))
			{
				Uri.TryCreate(intent.DataString, UriKind.RelativeOrAbsolute, out result);
			}
			return result;
		}
	}
}
