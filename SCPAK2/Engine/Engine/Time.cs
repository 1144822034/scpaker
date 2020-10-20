using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine
{
	public static class Time
	{
		public struct DelayedExecutionRequest
		{
			public double Time;

			public int FramesCount;

			public Action Action;
		}

		public static long m_applicationStartTicks = Stopwatch.GetTimestamp();

		public static List<DelayedExecutionRequest> m_delayedExecutionsRequests = new List<DelayedExecutionRequest>();

		public static int FrameIndex
		{
			get;
			set;
		}

		public static double RealTime => (double)(Stopwatch.GetTimestamp() - m_applicationStartTicks) / (double)Stopwatch.Frequency;

		public static double PreviousFrameStartTime
		{
			get;
			set;
		}

		public static double FrameStartTime
		{
			get;
			set;
		}

		public static float PreviousFrameDuration
		{
			get;
			set;
		}

		public static float FrameDuration
		{
			get;
			set;
		}

		public static bool PeriodicEvent(double period, double offset)
		{
			double num = FrameStartTime - offset;
			double num2 = MathUtils.Floor(num / period) * period;
			if (num >= num2)
			{
				return num - (double)FrameDuration < num2;
			}
			return false;
		}

		public static void QueueTimeDelayedExecution(double time, Action action)
		{
			m_delayedExecutionsRequests.Add(new DelayedExecutionRequest
			{
				Time = time,
				FramesCount = -1,
				Action = action
			});
		}

		public static void QueueFrameCountDelayedExecution(int framesCount, Action action)
		{
			m_delayedExecutionsRequests.Add(new DelayedExecutionRequest
			{
				Time = -1.0,
				FramesCount = framesCount,
				Action = action
			});
		}

		internal static void BeforeFrame()
		{
			double realTime = RealTime;
			PreviousFrameDuration = FrameDuration;
			FrameDuration = (float)(realTime - FrameStartTime);
			PreviousFrameStartTime = FrameStartTime;
			FrameStartTime = realTime;
			int num = 0;
			while (num < m_delayedExecutionsRequests.Count)
			{
				DelayedExecutionRequest delayedExecutionRequest = m_delayedExecutionsRequests[num];
				if ((delayedExecutionRequest.Time >= 0.0 && FrameStartTime >= delayedExecutionRequest.Time) || (delayedExecutionRequest.FramesCount >= 0 && FrameIndex >= delayedExecutionRequest.FramesCount))
				{
					m_delayedExecutionsRequests.RemoveAt(num);
					delayedExecutionRequest.Action();
				}
				else
				{
					num++;
				}
			}
		}

		internal static void AfterFrame()
		{
			FrameIndex++;
		}
	}
}
