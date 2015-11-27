<?xml version="1.0" encoding="utf-8"?>
<!--
    This file is part of Microsoft Robotics Developer Studio Code Samples.
    Copyright (C) Microsoft Corporation.  All rights reserved.
    $File: MicrosoftGpsMap.user.xslt $ $Revision: 1 $
-->
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:gps="http://schemas.microsoft.com/robotics/2007/03/microsoftgps.user.html"
  >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        Microsoft Gps Service
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides mapping and navigation data using Microsoft Virtual Earth V6.3.
      </xsl:with-param>
      <xsl:with-param name="head">
        <link href="/mountpoint/store/styles/microsoftgps.css" rel="stylesheet" type="text/css" />
        <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.3"></script>
          <script language="javascript" type="text/javascript">
          <xsl:comment>
            <![CDATA[
var map = null;
var LoadErr = 0;
var pinID = 1;

function GetMap(lat, lon)
{
  try
  {
    map = new VEMap('GpsMap');
    // SetDashboardSize must be called before calling LoadMap
    map.SetDashboardSize(VEDashboardSize.Small);
    map.LoadMap(new VELatLong(lat, lon), 19 ,'h' , false);
    //map.LoadMap(new VELatLong(lat, lon), 9, VEMapStyle.Road);
    AddPin(lat, lon);
    map.ShowDashboard();
  }
  catch(ex)
  {
    alert('The Virtual Earth Mapping Control failed to load!\r\rPlease verify your internet connection and\rAdd *.virtualearth.net to your trusted sites list.\r\rError: ' + ex.message);
  }
}

function AddPin(lat, lon)
{
  try
  {
    var pin = new VEPushpin(pinID,
      new VELatLong(lat, lon),
      null,
          'Your GPS',
          'You are here.'
    );

    map.AddPushpin(pin);
    pinID++;
  }
  catch(ex)
  {
    // Pushpin didn't work!
  }
}           ]]>

dssRuntime.init = function()
{
    <xsl:value-of select="concat('GetMap(',/gps:GpsState/gps:GpRmc/gps:Latitude,',',/gps:GpsState/gps:GpRmc/gps:Longitude,');')" />
}
            //
          </xsl:comment>
        </script>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="/gps:GpsState">
    <xsl:apply-templates select="gps:GpGll"/>
    <xsl:apply-templates select="gps:GpRmc"/>
  </xsl:template>

  <xsl:template match="gps:GpGll">
    <div id='GpsMap' style="position:relative ; width:800px; height:600px;"></div>
  </xsl:template>
  <xsl:template match="gps:GpRmc">
    <div id='GpsMap' style="position:relative ; width:800px; height:600px;"></div>
  </xsl:template>

</xsl:stylesheet>
