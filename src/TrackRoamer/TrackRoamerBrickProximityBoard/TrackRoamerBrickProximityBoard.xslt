<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    xmlns:tpb="http://schemas.trackroamer.com/robotics/2011/01/trackroamerbrickproximityboard.html" >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        TrackRoamer Robot Brick Proximity Board State
      </xsl:with-param>
      <xsl:with-param name="description">
        View the TrackRoamer Robot Brick Proximity Board State
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

	<xsl:template match="tpb:TrackRoamerBrickProximityBoardState">
    <table>
      <tr>
        <td valign="top">
          <iframe src="/trproxboard/top" width="600" height="400" frameborder="0" scrolling="no"></iframe>
          <br/>
          <iframe src="/trproxboard/cylinder" width="600" height="100" frameborder="0" scrolling="no"></iframe>
          <br/>
          <table width="600">
            <tr class="odd">
              <th style="width:200px;">IsConnected:</th>
              <td>
                <xsl:value-of select="tpb:IsConnected"/>
              </td>
            </tr>
            <tr>
              <th>VendorId:</th>
              <td>
                <xsl:value-of select="tpb:VendorId"/>
              </td>
            </tr>
            <tr class="odd">
              <th>ProductId:</th>
              <td>
                <xsl:value-of select="tpb:ProductId"/>
              </td>
            </tr>
            <tr>
              <th>LastSampleTimestamp:</th>
              <td>
                <xsl:value-of select="tpb:LastSampleTimestamp"/>
              </td>
            </tr>
            <tr class="odd">
              <th>LinkState:</th>
              <td>
                <xsl:value-of select="tpb:LinkState"/>
              </td>
            </tr>
            <tr>
              <th>Description:</th>
              <td>
                <xsl:value-of select="tpb:Description"/>
              </td>
            </tr>
            <tr class="odd">
              <th>MostRecentSonar Count:</th>
              <td>
                <xsl:value-of select="tpb:MostRecentSonar/tpb:Count"/>
              </td>
            </tr>
            <tr>
              <th>AngularRange:</th>
              <td>
                <xsl:value-of select="tpb:AngularRange"/>
              </td>
            </tr>
            <tr class="odd">
              <th>AngularResolution:</th>
              <td>
                <xsl:value-of select="tpb:AngularResolution"/>
              </td>
            </tr>
            <tr>
              <th>MostRecentAccelerometer:</th>
              <td>
                <xsl:value-of select="tpb:MostRecentAccelerometer/tpb:TimeStamp"/>
                <br/>
                X=<xsl:value-of select="tpb:MostRecentAccelerometer/tpb:accX"/>
                <br/>
                Y=<xsl:value-of select="tpb:MostRecentAccelerometer/tpb:accY"/>
                <br/>
                Z=<xsl:value-of select="tpb:MostRecentAccelerometer/tpb:accZ"/>
              </td>
            </tr>
            <tr class="odd">
              <th>MostRecentProximity:</th>
              <td>
                <xsl:value-of select="tpb:MostRecentProximity/tpb:TimeStamp"/>
                <br/>
                <xsl:value-of select="tpb:MostRecentProximity/tpb:fl"/>-
                <xsl:value-of select="tpb:MostRecentProximity/tpb:ffl"/>-
                <xsl:value-of select="tpb:MostRecentProximity/tpb:ffr"/>-
                <xsl:value-of select="tpb:MostRecentProximity/tpb:fr"/>-
                <xsl:value-of select="tpb:MostRecentProximity/tpb:bl"/>-
                <xsl:value-of select="tpb:MostRecentProximity/tpb:bbl"/>-
                <xsl:value-of select="tpb:MostRecentProximity/tpb:bbr"/>-
                <xsl:value-of select="tpb:MostRecentProximity/tpb:br"/>
              </td>
            </tr>
            <tr>
              <th>MostRecentDirection:</th>
              <td>
                <xsl:value-of select="tpb:MostRecentDirection/tpb:TimeStamp"/>
                <br/>
                heading=<xsl:value-of select="tpb:MostRecentDirection/tpb:heading"/>
                <br/>
                bearing=<xsl:value-of select="tpb:MostRecentDirection/tpb:bearing"/>
              </td>
            </tr>
            <tr class="odd">
              <th>MostRecentParkingSensor:</th>
              <td>
                <xsl:value-of select="tpb:MostRecentParkingSensor/tpb:TimeStamp"/>
                <br/>
                parkingSensorMetersLF=<xsl:value-of select="tpb:MostRecentParkingSensor/tpb:parkingSensorMetersLF"/>
                <br/>
                parkingSensorMetersRF=<xsl:value-of select="tpb:MostRecentParkingSensor/tpb:parkingSensorMetersRF"/>
                <br/>
                parkingSensorMetersLB=<xsl:value-of select="tpb:MostRecentParkingSensor/tpb:parkingSensorMetersLB"/>
                <br/>
                parkingSensorMetersRB=<xsl:value-of select="tpb:MostRecentParkingSensor/tpb:parkingSensorMetersRB"/>
              </td>
            </tr>
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