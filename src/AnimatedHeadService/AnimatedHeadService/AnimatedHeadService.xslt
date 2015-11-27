<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:animatedheadservice="http://schemas.trackroamer.com/robotics/2013/12/animatedheadservice.html" >

  <xsl:import href="/resources/dss/Microsoft.Dss.Runtime.Home.MasterPage.xslt" />

  <xsl:template match="/">
    <xsl:call-template name="MasterPage">
      <xsl:with-param name="serviceName">
        Animated Head State
      </xsl:with-param>
      <xsl:with-param name="description">
        Provides access to the Animated Head Arduino based Controller.
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="animatedheadservice:AnimatedHeadServiceState">
    <table width="450">
      <tr class="odd">
        <td size="40" >Connected</td>
        <td align="right" size="21">
          <xsl:value-of select="animatedheadservice:Connected"/>
        </td>
      </tr>
      <tr>
        <td>Comm Port</td>
        <td align="right" size="21">
          <xsl:value-of select="animatedheadservice:AnimatedHeadServiceConfig/animatedheadservice:CommPort"/> (
          <xsl:value-of select="animatedheadservice:AnimatedHeadServiceConfig/animatedheadservice:PortName"/>)
        </td>
      </tr>
      <tr class="odd">
        <td>Baud Rate</td>
        <td align="right" size="21">
          <xsl:value-of select="animatedheadservice:AnimatedHeadServiceConfig/animatedheadservice:BaudRate"/>
        </td>
      </tr>
      <tr>
        <td>Configuration Status</td>
        <td align="right" size="21">
          <xsl:value-of select="animatedheadservice:AnimatedHeadServiceConfig/animatedheadservice:ConfigurationStatus"/>
        </td>
      </tr>
    </table>
  </xsl:template>
</xsl:stylesheet>
