<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns="http://schemas.microsoft.com/winfx/avalon/2005"
  xmlns:x="http://schemas.microsoft.com/winfx/xaml/2005"
  version="1.0">
	<xsl:template match="/">
		<!-- All generic xmls will be created with these FlowDocument properties-->
		<FlowDocument
			TextAlignment="Justify" 
			IsHyphenationEnabled="True"		
			ColumnWidth="6.5 cm" 
			ColumnGap="0.7 cm"
			ColumnRuleWidth="1 px"
			ColumnRuleBrush="LightGray"
			IsColumnWidthFlexible="True"
			IsColumnBalancingEnabled="False">
			<!-- Use the generic xml's values for Foreground and Background properties-->			
			<xsl:attribute name= 'Background'>
				<xsl:value-of select="//@Background" />
			</xsl:attribute>
			<xsl:attribute name= 'Foreground'>
				<xsl:value-of select="//@Foreground" />
			</xsl:attribute>

			<!--All FlowDocument styling is stored in the Resources template-->
			<xsl:call-template name="Resources" />

			<!-- Apply all the templates that match below-->
			<xsl:apply-templates />
		</FlowDocument>
	</xsl:template>

	<xsl:template name="Resources">
		<!-- If the DocumentStyle property on Body is set to 'SanSerifBody' use the below FlowDocument styling-->
		<xsl:choose>
			<xsl:when test="//@DocumentStyle = 'SanSerifBody'">
				<FlowDocument.Resources>		
					<Style x:Key="Title" TargetType="{{x:Type Paragraph}}">
						<Setter Property="FontFamily" Value="Pescadero" />
						<Setter Property="FontSize" Value="26 pt" />
						<Setter Property="TextAlignment" Value="Center" />
						<Setter Property="IsHyphenationEnabled" Value="False" />
					</Style>

					<Style x:Key="Heading" TargetType="{{x:Type Paragraph}}">
						<Setter Property="FontFamily" Value="Cambria" />
						<Setter Property="FontSize" Value="20 pt" />
						<Setter Property="TextAlignment" Value="Left" />
						<Setter Property="KeepWithNext" Value="True" />
					</Style>

					<Style x:Key="BodyText" TargetType="{{x:Type Paragraph}}">
						<Setter Property="FontFamily" Value="Calibri" />
						<Setter Property="FontSize" Value="12 pt" />
					</Style>

					<Style x:Key="FirstParagraph" TargetType="{{x:Type Paragraph}}" BasedOn="{{StaticResource BodyText}}">
						<Setter Property="FontFamily" Value="Cambria" />
						<Setter Property="FontSize" Value="16 pt" />
						<Setter Property="LineHeight" Value="25 px" />
					</Style>
				</FlowDocument.Resources>
			</xsl:when>
				
			<!-- If the DocumentStyle property on Body is set to 'EastAsian' use the below FlowDocument styling-->
			<xsl:when test="//@DocumentStyle = 'EastAsian'">
				<FlowDocument.Resources>
					<Style x:Key="Title" TargetType="{{x:Type Paragraph}}">
						<Setter Property="FontFamily" Value="Meiryo" />
						<Setter Property="FontSize" Value="22 pt" />
						<Setter Property="TextAlignment" Value="Center" />
					</Style>

					<Style x:Key="Heading" TargetType="{{x:Type Paragraph}}">
						<Setter Property="FontFamily" Value="Meiryo" />
						<Setter Property="FontSize" Value="20 pt" />
						<Setter Property="TextAlignment" Value="Left" />
						<Setter Property="KeepWithNext" Value="True" />
					</Style>

					<Style x:Key="BodyText" TargetType="{{x:Type Paragraph}}">
						<Setter Property="FontFamily" Value="Meiryo" />
						<Setter Property="FontSize" Value="10 pt" />
					</Style>

					<Style x:Key="FirstParagraph" TargetType="{{x:Type Paragraph}}" BasedOn="{{StaticResource BodyText}}">
						<Setter Property="FontFamily" Value="Meiryo" />
						<Setter Property="FontSize" Value="12 pt" />
						<Setter Property="LineHeight" Value="28 px" />
					</Style>
				</FlowDocument.Resources>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<!-- Below are the templates to match the various generic xml elements and attributes-->
	<xsl:template match="Title">
		<Paragraph Style="{{StaticResource Title}}">
			<xsl:apply-templates/>
		</Paragraph>
	</xsl:template>

	<xsl:template match="Para">
		<Paragraph Style="{{StaticResource BodyText}}">
			<xsl:apply-templates/>
		</Paragraph>
	</xsl:template>

	<xsl:template match="Para[@Style='First']">
		<Paragraph Style="{{StaticResource FirstParagraph}}">
			<xsl:apply-templates/>
		</Paragraph>
	</xsl:template>

	<xsl:template match="Heading">
		<Paragraph Style="{{StaticResource Heading}}">
			<xsl:apply-templates/>
		</Paragraph>
	</xsl:template>

	<!-- Add the Swash template below here: -->

	<!-- Add the <Illustration Style='Big'> template below here: -->

	<xsl:template match="Illustration[@Style='Medium']">
		<Paragraph>
			<Figure 
				HorizontalAnchor="ContentLeft"
				VerticalAnchor="ContentBottom" 
				Width=".5 content">
				<BlockUIContainer>
					<Viewbox>
						<Grid>
							<Image>
								<xsl:attribute name= 'Source'>
									<xsl:value-of select="./Pic" />
								</xsl:attribute>
							</Image>
							<TextFlow
								VerticalAlignment="Bottom" 
								HorizontalAlignment="Left" 	
								Width="180 px"									
								FontSize="16 pt"
								TextAlignment="Left"
								IsHyphenationEnabled="True">
								<Paragraph>
									<xsl:value-of select="./Caption" />
								</Paragraph>
							</TextFlow>
						</Grid>
					</Viewbox>
				</BlockUIContainer>
			</Figure>
		</Paragraph>
	</xsl:template>

	<xsl:template match="Illustration[@Style='Small']">
		<Floater>
			<BlockUIContainer>
				<DockPanel
					LastChildFill="True">
					<xsl:attribute name= 'Background'>
						<xsl:value-of select=".//Caption/@Background" />
					</xsl:attribute>
					<Image DockPanel.Dock="Top">
						<xsl:attribute name="Source">
							<xsl:value-of select=".//Pic" />
						</xsl:attribute>
					</Image>
					<TextFlow
						FontFamily="Cambria, Meiryo"
						IsHyphenationEnabled="True">
						<Paragraph>
							<xsl:value-of select=".//Caption" />
						</Paragraph>
					</TextFlow>
				</DockPanel>
			</BlockUIContainer>
		</Floater>
	</xsl:template>

	<xsl:template match="PullQuote">
		<Floater>
			<BlockUIContainer>
				<Border BorderThickness="10">
					<xsl:attribute name= 'BorderBrush'>
						<xsl:value-of select="//PullQuote/@BorderColor" />
					</xsl:attribute>
					<TextFlow
						FontFamily="Cambria, Meiryo"
						FontSize="20 px"
						Padding="20,20,20,20"
						IsHyphenationEnabled="True">
						<Paragraph>
							<xsl:apply-templates/>
							<LineBreak /> 
						</Paragraph>
					</TextFlow>
				</Border>
			</BlockUIContainer>
		</Floater>
	</xsl:template>
	
</xsl:stylesheet>