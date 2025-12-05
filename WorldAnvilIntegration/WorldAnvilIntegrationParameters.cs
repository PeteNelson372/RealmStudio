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
namespace RealmStudio.WorldAnvilIntegration
{
    internal class WorldAnvilIntegrationParameters
    {
        private string? _apiToken;          // the Realm Studio user API token taken from the World Anvil User API Tokens page at https://www.worldanvil.com/api/auth/key
                                            // this token is used to retrieve the World Anvil user id and username from the World Anvil API /identity endpoint

        private string? _apiKey;            // the Realm Studio application API key retrieved from the brookmonte.com endpoint
        private string? _worldId;           // the World Anvil world id for the selected world
        private string? _worldTitle;        // the World Anvil world title for the selected world
        private string? _waUserId;          // the World Anvil user id retrieved from the /identity endpoint using the API token
        private string? _waUsername;        // the World Anvil username retrieved from the /identity endpoint using the API token

        public string? ApiToken
        {
            get { return _apiToken; }
            set { _apiToken = value; }
        }

        public string? ApiKey
        {
            get { return _apiKey; }
            set { _apiKey = value; }
        }

        public string? WorldId
        {
            get { return _worldId; }
            set { _worldId = value; }
        }

        public string? WorldTitle
        {
            get { return _worldTitle; }
            set { _worldTitle = value; }
        }

        public string? WAUserId
        {
            get { return _waUserId; }
            set { _waUserId = value; }
        }

        public string? WAUsername
        {
            get { return _waUsername; }
            set { _waUsername = value; }
        }
    }
}
