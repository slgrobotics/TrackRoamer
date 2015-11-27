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

using TrackRoamer.Robotics.LibMapping;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for TurnStateViewControl.xaml
    /// </summary>
    public partial class TurnStateViewControl : UserControl
    {
        public TurnState turnState = null;

        private List<PiePiece> piePieces = new List<PiePiece>();

        public TurnStateViewControl()
        {
            InitializeComponent();
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            draw();
        }

        public void redraw()
        {
            draw();
        }
        
        public void draw()
        {
            canvas.Children.Clear();
            piePieces.Clear();

            if (turnState == null || !turnState.inTurn)
            {
                return;
            }

            double initialHeading = (double)turnState.directionInitial.heading;
            double targetTurnAngle = (double)turnState.directionDesired.heading - initialHeading;
            double currentTurnAngle = (double)turnState.directionCurrent.heading - initialHeading;

            //double targetTurnAngle = -240.0d;
            //double currentTurnAngle = -20.0d;

            if (Math.Abs(targetTurnAngle) > 1.0d)
            {
                double halfWidth = canvas.ActualWidth / 2.0d;
                double halfHeight = canvas.ActualHeight / 2.0d;
                double radius = (halfWidth + halfHeight) / 2.0d;
                double innerRadius = radius / 2.0d;
                double pushOut = 5.0d;

                // add the pie pieces
                bool targetTurnNegative = targetTurnAngle < 0.0d;
                bool currentTurnNegative = currentTurnAngle < 0.0d;

                PiePiece pieceTargetTurn = new PiePiece()
                {
                    Radius = radius,
                    InnerRadius = innerRadius,
                    CentreX = halfWidth,
                    CentreY = halfHeight,
                    PushOut = pushOut,
                    WedgeAngle = Math.Abs(targetTurnAngle),
                    PieceValue = Math.Round(targetTurnAngle),
                    RotationAngle = (targetTurnNegative ? targetTurnAngle : 0.0d) + initialHeading,
                    Fill = Brushes.Yellow,
                    Opacity = 0.4d,
                    Tag = string.Format("Target turn to {0}", targetTurnAngle),
                    ToolTip = new ToolTip()
                };

                pieceTargetTurn.ToolTipOpening += new ToolTipEventHandler(PiePieceToolTipOpening);

                piePieces.Add(pieceTargetTurn);
                canvas.Children.Add(pieceTargetTurn);

                PiePiece pieceCurrentTurn = new PiePiece()
                {
                    Radius = radius,
                    InnerRadius = innerRadius,
                    CentreX = halfWidth,
                    CentreY = halfHeight,
                    PushOut = 0,
                    WedgeAngle = Math.Abs(currentTurnAngle),
                    PieceValue = Math.Round(currentTurnAngle),
                    RotationAngle = (currentTurnNegative ? currentTurnAngle : 0.0d) + initialHeading,
                    Fill = Brushes.Red,
                    Opacity = 0.6d,
                    Tag = string.Format("Turned to {0}", currentTurnAngle),
                    ToolTip = new ToolTip()
                };

                pieceCurrentTurn.ToolTipOpening += new ToolTipEventHandler(PiePieceToolTipOpening);

                piePieces.Add(pieceCurrentTurn);
                canvas.Children.Add(pieceCurrentTurn);
            }
        }

        /// <summary>
        /// Handles the event which occurs just before a pie piece tooltip opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PiePieceToolTipOpening(object sender, ToolTipEventArgs e)
        {
            PiePiece piece = (PiePiece)sender;
            ToolTip tip = (ToolTip)piece.ToolTip;
            tip.Content = (string)piece.Tag;
        }
    }
}
