<?xml version="1.0" encoding="utf-8"?>
<!--
    This file is part of Microsoft Robotics Developer Studio Code Samples.
    Copyright (C) Microsoft Corporation.  All rights reserved.
    $File: MicrosoftGps.user.xslt $ $Revision: 1 $
-->
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:gps="http://schemas.microsoft.com/robotics/2007/03/microsoftgps.user.html"
  >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        Microsoft GPS Service - Sirf III, Sirf IV, MT3329 and others
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides GPS data in NMEA 0183 output format.
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="/gps:GpsState">
    <table width="100%">
      <tr>
        <td>
          <a href="#" onclick="self.location.href = self.location.href + '/top'; return false;">
            Top View
          </a>
        </td>
      </tr>
      <tr>
        <td>
          <a href="#" onclick="self.location.href = self.location.href + '/map'; return false;">
            Map View
          </a>
        </td>
      </tr>

      <tr>
        <th colspan="3" class="Major">Configuration</th>
      </tr>
      <xsl:apply-templates select="gps:MicrosoftGpsConfig"/>

      <tr>
        <th colspan="3" class="Major">Gps Data</th>
      </tr>
      <xsl:apply-templates select="gps:GpGga"/>
      <xsl:apply-templates select="gps:GpGll"/>
      <xsl:apply-templates select="gps:GpGsa"/>
      <xsl:apply-templates select="gps:GpGsv"/>
      <xsl:apply-templates select="gps:GpRmc"/>
      <tr>
        <th>
          <xsl:if test="gps:Connected = 'true'">
            <xsl:text>Connected</xsl:text>
          </xsl:if>
          <xsl:if test="gps:Connected != 'true'">
            <xsl:attribute name="style">
              <xsl:text>background:red;</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="gps:MicrosoftGpsConfig/gps:ConfigurationStatus"/>
          </xsl:if>
        </th>
        <th class="Minor" >
          <xsl:text>Last Updated</xsl:text>
        </th>
        <th style="text-align:right">
          <xsl:value-of select="substring-before(gps:GpRmc/gps:LastUpdate,'T')"/>
          <xsl:text> </xsl:text>
          <xsl:value-of select="substring-after(gps:GpRmc/gps:LastUpdate,'T')"/>
        </th>
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="gps:GpGga">
    <tr>
      <th colspan="3" class="Major">Time, position and fix type data (GPGGA)</th>
    </tr>
    <tr>
      <xsl:if test="gps:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <td>Position Fix Indicator</td>
      <td>
          <xsl:value-of select="gps:PositionFixIndicator"/>
      </td>
    </tr>
    <tr>
      <xsl:if test="gps:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <td>Altitude</td>
      <td>
        <xsl:value-of select="gps:MslAltitudeMeters"/>
      </td>
      <td>
        <xsl:value-of select="gps:AltitudeUnits"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
          <xsl:value-of select="substring-before(gps:LastUpdate,'T')"/>
          <xsl:text> </xsl:text>
          <xsl:value-of select="substring-before(substring-after(gps:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="gps:GpGll">
    <tr>
      <xsl:if test="gps:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">Latitude and Longitude (GPGLL)</th>
    </tr>
    <tr>
      <td>Latitude</td>
      <td>
          <xsl:value-of select="gps:Latitude"/>
      </td>
    </tr>
    <tr>
      <td>Longitude</td>
      <td>
          <xsl:value-of select="gps:Longitude"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(gps:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(gps:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="gps:GpGsv">
    <tr>
      <th colspan="3" class="Major">Satellites in view (GPGSV)</th>
    </tr>
    <tr>
      <xsl:if test="gps:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <td>Satellites In View</td>
      <td>
          <xsl:value-of select="gps:SatellitesInView"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(gps:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(gps:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="gps:GpGsa">
    <tr>
      <xsl:if test="gps:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">GPS receiver operating mode (GPGSA)</th>
    </tr>
    <tr>
      <td>Status</td>
      <td>
          <xsl:value-of select="gps:Status"/>
      </td>
    </tr>
    <tr>
      <td>Mode</td>
      <td>
          <xsl:value-of select="gps:Mode"/>
      </td>
    </tr>
    <tr>
      <td>Position Dilution Of Precision</td>
      <td>
          <xsl:value-of select="gps:SphericalDilutionOfPrecision"/>
      </td>
    </tr>
    <tr>
      <td>Horizontal Dilution Of Precision</td>
      <td>
          <xsl:value-of select="gps:HorizontalDilutionOfPrecision"/>
      </td>
    </tr>
    <tr>
      <td>Vertical Dilution Of Precision</td>
      <td>
          <xsl:value-of select="gps:VerticalDilutionOfPrecision"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(gps:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(gps:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="gps:GpRmc">
    <tr>
      <xsl:if test="gps:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">Time, date, position, course and speed data (GPRMC)</th>
    </tr>
    <tr>
      <td>Latitude</td>
      <td>
          <xsl:value-of select="gps:Latitude"/>
      </td>
    </tr>
    <tr>
      <td>Longitude</td>
      <td>
          <xsl:value-of select="gps:Longitude"/>
      </td>
    </tr>
    <tr>
      <td>Status</td>
      <td>
          <xsl:value-of select="gps:Status"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(gps:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(gps:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="gps:MicrosoftGpsConfig">
    <tr>
      <xsl:if test="gps:CommPort = '0'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <td colspan="3">
        <form method="POST" action="" name="ConfigurationForm" style="padding:0px;margin:0px;" width="100%">
          <input type="hidden" name="Action" value="MicrosoftGpsConfig" />
          <table>
            <tr>
              <td rowspan="3">
                <input id="Checkbox1" type="checkbox" name="CaptureHistory" title="Capture History">
                  <xsl:if test="gps:CaptureHistory = 'true'">
                    <xsl:attribute name="checked">CHECKED</xsl:attribute>
                    <xsl:attribute name="value">on</xsl:attribute>
                  </xsl:if>
                </input>
                <xsl:text> Capture History</xsl:text>
                <br/>
                <input id="Checkbox2" type="checkbox" name="CaptureNmea" title="Capture NMEA">
                  <xsl:if test="gps:CaptureNmea = 'true'">
                    <xsl:attribute name="checked">CHECKED</xsl:attribute>
                    <xsl:attribute name="value">on</xsl:attribute>
                  </xsl:if>
                </input>
                <xsl:text> Capture NMEA</xsl:text>
                <br/>
                <input id="Checkbox3" type="checkbox" name="RetrackNmea" title="Retrack Simulate NMEA">
                  <xsl:if test="gps:RetrackNmea = 'true'">
                    <xsl:attribute name="checked">CHECKED</xsl:attribute>
                    <xsl:attribute name="value">on</xsl:attribute>
                  </xsl:if>
                </input>
                <xsl:text> Retrack/Simulate NMEA</xsl:text>
              </td>
              <td rowspan="3" width="40">
              </td>
              <td>Comm Port:</td>
              <td align="right">
                <input>
                  <xsl:attribute name="type">text</xsl:attribute>
                  <xsl:attribute name="name">CommPort</xsl:attribute>
                  <xsl:attribute name="size">20</xsl:attribute>
                  <xsl:attribute name="value">
                    <xsl:value-of select="gps:CommPort"/>
                  </xsl:attribute>
                </input>
              </td>
              <td align="center">
                <input id="Button2" type="SUBMIT" value="Search" name="buttonOk" title="Search for Gps" />
              </td>
            </tr>
            <tr>
              <td>Baud Rate:</td>
              <td align="right">
                <xsl:element name="input">
                  <xsl:attribute name="type">text</xsl:attribute>
                  <xsl:attribute name="name">BaudRate</xsl:attribute>
                  <xsl:attribute name="size">20</xsl:attribute>
                  <xsl:attribute name="value">
                    <xsl:value-of select="gps:BaudRate"/>
                  </xsl:attribute>
                </xsl:element>
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td align="right">
                <input id="Button1" type="SUBMIT" value="Connect and Update" name="buttonOk" title="Connect and Update" />
              </td>
              <td></td>
            </tr>
          </table>
        </form>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
