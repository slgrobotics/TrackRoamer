<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:contactsensor="http://schemas.microsoft.com/2006/06/contactsensor.html" >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer Contact Sensor State
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides access to the TrackRoamerBot Whiskers, used as a bumper.<br />(Uses Generic Contact Sensors contract)
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="s:Header">

  </xsl:template>

	<xsl:template match="contactsensor:Sensors/contactsensor:ContactSensor">
    <table width="100%">
      <tr>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">odd</xsl:when>
          </xsl:choose>
        </xsl:attribute>
        <td>
          <table width="100%" border="0" cellpadding="5" cellspacing="5">
            <tr>
              <th width="20%">
                Name
              </th>
              <td width="80%">
                <xsl:value-of select="contactsensor:Name"/>
              </td>
            </tr>
            <tr>
              <th width="20%">
                HardwareIdentifier
              </th>
              <td width="80%">
                <xsl:value-of select="contactsensor:HardwareIdentifier"/>
              </td>
            </tr>
            <tr>
              <th width="20%">
                Pressed
              </th>
              <td width="80%">
                <xsl:choose>
                  <xsl:when test='contactsensor:Pressed = "true"'>
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
                <xsl:value-of select="contactsensor:Pressed"/>
              </td>
            </tr>
            <tr>
              <th width="20%">
                TimeStamp
              </th>
              <td width="80%">
                <xsl:value-of select="contactsensor:TimeStamp"/>
              </td>
            </tr>
          </table>
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
