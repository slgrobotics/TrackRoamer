<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:brand="http://schemas.microsoft.com/dss/2008/9/brand.html">

  <xsl:output method="html" version="4.01" encoding="utf-8" indent="yes"
  doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN"
  doctype-system="http://www.w3.org/TR/html4/loose.dtd" />

  <xsl:include href="Microsoft.Dss.Runtime.Home.Navigation.xslt"/>

  <xsl:variable name="BrandName">
    <xsl:value-of select="document('Microsoft.Dss.Runtime.Home.Brand.xml')/brand:Brand/brand:Name"/>
  </xsl:variable>
  <xsl:variable name="BrandCompany">
    <xsl:value-of select="document('Microsoft.Dss.Runtime.Home.Brand.xml')/brand:Brand/brand:Company"/>
  </xsl:variable>
  <xsl:variable name="BrandProduct">
    <xsl:value-of select="document('Microsoft.Dss.Runtime.Home.Brand.xml')/brand:Brand/brand:Product"/>
  </xsl:variable>

  <xsl:template name="MasterPage" match="/">
    <xsl:param name="serviceName" />
    <xsl:param name="title" />
    <xsl:param name="description" />
    <xsl:param name="head" />
    <xsl:param name="navigation" />
    <xsl:param name="xmlButton" />

    <html>
      <head>
        <title>
          <xsl:choose>
            <xsl:when test="$title != ''"><xsl:value-of select="$title"/></xsl:when>
            <xsl:when test="$serviceName != ''"><xsl:value-of select="$serviceName"/></xsl:when>
            <xsl:when test="$BrandName != ''">
              <xsl:value-of select="$BrandName"/>
            </xsl:when>
            <xsl:otherwise>Microsoft DSS</xsl:otherwise>
          </xsl:choose>
        </title>
        <link rel="stylesheet" type="text/css" href="/resources/dss/Microsoft.Dss.Runtime.Home.Styles.Common.css" />
        <script language="javascript" type="text/javascript" src="/resources/dss/Microsoft.Dss.Runtime.Home.JavaScript.Common.js"></script>
        <script language="javascript" type="text/javascript">
          window.onload = function() {
          <xsl:if test="$navigation = 'Closed'">
            dssRuntime.hideShowNav();
          </xsl:if>
            if (dssRuntime.init) dssRuntime.init();
          }
        </script>
        <xsl:copy-of select="$head"/>
      </head>
      <body scroll="no">
        <div id="topLogoBar">
          <!--START - Microsoft DSS Logo Bar-->
          <xsl:call-template name="TopLogoBar" />
          <!--END - Microsoft DSS Logo Bar-->
        </div>
        <div style="height:100%">
          <!--START - Left Navigation Pane, Divider, Service State Page-->
          <xsl:if test="$navigation != 'None'">
            <div id="navPane" style="float:left; height:100%">
              <!--START - Left Navigation Pane-->
              <xsl:call-template name="NavigationPane" />
              <div></div>
              <!--END - Left Navigation Pane-->
            </div>
            <div id="divider" style="float:left; height:100%">
              <!--START - Divider-->
              <xsl:call-template name="Divider" />
              <!--END - Divider-->
            </div>
          </xsl:if>
          <div id="serviceStatePage" align="left">
            <!--START - Service State Page-->
            <div id="serviceHeader" style="float:none;">
              <!--START - Service Header-->
              <xsl:call-template name="ServiceHeader">
                <xsl:with-param name="serviceName">
                  <xsl:choose>
                    <xsl:when test="$serviceName != ''">
                      <xsl:copy-of select="$serviceName"/>
                    </xsl:when>
                    <xsl:otherwise>Service State View</xsl:otherwise>
                  </xsl:choose>
                </xsl:with-param>
                <xsl:with-param name="description">
                  <xsl:choose>
                    <xsl:when test="$description != ''">
                      <xsl:copy-of select="$description"/>
                    </xsl:when>
                    <xsl:otherwise>&lt;Not available&gt;</xsl:otherwise>
                  </xsl:choose>
                </xsl:with-param>
                <xsl:with-param name="xmlButton">
                  <xsl:choose>
                    <xsl:when test="$xmlButton != ''">
                      <xsl:copy-of select="$xmlButton"/>
                    </xsl:when>
                    <xsl:otherwise>&lt;Not available&gt;</xsl:otherwise>
                  </xsl:choose>
                </xsl:with-param>
              </xsl:call-template>
              <!--END - Service Header-->
            </div>
            <div id="serviceContent" style="float:none;overflow:auto;">
              <!--START - Service Content-->
              <xsl:apply-templates />
              <!--END - Service Content-->
            </div>
            <!--END - Service State Page-->
          </div>
          <!--END - Left Navigation Pane, Divider, Service State Page-->
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="TopLogoBar">
    <div style="height:36px; background-image:url('/resources/dss/Microsoft.Dss.Runtime.Home.Images.Header_BG.jpg'); background-repeat:repeat-x;">
      <img src="/resources/dss/Microsoft.Dss.Runtime.Home.Images.Header_End.jpg" width="25" height="36" style="float:right" />
      <div class="MSRS_Header" onclick="window.open('/','_self');" style="cursor:pointer;">
        <span class="MSFT">
          <xsl:value-of select="$BrandCompany"/>
        </span><span style="vertical-align:super;font-size:.6em">&#xae;</span>
        <span class="MSRS">
          <xsl:value-of select="$BrandProduct"/>
        </span>
      </div>
    </div>
  </xsl:template>

  <xsl:template name="Divider">
    <div align="center" style="position:absolute; top:50%; cursor:pointer;">
      <a onclick="dssRuntime.hideShowNav()">
        <img id="dividerArrow" src="/resources/dss/Microsoft.Dss.Runtime.Home.Images.Divider-Collapse.gif" width="10" height="60" title="Collapse Navigation Pane" />
      </a>
    </div>
  </xsl:template>
  
  <xsl:template name="ServiceHeader">
    <xsl:param name="serviceName" />
    <xsl:param name="description" />
    <xsl:param name="xmlButton" />
    <div align="left" style="padding:.8em;">
      <div style="font-size:large">
        <div style="float:right; margin-right:.3em">
          <input id="dssRuntimeXmlButton" type="button" onclick="javascript:dssRuntime.viewXML();" value="XML" class="OrangeButton" title="View Raw XML">
            <xsl:if test="$xmlButton = 'None'">
              <xsl:attribute name="style">
                <xsl:text>display:none;</xsl:text>
              </xsl:attribute>
            </xsl:if>
          </input>
        </div>
        <div id="DssServiceName">
          <xsl:copy-of select="$serviceName" />
        </div>
      </div>
      <div style="padding-top:1.8em;">
        <strong>Description: </strong>
        <i>
          <div id="DssServiceDescription" style="display:inline">
            <xsl:copy-of select="$description" />
          </div>
        </i>
      </div>
      <hr style="height:1px" />
    </div>
  </xsl:template>
  
</xsl:stylesheet>
