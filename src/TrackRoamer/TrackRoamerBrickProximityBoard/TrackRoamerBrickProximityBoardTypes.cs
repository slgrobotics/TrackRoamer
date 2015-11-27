using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using Microsoft.Dss.Core.DsspHttp;

using TrackRoamer.Robotics.Utility.LibPicSensors;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard
{
    /// <summary>
    /// TrackRoamerBrickProximityBoard contract class
    /// </summary>
    public sealed class Contract
    {
        /// <summary>
        /// DSS contract identifer for TrackRoamerBrickProximityBoard
        /// </summary>
        [DataMember]
        public const string Identifier = "http://schemas.trackroamer.com/robotics/2011/01/trackroamerbrickproximityboard.html";
    }

    /// <summary>
    /// TrackRoamerBrickProximityBoard state
    /// </summary>
    [DataContract]
    public class TrackRoamerBrickProximityBoardState
    {
        [DataMember]
        public bool IsConnected = false;
        [DataMember]
        public Int32 VendorId;
        [DataMember]
        public Int32 ProductId;
        [DataMember]
        public DateTime LastSampleTimestamp;
        [DataMember]
        public string LinkState = "undefined";
        [DataMember]
        public string Description = "undefined";
        /// <summary>
        /// Last sonar reading.
        /// </summary>
        [DataMember, Browsable(true)]
        public SonarDataDssSerializable MostRecentSonar = null;
        /// <summary>
        /// Last compass reading.
        /// </summary>
        [DataMember, Browsable(true)]
        public DirectionDataDssSerializable MostRecentDirection = null;
        /// <summary>
        /// Last Accelerometer reading.
        /// </summary>
        [DataMember, Browsable(true)]
        public AccelerometerDataDssSerializable MostRecentAccelerometer = null;
        /// <summary>
        /// Last IR Proximity sensors reading.
        /// </summary>
        [DataMember, Browsable(true)]
        public ProximityDataDssSerializable MostRecentProximity = null;
        /// <summary>
        /// Last Parking Sensor sensors reading.
        /// </summary>
        [DataMember, Browsable(true)]
        public ParkingSensorDataDssSerializable MostRecentParkingSensor = null;
        /// <summary>
        /// Last POT reading. (pin 2 AN0 of PIC 4550 etc.)
        /// </summary>
        [DataMember, Browsable(true)]
        public AnalogDataDssSerializable MostRecentAnalogData;
        /// <summary>
        /// Angular range of the sonar sweep measurement.
        /// </summary>
        [DataMember]
        [Description("The angular range of the sonar sweep measurement.")]
        public int AngularRange;
        /// <summary>
        /// Angular resolution of a given sonar sweep ray.
        /// </summary>
        [DataMember]
        [Description("The angular resolution of the sonar sweep measurement.")]
        public double AngularResolution;
    }

    /// <summary>
    /// TrackRoamerBrickProximityBoard main operations port
    /// </summary>
    [ServicePort]
    class TrackRoamerBrickProximityBoardOperations :
        PortSet<
            DsspDefaultLookup,
            DsspDefaultDrop,
            Get,
            Replace,
            ReliableSubscribe,
            Subscribe,
            UpdateSonarData,
            UpdateDirectionData,
            UpdateAccelerometerData,
            UpdateProximityData,
            UpdateParkingSensorData,
            UpdateAnalogData,
            Reset,
            HttpGet
        >
    {
    }

    /// <summary>
    /// TrackRoamerBrickProximityBoard get operation
    /// </summary>
    public class Get : Get<GetRequestType, PortSet<TrackRoamerBrickProximityBoardState, Fault>>
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
        /// <param name="body">the request message body</param>
        public Get(GetRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Get
        /// </summary>
        /// <param name="body">the request message body</param>
        /// <param name="responsePort">the response port for the request</param>
        public Get(GetRequestType body, PortSet<TrackRoamerBrickProximityBoardState, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// TrackRoamerBrickProximityBoard subscribe operation
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
        /// <param name="body">the request message body</param>
        public Subscribe(SubscribeRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">the request message body</param>
        /// <param name="responsePort">the response port for the request</param>
        public Subscribe(SubscribeRequestType body, PortSet<SubscribeResponseType, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    class ReliableSubscribe : Subscribe<ReliableSubscribeRequestType, DsspResponsePort<SubscribeResponseType>, TrackRoamerBrickProximityBoardOperations> { }

    /// <summary>
    /// Replace message.
    /// Send this message to the SickLRF service port to replace the state of the service.
    /// </summary>
    [DisplayName("Measurement")]
    [Description("Indicates when the Proximity Board reports a new measurement.")]
    class Replace : Replace<TrackRoamerBrickProximityBoardState, DsspResponsePort<DefaultReplaceResponseType>> { }

    [Description("Resets the Proximity Board.")]
    class Reset : Submit<ResetType, DsspResponsePort<DefaultSubmitResponseType>>
    {
    }
    /// <summary>
    /// ResetType
    /// </summary>
    [DataContract]
    public class ResetType
    {
    }

    // Notification messages:

    [DisplayName("UpdateSonarData")]
    [Description("Updates or indicates an arrival of the Sonar sweep frame.")]
    public class UpdateSonarData : Update<SonarDataDssSerializable, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }

    [DisplayName("UpdateDirectionData")]
    [Description("Updates or indicates an arrival of the Direction data.")]
    public class UpdateDirectionData : Update<DirectionDataDssSerializable, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }

    [DisplayName("UpdateAccelerometerData")]
    [Description("Updates or indicates an arrival of the Accelerometer data.")]
    public class UpdateAccelerometerData : Update<AccelerometerDataDssSerializable, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }

    [DisplayName("UpdateProximityData")]
    [Description("Updates or indicates an arrival of the Proximity data.")]
    public class UpdateProximityData : Update<ProximityDataDssSerializable, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }

    [DisplayName("UpdateParkingSensorData")]
    [Description("Updates or indicates an arrival of the Parking Sensor data.")]
    public class UpdateParkingSensorData : Update<ParkingSensorDataDssSerializable, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }

    [DisplayName("UpdateAnalogData")]
    [Description("Updates or indicates an arrival of the analog data (pin 2 AN0 of PIC4550 etc.).")]
    public class UpdateAnalogData : Update<AnalogDataDssSerializable, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }

}


