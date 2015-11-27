<?xml version="1.0" encoding="utf-8"?>
<!-- 
    Original:
    This file is part of Microsoft Robotics Developer Studio Code Samples.
    Copyright (C) Microsoft Corporation.  All rights reserved.
    $File: RoombaGenericDriveState.xslt $ $Revision: 1 $
-->
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:d="http://schemas.microsoft.com/robotics/2006/05/drive.html"
    xmlns:m="http://schemas.microsoft.com/robotics/2006/05/motor.html"
    xmlns:e="http://schemas.microsoft.com/robotics/2006/05/encoder.html"
                >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer Differential Drive
      </xsl:with-param>
      <xsl:with-param name="description">
        View the TrackRoamer Differential Drive State
      </xsl:with-param>
    </xsl:call-template>
    <style>
      TH
      {
      text-align: right;
      }
    </style>
  </xsl:template>

  <xsl:template match="s:Header">

  </xsl:template>

  <xsl:template match="d:DriveDifferentialTwoWheelState">
    <table>
      <tr class="odd">
        <th style="min-width:250px;">Is Enabled:</th>
        <td style="min-width:425px;text-align:center;">
          <xsl:value-of select="d:IsEnabled"/>
        </td>
      </tr>
      <tr>
        <th>Distance Between Wheels:</th>
        <td>
          <xsl:value-of select="d:DistanceBetweenWheels"/>
        </td>
      </tr>
      <tr class="odd">
        <th>TimeStamp:</th>
        <td>
          <xsl:value-of select="d:TimeStamp"/>
        </td>
      </tr>
      <tr>
        <th>DriveDistanceStage:</th>
        <td>
          <xsl:value-of select="d:DriveDistanceStage"/>
        </td>
      </tr>
      <tr class="odd">
        <th>RotateDegreesStage:</th>
        <td>
          <xsl:value-of select="d:RotateDegreesStage"/>
        </td>
      </tr>
      <tr>
        <th>DriveState:</th>
        <td>
          <xsl:value-of select="d:DriveState"/>
        </td>
      </tr>
    </table>

    <table>
      <tr>
        <th style="min-width:250px;">*</th>
        <th style="min-width:200px;text-align:center;">Left</th>
        <th style="min-width:200px;text-align:center;">Right</th>
      </tr>

      <tr>
        <th>Wheel Speed:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:WheelSpeed"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:WheelSpeed"/>
        </td>
      </tr>
      <tr class="odd">
        <th>Wheel Radius:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:Radius"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:Radius"/>
        </td>
      </tr>
      <tr>
        <th>Gear Ratio:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:GearRatio"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:GearRatio"/>
        </td>
      </tr>
      <tr class="odd">
        <th>MotorState Name:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:MotorState/m:Name"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:MotorState/m:Name"/>
        </td>
      </tr>
      <tr>
        <th>MotorState Hardware Identifier:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:MotorState/m:HardwareIdentifier"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:MotorState/m:HardwareIdentifier"/>
        </td>
      </tr>
      <tr class="odd">
        <th>MotorState Reverse Polarity:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:MotorState/m:ReversePolarity"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:MotorState/m:ReversePolarity"/>
        </td>
      </tr>
      <tr>
        <th>MotorState Power Scaling Factor:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:MotorState/m:PowerScalingFactor"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:MotorState/m:PowerScalingFactor"/>
        </td>
      </tr>
      <tr class="odd">
        <th>MotorState Current Power:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:MotorState/m:CurrentPower"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:MotorState/m:CurrentPower"/>
        </td>
      </tr>
      <tr>
        <th>EncoderState TimeStamp:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:EncoderState/e:TimeStamp"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:EncoderState/e:TimeStamp"/>
        </td>
      </tr>
      <tr class="odd">
        <th>EncoderState TicksSinceReset:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:EncoderState/e:TicksSinceReset"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:EncoderState/e:TicksSinceReset"/>
        </td>
      </tr>
      <tr>
        <th>EncoderState CurrentAngle:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:EncoderState/e:CurrentAngle"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:EncoderState/e:CurrentAngle"/>
        </td>
      </tr>
      <tr class="odd">
        <th>EncoderState CurrentReading:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:EncoderState/e:CurrentReading"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:EncoderState/e:CurrentReading"/>
        </td>
      </tr>
      <tr>
        <th>EncoderState TicksPerRevolution:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:EncoderState/e:TicksPerRevolution"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:EncoderState/e:TicksPerRevolution"/>
        </td>
      </tr>
      <tr class="odd">
        <th>EncoderState HardwareIdentifier:</th>
        <td>
          <xsl:value-of select="d:LeftWheel/m:EncoderState/e:HardwareIdentifier"/>
        </td>
        <td>
          <xsl:value-of select="d:RightWheel/m:EncoderState/e:HardwareIdentifier"/>
        </td>
      </tr>

    </table>
    
    <script type="text/javascript">

      var refreshEnabled = true;

      function toggleRefresh() {
      if(refreshEnabled) {
      refreshEnabled = false;
      } else {
      location.reload(true);
      }
      }

      function timedRefresh(timeoutPeriod) {
      setTimeout(doRefresh,timeoutPeriod);
      }

      function doRefresh()
      {
      if(refreshEnabled) {
      location.reload(true);
      }
      }
      timedRefresh(1000);
    </script>

  </xsl:template>
</xsl:stylesheet>
