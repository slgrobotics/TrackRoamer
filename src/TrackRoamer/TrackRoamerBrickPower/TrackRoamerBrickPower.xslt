<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:trackroamerbrickpower="http://schemas.trackroamer.com/robotics/2009/04/trackroamerbrickpower.html">

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer Power Brick (wheel controller hardware) State
      </xsl:with-param>
      <xsl:with-param name="description">
        View the TrackRoamer Brick (wheel controller hardware) State. The Brick directly communicates to RoboteQ AX2850 controller.
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="s:Header">

  </xsl:template>

  <xsl:template match="trackroamerbrickpower:TrackRoamerBrickPowerState">
    <table width="100%">
      <tr>
        <th width="20%">
          Delay (ms):
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:PowerControllerConfig/trackroamerbrickpower:Delay"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Serial Port:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:PowerControllerConfig/trackroamerbrickpower:PortName"/>
          (<xsl:value-of select="trackroamerbrickpower:PowerControllerConfig/trackroamerbrickpower:BaudRate"/> Baud)
        </td>
      </tr>
      <tr>
        <th width="20%">
          Connected:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:Connected"/>
          (<xsl:value-of select="trackroamerbrickpower:PowerControllerConfig/trackroamerbrickpower:ConfigurationStatus"/>)
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Connect Attempts:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:ConnectAttempts"/>
        </td>
      </tr>
      <tr>
        <th width="20%">
          Frame Counter (frames exchanged):
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:FrameCounter"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Frame Rate (frames/sec):
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:FrameRate"/>
        </td>
      </tr>
      <tr>
        <th width="20%">
          Error Counter:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:ErrorCounter"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Error Rate (errors/sec):
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:ErrorRate"/>
        </td>
      </tr>
    </table>

    <table>
      <tr>
        <th width="20%">
          Analog_Input_1:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Analog_Input_1"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Analog_Input_2:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Analog_Input_2"/>
        </td>
      </tr>

      <tr>
        <th width="20%">
          Main_Battery_Voltage:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Main_Battery_Voltage"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Internal_Voltage:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Internal_Voltage"/>
        </td>
      </tr>

      <tr>
        <th width="20%">
          Digital_Input_E:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Digital_Input_E"/>
        </td>
      </tr>

      <tr class="odd">
        <th width="20%">
          OutputC :
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:OutputC "/>
        </td>
      </tr>

      <tr>
        <th width="20%">
          Time Stamp:
        </th>
        <td width="80%">
          <xsl:value-of select="trackroamerbrickpower:TimeStamp"/>
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
        <th>
          Speed:
        </th>
        <td>
          <xsl:value-of select="trackroamerbrickpower:MotorSpeed/trackroamerbrickpower:LeftSpeed"/>
        </td>
        <td>
          <xsl:value-of select="trackroamerbrickpower:MotorSpeed/trackroamerbrickpower:RightSpeed"/>
        </td>
      </tr>
      <tr class="odd">
        <th>
          Whisker:
        </th>
        <td>
          <xsl:choose>
            <xsl:when test='trackroamerbrickpower:Whiskers/trackroamerbrickpower:FrontWhiskerLeft = "true"'>
              <xsl:attribute name="bgcolor">
                <xsl:text disable-output-escaping="yes">#f67f7f</xsl:text>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="bgcolor">
                <xsl:text disable-output-escaping="yes">#9ecba3</xsl:text>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:value-of select="trackroamerbrickpower:Whiskers/trackroamerbrickpower:FrontWhiskerLeft"/>
        </td>
        <td>
          <xsl:choose>
            <xsl:when test='trackroamerbrickpower:Whiskers/trackroamerbrickpower:FrontWhiskerRight = "true"'>
              <xsl:attribute name="bgcolor">
                <xsl:text disable-output-escaping="yes">#f67f7f</xsl:text>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="bgcolor">
                <xsl:text disable-output-escaping="yes">#9ecba3</xsl:text>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:value-of select="trackroamerbrickpower:Whiskers/trackroamerbrickpower:FrontWhiskerRight"/>
        </td>
      </tr>
      <tr>
        <th>
          Motor Power:
        </th>
        <td>
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Motor_Power_Left"/>
        </td>
        <td>
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Motor_Power_Right"/>
        </td>
      </tr>
      <tr class="odd">
        <th>
          Motor Amps:
        </th>
        <td>
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Motor_Amps_Left"/>
        </td>
        <td>
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Motor_Amps_Right"/>
        </td>
      </tr>
      <tr>
        <th>
          Heatsink Temperature:
        </th>
        <td>
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Heatsink_Temperature_Left"/>
        </td>
        <td>
          <xsl:value-of select="trackroamerbrickpower:PowerControllerState/trackroamerbrickpower:Heatsink_Temperature_Right"/>
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
