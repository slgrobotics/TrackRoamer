//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: SickLRFTypes.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;


using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using sicklrf = TrackRoamer.Robotics.Services.TrackRoamerUsrf;
using Microsoft.Dss.Core.DsspHttp;
using System.ComponentModel;

namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
    /// <summary>
    /// Contract type
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// SickLRF service namespace
        /// </summary>
        public const string Identifier = "http://schemas.trackroamer.com/robotics/2009/04/usrf.html";
    }

    /// <summary>
    /// Main service port type
    /// </summary>
    class SickLRFOperations
        : PortSet<
            DsspDefaultLookup,
            DsspDefaultDrop,
            Get,
            Replace,
            ReliableSubscribe,
            Subscribe,
            Reset,
            HttpGet>
    {
    }

    /// <summary>
    /// Get message.
    /// Send this message to the SickLRF service port to get the state of the service.
    /// </summary>
    [Description("Gets the current state of the laser range finder.")]
    class Get : Get<GetRequestType, DsspResponsePort<State>>{}

    /// <summary>
    /// Replace message.
    /// Send this message to the SickLRF service port to replace the state of the service.
    /// </summary>
    [DisplayName ("Measurement")]
    [Description("Indicates when the laser range finder reports a new measurement.")]
    class Replace : Replace<State, DsspResponsePort<DefaultReplaceResponseType>> {}

    /// <summary>
    /// Subscribe message.
    /// Send this message to the SickLRF service port to subscribe to the SickLRF service.
    /// </summary>
    class Subscribe : Subscribe<SubscribeRequestType, DsspResponsePort<SubscribeResponseType>, SickLRFOperations> {}

    class ReliableSubscribe : Subscribe<ReliableSubscribeRequestType, DsspResponsePort<SubscribeResponseType>, SickLRFOperations> { }

    [Description("Resets the laser range finder.")]
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
}