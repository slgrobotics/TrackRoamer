using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Globalization;

using TrackRoamer.Robotics.Utility.LibPicSensors;

namespace TrackRoamer.Robotics.LibGuiWpf
{

    /*
     * To connect a slider to SweepViewControl (or SonarViewControl), use the RangeReadingConverter as follows:
     * 
     *      xmlns:local1="clr-namespace:TrackRoamer.Robotics.WpfProximityModuleDemo"
     * 
            <Window.Resources>
                <local1:RangeReadingConverter x:Key="rangeReadingConverter"/>
            </Window.Resources>
     * 
            <my1:SweepViewControl HorizontalAlignment="Left" Margin="24,261,0,0" x:Name="sweepViewControl1" VerticalAlignment="Top" CurrentValue="{Binding ElementName=sweepSlider, Path=Value, Converter={StaticResource rangeReadingConverter}}" />

     */
    public class RangeReadingConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            double dblVal = (double)value;

            RangeReading rr = new RangeReading((int)dblVal, 3.0d, -1L);

            rr.angleDegrees = dblVal;

            return rr;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

