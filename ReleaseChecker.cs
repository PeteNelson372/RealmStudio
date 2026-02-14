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
using System.Text.Json;

namespace RealmStudioX
{
    internal sealed class ReleaseChecker
    {
        private static readonly HttpClient _httpClient = new();

        public static async Task<List<(string, string)>> FetchRealmStudioGithubReleasesAsync()
        {
            string url = $"https://api.github.com/repos/PeteNelson372/RealmStudio/releases";

            List<(string, string)> lstReleases = [];

            try
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("RealmStudio/1.0");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var releases = JsonDocument.Parse(json).RootElement;

                foreach (var release in releases.EnumerateArray())
                {
                    string? name = release.GetProperty("name").GetString();
                    string? date = release.GetProperty("published_at").GetString();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(date))
                    {
                        DateTime parsedDate = DateTime.Parse(date);
                        lstReleases.Add((parsedDate.ToString("yyyy-MM-dd"), name));
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request error: {ex.Message}");
            }
            catch (JsonException)
            {
                MessageBox.Show("Failed to parse GitHub response.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}");
            }

            return lstReleases;
        }

        internal static string? HasNewRelease(List<(string, string)> releases, string currentVersion)
        {
            if (releases.Count > 0)
            {
                // get the latest release and compare it to the current release
                (string, string)? latestRelease = releases[0];
                if (latestRelease != null)
                {
                    string latestReleaseDate = latestRelease.Value.Item1;
                    string latestReleaseName = latestRelease.Value.Item2;

                    string latestReleaseVersionNumber = latestReleaseName[(latestReleaseName.LastIndexOf(' ') + 1)..].Trim();
                    string currentVersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

                    string[] latestVersionNumbers = latestReleaseVersionNumber.Split('.');
                    string[] currentVersionNumbers = currentVersionNumber.Split('.');

                    bool isNewer = false;

                    if (latestVersionNumbers.Length != 4 && latestVersionNumbers.Length != currentVersionNumbers.Length)
                    {
                        return null;  // version numbers must have four segments to compare
                    }

                    // compare each segment of the version numbers
                    // e.g. 1.2.3 vs 1.2.4
                    // if the latest version number is greater than the current version number, then it is a newer release
                    // if the latest version number is less than the current version number, then it is not a newer release
                    // if the version numbers are equal, then it is not a newer release
                    // this assumes that the version numbers are in the format Major.Minor.Patch.Build (e.g. 1.2.3.5)
                    // and that the version numbers are all integers

                    if (int.TryParse(currentVersionNumbers[0], out int currentMajorVersion))
                    {
                        if (int.TryParse(latestVersionNumbers[0], out int latestMajorVersion))
                        {
                            if (latestMajorVersion > currentMajorVersion)
                            {
                                isNewer = true;
                            }
                        }
                    }

                    if (!isNewer)
                    {
                        if (int.TryParse(currentVersionNumbers[1], out int currentMinorVersion) && int.TryParse(latestVersionNumbers[1], out int latestMinorVersion))
                        {
                            if (latestMinorVersion > currentMinorVersion)
                            {
                                isNewer = true;
                            }
                        }
                    }

                    if (!isNewer)
                    {
                        if (int.TryParse(currentVersionNumbers[2], out int currentPatchVersion) && int.TryParse(latestVersionNumbers[2], out int latestPatchVersion))
                        {
                            if (latestPatchVersion > currentPatchVersion)
                            {
                                isNewer = true;
                            }
                        }
                    }

                    if (!isNewer)
                    {
                        if (int.TryParse(currentVersionNumbers[3], out int currentBuildVersion) && int.TryParse(latestVersionNumbers[3], out int latestBuildVersion))
                        {
                            if (latestBuildVersion > currentBuildVersion)
                            {
                                isNewer = true;
                            }
                        }
                    }

                    if (isNewer)
                    {
                        return latestReleaseVersionNumber; // return the latest release version number
                    }
                }
            }

            return null; // No new release found or no releases available
        }
    }
}


