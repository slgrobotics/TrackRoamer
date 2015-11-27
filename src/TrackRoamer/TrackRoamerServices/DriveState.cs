//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: DriveState.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Dss.Core.Attributes;
using motor = Microsoft.Robotics.Services.Motor;

namespace Microsoft.Robotics.Services.Drive
{
    /// <summary>
    /// Differential Drive State Definition
    /// </summary>
    [DataContract]
    [Description("The state of the differential drive.")]
    public class DriveDifferentialTwoWheelState
    {
        private motor.WheeledMotorState _leftWheel;
        private motor.WheeledMotorState _rightWheel;
        private bool _isEnabled;
        private double _distanceBetweenWheels;
        private DriveStage _driveDistanceStage = DriveStage.InitialRequest;
        private DriveStage _rotateDegreesStage = DriveStage.InitialRequest;

        private DriveRequestOperation _internalPendingDriveOperation;
        DateTime _timeStamp;

        ///<summary>
        /// The last drive request operation
        ///</summary>
        public DriveRequestOperation InternalPendingDriveOperation
        {
            get { return _internalPendingDriveOperation; }
            set { _internalPendingDriveOperation = value; }
        }

        /// <summary>
        /// The timestamp of the last state change.
        /// </summary>
        [DataMember(XmlOmitDefaultValue = true)]
        [Browsable (false)]
        [Description("Indicates the timestamp of the last state change.")]
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        /// <summary>
        /// The left wheel's state.
        /// </summary>
        [DataMember]
        [Description("The left wheel's state.")]
        public motor.WheeledMotorState LeftWheel
        {
            get { return this._leftWheel; }
            set { this._leftWheel = value; }
        }

        /// <summary>
        /// The right wheel's state.
        /// </summary>
        [DataMember]
        [Description("The right wheel's state.")]
        public motor.WheeledMotorState RightWheel
        {
            get { return this._rightWheel; }
            set { this._rightWheel = value; }
        }

        /// <summary>
        /// The distance between the drive wheels (meters).
        /// </summary>
        [DataMember]
        [Description("Indicates the distance between the drive wheels (meters).")]
        public double DistanceBetweenWheels
        {
            get { return this._distanceBetweenWheels; }
            set { this._distanceBetweenWheels = value; }
        }

        /// <summary>
        /// Indicates whether the drive has been enabled.
        /// </summary>
        [DataMember]
        [Description("Indicates whether the drive has been enabled.")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        /// <summary>
        /// The current stage of a driveDistance operation.
        /// </summary>
        [DataMember]
        [Description("The current stage of a driveDistance operation.")]
        public DriveStage DriveDistanceStage
        {
            get { return _driveDistanceStage; }
            set { _driveDistanceStage = value; }
        }

        /// <summary>
        /// The current stage of a rotateDegrees operation.
        /// </summary>
        [DataMember]
        [Description("The current stage of a rotateDegrees operation.")]
        public DriveStage RotateDegreesStage
        {
            get { return _rotateDegreesStage; }
            set { _rotateDegreesStage = value; }
        }

        /// <summary>
        /// Indicates the current state of the Drive.
        /// </summary>
        [DataMember]
        [Description("Indicates the current state of the drive.")]
        [Browsable(false)]
        public DriveState DriveState;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DriveDifferentialTwoWheelState() { }

        /// <summary>
        /// Initialization constructor
        /// </summary>
        /// <param name="distanceBetweenWheels"></param>
        /// <param name="leftWheel"></param>
        /// <param name="rightWheel"></param>
        public DriveDifferentialTwoWheelState(double distanceBetweenWheels, motor.WheeledMotorState leftWheel, motor.WheeledMotorState rightWheel)
        {
            _distanceBetweenWheels = distanceBetweenWheels;
            _leftWheel = leftWheel;
            _rightWheel = rightWheel;
        }
    }

    /// <summary>
    /// The current Drive State
    /// </summary>
    [DataContract]
    public enum DriveState
    {
        /// <summary>
        /// Not Specified
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// Stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Drive Distance
        /// </summary>
        DriveDistance,

        /// <summary>
        /// Drive Power
        /// </summary>
        DrivePower,

        /// <summary>
        /// Rotate Degrees
        /// </summary>
        RotateDegrees,

        /// <summary>
        /// DriveSpeed
        /// </summary>
        DriveSpeed
    }

    /// <summary>
    /// The status of the current drive operation (driveDistance or rotateDegrees)
    /// Only one operation can be pending (else it is canceled).
    /// 
    ///  Stage transitions:
    ///     InitialRequest -> Started -> Completed
    ///        Or:
    ///     InitialRequest -> Started -> Canceled
    /// </summary>
    [DataContract]
    public enum DriveStage
    {
        /// <summary>
        /// A request to initiate a drive distance or rotate degrees operation
        /// </summary>
        InitialRequest = 0,

        /// <summary>
        /// A drive operation (drive distance or rotate degrees) has started
        /// </summary>
        Started,

        /// <summary>
        /// The pending drive operation was canceled
        /// </summary>
        Canceled,

        /// <summary>
        /// Successful completion of a drive distance or rotate degrees operation
        /// </summary>
        Completed
    }

    /// <summary>
    /// The request operation
    /// </summary>
    [DataContract]
    public enum DriveRequestOperation
    {
        /// <summary>
        /// Not Specified
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// All Stop
        /// </summary>
        AllStop,

        /// <summary>
        /// Drive Distance
        /// </summary>
        DriveDistance,

        /// <summary>
        /// Drive Power
        /// </summary>
        SetDrivePower,

        /// <summary>
        /// Rotate Degrees
        /// </summary>
        RotateDegrees,

        /// <summary>
        /// Drive Speed
        /// </summary>
        DriveSpeed,

        /// <summary>
        /// Enable Drive
        /// </summary>
        EnableDrive
    }

    /// <summary>
    /// Update the target power of each motor.
    /// </summary>
    [DataContract]
    [DataMemberConstructor]
    public class SetDrivePowerRequest
    {
        private double _leftWheelPower;
        private double _rightWheelPower;

        /// <summary>
        /// Set Power for Left Wheel. Range is -1.0 to 1.0
        /// </summary>
        [DataMember]
        [Description("Indicates the power setting for the left wheel; range is -1.0 to 1.0.")]
        public double LeftWheelPower
        {
            get { return _leftWheelPower; }
            set { _leftWheelPower = value; }
        }

        /// <summary>
        /// Set Power for Right Wheel. Range is -1.0 to 1.0
        /// </summary>
        [DataMember]
        [Description("Indicates the power setting for the right wheel; range is -1.0 to 1.0.")]
        public double RightWheelPower
        {
            get { return _rightWheelPower; }
            set { _rightWheelPower = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SetDrivePowerRequest()
        {
        }

        /// <summary>
        /// Initialization constructor
        /// </summary>
        /// <param name="leftWheelPower">Range is -1.0 to 1.0 (positive value is forward)</param>
        /// <param name="rightWheelPower">Range is -1.0 to 1.0(positive value is forward)</param>
        public SetDrivePowerRequest(double leftWheelPower, double rightWheelPower)
        {
            _leftWheelPower = leftWheelPower;
            _rightWheelPower = rightWheelPower;
        }
    }

    /// <summary>
    /// Update the target speed of a motor.
    /// <remarks>set speed for each wheel in meters per second</remarks>
    /// </summary>
    [DataContract]
    [DataMemberConstructor]
    public class SetDriveSpeedRequest
    {
        private double _leftWheelSpeed;
        private double _rightWheelSpeed;

        /// <summary>
        /// Set Speed for Left Wheel (m/s)
        /// </summary>
        [DataMember]
        [Description("Indicates the speed setting for the left wheel (in m/s).")]
        public double LeftWheelSpeed
        {
            get { return _leftWheelSpeed; }
            set { _leftWheelSpeed = value; }
        }

        /// <summary>
        /// Set Speed for Right Wheel (m/s)
        /// </summary>
        [DataMember]
        [Description("Indicates the speed setting for the right wheel (in m/s).")]
        public double RightWheelSpeed
        {
            get { return _rightWheelSpeed; }
            set { _rightWheelSpeed = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SetDriveSpeedRequest() { }

        /// <summary>
        /// Initialization Constructor
        /// </summary>
        /// <param name="leftWheelSpeed"></param>
        /// <param name="rightWheelSpeed"></param>
        public SetDriveSpeedRequest(double leftWheelSpeed, double rightWheelSpeed)
        {
            _leftWheelSpeed = leftWheelSpeed;
            _rightWheelSpeed = rightWheelSpeed;
        }
    }

    /// <summary>
    /// Drive straight for specified distance
    /// </summary>
    [DataContract]
    [DataMemberConstructor]
    public class DriveDistanceRequest
    {
        double _distance;
        double _power;
        DriveStage _driveDistanceStage;

        /// <summary>
        /// Distance in meters
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the distance to drive (meters).")]
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        /// <summary>
        /// The drive's power setting (-1.0 to 1.0 -- Forward: positive value, Reverse: negative value.)
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the power setting for driving (-1.0 to 1.0 -- Forward: positive value, Reverse: negative value.).")]
        public double Power
        {
            get { return _power; }
            set { _power = value; }
        }

        /// <summary>
        /// Distance stage
        /// </summary>
        [DataMember]
        [DataMemberConstructor(Order=-1)]
        [Description("Specifies the current drive distance stage.")]
        public DriveStage DriveDistanceStage
        {
            get { return _driveDistanceStage; }
            set { _driveDistanceStage = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DriveDistanceRequest() { }

        /// <summary>
        /// Initialization constructor
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="pow"></param>
        public DriveDistanceRequest(double distance, double pow)
        {
            _distance = distance;
            _power = pow;
            _driveDistanceStage = DriveStage.InitialRequest;
        }
    }

    /// <summary>
    /// Request the drive to rotate or turn in position (positive values turn counterclockwise).
    /// </summary>
    [DataContract, Description("Request the drive to rotate or turn in position (positive values turn counterclockwise).")]
    [DataMemberConstructor]
    public class RotateDegreesRequest
    {
        double _degrees;
        double _power;
        DriveStage _rotateDegreesStage;

        /// <summary>
        /// Degrees of rotation (positive values turn counterclockwise).
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the drive setting in degrees of rotation (positive values turn counterclockwise).")]
        public double Degrees
        {
            get { return _degrees; }
            set { _degrees = value; }
        }

        /// <summary>
        /// The drive's power setting (-1.0 to 1.0 -- Forward: positive value, Reverse: negative value.)
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the drive's power setting (-1.0 to 1.0 -- Forward: positive value, Reverse: negative value.")]
        public double Power
        {
            get { return _power; }
            set { _power = value; }
        }

        /// <summary>
        /// RotateDegrees stage
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataMemberConstructor(Order = -1)]
        [Description("Specifies the current rotate degrees stage.")]
        public DriveStage RotateDegreesStage
        {
            get { return _rotateDegreesStage; }
            set { _rotateDegreesStage = value; }
        }

        /// <summary>
        /// Request the drive to rotate or turn in position (positive values turn counterclockwise).
        /// </summary>
        public RotateDegreesRequest() { }

        /// <summary>
        /// Request the drive to rotate or turn in position (positive values turn counterclockwise).
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="power"></param>
        public RotateDegreesRequest(double degrees, double power)
        {
            _degrees = degrees;
            _power = power;
            _rotateDegreesStage = DriveStage.InitialRequest;
        }
    }

    /// <summary>
    /// Enables or disables the drive
    /// </summary>
    [DataContract]
    [DataMemberConstructor]
    public class EnableDriveRequest
    {
        private bool _enable;

        /// <summary>
        /// Enable drive
        /// </summary>
        [DataMember]
        [Description("Identifies if the drive is enabled.")]
        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EnableDriveRequest()
        {
            _enable = true;
        }

        /// <summary>
        /// Initialization constructor
        /// </summary>
        /// <param name="enable"></param>
        public EnableDriveRequest(bool enable)
        {
            _enable = enable;
        }
    }

    /// <summary>
    /// Emergency stop
    /// </summary>
    [DataContract]
    public class AllStopRequest
    {
    }

    /// <summary>
    /// Set Motor Uri Request
    /// </summary>
    [DataContract]
    public class SetMotorUriRequest
    {
        private Uri _leftMotorUri;
        private Uri _rightMotorUri;

        /// <summary>
        /// Left Motor Uri
        /// </summary>
        [DataMember]
        [Description("Identifies the left motor URI setting.")]
        public string LeftMotorUri
        {
            get { return _leftMotorUri.AbsoluteUri; }
            set { _leftMotorUri = new Uri(value); }
        }

        /// <summary>
        /// Right Motor Uri
        /// </summary>
        [DataMember]
        [Description("Identifieis the right motor URI setting.")]
        public string RightMotorUri
        {
            get { return _rightMotorUri.AbsoluteUri; }
            set { _rightMotorUri = new Uri(value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SetMotorUriRequest() { }

        /// <summary>
        /// Initialization constructor
        /// </summary>
        /// <param name="leftMotor"></param>
        /// <param name="rightMotor"></param>
        public SetMotorUriRequest(Uri leftMotor, Uri rightMotor)
        {
            _leftMotorUri = leftMotor;
            _rightMotorUri = rightMotor;
        }
    }

    #region internal to service
    //==================================================================
    // Internal to service

    /// <summary>
    /// Cancel Pending Drive Operation Request Request
    /// (cancels a pending driveDistance or RotateDegrees request)
    /// </summary>
    [DataContract]
    public class CancelPendingDriveOperationRequest
    {
    }
    #endregion
}
