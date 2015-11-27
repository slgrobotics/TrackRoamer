<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:motor="http://schemas.microsoft.com/robotics/2006/05/motor.html"
    xmlns:physical="http://schemas.microsoft.com/robotics/2006/07/physicalmodel.html">

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer Motor State
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides access to the TrackRoamerBot motor.<br /> (Uses the Generic Motor contract)
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="s:Header">

  </xsl:template>

  <xsl:template match="motor:MotorState">
    <table width="100%">
      <tr>
        <th width="20%">
          Name:
        </th>
        <td width="80%">
          <xsl:value-of select="motor:Name"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Identifier:
        </th>
        <td width="80%">
          <xsl:value-of select="motor:HardwareIdentifier"/>
        </td>
      </tr>
      <tr>
        <th width="20%">
          Current Power:
        </th>
        <td width="80%">
          <xsl:value-of select="motor:CurrentPower"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Power Scaling Factor:
        </th>
        <td width="80%">
          <xsl:value-of select="motor:PowerScalingFactor"/>
        </td>
      </tr>
      <tr>
        <th width="20%">
          Reverse Polarity:
        </th>
        <td width="80%">
          <xsl:value-of select="motor:ReversePolarity"/>
        </td>
      </tr>
      <tr class="odd">
        <th>Position X:</th>
        <td>
          <xsl:value-of select="motor:Pose/physical:Position/physical:X"/>
        </td>
      </tr>
      <tr>
        <th>Position Y:</th>
        <td>
          <xsl:value-of select="motor:Pose/physical:Position/physical:Y"/>
        </td>
      </tr>
      <tr class="odd">
        <th>Position Z:</th>
        <td>
          <xsl:value-of select="motor:Pose/physical:Position/physical:Z"/>
        </td>
      </tr>
      <tr>
        <th>Orientation X:</th>
        <td>
          <xsl:value-of select="motor:Pose/physical:Orientation/physical:X"/>
        </td>
      </tr>
      <tr class="odd">
        <th>Orientation Y:</th>
        <td>
          <xsl:value-of select="motor:Pose/physical:Orientation/physical:Y"/>
        </td>
      </tr>
      <tr>
        <th>Orientation Z:</th>
        <td>
          <xsl:value-of select="motor:Pose/physical:Orientation/physical:Z"/>
        </td>
      </tr>
      <tr class="odd">
        <th>Orientation W:</th>
        <td>
          <xsl:value-of select="motor:Pose/physical:Orientation/physical:W"/>
        </td>
      </tr>
      <tr>
        <th>Time Stamp:</th>
        <td>
          <xsl:value-of select="motor:TimeStamp"/>
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
