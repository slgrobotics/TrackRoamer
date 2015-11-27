//------------------------------------------------------------------------------
// TrackRoamerEncoderTypes.cs
//
//     TrackRoamer Encoder. Provides access to an encoder.
//
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Dss.Core.Attributes;

using pxencoder = Microsoft.Robotics.Services.Encoder.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerServices.Encoder
{
    /// <summary>
    /// Encoder Contract
    /// </summary>
    [DisplayName("TrackRoamer Encoder")]
    [Description("Provides access to an encoder.")]
    [DssServiceDescription("http://msdn.microsoft.com/library/dd145252.aspx")]
    public static class Contract
    {
		public const string Identifier = "http://schemas.trackroamer.com/robotics/2009/04/trackroamerencoder.html";
    }
}
