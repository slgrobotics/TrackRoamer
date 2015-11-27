using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLidarLiteTest
{
    public class LidarData
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
}
