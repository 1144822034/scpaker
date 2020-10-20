using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.Input
{
	public static class Keyboard
	{
		public const double m_keyFirstRepeatTime = 0.2;

		public const double m_keyNextRepeatTime = 0.033;

		public static bool[] m_keysDownArray = new bool[Enum.GetValues(typeof(Key)).Length];

		public static bool[] m_keysDownOnceArray = new bool[Enum.GetValues(typeof(Key)).Length];

		public static double[] m_keysDownRepeatArray = new double[Enum.GetValues(typeof(Key)).Length];

		public static Key? m_lastKey;

		public static char? m_lastChar;

		public static Key? LastKey => m_lastKey;

		public static char? LastChar => m_lastChar;

		public static bool IsKeyboardVisible
		{
			get;
			set;
		}

		public static bool BackButtonQuitsApp
		{
			get;
			set;
		}

		public static event Action<Key> KeyDown;

		public static event Action<Key> KeyUp;
		public static event Action<char> CharacterEntered;
		public static bool IsKeyDown(Key key)
		{
			return m_keysDownArray[(int)key];
		}

		public static bool IsKeyDownOnce(Key key)
		{
			return m_keysDownOnceArray[(int)key];
		}

		public static bool IsKeyDownRepeat(Key key)
		{
			double num = m_keysDownRepeatArray[(int)key];
			if (!(num < 0.0))
			{
				if (num != 0.0)
				{
					return Time.FrameStartTime >= num;
				}
				return false;
			}
			return true;
		}

		public static void ShowKeyboard(string title, string description, string defaultText, bool passwordMode, Action<string> enter, Action cancel)
		{
			if (title == null)
			{
				throw new ArgumentNullException("title");
			}
			if (description == null)
			{
				throw new ArgumentNullException("description");
			}
			if (defaultText == null)
			{
				throw new ArgumentNullException("defaultText");
			}
			if (!IsKeyboardVisible)
			{
				Clear();
				Touch.Clear();
				Mouse.Clear();
				IsKeyboardVisible = true;
				try
				{
					ShowKeyboardInternal(title, description, defaultText, passwordMode, delegate(string text)
					{
						Dispatcher.Dispatch(delegate
						{
							IsKeyboardVisible = false;
							if (enter != null)
							{
								enter(text ?? string.Empty);
							}
						});
					}, delegate
					{
						Dispatcher.Dispatch(delegate
						{
							IsKeyboardVisible = false;
							if (cancel != null)
							{
								cancel();
							}
						});
					});
				}
				catch
				{
					IsKeyboardVisible = false;
					throw;
				}
			}
		}

		public static void Clear()
		{
			m_lastKey = null;
			m_lastChar = null;
			for (int i = 0; i < m_keysDownArray.Length; i++)
			{
				m_keysDownArray[i] = false;
				m_keysDownOnceArray[i] = false;
				m_keysDownRepeatArray[i] = 0.0;
			}
		}

		internal static void BeforeFrame()
		{
		}

		internal static void AfterFrame()
		{
			if (BackButtonQuitsApp && IsKeyDownOnce(Key.Back))
			{
				Window.Close();
			}
			m_lastKey = null;
			m_lastChar = null;
			for (int i = 0; i < m_keysDownOnceArray.Length; i++)
			{
				m_keysDownOnceArray[i] = false;
			}
			for (int j = 0; j < m_keysDownRepeatArray.Length; j++)
			{
				if (m_keysDownArray[j])
				{
					if (m_keysDownRepeatArray[j] < 0.0)
					{
						m_keysDownRepeatArray[j] = Time.FrameStartTime + 0.2;
					}
					else if (Time.FrameStartTime >= m_keysDownRepeatArray[j])
					{
						m_keysDownRepeatArray[j] = MathUtils.Max(Time.FrameStartTime, m_keysDownRepeatArray[j] + 0.033);
					}
				}
				else
				{
					m_keysDownRepeatArray[j] = 0.0;
				}
			}
		}

		public static bool ProcessKeyDown(Key key)
		{
			if (!Window.IsActive || IsKeyboardVisible)
			{
				return false;
			}
			m_lastKey = key;
			if (!m_keysDownArray[(int)key])
			{
				m_keysDownArray[(int)key] = true;
				m_keysDownOnceArray[(int)key] = true;
				m_keysDownRepeatArray[(int)key] = -1.0;
			}
			Keyboard.KeyDown?.Invoke(key);
			return true;
		}

		public static bool ProcessKeyUp(Key key)
		{
			if (!Window.IsActive || IsKeyboardVisible)
			{
				return false;
			}
			if (m_keysDownArray[(int)key])
			{
				m_keysDownArray[(int)key] = false;
			}
			Keyboard.KeyUp?.Invoke(key);
			return true;
		}

		public static bool ProcessCharacterEntered(char ch)
		{
			if (!Window.IsActive || IsKeyboardVisible)
			{
				return false;
			}
			m_lastChar = ch;
			Keyboard.CharacterEntered?.Invoke(ch);
			return true;
		}

		internal static void Initialize()
		{
		}

		internal static void Dispose()
		{
		}

		internal static void HandleKeyDown(Keycode keyCode)
		{
			Key key = TranslateKey(keyCode);
			if (key != (Key)(-1))
			{
				ProcessKeyDown(key);
			}
		}

		internal static void HandleKeyUp(Keycode keyCode)
		{
			Key key = TranslateKey(keyCode);
			if (key != (Key)(-1))
			{
				ProcessKeyUp(key);
			}
		}

		internal static void HandleKeyPress(int unicodeCharacter)
		{
			ProcessCharacterEntered((char)unicodeCharacter);
		}

		public static Key TranslateKey(Keycode keyCode)
		{
			switch (keyCode)
			{
			case Keycode.Home:
				return Key.Home;
			case Keycode.Back:
				return Key.Back;
			case Keycode.Num0:
				return Key.Number0;
			case Keycode.Num1:
				return Key.Number1;
			case Keycode.Num2:
				return Key.Number2;
			case Keycode.Num3:
				return Key.Number3;
			case Keycode.Num4:
				return Key.Number4;
			case Keycode.Num5:
				return Key.Number5;
			case Keycode.Num6:
				return Key.Number6;
			case Keycode.Num7:
				return Key.Number7;
			case Keycode.Num8:
				return Key.Number8;
			case Keycode.Num9:
				return Key.Number9;
			case Keycode.A:
				return Key.A;
			case Keycode.B:
				return Key.B;
			case Keycode.C:
				return Key.C;
			case Keycode.D:
				return Key.D;
			case Keycode.E:
				return Key.E;
			case Keycode.F:
				return Key.F;
			case Keycode.G:
				return Key.G;
			case Keycode.H:
				return Key.H;
			case Keycode.I:
				return Key.I;
			case Keycode.J:
				return Key.J;
			case Keycode.K:
				return Key.K;
			case Keycode.L:
				return Key.L;
			case Keycode.M:
				return Key.M;
			case Keycode.N:
				return Key.N;
			case Keycode.O:
				return Key.O;
			case Keycode.P:
				return Key.P;
			case Keycode.Q:
				return Key.Q;
			case Keycode.R:
				return Key.R;
			case Keycode.S:
				return Key.S;
			case Keycode.T:
				return Key.T;
			case Keycode.U:
				return Key.U;
			case Keycode.V:
				return Key.V;
			case Keycode.W:
				return Key.W;
			case Keycode.X:
				return Key.X;
			case Keycode.Y:
				return Key.Y;
			case Keycode.Z:
				return Key.Z;
			case Keycode.Comma:
				return Key.Comma;
			case Keycode.Period:
				return Key.Period;
			case Keycode.ShiftLeft:
				return Key.Shift;
			case Keycode.ShiftRight:
				return Key.Shift;
			case Keycode.Tab:
				return Key.Tab;
			case Keycode.Space:
				return Key.Space;
			case Keycode.Enter:
				return Key.Enter;
			case Keycode.Del:
				return Key.Delete;
			case Keycode.Minus:
				return Key.Minus;
			case Keycode.LeftBracket:
				return Key.LeftBracket;
			case Keycode.RightBracket:
				return Key.RightBracket;
			case Keycode.Semicolon:
				return Key.Semicolon;
			case Keycode.Slash:
				return Key.Slash;
			case Keycode.Plus:
				return Key.Plus;
			case Keycode.PageUp:
				return Key.PageUp;
			case Keycode.PageDown:
				return Key.PageDown;
			case Keycode.Escape:
				return Key.Escape;
			case Keycode.ForwardDel:
				return Key.Delete;
			case Keycode.CtrlLeft:
				return Key.Control;
			case Keycode.CtrlRight:
				return Key.Control;
			case Keycode.CapsLock:
				return Key.CapsLock;
			case Keycode.Insert:
				return Key.Insert;
			case Keycode.F1:
				return Key.F1;
			case Keycode.F2:
				return Key.F2;
			case Keycode.F3:
				return Key.F3;
			case Keycode.F4:
				return Key.F4;
			case Keycode.F5:
				return Key.F5;
			case Keycode.F6:
				return Key.F6;
			case Keycode.F7:
				return Key.F7;
			case Keycode.F8:
				return Key.F8;
			case Keycode.F9:
				return Key.F9;
			case Keycode.F10:
				return Key.F10;
			case Keycode.F11:
				return Key.F11;
			case Keycode.F12:
				return Key.F12;
			default:
				return (Key)(-1);
			}
		}

		public static void ShowKeyboardInternal(string title, string description, string defaultText, bool passwordMode, Action<string> enter, Action cancel)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(Window.Activity);
			builder.SetTitle(title);
			builder.SetMessage(description);
			EditText editText = new EditText(Window.Activity);
			editText.Text = defaultText;
			builder.SetView(editText);
			builder.SetPositiveButton("Ok", delegate
			{
				enter(editText.Text);
			});
			builder.SetNegativeButton("Cancel", delegate
			{
				cancel();
			});
			AlertDialog alertDialog = builder.Create();
			alertDialog.DismissEvent += delegate
			{
				cancel();
			};
			alertDialog.CancelEvent += delegate
			{
				cancel();
			};
			alertDialog.Show();
		}
	}
}
