using DevExpress.Utils;
using DevExpress.XtraMap;
using System;
using System.Globalization;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CustomizeShapeTitle {

    public partial class Form1 : Form {
        const string dataPath = @"..\..\capitals.xml";
        
        public Form1() {
            InitializeComponent();
            InitializeMap();     
        }

        private void InitializeMap() {
            // Create a Map control and add it to the form.
            MapControl map = new MapControl() { 
                Dock = DockStyle.Fill, CenterPoint = new GeoPoint(47, 2), 
                ZoomLevel = 4, ToolTipController = new ToolTipController()
            };
            this.Controls.Add(map);
            
            // Create a layer to load image tiles.
            ImageLayer tilesLayer = new ImageLayer() { 
                DataProvider = new OpenStreetMapDataProvider() 
            };
            map.Layers.Add(tilesLayer);

            // Create a layer to display shapes.
            VectorItemsLayer itemsLayer = new VectorItemsLayer() { 
                // Provide data to generate shape items.
                Data = LoadDataFromXML(dataPath), 

                // Enable shape titles.
                ShapeTitlesVisibility = VisibilityMode.Visible,

                // Each shape has a CityName attribute, which stores the capital name.
                // To display this value as a shape title, let's specify the attribute name in braces.
                ShapeTitlesPattern = "{CityName}",

                // To display a Population attribute as a tooltip, 
                // let's specify the ToolTipPattern property as follows.
                ToolTipPattern = "Population: {Population}"
            };
            map.Layers.Add(itemsLayer);
        }

        private MapItemStorage LoadDataFromXML(string filePath) {
            MapItemStorage storage = new MapItemStorage();
            
            // Load an XML document from the specified file path.
            XDocument document = XDocument.Load(filePath);
            if (document != null) {
                foreach (XElement element in document.Element("Capitals").Elements()) {
                    // Specify shapes attributes by loaded from an XML file values.
                    double latitude = Convert.ToDouble(element.Element("Latitude").Value, 
                        CultureInfo.InvariantCulture
                    );
                    double longitude = Convert.ToDouble(element.Element("Longitude").Value, 
                        CultureInfo.InvariantCulture
                    );
                    string name = element.Element("Name").Value;
                    uint population = Convert.ToUInt32(element.Element("Population").Value);

                    MapDot capital = new MapDot() { 
                        Location = new GeoPoint(latitude, longitude), Size = 20 
                    };
                    capital.Attributes.Add(new MapItemAttribute() 
                        { Name = "CityName", Type = typeof(string), Value = name });
                    capital.Attributes.Add(new MapItemAttribute() 
                        { Name = "Population", Type = typeof(uint), Value = population });
                    storage.Items.Add(capital);
                }
            }
            return storage;
        }

    }
}
