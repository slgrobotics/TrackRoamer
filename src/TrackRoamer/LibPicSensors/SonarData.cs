using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    /// <summary>
    /// container for multiple readings of sonar, covering angular range and some time interval back
    /// </summary>
    public class SonarData
    {
        private const int intervalToRememberS = 10;
        private const int intervalToPurgeS = 5;

        private long lastPurgeTimestamp = 0L;

        public long TimeStamp = 0L;

        public SortedList<int, RangeReading> angles = new SortedList<int, RangeReading>();

        public void addRangeReading(RangeReading rr)
        {
            lock (angles)
            {
                if (angles.ContainsKey(rr.angleRaw))
                {
                    angles.Remove(rr.angleRaw);
                }
                angles.Add(rr.angleRaw, rr);
                purge();
            }
        }

		public void addRangeReading(int angleRaw, double rangeMeters, long timestamp)
		{
            RangeReading rr = new RangeReading(angleRaw, rangeMeters, timestamp);
            addRangeReading(rr);
		}

		public void addRangeReading(int angleRaw1, double rangeMeters1, int angleRaw2, double rangeMeters2, long timestamp)
		{
			lock (this)
			{
                RangeReading rr1 = new RangeReading(angleRaw1, rangeMeters1, timestamp);
                RangeReading rr2 = new RangeReading(angleRaw2, rangeMeters2, timestamp);

                if (angles.ContainsKey(rr1.angleRaw))
                {
                    angles.Remove(rr1.angleRaw);
                }
                angles.Add(rr1.angleRaw, rr1);

                if (angles.ContainsKey(rr2.angleRaw))
                {
                    angles.Remove(rr2.angleRaw);
                }
                angles.Add(rr2.angleRaw, rr2);

                TimeStamp = timestamp;

                purge();
			}
		}

        public void addRangeReading(RangeReading rr1, RangeReading rr2, long timestamp)
		{
			lock (this)
			{
                if (angles.ContainsKey(rr1.angleRaw))
                {
                    angles.Remove(rr1.angleRaw);
                }
                angles.Add(rr1.angleRaw, rr1);

                if (angles.ContainsKey(rr2.angleRaw))
                {
                    angles.Remove(rr2.angleRaw);
                }
                angles.Add(rr2.angleRaw, rr2);

                TimeStamp = timestamp;

                purge();
			}
		}

        public RangeReading getLatestReadingAt(int angleRaw)
        {
            RangeReading ret = null;

            lock (angles)
            {
                if (angles.ContainsKey(angleRaw))
                {
                    ret = angles[angleRaw];
                }
            }

            return ret;
        }

        private void purge()
        {
            long tNow = DateTime.Now.Ticks;

            if (tNow > lastPurgeTimestamp + intervalToPurgeS * TimeSpan.TicksPerSecond)
            {
                long timeToForget = tNow - intervalToRememberS * TimeSpan.TicksPerSecond;

                int[] keysToDelete = (from aa in angles.Values where aa.timestamp < timeToForget select aa.angleRaw).ToArray<int>();

                foreach (int key in keysToDelete)
                {
                    angles.Remove(key);
                }

                lastPurgeTimestamp = tNow;
            }
        }
    }

    /// <summary>
    /// serves as a bridge between SonarData and 
    /// </summary>
    [Serializable]
    public class LaserDataSerializable
    {
        public long TimeStamp = 0L;

        public int[] DistanceMeasurements;
    }
}
