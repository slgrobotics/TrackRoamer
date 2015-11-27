using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLidarLiteTest
{
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

