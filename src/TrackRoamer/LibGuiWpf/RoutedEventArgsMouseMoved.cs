using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

using TrackRoamer.Robotics.LibMapping;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    public class RoutedEventArgsMouseMoved : RoutedEventArgs
    {
        public double xMeters;
        public double yMeters;

        public GeoPosition geoPosition;

        public RoutedEventArgsMouseMoved(RoutedEvent e) : base(e)
        {
        }
    }
}
