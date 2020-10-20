using System;
using System.IO;

namespace Engine
{
	public class StreamLogSink : ILogSink
	{
		public StreamWriter m_writer;

		public LogType MinimumLogType
		{
			get;
			set;
		}

		public StreamLogSink(Stream stream)
		{
			m_writer = new StreamWriter(stream);
			stream.Position = stream.Length;
		}

		public void Log(LogType logType, string message)
		{
			if (logType >= MinimumLogType)
			{
				string str;
				switch (logType)
				{
				case LogType.Debug:
					str = "DEBUG: ";
					break;
				case LogType.Verbose:
					str = "INFO: ";
					break;
				case LogType.Information:
					str = "INFO: ";
					break;
				case LogType.Warning:
					str = "WARNING: ";
					break;
				case LogType.Error:
					str = "ERROR: ";
					break;
				default:
					str = string.Empty;
					break;
				}
				m_writer.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + str + message);
				m_writer.Flush();
			}
		}

		public void Dispose()
		{
			m_writer.Dispose();
		}
	}
}
