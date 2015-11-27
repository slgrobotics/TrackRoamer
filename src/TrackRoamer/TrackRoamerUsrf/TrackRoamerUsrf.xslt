<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:sick="http://schemas.microsoft.com/xw/2005/12/sicklrf.html" >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer USRF State
      </xsl:with-param>
      <xsl:with-param name="description">
        View the TrackRoamer USRF State
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

	<xsl:template match="sick:State">
    <table>
      <tr>
        <td valign="top">
          <iframe src="/usrf/top" width="540" height="270" frameborder="0" scrolling="no"></iframe>
          <br/>
          <iframe src="/usrf/cylinder" width="540" height="100" frameborder="0" scrolling="no"></iframe>
          <br/>
          <table width="540">
            <tr class="odd">
              <th style="width:270px;">Angular Range:</th>
              <td>
                <xsl:value-of select="sick:AngularRange"/>
              </td>
            </tr>
            <tr>
              <th>Units:</th>
              <td>
                <xsl:value-of select="sick:Units"/>
              </td>
            </tr>
            <tr class="odd">
              <th>Angular Resolution:</th>
              <td>
                <xsl:value-of select="sick:AngularResolution"/>
              </td>
            </tr>
            <tr>
              <th>Time Stamp:</th>
              <td>
                <xsl:value-of select="sick:TimeStamp"/>
              </td>
            </tr>
            <tr class="odd">
              <th>Link State:</th>
              <td>
                <xsl:value-of select="sick:LinkState"/>
              </td>
            </tr>

            <xsl:for-each select="sick:DistanceMeasurements/sick:int">
              <tr>
                <xsl:choose>
                  <xsl:when test="not(position() mod 2)">
                    <xsl:attribute name="class">
                      <xsl:text>odd</xsl:text>
                    </xsl:attribute>
                  </xsl:when>
                </xsl:choose>

                <th>
                  <xsl:value-of select="position()"/>
                </th>
                <td>
                  <xsl:value-of select="."/>
                </td>
              </tr>
            </xsl:for-each>
          </table>
        </td>
      </tr>
    </table>


    <script type="text/javascript">
      function timedRefresh(timeoutPeriod) {
        setTimeout(doRefresh,timeoutPeriod);
      }
      function doRefresh()
      {
        location.reload(true);
      }
      timedRefresh(1000);
    </script>
    
  </xsl:template>

</xsl:stylesheet>