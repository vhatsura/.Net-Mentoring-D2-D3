<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:custom="urn:my-scripts"
                exclude-result-prefixes="msxsl custom lib"
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
    <feed>
      <title>New arrival</title>
      <subtitle>at library</subtitle>
      <link href="http://library.by/catalog/feed"/>
      <updated>
        <xsl:value-of select="custom:GetDate('dd MMMM yyyy hh:mm:ss tt')"/>
      </updated>
      <author>
        <name>Vadzim Hatsura</name>
        <email>address@mail.com</email>
      </author>
      <id>book_library</id>
      <xsl:apply-templates/>
    </feed>
  </xsl:template>

  <xsl:template match="lib:book">
    <xsl:element name="entry">
      <title><xsl:value-of select="lib:title"/></title>

      <xsl:variable name="isbn" select="lib:isbn"/>
      <xsl:variable name="genre" select="lib:genre"/>
      <xsl:if test="$isbn != '' and $genre = 'Computer'">
        <xsl:element name="link">
          <xsl:attribute name="href">
            <xsl:value-of select="concat('http://my.safaribooksonline.com/', $isbn)"/>
          </xsl:attribute>
        </xsl:element>
      </xsl:if>

      <updated><xsl:value-of select="lib:registration_date"/></updated>
      <id><xsl:value-of select="@id"></xsl:value-of></id>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
