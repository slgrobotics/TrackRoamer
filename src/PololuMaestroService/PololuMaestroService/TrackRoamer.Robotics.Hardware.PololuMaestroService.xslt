<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:pololumaestroservice="http://schemas.trackroamer.com/2012/02/pololumaestroservice.html" >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        Pololu Maestro State
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides access to the Pololu Maestro Servo Controller.
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="pololumaestroservice:PololuMaestroServiceState">
    
    <h3>Safe Positions</h3>
    <p>this service will set channels (0...23) to values (microseconds, typically 850...2150) listed below when it starts and when it terminates.</p>
  
    <table width="100%">
      <tr class="odd">
        <th width="20%">
          Channel
        </th>
        <th>
          Safe Position us
        </th>
      </tr>
      <xsl:for-each select="pololumaestroservice:SafePositions/pololumaestroservice:SafePosition">
        <tr>
          <td>
            <xsl:value-of select="pololumaestroservice:channel"/>
          </td>
          <td>
            <xsl:value-of select="pololumaestroservice:positionUs"/>
          </td>
        </tr>
      </xsl:for-each>
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