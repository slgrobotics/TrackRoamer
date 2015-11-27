using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for SweepViewControl.xaml
    /// </summary>
    public partial class SweepViewControl : UserControl
    {
        /// <summary>
        /// Sets the Current LaserData value - a way to set whole scan area in one call
        /// </summary>
        public LaserDataSerializable CurrentLaserData
        {
            set {
                sonarViewControl1.CurrentLaserData = value;

                pmBearingLabel.Content = "";

                pmRangeLabel.Content = "";

                pmNraysLabel.Content = String.Format("{0} rays", sonarViewControl1.numRays);
            }
        }

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(RangeReading), typeof(SweepViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SweepViewControl.OnCurrentValuePropertyChanged)));


        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public RangeReading CurrentValue
        {
            get
            {
                return (RangeReading)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        public SonarData sonarData { get { return sonarViewControl1.sonarData; } }

        public bool PointerVisible
        {
            get { return sonarViewControl1.PointerVisible; }
            set { sonarViewControl1.PointerVisible = value; }
        }

        public string Heading { set { groupBox1.Header = value; } }



        public SweepViewControl()
        {
            InitializeComponent();
        }

#region Methods

        private static void OnCurrentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of SonarViewControl whose property value changed
            SweepViewControl gauge = d as SweepViewControl;
            gauge.OnCurrentValueChanged(e);

        }

        public virtual void OnCurrentValueChanged(DependencyPropertyChangedEventArgs e)
        {
            RangeReading newValue = (RangeReading)e.NewValue;

            //sonarViewControl1.SetCurrentValue(e.Property, e.NewValue);

            sonarViewControl1.CurrentValue = newValue;

            pmBearingLabel.Content = String.Format("{0}/{1}", newValue.angleRaw, Math.Round(newValue.angleDegrees));

            pmRangeLabel.Content = String.Format("{0}m", Math.Round(newValue.rangeMeters, 2));

            pmNraysLabel.Content = String.Format("{0} rays", sonarViewControl1.numRays);
        }

#endregion // Methods
    }
}

