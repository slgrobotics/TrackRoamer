<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:nav="http://schemas.microsoft.com/dss/2007/3/navigation.html">

  <xsl:template name="NavigationPane">
    <xsl:apply-templates select="document('Microsoft.Dss.Runtime.Home.Navigation.xml')/nav:Navigation/nav:Menu" />
    <div style="height:5%;"></div>
  </xsl:template>

  <xsl:template name="NavigationMenu" match="nav:Navigation/nav:Menu">
    <xsl:variable name="collapsed" select="@collapsed = 'True'" />
    <div class="nav_Menu" onselect="return false;">
      <div class="nav_MenuTitle" onclick="dssRuntime.hideShowMenu(this)">
        <div class="nav_MenuIcon" title="Close Menu" align="right">
          <div>
            <xsl:attribute name="class">
              <xsl:choose>
                <xsl:when test="$collapsed">nav_MenuIconInner ArrowDown</xsl:when>
                <xsl:otherwise>nav_MenuIconInner ArrowUp</xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <!--CSS Arrow-->
          </div>
        </div>
        <xsl:value-of select="@name"/>
      </div>
      <ul>
        <xsl:for-each select="nav:Item">
        <li>
          <xsl:if test="$collapsed">
            <xsl:attribute name="style">display:none;</xsl:attribute>
          </xsl:if>
          <a>
            <xsl:attribute name="href">
              <xsl:value-of select="@link"/>
            </xsl:attribute>
            <xsl:if test="@external = 'True'">
              <xsl:attribute name="target">
                <xsl:value-of select="'_blank'"/>
              </xsl:attribute>
            </xsl:if>
            <xsl:if test="nav:Description">
              <xsl:attribute name="title">
                <xsl:value-of select="nav:Description"/>
              </xsl:attribute>
            </xsl:if>
            <xsl:value-of select="@name"/>
          </a>
        </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

</xsl:stylesheet>
