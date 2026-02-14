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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RealmStudioX
{
    internal sealed class AiIntegration
    {

        private static readonly HttpClient _httpClient = new();

        public static async Task<string?> GetJwtTokenAsync()
        {
            string? tokenEndpoint = Environment.GetEnvironmentVariable("BKMT_WP_API_TOKEN_ENDPOINT");
            string? tokenUid = Environment.GetEnvironmentVariable("BKMT_WP_API_TOKEN_UID");
            string? tokenPw = Environment.GetEnvironmentVariable("BKMT_WP_API_TOKEN_PW");

            var requestData = new
            {
                username = tokenUid,
                password = tokenPw
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("token").GetString();
        }

        public static async Task<string?> GetAiCallData(string token)
        {
            string? aiEndpoint = Environment.GetEnvironmentVariable("BKMT_WP_AI_CALLDATA_ENDPOINT");

            var request = new HttpRequestMessage(HttpMethod.Get, aiEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            return root.GetProperty("message").GetString();
        }

        public static async Task<string?> GetAIDescriptionAsync(string aiCallData, string query)
        {
            if (string.IsNullOrEmpty(aiCallData))
            {
                throw new Exception("AI call data format is empty or null.");
            }

            string[] callFields = aiCallData.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (callFields.Length != 3)
            {
                throw new Exception("Invalid AI call data format.");
            }

            string uid = callFields[0].Trim();
            string pw = callFields[1].Trim();
            string wpUrl = callFields[2].Trim();

            var request = new HttpRequestMessage(HttpMethod.Post, wpUrl);
            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{uid}:{pw}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            string requestBody = "{\"role\": \"user\", \"message\": \"" + query + "\"}";

            try
            {
                string json = JsonSerializer.Serialize(requestBody);
            }
            catch (Exception ex)
            {
                throw new Exception("Error serializing JSON data", ex);
            }

            StringContent content = new(requestBody, Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            string? description = root.GetProperty("data").GetString();

            return description;
        }
    }
}
