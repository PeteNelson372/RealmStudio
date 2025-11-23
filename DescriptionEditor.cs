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
    public partial class DescriptionEditor : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private Type MapObjectType { get; set; }
        private string MapObjectName { get; set; } = string.Empty;

        private string MapObjectTypeValue { get; set; } = string.Empty;

        private List<string> MapObjectCharacteristics = [];

        private string _descriptionText = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DescriptionText
        {
            get => _descriptionText;
            set
            {
                _descriptionText = value;
            }
        }

        public DescriptionEditor(Type mapObjectType, string mapObjectName, string existingDescription)
        {
            InitializeComponent();
            MapObjectType = mapObjectType;
            MapObjectName = mapObjectName;
            DescriptionText = existingDescription;
            DescriptionTextbox.Text = existingDescription;
        }

        private void CloseDescriptionButton_Click(object sender, EventArgs e)
        {
            DescriptionText = DescriptionTextbox.Text;
            Close();
        }

        private void DescriptionAIButton_Click(object sender, EventArgs e)
        {
            string query = string.Empty;

            switch (MapObjectType.Name)
            {
                case "RealmStudioMap":
                    {
                        query = $"Generate a description for the fantasy map realm";
                        if (!string.IsNullOrEmpty(MapObjectTypeValue))
                        {
                            query += $"Generate a description for the fantasy map '{MapObjectTypeValue}'";
                        }
                    }
                    break;
                case "Landform":
                    {
                        query = $"Generate a description for the fantasy map landform";

                        if (!string.IsNullOrEmpty(MapObjectTypeValue))
                        {
                            query += $"Generate a description for the fantasy map '{MapObjectTypeValue}'";
                        }
                    }
                    break;
                case "WaterFeature":
                    {
                        query = $"Generate a description for the fantasy map water feature (lake, pond, etc.)";

                        if (!string.IsNullOrEmpty(MapObjectTypeValue))
                        {
                            query += $"Generate a description for the fantasy map '{MapObjectTypeValue}'";
                        }
                    }
                    break;
                case "River":
                    {
                        query = $"Generate a description for the fantasy map river";

                        if (!string.IsNullOrEmpty(MapObjectTypeValue))
                        {
                            query += $"Generate a description for the fantasy map '{MapObjectTypeValue}'";
                        }
                    }
                    break;
                case "MapPath":
                    {
                        query = $"Generate a description for the fantasy map path (road, trail, avenue, etc.)";

                        if (!string.IsNullOrEmpty(MapObjectTypeValue))
                        {
                            query += $"Generate a description for the fantasy map '{MapObjectTypeValue}'";
                        }
                    }
                    break;
                case "MapRegion":
                    {
                        query = $"Generate a description for the fantasy map region (country, county, state, area, ocean, bay, gulf)";

                        if (!string.IsNullOrEmpty(MapObjectTypeValue))
                        {
                            query += $"Generate a description for the fantasy map '{MapObjectTypeValue}'";
                        }
                    }
                    break;
                case "MapSymbol":
                    {
                        query = $"Generate a description for the fantasy map object (tree, house, castle, hill, mountain)";

                        if (!string.IsNullOrEmpty(MapObjectTypeValue))
                        {
                            query += $"Generate a description for the fantasy map '{MapObjectTypeValue}'";
                        }
                    }
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(query))
            {
                if (!string.IsNullOrEmpty(MapObjectName))
                {
                    query += $" named '{MapObjectName}'";
                }

                if (MapObjectCharacteristics.Count > 0)
                {
                    query += $" with the following characteristics: {string.Join(", ", MapObjectCharacteristics)}";
                }

                //

                try
                {
                    Cursor = Cursors.WaitCursor;
                    GetMapObjectDescription(query);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while generating the description: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    DescriptionTextbox.Refresh();
                    Cursor = Cursors.Default;
                }

            }
        }

        private void DescriptionAIButton_MouseHover(object sender, EventArgs e)
        {
            switch (MapObjectType.Name)
            {
                case "RealmStudioMap":
                    TOOLTIP.Show("Generate a description for the Realm using AI.", this, new Point(DescriptionAIButton.Left, DescriptionAIButton.Top - 20), 3000);
                    break;
                case "Landform":
                    TOOLTIP.Show("Generate a description for the Landform using AI.", this, new Point(DescriptionAIButton.Left, DescriptionAIButton.Top - 20), 3000);
                    break;
                case "WaterFeature":
                    TOOLTIP.Show("Generate a description for the Water Feature/Lake group using AI.", this, new Point(DescriptionAIButton.Left, DescriptionAIButton.Top - 20), 3000);
                    break;
                case "River":
                    TOOLTIP.Show("Generate a description for the River using AI.", this, new Point(DescriptionAIButton.Left, DescriptionAIButton.Top - 20), 3000);
                    break;
                case "MapPath":
                    TOOLTIP.Show("Generate a description for the Path using AI.", this, new Point(DescriptionAIButton.Left, DescriptionAIButton.Top - 20), 3000);
                    break;
                case "MapRegion":
                    TOOLTIP.Show("Generate a description for the Region using AI.", this, new Point(DescriptionAIButton.Left, DescriptionAIButton.Top - 20), 3000);
                    break;
                case "MapSymbol":
                    TOOLTIP.Show("Generate a description for the Symbol using AI.", this, new Point(DescriptionAIButton.Left, DescriptionAIButton.Top - 20), 3000);
                    break;
                default:
                    break;
            }
        }

        private async void GetMapObjectDescription(string query)
        {
            try
            {
                string? token = await AiIntegration.GetJwtTokenAsync();

                if (token != null)
                {
                    string? aiCallData = await AiIntegration.GetAiCallData(token);

                    if (aiCallData != null)
                    {
                        string? description = await AiIntegration.GetAIDescriptionAsync(aiCallData, query);

                        if (!string.IsNullOrEmpty(description))
                        {
                            DescriptionText = description;
                            Invoke(() => DescriptionTextbox.Text = DescriptionText);
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve AI call data.");
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve token for AI integration.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void MapObjectDetailButton_Click(object sender, EventArgs e)
        {
            MapObjectDetails mapObjectDetails = new(MapObjectType);
            DialogResult r = mapObjectDetails.ShowDialog(this);

            if (r == DialogResult.OK)
            {
                // set the characteristics of the object so the AI query can be constructed
                // with the additional information
                MapObjectTypeValue = mapObjectDetails.ObjectType;
                MapObjectCharacteristics = mapObjectDetails.ObjectCharacteristics;

                CharacteristicsLabel.Text = MapObjectTypeValue + ": " + string.Join(", ", MapObjectCharacteristics);
            }
        }

        private void MapObjectDetailButton_MouseHover(object sender, EventArgs e)
        {
            switch (MapObjectType.Name)
            {
                case "RealmStudioMap":
                    TOOLTIP.Show("Set the type and characteristics of the Realm.", this, new Point(MapObjectDetailButton.Left, MapObjectDetailButton.Top - 20), 3000);
                    break;
                case "Landform":
                    TOOLTIP.Show("Set the type and characteristics of the Landform.", this, new Point(MapObjectDetailButton.Left, MapObjectDetailButton.Top - 20), 3000);
                    break;
                case "WaterFeature":
                    TOOLTIP.Show("Set the type and characteristics of the Water Feature/Lake.", this, new Point(MapObjectDetailButton.Left, MapObjectDetailButton.Top - 20), 3000);
                    break;
                case "River":
                    TOOLTIP.Show("Set the type and characteristics of the River.", this, new Point(MapObjectDetailButton.Left, MapObjectDetailButton.Top - 20), 3000);
                    break;
                case "MapPath":
                    TOOLTIP.Show("Set the type and characteristics of the Path.", this, new Point(MapObjectDetailButton.Left, MapObjectDetailButton.Top - 20), 3000);
                    break;
                case "MapRegion":
                    TOOLTIP.Show("Set the type and characteristics of the Region.", this, new Point(MapObjectDetailButton.Left, MapObjectDetailButton.Top - 20), 3000);
                    break;
                case "MapSymbol":
                    TOOLTIP.Show("Set the type and characteristics of the Symbol.", this, new Point(MapObjectDetailButton.Left, MapObjectDetailButton.Top - 20), 3000);
                    break;
                default:
                    break;
            }
        }

        private void CreateDescriptionArticleButton_Click(object sender, EventArgs e)
        {

        }

        private void CreateDescriptionArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Open the World Anvil Article Integration dialog to create a World Anvil article.", this, new Point(CreateDescriptionArticleButton.Left, CreateDescriptionArticleButton.Top - 20), 3000);
        }
    }
}