using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using sonarproxy = Microsoft.Robotics.Services.SonarSensorArray.Proxy;
using irproxy = Microsoft.Robotics.Services.InfraredSensorArray.Proxy;
using batteryproxy = Microsoft.Robotics.Services.Battery.Proxy;

using adc = Microsoft.Robotics.Services.ADCPinArray;
using battery = Microsoft.Robotics.Services.Battery;
using ir = Microsoft.Robotics.Services.InfraredSensorArray;
using sonar = Microsoft.Robotics.Services.SonarSensorArray;

using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;

namespace TrackRoamer.Robotics.Services.TrackroamerRP2011AbstractionLayer
{
    /// <summary>
    /// TrackroamerRP2011AbstractionLayer contract class
    /// </summary>
    public sealed class Contract
    {
        /// <summary>
        /// DSS contract identifer for TrackroamerRP2011AbstractionLayer
        /// </summary>
        [DataMember]
        public const string Identifier = "http://schemas.trackroamer.com/robotics/2011/12/trackroamerrp2011abstractionlayer.html";
    }

    /// <summary>
    /// Trackroamer Reference Platform 2011 Abstraction Layer state
    /// </summary>
    [DataContract]
    public class TrackroamerRP2011AbstractionLayerState
    {
        /// <summary>
        /// Timestamp information for the service state
        /// </summary>
        [DataMember]
        public DateTime LastStartTime = DateTime.Now;

        /// <summary>
        /// Time in MS between retrieving sensor values
        /// </summary>
        [DataMember]
        public int SensorPollingInterval = 100;

        /// <summary>
        /// ADC pin containing the battery voltage
        /// </summary>
        [DataMember]
        public int BatteryVoltagePinIndex = 7;

        /// <summary>
        /// ADC pin containing the battery voltage
        /// </summary>
        [DataMember]
        public double BatteryVoltageDivider = 3.21;

        /// <summary>
        /// Part of raw to normalized conversion formula
        /// </summary>
        [DataMember]
        public double InfraredRawValueDivisorScalar = 22;

        /// <summary>
        /// Ration for IR sensor voltage function
        /// </summary>
        [DataMember]
        public double InfraredDistanceExponent = -1.20;

        /// <summary>
        /// Conversion of echo time to centimeters
        /// </summary>
        [DataMember]
        public double SonarTimeValueMultiplier = 0.1088928;

        /// <summary>
        /// Alternate contract state data for IR sensors
        /// </summary>
        [DataMember]
        public ir.InfraredSensorArrayState InfraredSensorState = new ir.InfraredSensorArrayState();

        /// <summary>
        /// Alternate contract state data for Sonar sensors
        /// </summary>
        [DataMember]
        public sonar.SonarSensorArrayState SonarSensorState = new sonar.SonarSensorArrayState();

        /// <summary>
        /// Alternate contract state data for Battery
        /// </summary>
        [DataMember]
        public battery.BatteryState BatteryState = new battery.BatteryState();

        // =============================================================
        // internal proximity brick specific values for troubleshooting:

        /// <summary>
        /// Sonar Timestamp - when last scan received
        /// </summary>
        [DataMember]
        public DateTime SonarTimeStamp = DateTime.MinValue;

        /// <summary>
        /// Sonar status
        /// </summary>
        [DataMember]
        public string SonarLinkState = string.Empty;

        /// <summary>
        /// Last sonar sensor scan.
        /// </summary>
        [DataMember, Browsable(true)]
        public proxibrick.SonarDataDssSerializable MostRecentSonar { get; set; }

        /// <summary>
        /// Last IR Directed (Proximity) sensors reading.
        /// </summary>
        [DataMember, Browsable(true)]
        public proxibrick.ProximityDataDssSerializable MostRecentProximity { get; set; }

        /// <summary>
        /// Last Parking Sensors reading.
        /// </summary>
        [DataMember, Browsable(true)]
        public proxibrick.ParkingSensorDataDssSerializable MostRecentParkingSensor { get; set; }

    }

    /// <summary>
    /// TrackroamerRP2011AbstractionLayer main operations port
    /// </summary>
    [ServicePort]
    public class TrackroamerRP2011AbstractionLayerOperations : PortSet<DsspDefaultLookup, DsspDefaultDrop, Get, HttpGet, Subscribe>
    {
    }

    /// <summary>
    /// TrackroamerRP2011AbstractionLayerState get operation
    /// Boilerplate interface definition, no code required
    /// </summary>
    public class Get : Get<GetRequestType, PortSet<TrackroamerRP2011AbstractionLayerState, Fault>>
    {
        /// <summary>
        /// Creates a new instance of Get
        /// </summary>
        public Get()
        {
        }

        /// <summary>
        /// Creates a new instance of Get
        /// </summary>
        /// <param name="body">The request message body</param>
        public Get(GetRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Get
        /// </summary>
        /// <param name="body">The request message body</param>
        /// <param name="responsePort">The response port for the request</param>
        public Get(GetRequestType body, PortSet<TrackroamerRP2011AbstractionLayerState, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// TrackroamerRP2011AbstractionLayerState Replace operation
    /// </summary>
    public class Replace : Replace<TrackroamerRP2011AbstractionLayerState, PortSet<DefaultReplaceResponseType, Fault>>
    {
        /// <summary>
        /// Default no-param ctor
        /// </summary>
        public Replace()
        {
        }

        /// <summary>
        /// Service State-based ctor
        /// </summary>
        /// <param name="state">Service State</param>
        public Replace(TrackroamerRP2011AbstractionLayerState state)
            : base(state)
        {
        }

        /// <summary>
        /// State and Port ctor
        /// </summary>
        /// <param name="state">Service State</param>
        /// <param name="responsePort">Response Port</param>
        public Replace(TrackroamerRP2011AbstractionLayerState state, PortSet<DefaultReplaceResponseType, Fault> responsePort)
            : base(state, responsePort)
        {
        }
    }

    /// <summary>
    /// TrackroamerRP2011AbstractionLayer subscribe operation
    /// </summary>
    public class Subscribe : Subscribe<SubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
    {
        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        public Subscribe()
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">The request message body</param>
        public Subscribe(SubscribeRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">The request message body</param>
        /// <param name="responsePort">The response port for the request</param>
        public Subscribe(SubscribeRequestType body, PortSet<SubscribeResponseType, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }
}


