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
using System.Text.Json;
using WorldAnvilIntegrationLib;
namespace RealmStudio
{
    public partial class WorldAnvilIntegration : Form
    {
        public WorldAnvilIntegration()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.ApiToken) ||
                string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.ApiKey) ||
                string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WAUserId))
            {
                DialogResult result = MessageBox.Show("World Anvil user id, API token, or API key are not set. Close Anyway?", "Incomplete Parameters", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(IntegrationManager.WorldAnvilParameters.WorldId))
            {
                DialogResult result = MessageBox.Show("No World Selected. Close Anyway?", "No World Selected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            Close();
        }

        private void ValidateTokenButton_Click(object sender, EventArgs e)
        {
            APITokenValidButton.IconChar = FontAwesome.Sharp.IconChar.Cancel;
            APITokenValidButton.IconColor = Color.Red;

            if (string.IsNullOrWhiteSpace(APITokenTextBox.Text))
            {
                MessageBox.Show("Invalid User API Token", "Please enter a valid API token.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Task apiKeyTask = Task.Run(() => IntegrationManager.WorldAnvilApi.GetWorldAnvilAPIKey());

                apiKeyTask.Wait();

                IntegrationManager.WorldAnvilApi.SetWorldAnvilCredentials(APITokenTextBox.Text);

                string? userId = IntegrationManager.WorldAnvilApi.GetUserIdentity();

                if (string.IsNullOrEmpty(userId))
                {
                    MessageBox.Show("Invalid User API Token", "The World Anvil API token provided is not valid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                JsonDocument? user = IntegrationManager.WorldAnvilApi.GetUserById(userId, 2);

                if (user is null)
                {
                    MessageBox.Show("Invalid User API Token", "No World Anvil user could be retrieved using the provided World Anvil API token.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Dictionary<string, string> userFlatJson = WorldAnvilApiMethods.FlattenJson(user.RootElement);
                userFlatJson.TryGetValue("/username", out string? username);

                userFlatJson.TryGetValue("/id", out string? retrievedUserId);

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(retrievedUserId))
                {
                    APITokenValidButton.IconChar = FontAwesome.Sharp.IconChar.Check;
                    APITokenValidButton.IconColor = Color.ForestGreen;

                    UserNameLabel.Text = username;
                    UserIdLabel.Text = retrievedUserId;

                    List<JsonDocument> userWorlds = IntegrationManager.WorldAnvilApi.ListWorldsForUser(retrievedUserId, 100, 0);

                    UserWorldsList.Items.Clear();

                    foreach (JsonDocument world in userWorlds)
                    {
                        Dictionary<string, string> worldFlatJson = WorldAnvilApiMethods.FlattenJson(world.RootElement);
                        worldFlatJson.TryGetValue("/title", out string? worldTitle);
                        worldFlatJson.TryGetValue("/id", out string? worldId);
                        if (!string.IsNullOrEmpty(worldTitle) && !string.IsNullOrEmpty(worldId))
                        {
                            ListViewItem item = new(worldTitle);
                            item.SubItems.Add(worldId);
                            UserWorldsList.Items.Add(item);
                        }
                    }

                    IntegrationManager.WorldAnvilParameters.ApiToken = APITokenTextBox.Text;
                    IntegrationManager.WorldAnvilParameters.ApiKey = IntegrationManager.WorldAnvilApi.WorldAnvilAPIKey;
                    IntegrationManager.WorldAnvilParameters.WAUserId = retrievedUserId;
                    IntegrationManager.WorldAnvilParameters.WAUsername = username;
                }
                else
                {
                    MessageBox.Show("Invalid User Object", "The user object retrieved from the World Anvil API is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UserWorldsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserWorldsList.SelectedItems.Count == 0 || UserWorldsList.SelectedItems.Count > 1)
            {
                return;
            }

            ListViewItem selectedItem = UserWorldsList.SelectedItems[0];

            IntegrationManager.WorldAnvilParameters.WorldTitle = selectedItem.Text;
            IntegrationManager.WorldAnvilParameters.WorldId = selectedItem.SubItems[1].Text;
        }
    }
}
