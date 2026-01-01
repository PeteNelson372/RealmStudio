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
using System.Text.Json;
using WorldAnvilIntegrationLib;
namespace RealmStudio.WorldAnvilIntegration;

public partial class WorldAnvilIntegrationParams : Form
{
    private static readonly ToolTip TOOLTIP = new();

    public WorldAnvilIntegrationParams()
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
            ValidateUserApiToken(APITokenTextBox.Text);
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

        if (MapStateMediator.MainUIMediator != null)
        {
            MapStateMediator.MainUIMediator.MainForm.WorldAnvilMapButton.Visible = true;
            MapStateMediator.MainUIMediator.MainForm.WorldAnvilMapButton.Enabled = true;
        }

    }

    private void ValidateTokenButton_MouseHover(object sender, EventArgs e)
    {
        TOOLTIP.Show("Validate the World Anvil User API Key with World Anvil", this, new Point(ValidateTokenButton.Left, ValidateTokenButton.Top - 20), 3000);
    }

    private bool ValidateUserApiToken(string userApiToken)
    {
        try
        {
            Task apiKeyTask = Task.Run(() => IntegrationManager.WorldAnvilApi.GetWorldAnvilAPIKey());

            apiKeyTask.Wait();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error getting World Anvil API Key: " + ex.Message, "API Key Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        string? userId = string.Empty;
        try
        {
            IntegrationManager.WorldAnvilApi.SetWorldAnvilCredentials(userApiToken);
            userId = IntegrationManager.WorldAnvilApi.GetUserIdentity();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error retrieving World Anvil user ID: " + ex.Message, "User ID Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (string.IsNullOrEmpty(userId))
        {
            MessageBox.Show("Invalid User API Token", "The World Anvil API token provided is not valid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        WorldAnvilUser? user = IntegrationManager.WorldAnvilApi.GetWorldAnvilUserObjectById(userId, 2);

        if (user is null)
        {
            MessageBox.Show("Invalid User API Token", "No World Anvil user could be retrieved using the provided World Anvil API token.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!string.IsNullOrEmpty(user.username) && !string.IsNullOrEmpty(user.id))
        {
            APITokenValidButton.IconChar = FontAwesome.Sharp.IconChar.Check;
            APITokenValidButton.IconColor = Color.ForestGreen;

            UserNameLabel.Text = user.username;
            UserIdLabel.Text = user.id;

            List<WorldAnvilWorld> userWorlds = IntegrationManager.WorldAnvilApi.ListWorldObjectsForUser(user.id, 100, 0);

            UserWorldsList.Items.Clear();

            foreach (WorldAnvilWorld world in userWorlds)
            {
                if (!string.IsNullOrEmpty(world.title) && !string.IsNullOrEmpty(world.id))
                {
                    ListViewItem item = new(world.title);
                    item.SubItems.Add(world.id);
                    UserWorldsList.Items.Add(item);
                }
            }

            IntegrationManager.WorldAnvilParameters.ApiToken = userApiToken;
            IntegrationManager.WorldAnvilParameters.ApiKey = IntegrationManager.WorldAnvilApi.WorldAnvilAPIKey;
            IntegrationManager.WorldAnvilParameters.WAUserId = user.id;
            IntegrationManager.WorldAnvilParameters.WAUsername = user.username;

            Settings.Default.WorldAnvilApiToken = userApiToken;
            Settings.Default.Save();

        }
        else
        {
            MessageBox.Show("Invalid User Object", "The user object retrieved from the World Anvil API is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    private void WorldAnvilIntegration_Shown(object sender, EventArgs e)
    {
        APITokenValidButton.IconChar = FontAwesome.Sharp.IconChar.Cancel;
        APITokenValidButton.IconColor = Color.Red;

        if (!string.IsNullOrEmpty(Settings.Default.WorldAnvilApiToken))
        {
            if (ValidateUserApiToken(Settings.Default.WorldAnvilApiToken))
            {
                APITokenTextBox.Text = IntegrationManager.WorldAnvilParameters.ApiToken;
                UserNameLabel.Text = IntegrationManager.WorldAnvilParameters.WAUsername;
                UserIdLabel.Text = IntegrationManager.WorldAnvilParameters.WAUserId;

                foreach (ListViewItem item in UserWorldsList.Items)
                {
                    if (item.SubItems[1].Text == IntegrationManager.WorldAnvilParameters.WorldId)
                    {
                        item.Selected = true;
                        item.Focused = true;
                        item.EnsureVisible();
                        break;
                    }
                }
            }
            else
            {
                ResetWorldAnvilIntegrationParameters();
            }
        }
    }

    private void ResetButton_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to clear all World Anvil Integration parameters?", "Confirm Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            ResetWorldAnvilIntegrationParameters();
        }
    }

    private void ResetButton_MouseHover(object sender, EventArgs e)
    {
        TOOLTIP.Show("Clear all World Anvil Integration Parameters", this, new Point(ResetButton.Left, ResetButton.Top - 20), 3000);
    }

    private void ResetWorldAnvilIntegrationParameters()
    {
        Settings.Default.WorldAnvilApiToken = string.Empty;

        IntegrationManager.WorldAnvilParameters.WAUsername = string.Empty;
        IntegrationManager.WorldAnvilParameters.WAUserId = string.Empty;
        IntegrationManager.WorldAnvilParameters.ApiKey = string.Empty;
        IntegrationManager.WorldAnvilParameters.ApiToken = string.Empty;
        IntegrationManager.WorldAnvilParameters.WorldId = string.Empty;

        MapStateMediator.MainUIMediator?.MainForm.WorldAnvilMapButton.Visible = false;
        MapStateMediator.MainUIMediator?.MainForm.WorldAnvilMapButton.Enabled = false;

        Settings.Default.Save();

        UserWorldsList.Items.Clear();

        APITokenValidButton.IconChar = FontAwesome.Sharp.IconChar.Cancel;
        APITokenValidButton.IconColor = Color.Red;

        APITokenTextBox.Text = Settings.Default.WorldAnvilApiToken;
        UserNameLabel.Text = IntegrationManager.WorldAnvilParameters.WAUsername;
        UserIdLabel.Text = IntegrationManager.WorldAnvilParameters.WAUserId;
    }
}
