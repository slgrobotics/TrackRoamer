using System;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Globalization;

namespace LibSystem
{
	public interface Loggable
	{
		string toLogValueString();
	}

	public class Logger
	{
		private static string m_traceFileName = null;
		private static Hashtable m_measuredValues;
		private static ArrayList m_loggedValueNames;
		private static int _id = 1;

		/// <summary>
		/// to log a set of values in .CSV format
		/// </summary>
		/// <param name="measuredValues"></param>
		/// <param name="loggedValueNames">can be empty or null, will be filled</param>
		public Logger(Hashtable measuredValues, ArrayList loggedValueNames)
		{
			m_measuredValues = measuredValues;
			m_loggedValueNames = loggedValueNames;

			if (m_loggedValueNames == null)
			{
				m_loggedValueNames = new ArrayList();
			}

			if (m_loggedValueNames.Count == 0)
			{
				foreach (string key in m_measuredValues.Keys)
				{
					m_loggedValueNames.Add(key);
				}
			}

			StringBuilder sb = new StringBuilder();

			sb.Append("id,TimeUniversal,Date,Time,");

			foreach (string vName in m_loggedValueNames)
			{
				sb.Append(vName);
				sb.Append(",");
			}
			sb.Remove(sb.Length - 1, 1);

			m_traceFileName = Path.Combine(Application.StartupPath, "log_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");

			Logger.writeLine(sb.ToString());
		}

		private static DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;

		public static void Log()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(String.Format("{0},", _id));
			_id++;

			DateTime now = DateTime.Now;

			sb.Append(now.ToString("yyyy-MM-ddThh:mm:ss"));
			//sb.Append(now.ToString(myDTFI.UniversalSortableDateTimePattern));
			sb.Append(String.Format(".{0},",now.Millisecond));
			//sb.Append(",");

			sb.Append(now.ToString("MM/dd/yyyy"));
			sb.Append(",");

			sb.Append(now.ToString("hh:mm:ss"));
			//sb.Append(String.Format(".{0},",now.Millisecond));
			sb.Append(",");

			for (int i = 0; i < m_loggedValueNames.Count; i++)
			{
				string vName = (string)m_loggedValueNames[i];
				if (m_measuredValues.ContainsKey(vName))
				{
					Loggable loggable =(Loggable)m_measuredValues[vName];
					sb.Append(loggable.toLogValueString());
				}
				sb.Append(",");
			}
			sb.Remove(sb.Length - 1, 1);

			Logger.writeLine(sb.ToString());
		}

		private static void writeLine(string str)
		{
			FileStream fs = new FileStream(m_traceFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			TextWriter m_tw = new StreamWriter(fs);
			m_tw.WriteLine(str);
			m_tw.Close();
		}
	}
}
