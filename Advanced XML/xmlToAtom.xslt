<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:custom="urn:my-scripts"
                exclude-result-prefixes="msxsl"
                xmlns:lib="http://library.by/catalog">

  <msxsl:script language="C#" implements-prefix="custom">
    <msxsl:assembly name="System.Web"/>
    <msxsl:using namespace="System.Web"/>
    <![CDATA[
      public string GetDate(string dateFormat)
      {
        return DateTime.Now.ToString(dateFormat);
      }
    ]]>
  </msxsl:script>

  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/lib:catalog">
    <xsl:element name="feed">
      <xsl:element name="title">
        <xsl:text>New arrival</xsl:text>
      </xsl:element>
      <xsl:element name="subtitle">
        <xsl:text>at library</xsl:text>
      </xsl:element>
      <xsl:element name="link">
        <xsl:attribute name="href">
          <xsl:text>http://library.by/catalog/feed</xsl:text>
        </xsl:attribute>
      </xsl:element>
      <xsl:element name="updated">
        <xsl:value-of select="custom:GetDate('dd MMMM yyyy hh:mm:ss tt')"/>
      </xsl:element>
      <xsl:element name="author">
        <xsl:element name="name">
          <xsl:text>Vadzim Hatsura</xsl:text>
        </xsl:element>
        <xsl:element name="email">
          <xsl:text>address@mail.com</xsl:text>
        </xsl:element>
      </xsl:element>
      <xsl:element name="id">
        <xsl:text>book_library</xsl:text>
      </xsl:element>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="lib:book">
    <xsl:element name="entry">
      <xsl:element name="title">
        <xsl:value-of select="lib:title"/>
      </xsl:element>

      <xsl:variable name="isbn" select="lib:isbn"/>
      <xsl:variable name="genre" select="lib:genre"/>
      <xsl:if test="$isbn != '' and $genre = 'Computer'">
        <xsl:element name="link">
          <xsl:attribute name="href">
            <xsl:value-of select="concat('http://my.safaribooksonline.com/', $isbn)"/>
          </xsl:attribute>
        </xsl:element>
      </xsl:if>

      <xsl:element name="updated">
        <xsl:value-of select="lib:registration_date"></xsl:value-of>
      </xsl:element>
      <xsl:element name="id">
        <xsl:value-of select="@id"></xsl:value-of>
      </xsl:element>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
