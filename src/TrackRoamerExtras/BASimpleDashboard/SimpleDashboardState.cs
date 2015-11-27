//-----------------------------------------------------------------------
//  This file is part of the Microsoft Robotics Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: SimpleDashboardState.cs $ $Revision: 6 $
//-----------------------------------------------------------------------

using Microsoft.Dss.Core.Attributes;
using System;
using System.Collections.Generic;

using drive = Microsoft.Robotics.Services.Drive.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

namespace Microsoft.Robotics.Services.SimpleDashboard
{
    /// <summary>
    /// SimpleDashboard StateType
    /// </summary>
    [DataContract]
    public class SimpleDashboardState
    {
        [DataMember]
        public bool Log;
        [DataMember]
        public string LogFile;
    }
}
