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

using System.Xml;
//using System.Xaml;
using System.Windows.Markup;

using TrackRoamer.Robotics.Utility.LibPicSensors;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for ParkingSensorViewControl.xaml
    /// </summary>
    public partial class ParkingSensorViewControl : UserControl
    {
        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(ParkingSensorData), typeof(ParkingSensorViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(ParkingSensorViewControl.OnCurrentValuePropertyChanged)));


        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public ParkingSensorData CurrentValue
        {
            get
            {
                return (ParkingSensorData)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        private ScaleTransform[] beamsSt = new ScaleTransform[4];
        // angles are related to the X axis (pointing to the right):
        //private static double[] angles = { -45.0d, 45.0d, 135.0d, 225.0d };   // evenly spread angle = -90.0d + 360.0d / 4.0d * (i + 0.5d);
        private static double[] angles = { -75.0d, 75.0d, 105.0d, 255.0d };

        public ParkingSensorViewControl()
        {
            InitializeComponent();

            double scale = 1.0;

            for (int i = 0; i < 4; i++)
            {
                //double angle = -90.0d + 360.0d / 4.0d * (i + 0.5d);
                double angle = angles[i];

                // http://www.comanswer.com/question/silverlight-xamlwriter

                var dt = (DataTemplate)Resources["TemplateXaml"];
                Path pointer = (Path)dt.LoadContent();

                ScaleTransform st = new ScaleTransform(scale, 1.0d);
                beamsSt[i] = st;
                RotateTransform rt = new RotateTransform(angle, 0, 10);
                TranslateTransform tt = new TranslateTransform(50.0d, 0.0d);    // half the length of the beam
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(st);
                tg.Children.Add(rt);
                tg.Children.Add(tt);
                pointer.RenderTransform = tg;

                mainGrid.Children.Add(pointer);
            }
        }

        private FrameworkElement CloneControl(FrameworkElement e)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }

        #region Methods

        private static void OnCurrentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of ParkingSensorViewControl whose property value changed
            ParkingSensorViewControl gauge = d as ParkingSensorViewControl;
            gauge.OnCurrentValueChanged(e);
        }

        public virtual void OnCurrentValueChanged(DependencyPropertyChangedEventArgs e)
        {
            ParkingSensorData newValue = (ParkingSensorData)e.NewValue;

            beamsSt[0].ScaleX = mToScale(newValue.parkingSensorMetersRF);
            beamsSt[1].ScaleX = mToScale(newValue.parkingSensorMetersRB);
            beamsSt[2].ScaleX = mToScale(newValue.parkingSensorMetersLB);
            beamsSt[3].ScaleX = mToScale(newValue.parkingSensorMetersLF);
        }

        /// <summary>
        /// meters to scale converter for ParkingSensor Sensor
        /// </summary>
        /// <param name="m">distance in meters</param>
        /// <returns>scale</returns>
        private double mToScale(double m)
        {
            return m * 5.0d / 8.0d + 0.2d;
        }

        #endregion // Methods
    }
}
