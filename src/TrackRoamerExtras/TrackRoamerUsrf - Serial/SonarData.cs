using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
	/// <summary>
	/// container for a single reading from sonar
	/// </summary>
	public class RangeReading
	{
		public int angleRaw;			// servo pulse width or any other measure, range for example 150 - 1160 for Futaba servo on Parallax BasicStamp
		public double angleDegrees;		// degrees -- not ised here, but may be used on the processing end

		public double rangeMeters;		// meters

		public long timestamp;

        public RangeReading(int _angleRaw, double _rangeMeters, long _timestamp)
		{
			angleRaw = _angleRaw;
			rangeMeters = _rangeMeters;
			timestamp = _timestamp;
		}
	}

	/// <summary>
	/// container for multiple readings of sonar, covering angular range and some time interval back
	/// </summary>
	public class SonarData
	{
		public ArrayList rangeReadings = new ArrayList();
		public SortedList angles = new SortedList();
		private int intervalToRememberS = 100;

		public void addRangeReading(int angleRaw, double rangeMeters, long timestamp)
		{
			lock (this)
			{
                RangeReading rr = new RangeReading(angleRaw, rangeMeters, timestamp);
				rangeReadings.Add(rr);
				purge();
			}
		}

		public RangeReading getLatestReadingAt(int angleRaw)
		{
			RangeReading ret = null;

			lock (this)
			{
				foreach (RangeReading rr in rangeReadings)
				{
					if (rr.angleRaw == angleRaw)
					{
						if (ret == null || ret.timestamp < rr.timestamp)
						{
							ret = rr;
						}
					}
				}
			}

			return ret;
		}

		public SortedList getAllReadingsAt(int angleRaw)
		{
			SortedList ret = new SortedList();

			lock (this)
			{
				foreach (RangeReading rr in rangeReadings)
				{
					if (rr.angleRaw == angleRaw)
					{
						ret.Add(rr.timestamp, rr);
					}
				}
			}

			return ret;
		}

		private void purge()
		{
			long timeToForget = DateTime.Now.Ticks - intervalToRememberS * 10000000;

			int i = 0;
			angles.Clear();

			while (i < rangeReadings.Count)
			{
				RangeReading rrCurr = (RangeReading)rangeReadings[i];
				if (rrCurr.timestamp < timeToForget)
				{
					rangeReadings.RemoveAt(i);
				}
				else
				{
					if (!angles.Contains(rrCurr.angleRaw))
					{
						angles.Add(rrCurr.angleRaw, rrCurr.angleRaw);
					}
					i++;
				}
			}
		}
	}
}
