using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    internal class Lbl
    {
        public float lx;
        public float ly;
        public string label;
        public Brush brush;
    }

    internal class StatusGraphics : IDisposable
    {
        public const int imageWidth = 600;                  // for all three images
        public const int imageHeight = imageWidth / 2;      // height is always half of it
        public const int xCenter = imageWidth / 2;
        public const int yCenter = imageHeight;
        public const int extraHeight = 100;                 // extra space below on the top view shows state of proximity sensors

        // Ultrasonic sensor reaches to about 3.5 meters; we scale the height of our display to this range:
        public const double maxExpectedRange = 4000.0d;  // mm
        public const double scale = imageHeight / maxExpectedRange;

        // bitmaps can be used any time before first retrieval of streams:

        bool disposed = false;
        public Bitmap northBmp = new Bitmap(imageWidth, imageHeight + extraHeight);
        public Bitmap compositeBmp = new Bitmap(imageWidth, imageHeight);
        public Bitmap statusBmp = new Bitmap(imageWidth, imageHeight);       // also used to draw South part of map

        // have a history for all decisions:
        public static History HistoryDecisions = new History();

        // we provide one font for all labels:
        public static Font fontBmp = new Font(FontFamily.GenericSansSerif, 10, GraphicsUnit.Pixel);
        public static Font fontBmpL = new Font(FontFamily.GenericSansSerif, 14, GraphicsUnit.Pixel);

        public MemoryStream northMemory
        {
            get
            {
                if (disposed) return null;
                MemoryStream memory = new MemoryStream();
                // bitmap writing (drawing) happens inside similar lock. We make sure that drawing has finished:
                lock (northBmp)
                {
                    northBmp.Save(memory, ImageFormat.Jpeg);
                }
                memory.Position = 0;
                return memory;
            }
        }

        public MemoryStream compositeMemory
        {
            get
            {
                if (disposed) return null;
                MemoryStream memory = new MemoryStream();
                // bitmap writing (drawing) happens inside similar lock. We make sure that drawing has finished:
                lock (compositeBmp)
                {
                    compositeBmp.Save(memory, ImageFormat.Jpeg);
                }
                memory.Position = 0;
                return memory;
            }
        }

        public MemoryStream statusMemory
        {
            get
            {
                if (disposed) return null;
                MemoryStream memory = new MemoryStream();
                // bitmap writing (drawing) happens inside similar lock. We make sure that drawing has finished:
                lock (statusBmp)
                {
                    statusBmp.Save(memory, ImageFormat.Jpeg);
                }
                memory.Position = 0;
                return memory;
            }
        }


        public MemoryStream historySaidMemory
        {
            get
            {
                if (disposed) return null;

                MemoryStream memory = new MemoryStream();

                byte[] data = Talker.HistorySaid.getBytes();

                memory.Write(data, 0, data.Length);
                memory.Position = 0;
                return memory;
            }
        }

        public MemoryStream historyDecisionsMemory
        {
            get
            {
                if (disposed) return null;

                MemoryStream memory = new MemoryStream();

                byte[] data = HistoryDecisions.getBytes();

                memory.Write(data, 0, data.Length);
                memory.Position = 0;
                return memory;
            }
        }

        public void Dispose()
        {
            disposed = true;

            if (northBmp != null)
            {
                northBmp.Dispose();
            }

            if (compositeBmp != null)
            {
                compositeBmp.Dispose();
            }

            if (statusBmp != null)
            {
                statusBmp.Dispose();
            }
        }
    }
}
