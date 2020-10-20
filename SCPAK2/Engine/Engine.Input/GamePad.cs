using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Input
{
	public static class GamePad
	{
		public class State
		{
			public bool IsConnected;

			public Vector2[] Sticks = new Vector2[2];

			public float[] Triggers = new float[2];

			public bool[] Buttons = new bool[14];

			public bool[] LastButtons = new bool[14];

			public double[] ButtonsRepeat = new double[14];
		}

		public static Dictionary<int, int> m_deviceToIndex = new Dictionary<int, int>();

		public static List<int> m_toRemove = new List<int>();

		public const double m_buttonFirstRepeatTime = 0.2;

		public const double m_buttonNextRepeatTime = 0.04;

		public static State[] m_states = new State[4]
		{
			new State(),
			new State(),
			new State(),
			new State()
		};

		internal static void Initialize()
		{
		}

		internal static void Dispose()
		{
		}

		internal static void BeforeFrame()
		{
			if (Time.PeriodicEvent(2.0, 0.0))
			{
				m_toRemove.Clear();
				foreach (int key in m_deviceToIndex.Keys)
				{
					if (InputDevice.GetDevice(key) == null)
					{
						m_toRemove.Add(key);
					}
				}
				foreach (int item in m_toRemove)
				{
					Disconnect(item);
				}
			}
		}

		internal static void HandleKeyDown(int deviceId, Keycode keyCode)
		{
			int num = TranslateDeviceId(deviceId);
			if (num < 0)
			{
				return;
			}
			GamePadButton gamePadButton = TranslateKey(keyCode);
			if (gamePadButton >= GamePadButton.A)
			{
				m_states[num].Buttons[(int)gamePadButton] = true;
				return;
			}
			switch (keyCode)
			{
			case Keycode.ButtonL2:
				m_states[num].Triggers[0] = 1f;
				break;
			case Keycode.ButtonR2:
				m_states[num].Triggers[1] = 1f;
				break;
			}
		}

		internal static void HandleKeyUp(int deviceId, Keycode keyCode)
		{
			int num = TranslateDeviceId(deviceId);
			if (num < 0)
			{
				return;
			}
			GamePadButton gamePadButton = TranslateKey(keyCode);
			if (gamePadButton >= GamePadButton.A)
			{
				m_states[num].Buttons[(int)gamePadButton] = false;
				return;
			}
			switch (keyCode)
			{
			case Keycode.ButtonL2:
				m_states[num].Triggers[0] = 0f;
				break;
			case Keycode.ButtonR2:
				m_states[num].Triggers[1] = 0f;
				break;
			}
		}

		internal static void HandleMotionEvent(MotionEvent e)
		{
			int num = TranslateDeviceId(e.DeviceId);
			if (num >= 0)
			{
				m_states[num].Sticks[0] = new Vector2(e.GetAxisValue(Axis.X), 0f - e.GetAxisValue(Axis.Y));
				m_states[num].Sticks[1] = new Vector2(e.GetAxisValue(Axis.Z), 0f - e.GetAxisValue(Axis.Rz));
				m_states[num].Triggers[0] = MathUtils.Max(e.GetAxisValue(Axis.Ltrigger), e.GetAxisValue(Axis.Brake));
				m_states[num].Triggers[1] = MathUtils.Max(e.GetAxisValue(Axis.Rtrigger), e.GetAxisValue(Axis.Gas));
				float axisValue = e.GetAxisValue(Axis.HatX);
				float axisValue2 = e.GetAxisValue(Axis.HatY);
				m_states[num].Buttons[10] = (axisValue < -0.5f);
				m_states[num].Buttons[12] = (axisValue > 0.5f);
				m_states[num].Buttons[11] = (axisValue2 < -0.5f);
				m_states[num].Buttons[13] = (axisValue2 > 0.5f);
			}
		}

		public static int TranslateDeviceId(int deviceId)
		{
			if (m_deviceToIndex.TryGetValue(deviceId, out int value))
			{
				return value;
			}
			for (int i = 0; i < 4; i++)
			{
				if (!m_deviceToIndex.Values.Contains(i))
				{
					Connect(deviceId, i);
					return i;
				}
			}
			return -1;
		}

		public static GamePadButton TranslateKey(Keycode keyCode)
		{
			switch (keyCode)
			{
			case Keycode.ButtonA:
				return GamePadButton.A;
			case Keycode.ButtonB:
				return GamePadButton.B;
			case Keycode.ButtonX:
				return GamePadButton.X;
			case Keycode.ButtonY:
				return GamePadButton.Y;
			case Keycode.Back:
				return GamePadButton.Back;
			case Keycode.ButtonL1:
				return GamePadButton.LeftShoulder;
			case Keycode.ButtonR1:
				return GamePadButton.RightShoulder;
			case Keycode.ButtonThumbl:
				return GamePadButton.LeftThumb;
			case Keycode.ButtonThumbr:
				return GamePadButton.RightThumb;
			case Keycode.DpadLeft:
				return GamePadButton.DPadLeft;
			case Keycode.DpadRight:
				return GamePadButton.DPadRight;
			case Keycode.DpadUp:
				return GamePadButton.DPadUp;
			case Keycode.DpadDown:
				return GamePadButton.DPadDown;
			case Keycode.ButtonSelect:
				return GamePadButton.Back;
			case Keycode.ButtonStart:
				return GamePadButton.Start;
			default:
				return (GamePadButton)(-1);
			}
		}

		public static void Connect(int deviceId, int index)
		{
			m_deviceToIndex.Add(deviceId, index);
			m_states[index].IsConnected = true;
		}

		public static void Disconnect(int deviceId)
		{
			if (m_deviceToIndex.TryGetValue(deviceId, out int value))
			{
				m_deviceToIndex.Remove(deviceId);
				m_states[value].IsConnected = false;
			}
		}

		public static bool IsConnected(int gamePadIndex)
		{
			if (gamePadIndex < 0 || gamePadIndex >= m_states.Length)
			{
				throw new ArgumentOutOfRangeException("gamePadIndex");
			}
			return m_states[gamePadIndex].IsConnected;
		}

		public static Vector2 GetStickPosition(int gamePadIndex, GamePadStick stick, float deadZone = 0f)
		{
			if (deadZone < 0f || deadZone >= 1f)
			{
				throw new ArgumentOutOfRangeException("deadZone");
			}
			if (IsConnected(gamePadIndex))
			{
				Vector2 result = m_states[gamePadIndex].Sticks[(int)stick];
				if (deadZone > 0f)
				{
					float num = result.Length();
					if (num > 0f)
					{
						float num2 = ApplyDeadZone(num, deadZone);
						result *= num2 / num;
					}
				}
				return result;
			}
			return Vector2.Zero;
		}

		public static float GetTriggerPosition(int gamePadIndex, GamePadTrigger trigger, float deadZone = 0f)
		{
			if (deadZone < 0f || deadZone >= 1f)
			{
				throw new ArgumentOutOfRangeException("deadZone");
			}
			if (IsConnected(gamePadIndex))
			{
				return ApplyDeadZone(m_states[gamePadIndex].Triggers[(int)trigger], deadZone);
			}
			return 0f;
		}

		public static bool IsButtonDown(int gamePadIndex, GamePadButton button)
		{
			if (IsConnected(gamePadIndex))
			{
				return m_states[gamePadIndex].Buttons[(int)button];
			}
			return false;
		}

		public static bool IsButtonDownOnce(int gamePadIndex, GamePadButton button)
		{
			if (IsConnected(gamePadIndex))
			{
				if (m_states[gamePadIndex].Buttons[(int)button])
				{
					return !m_states[gamePadIndex].LastButtons[(int)button];
				}
				return false;
			}
			return false;
		}

		public static bool IsButtonDownRepeat(int gamePadIndex, GamePadButton button)
		{
			if (IsConnected(gamePadIndex))
			{
				if (m_states[gamePadIndex].Buttons[(int)button] && !m_states[gamePadIndex].LastButtons[(int)button])
				{
					return true;
				}
				double num = m_states[gamePadIndex].ButtonsRepeat[(int)button];
				if (num != 0.0)
				{
					return Time.FrameStartTime >= num;
				}
				return false;
			}
			return false;
		}

		public static void Clear()
		{
			for (int i = 0; i < m_states.Length; i++)
			{
				for (int j = 0; j < m_states[i].Sticks.Length; j++)
				{
					m_states[i].Sticks[j] = Vector2.Zero;
				}
				for (int k = 0; k < m_states[i].Triggers.Length; k++)
				{
					m_states[i].Triggers[k] = 0f;
				}
				for (int l = 0; l < m_states[i].Buttons.Length; l++)
				{
					m_states[i].Buttons[l] = false;
					m_states[i].ButtonsRepeat[l] = 0.0;
				}
			}
		}

		internal static void AfterFrame()
		{
			for (int i = 0; i < m_states.Length; i++)
			{
				if (Keyboard.BackButtonQuitsApp && IsButtonDownOnce(i, GamePadButton.Back))
				{
					Window.Close();
				}
				State state = m_states[i];
				for (int j = 0; j < state.Buttons.Length; j++)
				{
					if (state.Buttons[j])
					{
						if (!state.LastButtons[j])
						{
							state.ButtonsRepeat[j] = Time.FrameStartTime + 0.2;
						}
						else if (Time.FrameStartTime >= state.ButtonsRepeat[j])
						{
							state.ButtonsRepeat[j] = MathUtils.Max(Time.FrameStartTime, state.ButtonsRepeat[j] + 0.04);
						}
					}
					else
					{
						state.ButtonsRepeat[j] = 0.0;
					}
					state.LastButtons[j] = state.Buttons[j];
				}
			}
		}

		public static float ApplyDeadZone(float value, float deadZone)
		{
			return MathUtils.Sign(value) * MathUtils.Max(MathUtils.Abs(value) - deadZone, 0f) / (1f - deadZone);
		}
	}
}
