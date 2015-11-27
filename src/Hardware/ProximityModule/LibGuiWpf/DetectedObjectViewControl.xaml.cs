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

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for DetectedObjectViewControl.xaml
    /// </summary>
    public partial class DetectedObjectViewControl : UserControl
    {
        public Color color { set { ellipse1.Stroke = new SolidColorBrush(value); } }

        public int size { set { ellipse1.Width = value; ellipse1.Height = value; ellipse1.Margin = new Thickness(-(value / 2 + 1) + 6, -(value / 2 + 1) + 6, 0, 0); } }

        public DetectedObjectViewControl()
        {
            InitializeComponent();
        }
    }
}
