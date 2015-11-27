using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLidarLiteTest
{
    /// <summary>
    /// Interaction logic for LidarViewControl.xaml
    /// </summary>
    public partial class LidarViewControl : UserControl
    {
        public LidarViewControl()
        {
            InitializeComponent();
        }
        public double rangeMaxValueM = 50d;
        public double angleMinValue = 0.0d;
        public double angleMaxValue = 180.0d;
        public double ScaleStartAngle = 180.0d;
        public double ScaleSweepAngle = 180.0d;
        private int animatingSpeedFactor = 2;
        private bool isInitialValueSet = false;

        public int numRays { get; private set; }
        private double angleDegreesCurr = 0.0d;
        private long timestampLastReading = 0L;

        private LidarData _lidarData = new LidarData();

        public LidarData lidarData { get { return _lidarData; } }

        public bool flipped = false;

        private bool pointerVisible = true;

        /// <summary>
        /// Sets the Current LaserData value - a way to set whole scan area in one call
        /// </summary>
        public LaserDataSerializable CurrentLaserData
        {
            set {

                numRays = value.DistanceMeasurements.Length;

                StringBuilder psb = new StringBuilder();

                // the "Path" coordinates of the center of ray rotation: 
                int centerX = (int)Math.Round(ScannedArea.ActualWidth / 2.0d);
                int centerY = (int)ScannedArea.ActualHeight;

                psb.AppendFormat("M{0},{1} ", centerX, centerY);

                int i = 0;
                foreach (int dm in value.DistanceMeasurements)
                {
                    double rangeMeters = dm / 100.0d;  // dm in centimeters
                    // double rangeMeters = 3.0d;       // DEBUG - calibrate

                    // 0 = full range,  centerY = zero range
                    double lineLength = rangeMeters * ScannedArea.ActualWidth / (rangeMaxValueM * 2.0d);
                    double angleDegrees = (angleMaxValue - angleMinValue) / 2.0d - i * 180.0d / numRays;
                    double angleRads = angleDegrees * Math.PI / 180.0d;

                    int x1 = (int)Math.Round(lineLength * Math.Sin(angleRads)) + centerX;
                    int y1 = -(int)Math.Round(lineLength * Math.Cos(angleRads)) + centerY;

                    // Tracer.Trace("angle: " + angleDegrees + "/" + angleRads + " x=" + x1 + " y=" + y1);

                    psb.AppendFormat("L{0},{1} ", x1, y1);

                    i++;
                }

                psb.Append("z");

                // Tracer.Trace(psb.ToString());

                PathFigureCollectionConverter fcvt = new PathFigureCollectionConverter();
                PathFigureCollection figures = (PathFigureCollection)fcvt.ConvertFromString(psb.ToString());    //"M200,200 L100,10 L100,10 L50,100 z");
                ScannedArea.Data = new PathGeometry(figures);

                drawReferenceCircles();
            }
        }

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(RangeReading), typeof(LidarViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(LidarViewControl.OnCurrentValuePropertyChanged)));


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

        /// <summary>
        /// Gets/Sets the pointer visibility
        /// </summary>
        public bool PointerVisible
        {
            get
            {
                return pointerVisible;
            }
            set
            {
                pointerVisible = value;

                BeamPointer.Visibility = pointerVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }


        #region Methods

        public void Reset()
        {
            _lidarData = new LidarData();
        }

        private static void OnCurrentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of LidarViewControl whose property value changed
            LidarViewControl gauge = d as LidarViewControl;
            gauge.OnCurrentValueChanged(e);

        }

        public virtual void OnCurrentValueChanged(DependencyPropertyChangedEventArgs e)
        {
            RangeReading rrNewValue = (RangeReading)e.NewValue;
            RangeReading rrOldValue = (RangeReading)e.OldValue;

            //Validate and set the new value
            if (rrNewValue.angleDegrees > this.angleMaxValue)
            {
                rrNewValue.angleDegrees = (int)this.angleMaxValue;
            }
            else if (rrNewValue.angleDegrees < this.angleMinValue)
            {
                rrNewValue.angleDegrees = (int)this.angleMinValue;
            }

            if (rrOldValue != null)
            {
                if (rrOldValue.angleDegrees > this.angleMaxValue)
                {
                    rrOldValue.angleDegrees = (int)this.angleMaxValue;
                }
                else if (rrOldValue.angleDegrees < this.angleMinValue)
                {
                    rrOldValue.angleDegrees = (int)this.angleMinValue;
                }
            }

            //Tracer.Trace("sonar: " + rrNewValue.angleDegrees + "   " + rrNewValue.rangeMeters);

            angleDegreesCurr = rrNewValue.angleDegrees;
            timestampLastReading = rrNewValue.timestamp;

            _lidarData.addRangeReading(rrNewValue);

            //Tracer.Trace("angles: " + _lidarData.angles.Count);

            numRays = _lidarData.angles.Count;

            if (pointerVisible)
            {
                double db1 = 0;
                Double oldcurr_realworldunit = 0;
                Double newcurr_realworldunit = 0;
                Double realworldunit = (ScaleSweepAngle / (angleMaxValue - angleMinValue));

                if (rrOldValue != null)
                {
                    //Resetting the old value to min value the very first time.
                    if (rrOldValue.angleDegrees == 0 && !isInitialValueSet)
                    {
                        rrOldValue.angleDegrees = (int)angleMinValue;
                        isInitialValueSet = true;

                    }
                    if (rrOldValue.angleDegrees < 0)
                    {
                        db1 = angleMinValue + Math.Abs(rrOldValue.angleDegrees);
                        oldcurr_realworldunit = ((double)(Math.Abs(db1 * realworldunit)));
                    }
                    else
                    {
                        db1 = Math.Abs(angleMinValue) + rrOldValue.angleDegrees;
                        oldcurr_realworldunit = ((double)(db1 * realworldunit));
                    }
                }

                if (rrNewValue.angleDegrees < 0)
                {
                    db1 = angleMinValue + Math.Abs(rrNewValue.angleDegrees);
                    newcurr_realworldunit = ((double)(Math.Abs(db1 * realworldunit)));
                }
                else
                {
                    db1 = Math.Abs(angleMinValue) + rrNewValue.angleDegrees;
                    newcurr_realworldunit = ((double)(db1 * realworldunit));
                }

                double oldcurrentvalueAngle = (ScaleStartAngle + oldcurr_realworldunit);
                double newcurrentvalueAngle = (ScaleStartAngle + newcurr_realworldunit);

                double newcurrentvalueScale = rrNewValue.rangeMeters * 2.0d / rangeMaxValueM;

                //Animate the pointer from the old value to the new value
                //MovePointer(newcurrentvalueAngle);
                AnimatePointer(oldcurrentvalueAngle, newcurrentvalueAngle, newcurrentvalueScale);
            }
            else
            {
                drawScannedArea();
            }
        }

        /// <summary>
        /// Animates the pointer to the current value to the new one
        /// </summary>
        /// <param name="oldcurrentvalueAngle"></param>
        /// <param name="newcurrentvalueAngle"></param>
        void AnimatePointer(double oldcurrentvalueAngle, double newcurrentvalueAngle, double newcurrentvalueScale)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = oldcurrentvalueAngle;
            da.To = newcurrentvalueAngle;

            _newcurrentvalueScale = newcurrentvalueScale;

            double animDuration = Math.Abs(oldcurrentvalueAngle - newcurrentvalueAngle) * animatingSpeedFactor;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(animDuration));

            Storyboard sb = new Storyboard();
            sb.Completed += new EventHandler(sb_Completed);
            sb.Children.Add(da);
            Storyboard.SetTarget(da, BeamPointer);

            object tg = BeamPointer.RenderTransform;

            if (tg is System.Windows.Media.RotateTransform)
            {
                Storyboard.SetTargetProperty(da, new PropertyPath("(Path.RenderTransform).(Angle)"));
            }
            else
            {
                Storyboard.SetTargetProperty(da, new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[1].(RotateTransform.Angle)"));
            }

            if (newcurrentvalueAngle != oldcurrentvalueAngle)
            {
                sb.Begin();
            }
        }

        /// <summary>
        /// Move pointer without animating
        /// </summary>
        /// <param name="angleValue"></param>
        void MovePointer(double angleValue, double newcurrentvalueScale)
        {
            _newcurrentvalueScale = newcurrentvalueScale;

            RotateTransform rt;

            object tg = BeamPointer.RenderTransform;

            if (tg is System.Windows.Media.RotateTransform)
            {
                rt = (RotateTransform)tg;
            }
            else
            {
                rt = ((TransformGroup)tg).Children[1] as RotateTransform;
                ((ScaleTransform)((TransformGroup)BeamPointer.RenderTransform).Children[0]).ScaleX = newcurrentvalueScale;
            }
            rt.Angle = angleValue;
        }

        private double _newcurrentvalueScale = 2.0d;

        /// <summary>
        /// Called after the pointer completes animating - redraw the scanned area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sb_Completed(object sender, EventArgs e)
        {
            ((ScaleTransform)((TransformGroup)BeamPointer.RenderTransform).Children[0]).ScaleX = _newcurrentvalueScale;

            drawScannedArea();
        }

        /// <summary>
        /// using previously accumulated LidarData, draws the boundaries of the scanned area.
        /// </summary>
        private void drawScannedArea()
        {
   			if (numRays >= 2)
			{
                StringBuilder psb = new StringBuilder();

                // the "Path" coordinates of the center of ray rotation: 
                int centerX = (int)Math.Round(ScannedArea.ActualWidth / 2.0d);
                int centerY = (int)ScannedArea.ActualHeight;

                psb.AppendFormat("M{0},{1} ", centerX, centerY);

                foreach (int angleRaw in _lidarData.angles.Keys)
                {
                    RangeReading reading = _lidarData.getLatestReadingAt(angleRaw);

                    if (reading != null)
                    {
                        // 0 = full range,  centerY = zero range
                        double lineLength = reading.rangeMeters * ScannedArea.ActualHeight / rangeMaxValueM;
                        double angleDegrees = reading.angleDegrees - (angleMaxValue - angleMinValue)/2.0d;
                        double angleRads = angleDegrees * Math.PI / 180.0d;

                        int x1 = (int)Math.Round(lineLength * Math.Sin(angleRads)) + centerX;
                        int y1 = -(int)Math.Round(lineLength * Math.Cos(angleRads)) + centerY;

                        // Tracer.Trace("angle: " + angleDegrees + "/" + angleRads + " x=" + x1 + " y=" + y1);

                        psb.AppendFormat("L{0},{1} ", x1, y1);
                    }
                }

                psb.Append("z");

                // Tracer.Trace(psb.ToString());

                PathFigureCollectionConverter fcvt = new PathFigureCollectionConverter();
                PathFigureCollection figures = (PathFigureCollection)fcvt.ConvertFromString(psb.ToString());    //"M200,200 L100,10 L100,10 L50,100 z");
                ScannedArea.Data = new PathGeometry(figures);

                drawReferenceCircles();
            }                                
        }

        private static double[] circleDistances = new double[] { 1.0d, 2.0d, 3.0d };

        private void drawReferenceCircles()
        {
            int i = 0;
            string controlNameBase = "ReferenceCircle";
            string name = controlNameBase + i;

            // do it once, only after the control has been loaded:
            if (MainGrid.IsLoaded && (MainGrid.FindName(name) as Ellipse) == null)
            {
                foreach (double radius in circleDistances)
                {
                    // keep in mind that ScannedArea is a bit raised over what we consider the robot center.

                    // radius = MapperSettings.referenceCircleRadiusMeters;
                    double diamW = radius * ScannedArea.ActualWidth / rangeMaxValueM;
                    double diamH = radius * ScannedArea.ActualHeight * 2.0d / rangeMaxValueM;

                    // draw reference circle (usually 2 meters):
                    Ellipse circle = new Ellipse()
                    {
                        Name = controlNameBase + i,
                        Width = diamW,
                        Height = diamH,
                        Margin = new Thickness((MainGrid.ActualWidth - diamW) / 2.0d, (MainGrid.ActualHeight * 2.0d - diamH) / 2.0d, 0, 0),
                        Stroke = Brushes.Cyan,
                        StrokeThickness = 1
                    };
                    MainGrid.Children.Add(circle);
                    MainGrid.RegisterName(circle.Name, circle);
                    i++;
                }
            }
        }

        #endregion // Methods
    }
}
