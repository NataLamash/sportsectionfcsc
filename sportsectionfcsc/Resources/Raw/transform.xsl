<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:html="http://www.w3.org/TR/REC-html40">

	<xsl:output method="html" encoding="UTF-8" indent="yes"/>

	
	<xsl:template match="/">
		<html>
			<head>
				<title>Sports Faculty</title>
				<style>
					/* Додайте тут ваш CSS для форматування */
					body { font-family: Arial, sans-serif; }
					h1 { color: #333; }
					h2 { color: #555; }
					table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
					th, td { border: 1px solid #ccc; padding: 8px; text-align: left; }
					th { background-color: #bac9b8; }
				</style>
			</head>
			<body>
				<h1>Sports Faculty Sections</h1>
				<xsl:apply-templates select="SportsFaculty/Section"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="Section">
		<h2>
			<xsl:value-of select="@name"/> - Coach: <xsl:value-of select="@coach"/>
		</h2>
		<p>
			Location: <xsl:value-of select="@location"/>
		</p>

		
		<h3>Schedule</h3>
		<table>
			<tr>
				<th>Day</th>
				<th>Time</th>
			</tr>
			<xsl:for-each select="Schedule/Training">
				<tr>
					<td>
						<xsl:value-of select="@day"/>
					</td>
					<td>
						<xsl:value-of select="@time"/>
					</td>
				</tr>
			</xsl:for-each>
		</table>

		<h3>Members</h3>
		<table>
			<tr>
				<th>ID</th>
				<th>Name</th>
				<th>Gender</th>
				<th>Age</th>
				<th>Role</th>
			</tr>
			<xsl:for-each select="Members/*">
				<tr>
					<td>
						<xsl:value-of select="@id"/>
					</td>
					<td>
						<xsl:value-of select="@name"/>
					</td>
					<td>
						<xsl:value-of select="@gender"/>
					</td>
					<td>
						<xsl:value-of select="@age"/>
					</td>
					<td>
						<xsl:value-of select="local-name()"/>
					</td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>

</xsl:stylesheet>
