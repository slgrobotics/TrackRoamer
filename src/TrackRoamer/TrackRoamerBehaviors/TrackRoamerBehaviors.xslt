<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:tpb="http://schemas.trackroamer.com/robotics/2011/01/trackroamerbrickproximityboard.html"
    xmlns:behaviors="http://schemas.trackroamer.com/robotics/2009/04/trackroamerbehaviors.html" >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer Robot Behaviors State
      </xsl:with-param>
      <xsl:with-param name="description">
        View the TrackRoamer Robot Behaviors State
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

	<xsl:template match="behaviors:TrackRoamerBehaviorsState">
    <xsl:value-of select="behaviors:MovingState"/> ====== <xsl:value-of select="behaviors:MovingStateDetail"/>
    <table>
      <tr>
        <td valign="top">
          <img src="/trackroamerbehaviors/north" width="600" height="400" border="0" onclick="javascript:toggleRefresh()"></img>
          <br/>
          <table width="600">
            <tr class="odd">
              <th style="width:200px;">Robot State:</th>
              <td>
                <xsl:value-of select="behaviors:WorkMode"/>
              </td>
            </tr>
            <tr>
              <th>IsTurning:</th>
              <td>
                <xsl:value-of select="behaviors:IsTurning"/>
              </td>
            </tr>
            <tr class="odd">
              <th>LastTurnStarted:</th>
              <td>
                <xsl:value-of select="behaviors:LastTurnStarted"/>
              </td>
            </tr>
            <tr>
              <th>LastTurnCompleted:</th>
              <td>
                <xsl:value-of select="behaviors:LastTurnCompleted"/>
              </td>
            </tr>
            <tr class="odd">
              <th>Countdown:</th>
              <td>
                <xsl:value-of select="behaviors:Countdown"/>
              </td>
            </tr>
            <tr>
              <th>Dropping:</th>
              <td>
                <xsl:value-of select="behaviors:Dropping"/>
              </td>
            </tr>
            <tr class="odd">
              <th>MovingState:</th>
              <td>
                <xsl:value-of select="behaviors:MovingState"/><br/><xsl:value-of select="behaviors:MovingStateDetail"/>
              </td>
            </tr>
            <tr>
              <th>NewHeading:</th>
              <td>
                <xsl:value-of select="behaviors:NewHeading"/>
              </td>
            </tr>
            <tr class="odd">
              <th>Velocity:</th>
              <td>
                <xsl:value-of select="behaviors:Velocity"/>
              </td>
            </tr>
            <tr>
              <th>Mapped:</th>
              <td>
                <xsl:value-of select="behaviors:Mapped"/>
              </td>
            </tr>
            <tr class="odd">
              <th>MostRecentLaserTimeStamp:</th>
              <td>
                <xsl:value-of select="behaviors:MostRecentLaserTimeStamp"/>
              </td>
            </tr>
            <tr>
              <th>MostRecentAccelerometer:</th>
              <td>
                <xsl:value-of select="behaviors:MostRecentAccelerometer/tpb:TimeStamp"/>
                <br/>
                X=<xsl:value-of select="behaviors:MostRecentAccelerometer/tpb:accX"/>
                <br/>
                Y=<xsl:value-of select="behaviors:MostRecentAccelerometer/tpb:accY"/>
                <br/>
                Z=<xsl:value-of select="behaviors:MostRecentAccelerometer/tpb:accZ"/>
              </td>
            </tr>
            <tr class="odd">
              <th>MostRecentProximity:</th>
              <td>
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:TimeStamp"/>
                <br/>
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:fl"/>-
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:ffl"/>-
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:ffr"/>-
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:fr"/>-
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:bl"/>-
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:bbl"/>-
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:bbr"/>-
                <xsl:value-of select="behaviors:MostRecentProximity/tpb:br"/>
              </td>
            </tr>
            <tr>
              <th>MostRecentDirection:</th>
              <td>
                <xsl:value-of select="behaviors:MostRecentDirection/tpb:TimeStamp"/>
                <br/>
                heading=<xsl:value-of select="behaviors:MostRecentDirection/tpb:heading"/>
                <br/>
                bearing=<xsl:value-of select="behaviors:MostRecentDirection/tpb:bearing"/>
              </td>
            </tr>
            <tr class="odd">
              <th>MostRecentParkingSensor:</th>
              <td>
                <xsl:value-of select="behaviors:MostRecentParkingSensor/tpb:TimeStamp"/>
                <br/>
                parkingSensorMetersLF=<xsl:value-of select="behaviors:MostRecentParkingSensor/tpb:parkingSensorMetersLF"/>
                <br/>
                parkingSensorMetersRF=<xsl:value-of select="behaviors:MostRecentParkingSensor/tpb:parkingSensorMetersRF"/>
                <br/>
                parkingSensorMetersLB=<xsl:value-of select="behaviors:MostRecentParkingSensor/tpb:parkingSensorMetersLB"/>
                <br/>
                parkingSensorMetersRB=<xsl:value-of select="behaviors:MostRecentParkingSensor/tpb:parkingSensorMetersRB"/>
              </td>
            </tr>
            <tr>
              <th>Collision State:</th>
              <td>
                <xsl:value-of select="behaviors:collisionState/behaviors:message"/>
              </td>
            </tr>
            <tr>
              <th>Kinect Sound Direction:</th>
              <td>
                SoundBeam: <xsl:value-of select="behaviors:SoundBeamTimeStamp"/> : 
                <xsl:value-of select="behaviors:SoundBeamDirection"/> degrees<br/>
                Speech: <xsl:value-of select="behaviors:AnySpeechTimeStamp"/> : 
                <xsl:value-of select="behaviors:AnySpeechDirection"/> degrees<br/>
              </td>
            </tr>
            <tr>
              <th>Speech Recognizer:</th>
              <td>
                <xsl:value-of select="behaviors:VoiceCommandState/behaviors:TimeStamp"/><br/> 
                <xsl:value-of select="behaviors:VoiceCommandState/behaviors:Text"/> =>
                <strong><xsl:value-of select="behaviors:VoiceCommandState/behaviors:Semantics"/></strong><br/>
                at <xsl:value-of select="behaviors:VoiceCommandState/behaviors:Direction"/> degrees, confidence
                <xsl:value-of select="behaviors:VoiceCommandState/behaviors:ConfidencePercent"/> %
              </td>
            </tr>
          </table>
        </td>
        <td valign="top">
          <!-- iframe src="/trackroamerbehaviors/composite" width="600" height="300" frameborder="0" scrolling="no"></iframe>
          <br/ -->
          <iframe src="/trackroamerbehaviors/history" width="400" height="620" frameborder="0" scrolling="auto"></iframe>
          <iframe src="/trackroamerbehaviors/decisions" width="400" height="620" frameborder="0" scrolling="auto"></iframe>
          <table width="800">
            <tr>
              <th width="40"></th>
              <th width="80">Is Tracked</th>
              <th width="180">Last Seen</th>
              <th width="80">Pan</th>
              <th width="80">Tilt</th>
              <th></th>
            </tr>
            <xsl:for-each select="behaviors:HumanInteractionStates/behaviors:HumanInteractionState">
              <tr>
                <td align="right">
                  <xsl:value-of select="position()"/>
                </td>
                <td align="center">
                  <xsl:value-of select="behaviors:IsTracked"/>
                </td>
                <td align="center">
                  <xsl:value-of select="behaviors:TimeStamp"/>
                </td>
                <td align="right">
                  <xsl:value-of select="format-number(behaviors:DirectionPan,'##.0')"/>
                </td>
                <td align="right">
                  <xsl:value-of select="format-number(behaviors:DirectionTilt,'##.0')"/>
                </td>
                <td>
                  
                </td>
              </tr>
            </xsl:for-each>
          </table>
          <table>
            <tr>
              <th style="width:230px;">GPS State - GPGGA:<br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPGGA_PositionFixIndicator"/><br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPGGA_LastUpdate"/>
              </th>
              <td style="width:170px;">
                Alt: <xsl:value-of select="behaviors:gpsState/behaviors:GPGGA_AltitudeMeters"/> M<br/>
                Lat: <xsl:value-of select="format-number(behaviors:gpsState/behaviors:GPGGA_Latitude,'##.000000')"/><br/>
                Long: <xsl:value-of select="format-number(behaviors:gpsState/behaviors:GPGGA_Longitude,'##.000000')"/><br/>
                H Dilution: <xsl:value-of select="behaviors:gpsState/behaviors:GPGGA_HorizontalDilutionOfPrecision"/><br/>
                Satellites Used: <xsl:value-of select="behaviors:gpsState/behaviors:GPGGA_SatellitesUsed"/>
              </td>
            </tr>
            <tr>
              <th>GPS State - GPGLL:<br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPGLL_Status"/><br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPGLL_LastUpdate"/>
              </th>
              <td>
                Margin of Error: <xsl:value-of select="behaviors:gpsState/behaviors:GPGLL_MarginOfError"/><br/>                
                Lat: <xsl:value-of select="format-number(behaviors:gpsState/behaviors:GPGLL_Latitude,'##.000000')"/><br/>
                Long: <xsl:value-of select="format-number(behaviors:gpsState/behaviors:GPGLL_Longitude,'##.000000')"/>
              </td>
            </tr>
            <tr>
              <th>GPS State - GPGSA:<br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPGSA_Status"/><br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPGSA_LastUpdate"/>
              </th>
              <td>
                Mode: <xsl:value-of select="behaviors:gpsState/behaviors:GPGSA_Mode"/><br/>
                Spherical Dilution: <xsl:value-of select="behaviors:gpsState/behaviors:GPGSA_SphericalDilutionOfPrecision"/>
                <br/>
              </td>
            </tr>
            <tr>
              <th>
                GPS State - GPGSV:<br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPGSV_LastUpdate"/>
              </th>
              <td>
                Satellites in View: <xsl:value-of select="behaviors:gpsState/behaviors:GPGSV_SatellitesInView"/><br/>
                <br/>
              </td>
            </tr>
            <tr>
              <th>
                GPS State - GPRMC:<br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPRMC_Status"/><br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPRMC_LastUpdate"/>
              </th>
              <td>
                Lat: <xsl:value-of select="format-number(behaviors:gpsState/behaviors:GPRMC_Latitude,'##.000000')"/><br/>
                Long: <xsl:value-of select="format-number(behaviors:gpsState/behaviors:GPRMC_Longitude,'##.000000')"/>
                <br/>
              </td>
            </tr>
            <tr>
              <th>
                GPS State - GPVTG:<br/>
                <xsl:value-of select="behaviors:gpsState/behaviors:GPVTG_LastUpdate"/>
              </th>
              <td>
                Course: <xsl:value-of select="behaviors:gpsState/behaviors:GPVTG_CourseDegrees"/> Degrees<br/>
                Speed: <xsl:value-of select="format-number(behaviors:gpsState/behaviors:GPVTG_SpeedMetersPerSecond,'##.000')"/> M/s
                <br/>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>


    <script type="text/javascript">

      // collapse the left panel to show as much space as possible:

      var navPane = document.getElementById("navPane");
      var dividerArrow = document.getElementById("dividerArrow");

      if (navPane) {
        if (dividerArrow) {
          var isVisible = (navPane.style.display.length == 0);
          if (isVisible) {
            navPane.style.display = "none";
            dividerArrow.src = dividerArrow.src.replace("Collapse", "Expand");
            dividerArrow.title = dividerArrow.title.replace("Collapse", "Expand");
          }
        }
      }

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