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
    /// Interaction logic for ProximityViewControl.xaml
    /// </summary>
    public partial class ProximityViewControl : UserControl
    {
        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(ProximityData), typeof(ProximityViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(ProximityViewControl.OnCurrentValuePropertyChanged)));


        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public ProximityData CurrentValue
        {
            get
            {
                return (ProximityData)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        private ScaleTransform[] beamsSt = new ScaleTransform[8];
        // angles are related to the X axis (pointing to the right):
        //private static double[] angles = { -67.5d, -22.5d, 22.5d, 67.5d, 112.5d, 157.5d, 202.5d, 247.5d };    //evenly spread angle = -90.0d + 360.0d / 8.0d * (i + 0.5d);
        private static double[] angles = { -57.5d, -32.5d, 32.5d, 57.5d, 122.5d, 147.5d, 212.5d, 237.5d };

        public ProximityViewControl()
        {
            InitializeComponent();

            double scale = 1.0;

            for (int i = 0; i < 8; i++)
            {
                //double angle = -90.0d + 360.0d / 8.0d * (i + 0.5d);
                double angle = angles[i];

                // http://www.comanswer.com/question/silverlight-xamlwriter

                var dt = (DataTemplate)Resources["TemplateXaml"];
                Path pointer = (Path)dt.LoadContent();

                ScaleTransform st = new ScaleTransform(scale, 1.0d);
                beamsSt[i] = st;
                RotateTransform rt = new RotateTransform(angle, 0, 10);
                TranslateTransform tt = new TranslateTransform(50.0d, 0.0d);   // half the length of the beam
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
            //Get access to the instance of ProximityViewControl whose property value changed
            ProximityViewControl gauge = d as ProximityViewControl;
            gauge.OnCurrentValueChanged(e);
        }

        public virtual void OnCurrentValueChanged(DependencyPropertyChangedEventArgs e)
        {
            ProximityData newValue = (ProximityData)e.NewValue;

            beamsSt[0].ScaleX = mToScale(newValue.mffr);
            beamsSt[1].ScaleX = mToScale(newValue.mfr);
            beamsSt[2].ScaleX = mToScale(newValue.mbr);
            beamsSt[3].ScaleX = mToScale(newValue.mbbr);
            beamsSt[4].ScaleX = mToScale(newValue.mbbl);
            beamsSt[5].ScaleX = mToScale(newValue.mbl);
            beamsSt[6].ScaleX = mToScale(newValue.mfl);
            beamsSt[7].ScaleX = mToScale(newValue.mffl);
        }

        /// <summary>
        /// meters to scale converter for IR Proximity Sensors
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
