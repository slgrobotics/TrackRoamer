using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    using System.IO;
    using System.Windows;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Microsoft.Ccr.Core;
    using Microsoft.Kinect;
    using Microsoft.Robotics;
    using ccr = Microsoft.Ccr.Core;
    using common = Microsoft.Robotics.Common;
    using kinect = Microsoft.Robotics.Services.Sensors.Kinect;
    using kinectProxy = Microsoft.Robotics.Services.Sensors.Kinect.Proxy;
    using pm = Microsoft.Robotics.PhysicalModel;

    using Emgu.CV;
    using Emgu.CV.Structure;
    using Emgu.CV.UI;
    using Emgu.CV.CvEnum;
    using Emgu.CV.VideoSurveillance;
    using Emgu.Util;

    using System.Drawing;


    /*
     * Note: install   libemgucv-windows-x86-2.3.0.1416.exe   to C:\Microsoft Robotics Dev Studio 4\libemgucv\.
     * Copy all *.dll files from  C:\Microsoft Robotics Dev Studio 4\libemgucv\bin   to  C:\Microsoft Robotics Dev Studio 4\bin\.
     * 
     * useful links:
     * 
     * http://opencv.willowgarage.com/wiki/
     * http://siddhantahuja.wordpress.com/2011/07/18/getting-started-with-opencv-2-3-in-microsoft-visual-studio-2010-in-windows-7-64-bit/  - blog about installing OpenCV-2
     * install it in C:\Program Files\  - run as Administrator; will create opencv folder there
     * http://blog.martinperis.com/2010/12/microsoft-robotics-developer-studio.html  - blog about MRDS and OpenCV-2
     * http://www.emgu.com/wiki/index.php/Download_And_Installation    - EMGU OpenCV-2 wrapper - installed in C:\Microsoft Robotics Dev Studio 4\libemgucv; DLLs copied to MRDS bin.
     * http://sourceforge.net/projects/opencvlibrary/files/opencv-win/2.3.1/     - OpenCV-2 library - installed in C:\Program Files\opencv  (not needed, as Emgu contains its DLLs))
     * http://rdscv.codeplex.com/    - OpenCV / Emgu services for Robotics Developer Studio - installed in C:\Microsoft Robotics Dev Studio 4\samples\rdscv-3102
     */


    public partial class FramePreProcessor
    {
        //private const double fontScale = 0.5d;
        //private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, fontScale, fontScale);

        private BlobTrackerAuto<Bgr> _tracker = null;
        private IBGFGDetector<Bgr> _detector = null;

        public VideoSurveillanceDecider videoSurveillanceDecider = null;    // collects all info about targets in the camera view.

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;

        public Bitmap BitmapProcessed;

        int bubbleOuterRadiusMm = 2000;
        int bubbleThicknessMm = 500;
        bool markAsGreen = true;

        // R component (2) must be greater than B (0) and G (1); the colorFactor is a multiplier in a formula.
        // 0.8 is a good starting point; lesser values allow more colors to be selected; greater values make filter more selective.
        double colorFactor = 0.85d;

        double colorTresholdMain = 40.0d;       // initial value, will be adjusted
        double averageBrightness = 128.0d;      // initial value, will be adjusted

        //double colorTreshold = 2.0d;

        public bool doSurveillance = false;
        public bool doColorRecognition = true;

        public bool doSaveOneImage = false;
        public double currentPanKinect;         // must be set before calling ProcessImageFrame() and other processing methods
        public double currentTiltKinect;

        public IEnumerator<ITask> ProcessImageFrame()
        {
            DateTime started = DateTime.Now;

            // default RGB image size is 640 x 480
            // setting other value (i.e. 320x240) in ...\TrackRoamer\TrackRoamerServices\Config\TrackRoamer.TrackRoamerBot.Kinect.Config.xml does not seem to work (causes NUI initialization failure).

            byte[] srcImageBits = this.RawFrames.RawColorFrameData;
            int srcImageBytesPerPixel = this.RawFrames.RawColorFrameInfo.BytesPerPixel;
            int srcImageWidth = this.RawFrames.RawColorFrameInfo.Width;
            int srcImageHeight = this.RawFrames.RawColorFrameInfo.Height;

            //if (ProcessedImageWidth != this.RawFrames.RawImageFrameData.Image.Width || ProcessedImageHeight != this.RawFrames.RawImageFrameData.Image.Height)
            //{
            //    ProcessedImageWidth = this.RawFrames.RawImageFrameData.Image.Width;
            //    ProcessedImageHeight = this.RawFrames.RawImageFrameData.Image.Height;

            //    ImageBitsProcessed = new byte[ProcessedImageWidth * ProcessedImageHeight * 4];
            //}

            // we need to convert Kinect/MRDS service Image to OpenCV Image - that takes converting first to a BitmapSource and then to System.Drawing.Bitmap:
            BitmapSource srcBitmapSource = BitmapSource.Create(srcImageWidth, srcImageHeight, 96, 96, PixelFormats.Bgr32, null, srcImageBits, srcImageWidth * srcImageBytesPerPixel);

            if (doSaveOneImage)
            {
                doSaveOneImage = false;

                SaveBitmapSource(srcBitmapSource);
            }

            Image<Bgr, byte> img = new Image<Bgr, byte>(BitmapSourceToBitmap(srcBitmapSource));
            Image<Gray, byte> gimg = null;
            Image<Bgr, byte> filtered = null;

            img._SmoothGaussian(11); //filter out noises

            // from here we can operate OpenCV / Emgu Image, at the end converting Image to BitmapProcessed:

            if (videoSurveillanceDecider == null)
            {
                videoSurveillanceDecider = new VideoSurveillanceDecider(img.Width, img.Height);
            }

            videoSurveillanceDecider.Init();

            if (doColorRecognition)
            {
                // color detection (T-shirt, cone...):

                //lock (videoSurveillanceDecider)
                //{
                //    videoSurveillanceDecider.purgeColorBlobs();
                //}

                filtered = img.Clone().SmoothBlur(13, 13);       //.SmoothGaussian(9);

                byte[, ,] data = filtered.Data;
                int nRows = filtered.Rows;
                int nCols = filtered.Cols;
                double averageBrightnessTmp = 0.0d;

                colorTresholdMain = averageBrightness / 2.0d;
                double colorFactorMain = 256.0d * colorFactor / averageBrightness;

                /*
                */
                // leave only pixels with distinct red color in the "filtered":
                for (int i = nRows - 1; i >= 0; i--)
                {
                    for (int j = nCols - 1; j >= 0; j--)
                    {
                        // R component (2) must be greater than B (0) and G (1) by the colorFactor; dark areas are excluded:
                        double compR = data[i, j, 2];
                        double compG = data[i, j, 1];
                        double compB = data[i, j, 0];

                        double compSum = compR + compG + compB;         // brightness
                        averageBrightnessTmp += compSum;

                        if (compR > colorTresholdMain) //&& compG > colorTreshold && compB > colorTreshold)
                        {
                            compR = (compR / compSum) / colorFactorMain;    // adjusted for brightness
                            compG = compG / compSum;
                            compB = compB / compSum;
                            if (compR > compG && compR > compB)
                            {
                                data[i, j, 0] = data[i, j, 1] = 0;    // B, G
                                data[i, j, 2] = 255;                  // R
                            }
                            else
                            {
                                data[i, j, 0] = data[i, j, 1] = data[i, j, 2] = 0;
                            }
                        }
                        else
                        {
                            // too dark.
                            data[i, j, 0] = data[i, j, 1] = data[i, j, 2] = 0;
                        }
                    }
                }

                averageBrightness = averageBrightnessTmp / (nRows * nCols * 3.0d);  // save it for the next cycle

                gimg = filtered.Split()[2];   // make a grey image out of the Red channel, supposedly containing all red objects.

                // contour detection:

                int areaTreshold = 300;     // do not consider red contours with area in pixels less than areaTreshold.

                Contour<System.Drawing.Point> contours;
                MemStorage store = new MemStorage();

                // make a linked list of contours from the red spots on the screen:
                contours = gimg.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, store);
                CvInvoke.cvZero(gimg.Ptr);

                if (contours != null)
                {
                    CvInvoke.cvDrawContours(img.Ptr, contours.Ptr, new MCvScalar(255, 0, 0), new MCvScalar(255, 255, 255), 2, 2, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, System.Drawing.Point.Empty);

                    List<ContourContainer> contourContainers = new List<ContourContainer>();

                    for (; contours != null; contours = contours.HNext)
                    {
                        contours.ApproxPoly(contours.Perimeter * 0.02, 0, contours.Storage);
                        if (contours.Area > areaTreshold)
                        {
                            contourContainers.Add(new ContourContainer() { contour = contours });

                            //int centerX = contours.BoundingRectangle.X + contours.BoundingRectangle.Width / 2;
                            //int centerY = contours.BoundingRectangle.Y + contours.BoundingRectangle.Height / 2;
                            //img.Draw(contours.BoundingRectangle, new Bgr(64.0, 64.0, 255.0), 2);
                            //img.Draw(Math.Round((double)(((int)contours.Area) / 100) * 100).ToString(), ref _font, new System.Drawing.Point(centerX, centerY), new Bgr(64.0, 64.0, 255.0));
                        }
                    }

                    // for the VideoSurveillanceDecider to work, we need to supply blobs IDs - generate them as numbers in a size-ordered list, offset by 1000:
                    var ccs = from cc in contourContainers
                              orderby cc.contour.Area descending
                              select cc;

                    int ccId = 0;
                    int goodCounter = 0;
                    lock (videoSurveillanceDecider)
                    {
                        videoSurveillanceDecider.purgeColorBlobs();

                        foreach (ContourContainer cc in ccs)
                        {
                            cc.ID = 1000 + ccId;      // offset not to overlap with VideoSurveillance-generated blobs
                            VideoSurveillanceTarget target = videoSurveillanceDecider.Update(cc, currentPanKinect, currentTiltKinect);
                            ccId++;
                            if (target != null && target.Rank > 1.0d)
                            {
                                goodCounter++;
                                if (goodCounter > 10000)  // take 10 largest good ones
                                {
                                    break;
                                }
                            }

                        }

                        if (!doSurveillance)
                        {
                            videoSurveillanceDecider.Commit();
                            videoSurveillanceDecider.ComputeMainColorTarget();
                            videoSurveillanceDecider.Draw(img);             // must run under lock
                        }
                    }
                }
            }

            if (doSurveillance)
            {
                // blob detection by Emgu.CV.VideoSurveillance:

                if (_tracker == null)
                {
                    _tracker = new BlobTrackerAuto<Bgr>();
                    _detector = new FGDetector<Bgr>(FORGROUND_DETECTOR_TYPE.FGD);
                }

                Image<Bgr, byte> imgSmall = img.Resize(0.5d, INTER.CV_INTER_NN);      // for the full image - _tracker.Process() fails to allocate 91Mb of memory

                #region use the BG/FG detector to find the forground mask
                _detector.Update(imgSmall);
                Image<Gray, byte> forgroundMask = _detector.ForgroundMask;
                #endregion

                _tracker.Process(imgSmall, forgroundMask);

                lock (videoSurveillanceDecider)
                {
                    videoSurveillanceDecider.PurgeAndCommit();      // make sure that obsolete Surveillance targets are removed

                    foreach (MCvBlob blob in _tracker)
                    {
                        // keep in mind that we were working on the scaled down (to 1/2 size) image. So all points should be multiplied by two.
                        VideoSurveillanceTarget target = videoSurveillanceDecider.Update(blob, currentPanKinect, currentTiltKinect);
                    }

                    videoSurveillanceDecider.ComputeMainColorTarget();
                    videoSurveillanceDecider.Draw(img);             // must run under lock
                }
            }

            Bgr color = new Bgr(0.0, 128.0, 128.0);

            // draw center vertical line:
            System.Drawing.Point[] pts = new System.Drawing.Point[2];
            pts[0] = new System.Drawing.Point(img.Width / 2, 0);
            pts[1] = new System.Drawing.Point(img.Width / 2, img.Height);

            img.DrawPolyline(pts, false, color, 1);

            // draw center horizontal line:
            pts[0] = new System.Drawing.Point(0, img.Height / 2);
            pts[1] = new System.Drawing.Point(img.Width, img.Height / 2);

            img.DrawPolyline(pts, false, color, 1);

            // draw a sighting frame for precise alignment:
            // Horisontally the frame spans 16.56 degrees on every side and 12.75 degrees either up or down (at 74" the size it covers is 44"W by 33.5"H, i.e. 33.12 degrees by 25.5 degrees)
            System.Drawing.Point[] pts1 = new System.Drawing.Point[5];
            pts1[0] = new System.Drawing.Point(img.Width / 4, img.Height / 4);
            pts1[1] = new System.Drawing.Point(img.Width * 3 / 4, img.Height / 4);
            pts1[2] = new System.Drawing.Point(img.Width * 3 / 4, img.Height * 3 / 4);
            pts1[3] = new System.Drawing.Point(img.Width / 4, img.Height * 3 / 4);
            pts1[4] = new System.Drawing.Point(img.Width / 4, img.Height / 4);

            img.DrawPolyline(pts1, false, color, 1);

            // end of OpenCV / Emgu Image processing, converting the Image to BitmapProcessed:

            BitmapProcessed = img.ToBitmap();     // image with all processing marked
            //BitmapProcessed = filtered.ToBitmap();  // red image out of the Red channel
            //BitmapProcessed = gimg.ToBitmap();      // grey image; is CvZero'ed by this point
            //BitmapProcessed = forgroundMask.ToBitmap();

            //Tracer.Trace("Video processed in " + (DateTime.Now - started).TotalMilliseconds + " ms");       // usually 40...70ms

            yield break;
        }

        #region ToBitmapSource() and other helpers

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        // see http://rwlodarcmsdnblog.codeplex.com/  and   http://www.emgu.com/wiki/index.php/WPF_in_CSharp

        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(hBitmap); //release the HBitmap

            return bitmapSource;
        }

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource for WPF</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap bitmap = image.Bitmap)
            {
                IntPtr hBitmap = bitmap.GetHbitmap();

                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(hBitmap); //release the HBitmap

                return bitmapSource;
            }
        }

        // see http://wrb.home.xs4all.nl/Articles_2010/Article_WPFBitmapConverter_01.htm

        private System.Drawing.Bitmap BitmapSourceToBitmap(BitmapSource bitmap, string imgType = ".bmp")
        // ref string sInfo)
        {
            BitmapEncoder enc = null;
            System.Drawing.Bitmap oB_cnv = null;
            System.Drawing.Bitmap oB_out = null;

            using (MemoryStream ms = new MemoryStream())
            {
                switch (imgType)
                {
                    case ".bmp":
                        enc = new BmpBitmapEncoder();
                        break;
                    case ".gif":
                        enc = new GifBitmapEncoder();
                        break;
                    case ".jpg":
                        enc = new JpegBitmapEncoder();
                        break;
                    case ".png":
                        enc = new PngBitmapEncoder();
                        break;
                    case ".tif":
                        enc = new TiffBitmapEncoder();
                        break;
                }
                enc.Frames.Add(BitmapFrame.Create(bitmap));
                enc.Save(ms);

                // MemoryStream to System.Drawing.Bitmap.
                oB_cnv = new System.Drawing.Bitmap(ms);

                // Dereference.
                oB_out = new System.Drawing.Bitmap(oB_cnv);

                // Test effect of BitmapEncoder type.
                // Remove this later, and parameter sInfo.
                // -----------------------------------
                //byte[] bytData = ms.GetBuffer();
                //string s = "";
                //s += "PixelFormat: " + oB_cnv.PixelFormat.ToString() + "\r\n";
                //s += "Size [bytes]: " + bytData.Length + "\r\n";
                //s += "Width: " + oB_cnv.Width.ToString() + "\r\n";
                //s += "Height: " + oB_cnv.Height.ToString();
                //sInfo = s;
                //// -----------------------------------
            }
            return oB_out;
        }

        private string imagesFolder = null;

        private void SaveBitmapSource(BitmapSource image)
        {
            if (imagesFolder == null)
            {
                imagesFolder = string.Format("{0}\\photos_{1:yyyyMMdd}", Project.LogPath, DateTime.Now);

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }
            }

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //Tracer.Trace("Codec Author is: " + encoder.CodecInfo.Author);

            encoder.FlipHorizontal = true;
            encoder.FlipVertical = false;
            encoder.QualityLevel = 75;
            //encoder.Rotation = Rotation.Rotate90;

            encoder.Frames.Add(BitmapFrame.Create(image));

            string imageName = System.IO.Path.Combine(imagesFolder, string.Format("image_{0}.jpg", DateTime.Now.Ticks));

            using (FileStream file = File.OpenWrite(imageName))
            {
                encoder.Save(file);
            }
        }

        #endregion // ToBitmapSource() and other helpers
    }
}
