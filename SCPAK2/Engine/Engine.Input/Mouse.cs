using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.Input
{
	public static class Mouse
	{
		public static bool[] m_mouseButtonsDownArray;

		public static bool[] m_mouseButtonsDownOnceArray;

		public static Point2 MouseMovement
		{
			get;
			set;
		}

		public static int MouseWheelMovement
		{
			get;
			set;
		}

		public static Point2? MousePosition
		{
			get;
			set;
		}

		public static bool IsMouseVisible
		{
			get;
			set;
		}

		public static event Action<MouseEvent> MouseMove;
		public static event Action<MouseButtonEvent> MouseDown;
		public static event Action<MouseButtonEvent> MouseUp;

		public static void SetMousePosition(int x, int y)
		{
		}

		internal static void Initialize()
		{
		}

		internal static void Dispose()
		{
		}

		internal static void BeforeFrame()
		{
		}

		static Mouse()
		{
			m_mouseButtonsDownArray = new bool[Enum.GetValues(typeof(MouseButton)).Length];
			m_mouseButtonsDownOnceArray = new bool[Enum.GetValues(typeof(MouseButton)).Length];
			IsMouseVisible = true;
		}

		public static bool IsMouseButtonDown(MouseButton mouseButton)
		{
			return m_mouseButtonsDownArray[(int)mouseButton];
		}

		public static bool IsMouseButtonDownOnce(MouseButton mouseButton)
		{
			return m_mouseButtonsDownOnceArray[(int)mouseButton];
		}

		public static void Clear()
		{
			for (int i = 0; i < m_mouseButtonsDownArray.Length; i++)
			{
				m_mouseButtonsDownArray[i] = false;
				m_mouseButtonsDownOnceArray[i] = false;
			}
		}

		internal static void AfterFrame()
		{
			for (int i = 0; i < m_mouseButtonsDownOnceArray.Length; i++)
			{
				m_mouseButtonsDownOnceArray[i] = false;
			}
			if (!IsMouseVisible)
			{
				MousePosition = null;
			}
		}

		public static void ProcessMouseDown(MouseButton mouseButton, Point2 position)
		{
			if (Window.IsActive && !Keyboard.IsKeyboardVisible)
			{
				m_mouseButtonsDownArray[(int)mouseButton] = true;
				m_mouseButtonsDownOnceArray[(int)mouseButton] = true;
				if (IsMouseVisible && Mouse.MouseDown != null)
				{
					Mouse.MouseDown(new MouseButtonEvent
					{
						Button = mouseButton,
						Position = position
					});
				}
			}
		}

		public static void ProcessMouseUp(MouseButton mouseButton, Point2 position)
		{
			if (Window.IsActive && !Keyboard.IsKeyboardVisible)
			{
				m_mouseButtonsDownArray[(int)mouseButton] = false;
				if (IsMouseVisible && Mouse.MouseUp != null)
				{
					Mouse.MouseUp(new MouseButtonEvent
					{
						Button = mouseButton,
						Position = position
					});
				}
			}
		}

		public static void ProcessMouseMove(Point2 position)
		{
			if (Window.IsActive && !Keyboard.IsKeyboardVisible && IsMouseVisible)
			{
				MousePosition = position;
				if (Mouse.MouseMove != null)
				{
					Mouse.MouseMove(new MouseEvent
					{
						Position = position
					});
				}
			}
		}
	}
}
