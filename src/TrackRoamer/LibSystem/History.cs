using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibSystem
{
    public class History
    {
        private readonly LimitedQueue<HistoryItem> Items;

        private byte[] bytes;

        public History() : this(25)
        {
        }

        public History(int itemsToRemember)
        {
            Items = new LimitedQueue<HistoryItem>(29);
        }

        public void Record(HistoryItem record)
        {
            lock (this)
            {
                Items.Enqueue(record);
                bytes = null;
            }
        }

        public HistoryItem Peek()
        {
            HistoryItem hi = null; 
            lock (this)
            {
                if (Items.Count > 0)
                {
                    hi = Items.Last();
                }
            }
            return hi;
        }

        // see http://localhost:50000/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt for master page
        private const string historyHeader = @"<html>
                <link rel='stylesheet' type='text/css' href='/resources/dss/Microsoft.Dss.Runtime.Home.Styles.Common.css' />
                <style>
                    body
                    {
                        background-color: LightGray !important;
                    }
                    .historytbl table tr
                    {
                        width: 400px;
                    }
                    .historytbl td
                    {
                        min-width: 33px;
                        font-size: small;
                        padding-left: 3px;
                    }
                </style>
                <body>";
        private const string historyFooter = @"</body></html>";

        public byte[] getBytes()
        {
            byte[] ret;

            lock (this)
            {
                // uncomment the following line if you want the bytes to be buffered. Timestamps will not be updated though.
                //if (bytes == null)
                {
                    HistoryItem[] hist = Items.ToArray();

                    long timestampNow = DateTime.Now.Ticks;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(historyHeader);

                    if (hist.Length > 0)
                    {
                        sb.Append("<table class=\"historytbl\">");
                        for (int i = hist.Length - 1; i >= 0; i--)
                        {
                            HistoryItem curr = hist[i];
                            long timediff = (timestampNow - curr.timestamp) / 10000L;
                            double td = Math.Round(timediff / 1000.0d, 1);
                            int colorlevel = (10 - curr.level) * 25;
                            string colorTag = String.Format("#ff{0:x02}{0:x02}", colorlevel, colorlevel / 2);
                            string str = string.Format("<tr bgcolor=\"{0}\"><td>{1}</td><td align='center'>{2}</td><td width='100%'>{3}</td></tr>", colorTag, td, curr.level, curr.message);
                            sb.Append(str);
                        }
                        sb.Append("</table>");
                    }
                    else
                    {
                        sb.Append("<p>history is empty</p>");
                    }
                    sb.Append(historyFooter);

                    bytes = UTF8Encoding.UTF8.GetBytes(sb.ToString());
                }
                ret = (byte[])bytes.Clone();
            }
            return ret;
        }
    }

    public class HistoryItem
    {
        public long timestamp;
        public int level;
        public string message;
    }

    public class LimitedQueue<T> : Queue<T>
    {
        private int _limit = -1;

        public int Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        public LimitedQueue(int limit)
            : base(limit)
        {
            this.Limit = limit;
        }

        public new void Enqueue(T item)
        {
            if (this.Count >= this.Limit)
            {
                this.Dequeue();
            }
            base.Enqueue(item);
        }
    }
}
