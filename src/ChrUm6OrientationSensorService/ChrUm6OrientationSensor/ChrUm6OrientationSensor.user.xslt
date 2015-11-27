<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:chr="http://schemas.trackroamer.com/2011/12/chrum6orientationsensor.html"
  >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        CH Robotics UM6 Orientation Sensor
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides 3D Orientation (AHRS) data from CH Robotics UM6 Orientation Sensor.
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="/chr:ChrUm6OrientationSensorState">
    <table width="100%">

      <tr>
        <th colspan="3" class="Major">Configuration</th>
      </tr>
      <xsl:apply-templates select="chr:ChrUm6OrientationSensorConfig"/>

      <tr>
        <th colspan="3" class="Major">CH Robotics UM6 Orientation Sensor Data</th>
      </tr>
      <xsl:apply-templates select="chr:ProcGyro"/>
      <xsl:apply-templates select="chr:ProcAccel"/>
      <xsl:apply-templates select="chr:ProcMag"/>
      <xsl:apply-templates select="chr:Euler"/>
      <xsl:apply-templates select="chr:Quaternion"/>
      <tr>
        <th>
          <xsl:if test="chr:Connected = 'true'">
            <xsl:text>Connected</xsl:text>
          </xsl:if>
          <xsl:if test="chr:Connected != 'true'">
            <xsl:attribute name="style">
              <xsl:text>background:red;</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="chr:ChrUm6OrientationSensorConfig/chr:ConfigurationStatus"/>
          </xsl:if>
        </th>
        <th class="Minor" >
          <xsl:text>Last Updated</xsl:text>
        </th>
        <th style="text-align:right">
          <xsl:value-of select="substring-before(chr:Quaternion/chr:LastUpdate,'T')"/>
          <xsl:text> </xsl:text>
          <xsl:value-of select="substring-after(chr:Quaternion/chr:LastUpdate,'T')"/>
        </th>
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="chr:ProcGyro">
    <tr>
      <xsl:if test="chr:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">ProcGyro - Processed data from three-axis rate gyro (biases subtracted, scale factors and alignment compensation applied)</th>
    </tr>
    <tr>
      <td>xRate</td>
      <td>
          <xsl:value-of select="chr:xRate"/>
      </td>
    </tr>
    <tr>
      <td>yRate</td>
      <td>
          <xsl:value-of select="chr:yRate"/>
      </td>
    </tr>
    <tr>
      <td>zRate</td>
      <td>
        <xsl:value-of select="chr:zRate"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(chr:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(chr:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="chr:ProcAccel">
    <tr>
      <xsl:if test="chr:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">ProcAccel - Processed data from three-axis accelerometer (biases subtracted, scale factors and alignment compensation applied)</th>
    </tr>
    <tr>
      <td>xAccel</td>
      <td>
        <xsl:value-of select="chr:xAccel"/>
      </td>
    </tr>
    <tr>
      <td>yAccel</td>
      <td>
        <xsl:value-of select="chr:yAccel"/>
      </td>
    </tr>
    <tr>
      <td>zAccel</td>
      <td>
        <xsl:value-of select="chr:zAccel"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(chr:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(chr:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="chr:ProcMag">
    <tr>
      <xsl:if test="chr:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">ProcMag - Processed data from three-axis magnetometer (biases subtracted, soft and hard iron compensation applied)</th>
    </tr>
    <tr>
      <td>x</td>
      <td>
        <xsl:value-of select="chr:x"/>
      </td>
    </tr>
    <tr>
      <td>y</td>
      <td>
        <xsl:value-of select="chr:y"/>
      </td>
    </tr>
    <tr>
      <td>z</td>
      <td>
        <xsl:value-of select="chr:z"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(chr:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(chr:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="chr:Euler">
    <tr>
      <xsl:if test="chr:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">Euler angles (degrees)</th>
    </tr>
    <tr>
      <td>Roll (phi)</td>
      <td>
        <xsl:value-of select="chr:phi"/>
      </td>
    </tr>
    <tr>
      <td>Pitch (theta)</td>
      <td>
        <xsl:value-of select="chr:theta"/>
      </td>
    </tr>
    <tr>
      <td>Yaw (psi)</td>
      <td>
        <xsl:value-of select="chr:psi"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(chr:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(chr:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="chr:Quaternion">
    <tr>
      <xsl:if test="chr:IsValid = 'false'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <th colspan="3" class="Major">Orientation data - Attitude quaternion</th>
    </tr>
    <tr>
      <td>a</td>
      <td>
        <xsl:value-of select="chr:a"/>
      </td>
    </tr>
    <tr>
      <td>b</td>
      <td>
        <xsl:value-of select="chr:b"/>
      </td>
    </tr>
    <tr>
      <td>c</td>
      <td>
        <xsl:value-of select="chr:c"/>
      </td>
    </tr>
    <tr>
      <td>d</td>
      <td>
        <xsl:value-of select="chr:d"/>
      </td>
    </tr>
    <tr>
      <td>Last Updated</td>
      <td colspan="2" style="text-align:right">
        <xsl:value-of select="substring-before(chr:LastUpdate,'T')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-before(substring-after(chr:LastUpdate,'T'),'.')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="chr:ChrUm6OrientationSensorConfig">
    <tr>
      <xsl:if test="chr:CommPort = '0'">
        <xsl:attribute name="style">
          <xsl:text>background:red;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <td colspan="3">
        <form method="POST" action="" name="ConfigurationForm" style="padding:0px;margin:0px;" width="100%">
          <input type="hidden" name="Action" value="ChrUm6OrientationSensorConfig" />
          <table width="100%">
            <tr>
              <td size="10" >Comm Port</td>
              <td align="right" size="21">
                <input>
                  <xsl:attribute name="type">text</xsl:attribute>
                  <xsl:attribute name="name">CommPort</xsl:attribute>
                  <xsl:attribute name="size">20</xsl:attribute>
                  <xsl:attribute name="value">
                    <xsl:value-of select="chr:CommPort"/>
                  </xsl:attribute>
                </input>
              </td>
              <td align="center">
                <input id="Button2" type="SUBMIT" value="Search" name="buttonOk" title="Search for CHR UM6 Orientation Sensor" />
              </td>
            </tr>
            <tr>
              <td>Baud Rate</td>
              <td align="right">
                <xsl:element name="input">
                  <xsl:attribute name="type">text</xsl:attribute>
                  <xsl:attribute name="name">BaudRate</xsl:attribute>
                  <xsl:attribute name="size">20</xsl:attribute>
                  <xsl:attribute name="value">
                    <xsl:value-of select="chr:BaudRate"/>
                  </xsl:attribute>
                </xsl:element>
              </td>
              <td></td>
            </tr>
            <tr>
              <td align="center">
                <input id="Button0" type="SUBMIT" value="Refresh Data" name="buttonOk" title="Refresh data" />
              </td>
              <td align="center">
                <input id="Button1" type="SUBMIT" value="Connect" name="buttonOk" title="Update and Connect" />
              </td>
              <td></td>
            </tr>
            <tr>
              <th colspan="3" class="Major">CH Robotics UM6 Commands</th>
            </tr>
            <tr>
              <td align="center">
                <input id="Button3" type="SUBMIT" value="Zero Rate Gyros" name="buttonOk" title="Zero Rate Gyros" /><br></br>When robot is stopped.
              </td>
              <td align="center">
                <input id="Button4" type="SUBMIT" value="Set Accelerometer Reference Vector" name="buttonOk" title="Set Accelerometer Reference Vector" /><br></br>When robot is level.
              </td>
              <td align="center">
                <input id="Button5" type="SUBMIT" value="Set Magnetometer Reference Vector" name="buttonOk" title="Set Magnetometer Reference Vector" /><br></br>When robot is aligned with magnetic North.
              </td>
            </tr>
          </table>
        </form>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
