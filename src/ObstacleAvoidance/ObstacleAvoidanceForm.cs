//------------------------------------------------------------------------------
//  <copyright file="ObstacleAvoidanceForm.cs" company="Microsoft Corporation">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
//  </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Robotics.Services.ObstacleAvoidanceDrive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Microsoft.Ccr.Core;
    using Microsoft.Robotics.Services.ObstacleAvoidanceDrive;

    using joystick = Microsoft.Robotics.Services.GameController.Proxy;
    using webcam = Microsoft.Robotics.Services.WebCam.Proxy;

    /// <summary>
    /// The main ObstacleAvoidance Form
    /// </summary>
    public partial class ObstacleAvoidanceForm : Form
    {
        /// <summary>
        /// The port for sending events
        /// </summary>
        private ObstacleAvoidanceFormEvents eventsPort;

        /// <summary>
        /// Initializes a new instance of the DashboardForm class
        /// </summary>
        /// <param name="theEventsPort">The Events Port for passing events back to the service</param>
        /// <param name="state">The service state</param>
        public ObstacleAvoidanceForm(ObstacleAvoidanceFormEvents theEventsPort, ObstacleAvoidanceDriveState state)
        {
            this.eventsPort = theEventsPort;

            this.InitializeComponent();

            this.UpdatePIDControllersValue(state);
        }

        /// <summary>
        /// Handle Form Load
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ObstacleAvoidanceForm_Load(object sender, EventArgs e)
        {
            this.eventsPort.Post(new OnLoad(this));
        }

        /// <summary>
        /// Handle Form Closed
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ObstacleAvoidanceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.eventsPort.Post(new OnClosed(this));
        }

        /// <summary>
        /// Update PID controller parameter values on the form.
        /// This is a one-off call during constructor where we need to 
        /// set the initial state of the PID controllers
        /// </summary>
        /// <param name="state">state</param>
        public void UpdatePIDControllersValue(ObstacleAvoidanceDriveState state)
        {
            // Set all the PID values on the form, copying them from state:
            this.textBoxAngularKp.Text = state.Controller.Kp.ToString();
            this.textBoxAngularKi.Text = state.Controller.Ki.ToString();
            this.textBoxAngularKd.Text = state.Controller.Kd.ToString();

            this.textBoxAngularMax.Text = state.Controller.MaxPidValue.ToString();
            this.textBoxAngularMin.Text = state.Controller.MinPidValue.ToString();
            this.textBoxAngularIntegralMax.Text = state.Controller.MaxIntegralError.ToString();
        }

        /// <summary>
        /// Set new PID controller values
        /// </summary>
        public void PostPIDControllersValue(bool doSaveState)
        {
            try
            {
                this.eventsPort.Post(new OnPIDChanges(this,
                                                        double.Parse(this.textBoxAngularKp.Text), double.Parse(this.textBoxAngularKi.Text), double.Parse(this.textBoxAngularKd.Text),
                                                        double.Parse(this.textBoxAngularMax.Text), double.Parse(this.textBoxAngularMin.Text), double.Parse(this.textBoxAngularIntegralMax.Text),
                                                        doSaveState
                                                     ));
                PIDControllerGroupBox.BackColor = Color.LightBlue;
            }
            catch
            {
                PIDControllerGroupBox.BackColor = Color.Red;
            }
        }

        /// <summary>
        /// A bitmap to hold the depth profile image
        /// </summary>
        private Bitmap depthProfileImage;

        /// <summary>
        /// Gets or sets the Depth Profile Image
        /// </summary>
        /// <remarks>Provides external access for updating the depth profile image</remarks>
        public Bitmap DepthProfileImage
        {
            get
            {
                return this.depthProfileImage;
            }

            set
            {
                this.depthProfileImage = value;

                Image old = this.depthProfileCtrl.Image;
                this.depthProfileCtrl.Image = value;

                // Dispose of the old bitmap to save memory
                // (It will be garbage collected eventually, but this is faster)
                if (old != null)
                {
                    old.Dispose();
                }
            }
        }

        private void buttonUpdatePidControllers_Click(object sender, EventArgs e)
        {
            this.PostPIDControllersValue(false);    // no SaveState
        }

        private void buttonSaveState_Click(object sender, EventArgs e)
        {
            this.PostPIDControllersValue(true);    // with SaveState
        }
    }

    /// <summary>
    /// Operations Port for ObstacleAvoidance Events
    /// </summary>
    public class ObstacleAvoidanceFormEvents :
        PortSet<OnLoad,
            OnClosed,
            OnQueryFrame,
            OnPIDChanges>
    {
    }

    /// <summary>
    /// Class used for events sent by the ObstacleAvoidance Form back to the service
    /// </summary>
    public class ObstacleAvoidanceFormEvent
    {
        /// <summary>
        ///  Obstacle Avoidance Form
        /// </summary>
        private ObstacleAvoidanceForm obstacleAvoidanceForm;

        /// <summary>
        /// Gets or sets the associated Form
        /// </summary>
        public ObstacleAvoidanceForm ObstacleAvoidanceForm
        {
            get { return this.obstacleAvoidanceForm; }
            set { this.obstacleAvoidanceForm = value; }
        }

        /// <summary>
        /// Initializes an instance of the ObstacleAvoidanceFormEvent class
        /// </summary>
        /// <param name="obstacleAvoidanceForm">The associated Form</param>
        public ObstacleAvoidanceFormEvent(ObstacleAvoidanceForm obstacleAvoidanceForm)
        {
            this.obstacleAvoidanceForm = obstacleAvoidanceForm;
        }
    }

    /// <summary>
    /// Form Loaded message
    /// </summary>
    public class OnLoad : ObstacleAvoidanceFormEvent
    {
        /// <summary>
        /// Initializes an instance of the OnLoad class
        /// </summary>
        /// <param name="form">The associated Form</param>
        public OnLoad(ObstacleAvoidanceForm form)
            : base(form)
        {
        }
    }

    /// <summary>
    /// Form Closed message
    /// </summary>
    public class OnClosed : ObstacleAvoidanceFormEvent
    {
        /// <summary>
        /// Initializes an instance of the OnClosed class
        /// </summary>
        /// <param name="form">The associated Form</param>
        public OnClosed(ObstacleAvoidanceForm form)
            : base(form)
        {
        }
    }

    /// <summary>
    /// Query Frame message
    /// </summary>
    public class OnQueryFrame : ObstacleAvoidanceFormEvent
    {
        /// <summary>
        /// Initializes an instance of the OnQueryFrame class
        /// </summary>
        /// <param name="form">The associated form</param>
        public OnQueryFrame(ObstacleAvoidanceForm form)
            : base(form)
        {
        }
    }

    /// <summary>
    /// PID parameter values changes
    /// </summary>
    public class OnPIDChanges : ObstacleAvoidanceFormEvent
    {
        /// <summary>
        /// Gets or sets the Proportional constant
        /// </summary>
        public double Kp { get; set; }

        /// <summary>
        /// Gets or sets the Proportional constant
        /// </summary>
        public double Ki { get; set; }

        /// <summary>
        /// Gets or sets the Derivative constant
        /// </summary>
        public double Kd { get; set; }

        /// <summary>
        /// Gets or sets the Max PID Value
        /// </summary>
        public double MaxPidValue { get; set; }

        /// <summary>
        /// Gets or sets the Min PID Value
        /// </summary>
        public double MinPidValue { get; set; }

        /// <summary>
        /// Gets or sets the Max Integral Error
        /// </summary>
        public double MaxIntegralError { get; set; }

        /// <summary>
        /// Gets or sets the DoSaveState flag
        /// </summary>
        public bool DoSaveState { get; set; }

        /// <summary>
        /// Initializes an instance of the OnPIDChanges class
        /// </summary>
        /// <param name="form"></param>
        /// <param name="kp"></param>
        /// <param name="ki"></param>
        /// <param name="kd"></param>
        /// <param name="vMax"></param>
        /// <param name="vMin"></param>
        /// <param name="vIntMax"></param>
        /// <param name="doSaveState"></param>
        public OnPIDChanges(ObstacleAvoidanceForm form, double kp, double ki, double kd, double vMax, double vMin, double vIntMax, bool doSaveState)
            : base(form)
        {
            this.Kp = kp;
            this.Ki = ki;
            this.Kd = kd;

            this.MaxPidValue = vMax;
            this.MinPidValue = vMin;
            this.MaxIntegralError = vIntMax;

            this.DoSaveState = doSaveState;
        }
    }
}
