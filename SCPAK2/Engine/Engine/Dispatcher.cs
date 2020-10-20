using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine
{
	public static class Dispatcher
	{
		public struct ActionInfo
		{
			public Action Action;

			public ManualResetEventSlim Event;
		}

		public static int? m_mainThreadId;

		public static List<ActionInfo> m_actionInfos = new List<ActionInfo>();

		public static List<ActionInfo> m_currentActionInfos = new List<ActionInfo>();

		public static int MainThreadId
		{
			get
			{
				if (!m_mainThreadId.HasValue)
				{
					throw new InvalidOperationException("Dispatcher is not initialized.");
				}
				return m_mainThreadId.Value;
			}
		}

		public static void Dispatch(Action action, bool waitUntilCompleted = false)
		{
			if (!m_mainThreadId.HasValue)
			{
				throw new InvalidOperationException("Dispatcher is not initialized.");
			}
			ActionInfo actionInfo;
			if (m_mainThreadId.Value == Environment.CurrentManagedThreadId)
			{
				action();
			}
			else if (waitUntilCompleted)
			{
				actionInfo = default(ActionInfo);
				actionInfo.Action = action;
				actionInfo.Event = new ManualResetEventSlim(initialState: false);
				ActionInfo item = actionInfo;
				lock (m_actionInfos)
				{
					m_actionInfos.Add(item);
				}
				item.Event.Wait();
				item.Event.Dispose();
			}
			else
			{
				lock (m_actionInfos)
				{
					List<ActionInfo> actionInfos = m_actionInfos;
					actionInfo = new ActionInfo
					{
						Action = action
					};
					actionInfos.Add(actionInfo);
				}
			}
		}

		internal static void Initialize()
		{
			m_mainThreadId = Environment.CurrentManagedThreadId;
		}

		internal static void Dispose()
		{
		}

		internal static void BeforeFrame()
		{
			m_currentActionInfos.Clear();
			lock (m_actionInfos)
			{
				m_currentActionInfos.AddRange(m_actionInfos);
				m_actionInfos.Clear();
			}
			foreach (ActionInfo currentActionInfo in m_currentActionInfos)
			{
				try
				{
					currentActionInfo.Action();
				}
				catch (Exception ex)
				{
					Log.Error("Dispatched action failed. Reason: {0}", ex);
				}
				finally
				{
					if (currentActionInfo.Event != null)
					{
						currentActionInfo.Event.Set();
					}
				}
			}
		}

		internal static void AfterFrame()
		{
		}
	}
}
