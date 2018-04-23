Imports DevExpress.Utils
Imports DevExpress.XtraMap
Imports System
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Xml.Linq

Namespace CustomizeShapeTitle

    Partial Public Class Form1
        Inherits Form

        Private Const dataPath As String = "..\..\capitals.xml"

        Public Sub New()
            InitializeComponent()
            InitializeMap()
        End Sub

        Private Sub InitializeMap()
            ' Create a Map control and add it to the form.
            Dim map As New MapControl() With {.Dock = DockStyle.Fill, .CenterPoint = New GeoPoint(47, 2), .ZoomLevel = 4, .ToolTipController = New ToolTipController()}
            Me.Controls.Add(map)

            ' Create a layer to load image tiles.
            Dim tilesLayer As New ImageLayer() With {.DataProvider = New OpenStreetMapDataProvider()}
            map.Layers.Add(tilesLayer)

            ' Create a layer to display shapes.
            Dim itemsLayer As New VectorItemsLayer() With {.Data = LoadDataFromXML(dataPath), .ShapeTitlesVisibility = VisibilityMode.Visible, .ShapeTitlesPattern = "{CityName}", .ToolTipPattern = "Population: {Population}"}
            map.Layers.Add(itemsLayer)
        End Sub

        Private Function LoadDataFromXML(ByVal filePath As String) As MapItemStorage
            Dim storage As New MapItemStorage()

            ' Load an XML document from the specified file path.
            Dim document As XDocument = XDocument.Load(filePath)
            If document IsNot Nothing Then
                For Each element As XElement In document.Element("Capitals").Elements()
                    ' Specify shapes attributes by loaded from an XML file values.
                    Dim latitude As Double = Convert.ToDouble(element.Element("Latitude").Value, CultureInfo.InvariantCulture)
                    Dim longitude As Double = Convert.ToDouble(element.Element("Longitude").Value, CultureInfo.InvariantCulture)

                    Dim name_Renamed As String = element.Element("Name").Value
                    Dim population As UInteger = Convert.ToUInt32(element.Element("Population").Value)

                    Dim capital As New MapDot() With {.Location = New GeoPoint(latitude, longitude), .Size = 20}
                    capital.Attributes.Add(New MapItemAttribute() With {.Name = "CityName", .Type = GetType(String), .Value = name_Renamed})
                    capital.Attributes.Add(New MapItemAttribute() With {.Name = "Population", .Type = GetType(UInteger), .Value = population})
                    storage.Items.Add(capital)
                Next element
            End If
            Return storage
        End Function

    End Class
End Namespace
