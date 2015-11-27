<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:encoder="http://schemas.microsoft.com/robotics/2006/05/encoder.html" >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer Encoder State
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides access to the TrackRoamerBot encoder. (Uses the Generic Encoder contract.)
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="s:Header">

  </xsl:template>

  <xsl:template match="encoder:EncoderState">
    <table width="100%">
      <tr class="odd">
        <th width="20%">
          Identifier (1-Left, 2-Right):
        </th>
        <td width="80%">
          <xsl:value-of select="encoder:HardwareIdentifier"/>
        </td>
      </tr>
      <tr>
        <th width="20%">
          Ticks Since Reset:
        </th>
        <td width="80%">
          <xsl:value-of select="encoder:TicksSinceReset"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Current Reading:
        </th>
        <td width="80%">
          <xsl:value-of select="encoder:CurrentReading"/>
        </td>
      </tr>
      <tr>
        <th width="20%">
          Current Angle:
        </th>
        <td width="80%">
          <xsl:value-of select="encoder:CurrentAngle"/>
        </td>
      </tr>
      <tr class="odd">
        <th width="20%">
          Ticks Per Revolution:
        </th>
        <td width="80%">
          <xsl:value-of select="encoder:TicksPerRevolution"/>
        </td>
      </tr>
      <tr>
        <th width="20%">
          Time Stamp:
        </th>
        <td width="80%">
          <xsl:value-of select="encoder:TimeStamp"/>
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