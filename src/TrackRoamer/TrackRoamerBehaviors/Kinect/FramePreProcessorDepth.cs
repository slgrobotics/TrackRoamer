using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    partial class FramePreProcessor
    {
        public IEnumerator<ITask> ProcessDepthFrame()
        {
            yield break;
        }
    }
}
