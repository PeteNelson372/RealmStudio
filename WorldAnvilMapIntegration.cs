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
using RealmStudio.Properties;
using System.IO;
using System.Text.Json;
using WorldAnvilIntegrationLib;

namespace RealmStudio
{
    public partial class WorldAnvilMapIntegration : Form
    {
        private static readonly ToolTip TOOLTIP = new();
        private int validatedImageId;

        public WorldAnvilMapIntegration()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CreateMapButton_Click(object sender, EventArgs e)
        {
            if (MapStateMediator.CurrentMap.WorldAnvilImageId != 0 && MapStateMediator.CurrentMap.WorldAnvilMapId != Guid.Empty)
            {
                // get image data and map data and update existing map
                if (!int.TryParse(MapImageIdTextBox.Text, out int mapImageId))
                {
                    MessageBox.Show("A valid World Anvil image ID must be entered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (mapImageId != MapStateMediator.CurrentMap.WorldAnvilImageId)
                {
                    CreateMapButton.Enabled = false;
                    CreateMapArticleButton.Enabled = false;

                    if (MessageBox.Show("The map image ID has changed. Do you want to change the image used for the map?", "Map Image Changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string apiToken = Settings.Default.WorldAnvilApiToken;
                        if (!string.IsNullOrEmpty(apiToken))
                        {
                            if (ValidateMapImageId(apiToken, mapImageId))
                            {
                                CreateMapButton.Enabled = true;
                                CreateMapArticleButton.Enabled = true;

                                UpdateMap();
                            }
                            else
                            {
                                CreateMapButton.Enabled = false;
                                CreateMapArticleButton.Enabled = false;
                            }
                        }
                    }                    
                }
                else
                {
                    string apiToken = Settings.Default.WorldAnvilApiToken;
                    if (!string.IsNullOrEmpty(apiToken))
                    {
                        if (ValidateMapImageId(apiToken, MapStateMediator.CurrentMap.WorldAnvilImageId))
                        {
                            CreateMapButton.Enabled = true;
                            CreateMapArticleButton.Enabled = true;

                            UpdateMap();
                        }
                        else
                        {
                            CreateMapButton.Enabled = false;
                            CreateMapArticleButton.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                // create new map
                CreateNewMap();
            }

        }

        private void CreateMapButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Create or update the Map object in World Anvil.\nThe map image must be validated before creating the World Anvil Map object.", this, new Point(ValidateImageIdButton.Left, ValidateImageIdButton.Top - 20), 3000);
        }

        private void CreateNewMap()
        {
            if (!string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WAUsername) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WAUserId) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.ApiKey) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.ApiToken) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WorldId))
            {
                try
                {
                    string mapTitle = MapTitleTextBox.Text;

                    WorldAnvilMap map = new()
                    {
                        title = mapTitle,

                        world = new WorldAnvilWorld
                        {
                            id = IntegrationManager.WorldAnvilParameters.WorldId
                        },

                        image = new WorldAnvilImage
                        {
                            id = validatedImageId
                        }
                    };

                    string json = JsonSerializer.Serialize(map, IntegrationManager.JsonSerializerHelper.CamelCaseIgnoreEmptyOptions);

                    // Parse the JSON text into a JsonDocument
                    JsonDocument createdMap = JsonDocument.Parse(json);

                    JsonElement mapRoot = createdMap.RootElement;
                    Dictionary<string, string> mapFlatJson = WorldAnvilApiMethods.FlattenJson(mapRoot);

                    mapFlatJson.TryGetValue("/title", out string? createdMapTitle);

                    JsonDocument? responseDocument = IntegrationManager.WorldAnvilApi.CreateMap(createdMap);

                    JsonElement createdMapRoot = responseDocument.RootElement;
                    Dictionary<string, string> createdMapFlatJson = WorldAnvilApiMethods.FlattenJson(createdMapRoot);

                    createdMapFlatJson.TryGetValue("/id", out string? createdMapId);

                    if (Guid.TryParse(createdMapId, out Guid createdMapGuid))
                    {
                        if (createdMapGuid != Guid.Empty)
                        {
                            MessageBox.Show("World Anvil Map object created successfully.\nMap Title: " + createdMapTitle + "\nMap ID: " + createdMapId, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MapStateMediator.CurrentMap.WorldAnvilMapId = createdMapGuid;
                            MapStateMediator.CurrentMap.WorldAnvilMapTitle = createdMapTitle ?? string.Empty;
                            MapStateMediator.CurrentMap.WorldAnvilImageId = validatedImageId;
                            MapStateMediator.CurrentMap.WorldAnvilUserId = Guid.Parse(IntegrationManager.WorldAnvilParameters.WAUserId);
                            MapStateMediator.CurrentMap.WorldAnvilWorldId = Guid.Parse(IntegrationManager.WorldAnvilParameters.WorldId);
                            MapIdLabel.Text = createdMapId;
                            MapTitleTextBox.Text = createdMapTitle;
                        }
                        else
                        {
                            MessageBox.Show("Failed to create World Anvil Map object.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating World Anvil Map object: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("World Anvil integration parameters are missing or invalid. Please validate the World Anvil API token on the World Anvil Integration dialog.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateMap()
        {
            if (!string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WAUsername) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WAUserId) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.ApiKey) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.ApiToken) &&
                !string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WorldId))
            {
                try
                {
                    string mapTitle = MapTitleTextBox.Text;

                    WorldAnvilMap map = new()
                    {
                        id = MapStateMediator.CurrentMap.WorldAnvilMapId.ToString(),
                        title = mapTitle,

                        world = new WorldAnvilWorld
                        {
                            id = IntegrationManager.WorldAnvilParameters.WorldId
                        },

                        image = new WorldAnvilImage
                        {
                            id = validatedImageId
                        }
                    };

                    string json = JsonSerializer.Serialize(map, IntegrationManager.JsonSerializerHelper.CamelCaseIgnoreEmptyOptions);

                    // Parse the JSON text into a JsonDocument
                    JsonDocument updatedMap = JsonDocument.Parse(json);

                    JsonElement mapRoot = updatedMap.RootElement;
                    Dictionary<string, string> mapFlatJson = WorldAnvilApiMethods.FlattenJson(mapRoot);

                    mapFlatJson.TryGetValue("/title", out string? updatedMapTitle);

                    JsonDocument? responseDocument = IntegrationManager.WorldAnvilApi.UpdateMap(map.id, updatedMap);

                    JsonElement updatedMapRoot = responseDocument.RootElement;
                    Dictionary<string, string> updatedMapFlatJson = WorldAnvilApiMethods.FlattenJson(updatedMapRoot);

                    updatedMapFlatJson.TryGetValue("/id", out string? updatedMapId);
                    updatedMapFlatJson.TryGetValue("/image/id", out string? updatedMapImageIdStr);

                    if (Guid.TryParse(updatedMapId, out Guid updatedMapGuid) && int.TryParse(updatedMapImageIdStr, out int updatedMapImageId))
                    {
                        if (updatedMapGuid != Guid.Empty && updatedMapImageId == validatedImageId && updatedMapTitle == MapTitleTextBox.Text)
                        {
                            MessageBox.Show("World Anvil Map object updated successfully.\nMap Title: " + updatedMapTitle + "\nMap ID: " + updatedMapId, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MapStateMediator.CurrentMap.WorldAnvilMapId = updatedMapGuid;
                            MapStateMediator.CurrentMap.WorldAnvilMapTitle = updatedMapTitle ?? string.Empty;
                            MapStateMediator.CurrentMap.WorldAnvilImageId = validatedImageId;
                            MapStateMediator.CurrentMap.WorldAnvilUserId = Guid.Parse(IntegrationManager.WorldAnvilParameters.WAUserId);
                            MapStateMediator.CurrentMap.WorldAnvilWorldId = Guid.Parse(IntegrationManager.WorldAnvilParameters.WorldId);
                            MapIdLabel.Text = updatedMapId;
                            MapTitleTextBox.Text = updatedMapTitle;
                        }
                        else
                        {
                            MessageBox.Show("Failed to update World Anvil Map object.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating World Anvil Map object: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("World Anvil integration parameters are missing or invalid. Please validate the World Anvil API token on the World Anvil Integration dialog.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateImageIdButton_Click(object sender, EventArgs e)
        {
            int mapImageId = 0;

            if (string.IsNullOrEmpty(MapImageIdTextBox.Text))
            {
                MessageBox.Show("Map Image ID is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateMapButton.Enabled = false;
                CreateMapArticleButton.Enabled = false;
                return;
            }

            if (int.TryParse(MapImageIdTextBox.Text, out mapImageId))
            {
                string apiToken = Settings.Default.WorldAnvilApiToken;

                if (string.IsNullOrEmpty(apiToken))
                {
                    MessageBox.Show("World Anvil API Token is not set. Please validate the World Anvil API token on the World Anvil Integration dialog.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CreateMapButton.Enabled = false;
                    CreateMapArticleButton.Enabled = false;
                    return;
                }

                if (ValidateMapImageId(apiToken, mapImageId))
                {
                    CreateMapButton.Enabled = true;
                    CreateMapArticleButton.Enabled = true;
                }
                else
                {
                    CreateMapButton.Enabled = false;
                    CreateMapArticleButton.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Map Image ID must be a valid integer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateMapButton.Enabled = false;
                CreateMapArticleButton.Enabled = false;
                return;
            }


        }

        private void ValidateImageIdButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Validate the map image identifier with World Anvil.\nThe map image must be uploaded to World Anvil before creating the World Anvil Map object.", this, new Point(ValidateImageIdButton.Left, ValidateImageIdButton.Top - 20), 3000);
        }

        private void WorldAnvilMapIntegration_Shown(object sender, EventArgs e)
        {
            CreateMapButton.Enabled = false;

            try
            {
                if (MapStateMediator.CurrentMap != null && MapStateMediator.CurrentMap.WorldAnvilMapId != Guid.Empty)
                {
                    MapIdLabel.Text = MapStateMediator.CurrentMap.WorldAnvilMapId.ToString();
                }

                if (MapStateMediator.CurrentMap != null && MapStateMediator.CurrentMap.WorldAnvilWorldId != Guid.Empty)
                {
                    MapWorldIdLabel.Text = MapStateMediator.CurrentMap.WorldAnvilWorldId.ToString();
                }

                if (MapStateMediator.CurrentMap != null && MapStateMediator.CurrentMap.WorldAnvilUserId != Guid.Empty)
                {
                    MapUserIdLabel.Text = MapStateMediator.CurrentMap.WorldAnvilUserId.ToString();
                }

                if (MapStateMediator.CurrentMap != null && !string.IsNullOrEmpty(MapStateMediator.CurrentMap.WorldAnvilMapTitle))
                {
                    MapTitleTextBox.Text = MapStateMediator.CurrentMap.WorldAnvilMapTitle;
                }

                if (MapStateMediator.CurrentMap != null && MapStateMediator.CurrentMap.WorldAnvilImageId != 0)
                {
                    MapImageIdTextBox.Text = MapStateMediator.CurrentMap.WorldAnvilImageId.ToString();

                    string apiToken = Settings.Default.WorldAnvilApiToken;
                    if (!string.IsNullOrEmpty(apiToken))
                    {
                        if (ValidateMapImageId(apiToken, MapStateMediator.CurrentMap.WorldAnvilImageId))
                        {
                            CreateMapButton.Enabled = true;
                            CreateMapArticleButton.Enabled = true;
                        }
                        else
                        {
                            CreateMapButton.Enabled = false;
                            CreateMapArticleButton.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading World Anvil Map data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateMapImageId(string userApiToken, int mapImageId)
        {
            Task apiKeyTask = Task.Run(() => IntegrationManager.WorldAnvilApi.GetWorldAnvilAPIKey());

            apiKeyTask.Wait();

            string? userId = string.Empty;
            try
            {
                IntegrationManager.WorldAnvilApi.SetWorldAnvilCredentials(userApiToken);
                userId = IntegrationManager.WorldAnvilApi.GetUserIdentity();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving World Anvil API Key: " + ex.Message, "API Key Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateMapButton.Enabled = false;
                return false;
            }

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Invalid User API Token", "The World Anvil API token provided is not valid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateMapButton.Enabled = false;
                return false;
            }

            JsonDocument? image = IntegrationManager.WorldAnvilApi.GetImageMetadataById(mapImageId.ToString(), 2);

            if (image is null)
            {
                MessageBox.Show("Invalid Image Id", "No World Anvil image could be retrieved using the provided image ID.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateMapButton.Enabled = false;
                return false;
            }

            Dictionary<string, string> userFlatJson = WorldAnvilApiMethods.FlattenJson(image.RootElement);
            userFlatJson.TryGetValue("/title", out string? title);
            userFlatJson.TryGetValue("/id", out string? retrievedImageId);
            userFlatJson.TryGetValue("/owner/id", out string? ownerId);
            userFlatJson.TryGetValue("/world/id", out string? worldId);

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(retrievedImageId))
            {
                ImageIdValidButton.IconChar = FontAwesome.Sharp.IconChar.Check;
                ImageIdValidButton.IconColor = Color.ForestGreen;

                if (string.IsNullOrEmpty(MapTitleTextBox.Text))
                {
                    MapTitleTextBox.Text = Path.GetFileNameWithoutExtension(title);
                }
                else
                {
                    MessageBox.Show("Invalid Image Object", "The image object retrieved from the World Anvil API is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CreateMapButton.Enabled = false;
                    return false;
                }

                if (!string.IsNullOrEmpty(ownerId))
                {
                    MapUserIdLabel.Text = ownerId;
                }
                else
                {
                    MessageBox.Show("Invalid Image Object", "The image object retrieved from the World Anvil API is missing the owner ID.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CreateMapButton.Enabled = false;
                    return false;
                }

                if (!string.IsNullOrEmpty(worldId))
                {
                    MapWorldIdLabel.Text = worldId;
                }
                else
                {
                    MessageBox.Show("Invalid Image Object", "The image object retrieved from the World Anvil API is missing the world ID.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CreateMapButton.Enabled = false;
                    return false;
                }
            }

            if (int.TryParse(retrievedImageId, out validatedImageId))
            {
                return true;

            }
            else
            {
                MessageBox.Show("Invalid Image ID", "The image ID retrieved from the World Anvil API is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateMapButton.Enabled = false;
                return false;
            }
        }

        private void CreateMapArticleButton_Click(object sender, EventArgs e)
        {

        }

        private void CreateMapArticleButton_MouseEnter(object sender, EventArgs e)
        {

        }
    }
}
