/*
* Copyright (c) 2011..., Sergei Grichine
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Sergei Grichine nor the
*       names of contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Sergei Grichine ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Sergei Grichine BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
* this is a X11 (BSD Revised) license - you do not have to publish your changes,
* although doing so, donating and contributing is always appreciated
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using W3C.Soap;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickPower
{
    
    /// <summary>
    /// TrackRoamer Power Brick Contract
    /// </summary>
    public sealed class Contract
    {
        /// The Unique Contract Identifier for the TrackRoamer Power Brick core service
        public const String Identifier = "http://schemas.trackroamer.com/robotics/2009/04/trackroamerbrickpower.html";
    }

    /// <summary>
    /// Track Roamer Bot Operations
    /// </summary>
    public class TrackRoamerBrickPowerOperations : PortSet<
		DsspDefaultLookup,
		DsspDefaultDrop,
		Get,
		Replace,
		Subscribe,
		//QueryConfig,
		QueryWhiskers,
		QueryMotorSpeed,
		QueryMotorEncoderSpeed,
		UpdateConfig,
		UpdateMotorSpeed,
		UpdateMotorEncoder,
        UpdateMotorEncoderSpeed,
		ResetMotorEncoder,          // distances
        SetOutputC,
		UpdateWhiskers,
		HttpGet,
		HttpPost>
    {
    }

	#region Data Contracts
    [Description("The TrackRoamer Power Brick Power Controller (RoboteQ AX2850) Configuration State")]
	[DataContract()]
    public class TrackRoamerBrickPowerState
	{
		[Browsable(false)]
		[DataMember]
		public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Is the PowerController unit currently connected.
        /// </summary>
        [DataMember]
        [Description("Indicates that the Power Controller is connected.")]
        [Browsable(false)]
        public bool Connected { get; set; }

        /// <summary>
        /// Serial Port Configuration
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the serial port where the Power Controller is connected.")]
        public PowerControllerConfig PowerControllerConfig { get; set; }

        /// <summary>
        /// PowerController measured values
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("The TrackRoamer Power Brick Controller measured values.")]
        public PowerControllerState PowerControllerState { get; set; }


        [Browsable(false)]
        [DataMember]
        public bool Dropping { get; set; }

        [Browsable(false)]
		[DataMember]
        public long FrameCounter { get; set; }

		[Browsable(false)]
		[DataMember]
        public int FrameRate { get; set; }

		[Browsable(false)]
		[DataMember]
        public long ErrorCounter { get; set; }

		[Browsable(false)]
		[DataMember]
        public int ErrorRate { get; set; }

		[Browsable(false)]
		[DataMember]
        public int ConnectAttempts { get; set; }

		[Browsable(false)]
		[DataMember]
        public int? OutputC { get; set; }        // 0 or 1

		[Browsable(false)]
		[DataMember]
        public MotorSpeed MotorSpeed { get; set; }

		[Browsable(false)]
		[DataMember]
        public Whiskers Whiskers { get; set; }

		[Browsable(false)]
		[DataMember]
        public MotorEncoder MotorEncoder { get; set; }

		[Browsable(false)]
		[DataMember]
        public MotorEncoderSpeed MotorEncoderSpeed { get; set; }
	}

	[Description("The whiskers sensors state")]
	[DataContract]
	public class Whiskers
	{
		/*
			HardwareIdentifier coding:
			  1st digit   1=Left 2=Right
			  2d  digit   0=Front 1=Rear
			  3d  digit   1=Whisker 2=IRBumper 3=StepSensor
		 */

		[Browsable(false)]
		[DataMember]
		public DateTime Timestamp { get; set; }

		[Browsable(false)]
		[DataMember]
		public bool? FrontWhiskerLeft { get; set; }

		[Browsable(false)]
		[DataMember]
		public bool? FrontWhiskerRight { get; set; }
	}

	[Description("The current left and right motor speed settings")]
	[DataContract]
	public class MotorSpeed
	{
		[Browsable(false)]
		[DataMember]
        public DateTime Timestamp { get; set; }

		[Browsable(false)]
		[DataMember]
        public double? LeftSpeed { get; set; }

		[Browsable(false)]
		[DataMember]
        public double? RightSpeed { get; set; }

	}

	[Description("The current left or/and right motor encoder distance readings")]
	[DataContract]
	public class MotorEncoder
	{
		[Browsable(false)]
		[DataMember]
        public DateTime Timestamp { get; set; }

		[Browsable(false)]
		[DataMember]
        public int? HardwareIdentifier { get; set; }	// 1=Left, 2=Right matters on ResetMotorEncoder

		[Browsable(false)]
		[DataMember]
        public double? LeftDistance { get; set; }

		[Browsable(false)]
		[DataMember]
        public double? RightDistance { get; set; }
	}

    [Description("The current left or/and right motor encoder speed readings")]
    [DataContract]
    public class MotorEncoderSpeed
    {
        [Browsable(false)]
        [DataMember]
        public DateTime Timestamp { get; set; }

        [Browsable(false)]
        [DataMember]
        public double? LeftSpeed { get; set; }

        [Browsable(false)]
        [DataMember]
        public double? RightSpeed { get; set; }
    }

    /// <summary>
    /// PowerController measured values
    /// </summary>
    [DataContract]
    [DisplayName("(User) PowerController State")]
    [Description("The TrackRoamer Power Brick Controller measured values")]
    public class PowerControllerState
    {
        [DataMember]
        [Description("Analog_Input_1")]
        public double? Analog_Input_1 { get; set; }

        [DataMember]
        [Description("Analog_Input_2")]
        public double? Analog_Input_2 { get; set; }

        [DataMember]
        [Description("Digital_Input_E")]
        public double? Digital_Input_E { get; set; }

        [DataMember]
        [Description("Main_Battery_Voltage")]
        public double? Main_Battery_Voltage { get; set; }
        
        [DataMember]
        [Description("Internal_Voltage")]
        public double? Internal_Voltage { get; set; }
        
        [DataMember]
        [Description("Motor_Power_Left")]
        public double? Motor_Power_Left { get; set; }

        [DataMember]
        [Description("Motor_Power_Right")]
        public double? Motor_Power_Right { get; set; }

        [DataMember]
        [Description("Motor_Amps_Left")]
        public double? Motor_Amps_Left { get; set; }

        // note: Amps behave almost like integers, no precision here and low current will read as 0

        [DataMember]
        [Description("Motor_Amps_Right")]
        public double? Motor_Amps_Right { get; set; }

        [DataMember]
        [Description("Heatsink_Temperature_Left")]
        public double? Heatsink_Temperature_Left { get; set; }

        [DataMember]
        [Description("Heatsink_Temperature_Right")]
        public double? Heatsink_Temperature_Right { get; set; }
    }

    /// <summary>
    /// PowerController Serial Port Configuration
    /// </summary>
    [DataContract]
    [DisplayName("(User) PowerController Configuration")]
    [Description("The TrackRoamer Power Brick Controller Serial Port Configuration")]
    public class PowerControllerConfig : ICloneable
    {
        /// <summary>
        /// The Serial Comm Port
        /// </summary>
        [DataMember]
        [Description("PowerController COM Port")]
        public int CommPort { get; set; }

        /// <summary>
        /// The Serial Port Name or the File name containing PowerController readings
        /// </summary>
        [DataMember]
        [Description("The Serial Port Name or the File name containing PowerController readings (Default blank)")]
        public string PortName { get; set; }

        /// <summary>
        /// Baud Rate
        /// </summary>
        [DataMember]
        [Description("PowerController Baud Rate")]
        public int BaudRate { get; set; }

        /// <summary>
        /// Configuration Status
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Description("PowerController Configuration Status")]
        public string ConfigurationStatus { get; set; }

        [Description("Delay in milliseconds, sleep in the main loop of controller")]
		[DataMember]
		public int Delay { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PowerControllerConfig()
        {
            this.BaudRate = 9600;   // cannot be changed for AX2850
            this.CommPort = 0;
            this.PortName = string.Empty;
            this.Delay = 17;
        }

        #region ICloneable implementation

        public object Clone()
        {
            return new PowerControllerConfig()
            {
                BaudRate = this.BaudRate,
                CommPort = this.CommPort,
                PortName = this.PortName
            };
        }

        #endregion  // ICloneable implementation
    }

	#endregion

	#region Operation Ports
	[DisplayName("Get")]
    [Description("Gets the TrackRoamer Power Brick's current state.")]
    public class Get : Get<GetRequestType, PortSet<TrackRoamerBrickPowerState, Fault>>
	{
	}

    //[DisplayName("GetConfiguration")]
    //[Description("Gets the TrackRoamer Power Brick's current configuration.")]
    //public class QueryConfig : Query<PowerControllerConfig, PortSet<PowerControllerConfig, Fault>>
    //{
    //}

    [DisplayName("GetWhiskers")]
    [Description("Gets the whiskers sensors' state.")]
	public class QueryWhiskers : Query<Whiskers, PortSet<Whiskers, Fault>>
	{
	}

	[DisplayName("GetMotorEncoder")]
	[Description("Gets the motor encoder sensors' state - distances.")]
	public class QueryMotorEncoder : Query<MotorEncoder, PortSet<MotorEncoder, Fault>>
	{
	}

	[DisplayName("GetMotorEncoderSpeed")]
	[Description("Gets the motor encoder sensors' state - speed.")]
    public class QueryMotorEncoderSpeed : Query<MotorEncoderSpeed, PortSet<MotorEncoderSpeed, Fault>>
	{
	}

	[DisplayName("GetMotorSpeed")]
	[Description("Gets the motor speed.")]
	public class QueryMotorSpeed : Query<MotorSpeed, PortSet<MotorSpeed, Fault>>
	{
	}

	[DisplayName("UpdateConfiguration")]
    [Description("Updates or indicates an update to the TrackRoamer Power Brick's configuration.")]
    public class UpdateConfig : Update<PowerControllerConfig, PortSet<DefaultUpdateResponseType, Fault>>
	{
	}

	[DisplayName("UpdateSpeed")]
	[Description("Updates or indicates an update to motor speed.")]
	public class UpdateMotorSpeed : Update<MotorSpeed, PortSet<DefaultUpdateResponseType, Fault>>
	{
		public UpdateMotorSpeed()
		{
			if (this.Body == null)
			{
				this.Body = new MotorSpeed();
			}
		}

		public UpdateMotorSpeed(MotorSpeed motorSpeed)
		{
			this.Body = motorSpeed;
		}
	}

    [DisplayName("SetOutputC")]
    [Description("Sets Output C of the AX2850 to on or off")]
    public class SetOutputC : Update<bool, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }


    [DisplayName("UpdateWhiskers")]
    [Description("Updates or indicates an update to the whiskers sensors' state.")]
	public class UpdateWhiskers : Update<Whiskers, PortSet<DefaultUpdateResponseType, Fault>>
	{
	}

	[DisplayName("UpdateMotorEncoder")]
	[Description("Updates or indicates an update to the motor encoders' state - distances.")]
	public class UpdateMotorEncoder : Update<MotorEncoder, PortSet<DefaultUpdateResponseType, Fault>>
	{
	}

    [DisplayName("UpdateMotorEncoderSpeed")]
	[Description("Updates or indicates an update to the motor encoders' state - speeds.")]
    public class UpdateMotorEncoderSpeed : Update<MotorEncoderSpeed, PortSet<DefaultUpdateResponseType, Fault>>
	{
	}

	[DisplayName("ResetMotorEncoder")]
	[Description("Resets the motor encoders' ticks (distances) to 0.")]
	public class ResetMotorEncoder : Replace<MotorEncoder, PortSet<DefaultReplaceResponseType, Fault>>
	{
	}

	[DisplayName("ChangeState")]
    [Description("Changes or indicates a change to the TrackRoamer Power Brick's entire state.")]
    public class Replace : Replace<TrackRoamerBrickPowerState, PortSet<DefaultReplaceResponseType, Fault>>
	{
	}

	public class Subscribe : Subscribe<SubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
	{
	}

    /// <summary>
    /// A PowerController Command
    /// <remarks>Use with SendPowerControllerCommand()</remarks>
    /// </summary>
    [DataContract]
    [Description("A PowerController Command")]
    public class PowerControllerCommand
    {
        [DataMember]
        public string Command { get; set; }
    }

    /// <summary>
    /// standard subscribe request type
    /// </summary>
    [DataContract]
    [Description("Standard Subscribe request")]
    public class SubscribeRequest : SubscribeRequestType
    {
        /// <summary>
        /// Which message types to subscribe to
        /// </summary>
        [DataMember]
        public PowerControllerMessageType MessageTypes;

        /// <summary>
        /// Only subscribe to messages when IsValid is true
        /// </summary>
        [DataMember]
        public bool ValidOnly;
    }

    /// <summary>
    /// PowerController Message Type - bitwise OR for subscribing to selected types only
    /// </summary>
    [DataContract, Flags]
    [Description("Identifies the UM6 message types bitmask for subscriptions.")]
    public enum PowerControllerMessageType
    {
        /// <summary>
        /// No-message mask
        /// </summary>
        None = 0,

        /// <summary>
        /// processed gyroscope data
        /// </summary>
        GYRO_PROC = 0x01,

        /// <summary>
        /// processed accelerometer data
        /// </summary>
        ACCEL_PROC = 0x02,

        /// <summary>
        /// processed magnitometer data
        /// </summary>
        MAG_PROC = 0x04,

        /// <summary>
        /// Euler angles (beware of 90 degrees singularity)
        /// </summary>
        EULER = 0x08,

        /// <summary>
        /// quaternion data (no singularities)
        /// </summary>
        QUAT = 0x10,

        /// <summary>
        /// Subscribe to all message types
        /// </summary>
        All = GYRO_PROC | ACCEL_PROC | MAG_PROC | EULER | QUAT      // use "|" to combine all types here into a bitmask
    }

	#endregion
}
