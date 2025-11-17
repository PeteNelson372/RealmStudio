using System.ComponentModel;
/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/

namespace RealmStudio
{
    public partial class MapObjectDetails : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private readonly List<string> landformTypes =
        [
            "Continent",
            "Supercontinent",
            "Island",
        ];

        private readonly List<string> waterFeatureTypes =
        [
            "Lake",
            "Pond",
            "Loch",
            "Inland Lagoon",
            "Inland Sea",
            "Reservoir",
            "Tarn",
            "Waterfall",
            "Spring",
            "Crater Lake",
            "Swamp",
            "Marsh",
            "Bog",
            "Fen",
            "Seep",
            "Waterhole",
            "Thermal Spring",
            "Hot Spring",
            "Geyser",
            "Mire",
        ];

        private readonly List<string> riverTypes =
        [
            "River",
            "Stream",
            "Brook",
            "Creek",
            "Rivulet",
            "Rill",
            "Run",
            "Branch",
            "Fork",
            "Tributary",
            "Course",
            "Channel",
            "Burn",
            "Beck",
            "Gill",
            "Ghyll",
            "Bourne",
            "Bourn",
            "Kill",
            "Freshet",
            "Sike",
        ];

        private readonly List<string> pathTypes =
        [
            // Common Types of Paths and Roads
            "Road",
            "Path",
            "Trail",
            "Track",
            "Lane",
            "Alley",
            "Way",
            "Route",

            // Urban or Formal Road Types
            "Avenue",
            "Boulevard",
            "Street",
            "Drive",
            "Court",
            "Place",
            "Terrace",
            "Crescent",
            "Circle",

            // Rural or Natural Pathways
            "Footpath",
            "Bridleway",
            "Byway",
            "Cartway",
            "Trackway",
            "Greenway",
            "Boardwalk",

            // Poetic, Archaic, or Fantasy-Inspired
            "Passage",
            "Walk",
            "Causeway",
            "Trackless Way",
            "Thoroughfare",
            "Pilgrim's Way",
            "Shadowpath",
            "Starroad",
            "Wanderer's Trail"
        ];

        private readonly List<string> regionTypes =
        [
            // Political / Administrative Regions
            "Country",
            "Nation",
            "State",
            "Province",
            "Region",
            "Territory",
            "County",
            "District",
            "Municipality",
            "Commune",
            "Canton",
            "Prefecture",
            "Realm",
            "Dominion",

            // Natural Land Regions
            "Desert",
            "Forest",
            "Jungle",
            "Tundra",
            "Plain",
            "Prairie",
            "Savanna",
            "Steppe",
            "Wetland",
            "Marsh",
            "Swamp",
            "Glacier",
            "Highland",
            "Lowland",
            "Valley",
            "Canyon",

            // Oceanic and Coastal Regions
            "Ocean",
            "Sea",
            "Bay",
            "Gulf",
            "Cove",
            "Lagoon",
            "Sound",
            "Inlet",
            "Fjord",
            "Reef",
            "Atoll",
            "Strait",
            "Channel",
            "Continental Shelf"
        ];

        private readonly List<string> symbolTypes =
        [
            // Structures
            "House", "Home", "Hut", "Cottage", "Cabin", "Manor", "Villa", "Lodge", "Shack",
            "Inn", "Tavern", "Alehouse", "Pub", "Hostel", "Bunkhouse", "Hotel", "Motel", "Resort",
            "Bank", "Shop", "Store", "Market", "Temple", "Shrine", "Church", "Chapel", "Cathedral",
            "Library", "Hall", "Tower", "Sawmill", "Mill", "Warehouse", "Barn", "Stable", "Forge", "Workshop",
            "Academy", "Barracks", "Fort", "Keep", "Castle", "Citadel", "Outpost", "Garrison", "Watchtower",
            "Gatehouse", "Wall", "Palisade", "Fence", "Bridge", "Causeway", "Arch", "Dam", "Aqueduct", "Pier", "Dock",

            // Vegetation
            "Tree", "Oak", "Pine", "Willow", "Elm", "Fir", "Palm", "Maple", "Birch", "Cedar", "Cherry", "Apple", "Peach", "Redwood",
            "Shrub", "Bush", "Bramble", "Hedge", "Vine", "Reed", "Thicket",
            "Grass", "Turf", "Sod", "Moss", "Fern", "Lichen", "Flower", "Blossom", "Weed", "Undergrowth",

            // Terrain
            "Mountain", "Hill", "Peak", "Ridge", "Plateau", "Crag", "Bluff", "Cliff", "Escarpment",
            "Valley", "Ravine", "Gorge", "Gully", "Canyon", "Basin", "Hollow", "Dell",
            "Plain", "Steppe", "Field", "Meadow", "Dune", "Desert", "Moor", "Heath", "Marsh",
            "Bog", "Swamp", "Fen", "Mire", "Slope", "Rise", "Knoll", "Sinkhole"
        ];



        private string _objectType = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ObjectType
        {
            get => _objectType;
            set
            {
                _objectType = value;
            }
        }

        private List<string> _objectCharacteristics = [];
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> ObjectCharacteristics
        {
            get => _objectCharacteristics;
            set
            {
                _objectCharacteristics = value;
                ObjectCharacteristicsTextbox.Text = string.Join(", ", _objectCharacteristics);
            }
        }

        public MapObjectDetails(Type mapObjectType)
        {
            InitializeComponent();

            if (mapObjectType == typeof(Landform))
            {
                ObjectTypeCheckedList.Items.AddRange([.. landformTypes]);
            }
            else if (mapObjectType == typeof(WaterFeature))
            {
                ObjectTypeCheckedList.Items.AddRange([.. waterFeatureTypes]);
            }
            else if (mapObjectType == typeof(River))
            {
                ObjectTypeCheckedList.Items.AddRange([.. riverTypes]);
            }
            else if (mapObjectType == typeof(MapPath))
            {
                ObjectTypeCheckedList.Items.AddRange([.. pathTypes]);
            }
            else if (mapObjectType == typeof(MapRegion))
            {
                ObjectTypeCheckedList.Items.AddRange([.. regionTypes]);
            }
            else if (mapObjectType == typeof(MapSymbol))
            {
                ObjectTypeCheckedList.Items.AddRange([.. symbolTypes]);
            }
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CloseCharacteristicsButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddCharacteristicButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ObjectCharacteristicsTextbox.Text) && !ObjectCharacteristics.Contains(ObjectCharacteristicsTextbox.Text))
            {
                ObjectCharacteristics.Add(ObjectCharacteristicsTextbox.Text);
                ObjectCharacteristicsCheckedListBox.Items.Add(ObjectCharacteristicsTextbox.Text);
                ObjectCharacteristicsTextbox.Clear();
                ObjectCharacteristicsTextbox.Focus();
            }
        }

        private void AddCharacteristicButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Click to add a characteristic entered in the text box to the characteristics.", this, new Point(AddCharacteristicButton.Left, AddCharacteristicButton.Top - 20), 3000);
        }

        private void ObjectCharacteristicsCheckedListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                for (int i = ObjectCharacteristicsCheckedListBox.Items.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < ObjectCharacteristicsCheckedListBox.CheckedItems.Count; j++)
                    {
                        if (ObjectCharacteristicsCheckedListBox.Items[i] != null
                            && ObjectCharacteristicsCheckedListBox.CheckedItems[j] != null
                            && ObjectCharacteristicsCheckedListBox.Items[i].ToString() == ObjectCharacteristicsCheckedListBox.CheckedItems[j]?.ToString())
                        {
                            ObjectCharacteristics.Remove((string)ObjectCharacteristicsCheckedListBox.Items[i]);
                            ObjectCharacteristicsCheckedListBox.Items.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private void ObjectCharacteristicsTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent the beep sound on Enter key
                if (!string.IsNullOrWhiteSpace(ObjectCharacteristicsTextbox.Text) && !ObjectCharacteristics.Contains(ObjectCharacteristicsTextbox.Text))
                {
                    ObjectCharacteristics.Add(ObjectCharacteristicsTextbox.Text);
                    ObjectCharacteristicsCheckedListBox.Items.Add(ObjectCharacteristicsTextbox.Text);
                    ObjectCharacteristicsTextbox.Clear();
                    ObjectCharacteristicsTextbox.Focus();
                }
            }
        }

        private void ObjectTypeCheckedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObjectType = string.Empty; // Reset ObjectType to empty string

            for (int i = 0; i < ObjectTypeCheckedList.Items.Count; i++)
            {
                if (ObjectTypeCheckedList.Items[i] != null && ObjectTypeCheckedList.GetItemChecked(i))
                {
                    ObjectType = (string)ObjectTypeCheckedList.Items[i];
                }
            }
        }
    }
}
