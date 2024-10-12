/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
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
using System.Text.Json.Nodes;

namespace RealmStudio
{
    public partial class WonderdraftUserFolderImportDialog : Form
    {
        private readonly string AssetDirectory = Resources.ASSET_DIRECTORY;

        private readonly ToolTip TOOLTIP = new();
        private string WonderdraftAssetFolder = string.Empty;

        private readonly List<Tuple<string, string>> CollectionPaths = [];

        private readonly List<string> symbolCollectionFolders = [];
        private readonly List<string> frameFolders = [];
        private readonly List<string> boxFolders = [];
        private readonly List<string> groundTextureFolders = [];
        private readonly List<string> waterTextureFolders = [];

        public WonderdraftUserFolderImportDialog()
        {
            InitializeComponent();
        }

        private void UserFolderImportCloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChooseUserFolderButton_Click(object sender, EventArgs e)
        {
            string wonderdraftUserDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + Path.DirectorySeparatorChar
                + "Wonderdraft"
                + Path.DirectorySeparatorChar;

            FolderBrowserDialog fbd = new()
            {
                RootFolder = Environment.SpecialFolder.ApplicationData,
                InitialDirectory = wonderdraftUserDir,
                SelectedPath = wonderdraftUserDir + "assets",
                ShowNewFolderButton = false,
                ShowHiddenFiles = true,
                ShowPinnedPlaces = true,

            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = fbd.SelectedPath;
                WDUserFolderLabel.Text = selectedFolder;
                WonderdraftAssetFolder = selectedFolder;
            }
        }

        private void ChooseUserFolderButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Select Wonderdraft user asset folder", this);
        }

        private void ImportFoldersButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(WonderdraftAssetFolder))
            {
                string[] folders = Directory.GetDirectories(WonderdraftAssetFolder, "*", SearchOption.TopDirectoryOnly);

                foreach (string folder in folders)
                {
                    string collectionName = Path.GetFileNameWithoutExtension(folder);
                    CollectionListBox.Items.Add(collectionName);
                    CollectionPaths.Add(new Tuple<string, string>(collectionName, Path.GetFullPath(folder)));
                }
            }
        }

        private void AddAssetsButton_Click(object sender, EventArgs e)
        {
            boxFolders.Clear();
            frameFolders.Clear();
            groundTextureFolders.Clear();
            waterTextureFolders.Clear();
            symbolCollectionFolders.Clear();

            foreach (Tuple<string, string> collectionFolder in CollectionPaths)
            {
                string collectionName = collectionFolder.Item1;

                int itemIdx = CollectionListBox.Items.IndexOf(collectionName);

                if (itemIdx >= 0)
                {
                    if (CollectionListBox.GetItemChecked(itemIdx))
                    {
                        string folder = collectionFolder.Item2;

                        string[] collectionFiles = Directory.GetFiles(Path.GetFullPath(folder), "*", SearchOption.AllDirectories);

                        foreach (string file in collectionFiles)
                        {
                            if (file.EndsWith("\\textures\\boxes\\.wonderdraft_9patch"))
                            {
                                boxFolders.Add(Path.GetFullPath(folder));
                                break;
                            }
                            else if (file.EndsWith("\\textures\\frames\\.wonderdraft_9patch"))
                            {
                                frameFolders.Add(Path.GetFullPath(folder));
                                break;
                            }
                            else if (file.Contains("\\textures\\ground\\"))
                            {
                                groundTextureFolders.Add(Path.GetFullPath(folder));
                                break;
                            }
                            else if (file.Contains("\\textures\\water\\"))
                            {
                                waterTextureFolders.Add(Path.GetFullPath(folder));
                                break;
                            }
                            else if (file.Contains("\\sprites\\mountains\\") && file.EndsWith(".wonderdraft_symbols"))
                            {
                                symbolCollectionFolders.Add(Path.GetFullPath(folder));
                            }
                            else if (file.Contains("\\sprites\\trees\\") && file.EndsWith(".wonderdraft_symbols"))
                            {
                                symbolCollectionFolders.Add(Path.GetFullPath(folder));
                            }
                            else if (file.Contains("\\sprites\\symbols\\") && file.EndsWith(".wonderdraft_symbols"))
                            {
                                symbolCollectionFolders.Add(Path.GetFullPath(folder));
                            }
                        }
                    }
                }
            }

            if (symbolCollectionFolders.Count > 0
            || frameFolders.Count > 0
            || boxFolders.Count > 0
            || groundTextureFolders.Count > 0
            || waterTextureFolders.Count > 0)
            {
                string message = "Creating or adding:";

                if (symbolCollectionFolders.Count > 0)
                {
                    message += "\nSymbol collections: " + symbolCollectionFolders.Count;
                }

                if (frameFolders.Count > 0)
                {
                    message += "\nFrame sets: " + frameFolders.Count;
                }

                if (boxFolders.Count > 0)
                {
                    message += "\nBox outline sets: " + boxFolders.Count;
                }

                if (groundTextureFolders.Count > 0)
                {
                    message += "\nGround texture sets: " + groundTextureFolders.Count;
                }

                if (waterTextureFolders.Count > 0)
                {
                    message += "\nWater texture sets: " + waterTextureFolders.Count;
                }

                message += "\nContinue?";

                DialogResult result = MessageBox.Show(message, "Add Assets?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    if (symbolCollectionFolders.Count > 0)
                    {
                        CreateSymbolCollections(symbolCollectionFolders);
                    }

                    if (frameFolders.Count > 0)
                    {
                        LoadFrameAssets(frameFolders);
                    }

                    if (boxFolders.Count > 0)
                    {
                        LoadBoxOutlineAssets(boxFolders);
                    }

                    if (groundTextureFolders.Count > 0)
                    {
                        AddGroundTextures(groundTextureFolders);
                    }

                    if (waterTextureFolders.Count > 0)
                    {
                        AddWaterTextures(waterTextureFolders);
                    }
                }
            }
            else
            {
                MessageBox.Show("No selected asset folders found.", "No Selected Asset Folders", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void CreateSymbolCollections(List<string> sCollectionFolders)
        {
            string symbolAssetPath = AssetDirectory + Path.DirectorySeparatorChar + "Symbols" + Path.DirectorySeparatorChar;

            List<string> symbolFiles = [];

            foreach (string folder in sCollectionFolders)
            {
                string[] fileparts = folder.Split(Path.DirectorySeparatorChar);
                string collectionName = fileparts[^1];

                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = fileparts[^2];
                }

                string collectionPath = symbolAssetPath + collectionName;

                string[] frameFiles = Directory.GetFiles(Path.GetFullPath(folder), "*", SearchOption.AllDirectories);

                List<string> symbolTextures = [];
                string? wdSymbolFile = string.Empty;

                foreach (string file in frameFiles)
                {
                    if (file.StartsWith(folder) && file.EndsWith(".png"))
                    {
                        symbolTextures.Add(file);
                    }

                    if (file.StartsWith(folder) && file.EndsWith(".wonderdraft_symbols"))
                    {
                        wdSymbolFile = file;
                    }
                }

                // only create collection if a .wonderdraft_symbols file can be located
                if (wdSymbolFile != null)
                {
                    // create the collection directory if it doesn't exist
                    if (!Directory.Exists(collectionPath))
                    {
                        Directory.CreateDirectory(collectionPath);
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("The collection " + collectionName + " already exists. Overwrite it?", "Collection Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        if (result == DialogResult.Yes)
                        {
                            DialogResult confirm = MessageBox.Show("All files in " + collectionPath + " will be removed and replaced. Please confirm.", "Confirm Overwrite", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                            if (confirm == DialogResult.OK)
                            {
                                foreach (var filePath in Directory.EnumerateFiles(collectionPath))
                                {
                                    File.Delete(filePath);
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                    string[] wdfileparts = wdSymbolFile.Split(Path.DirectorySeparatorChar);

                    string wdFullPath = collectionPath + Path.DirectorySeparatorChar + wdfileparts[^1];

                    // extract the .wonderdraft_symbols file and save it to the collection directory
                    File.Copy(wdSymbolFile, wdFullPath, true);

                    // extract all of the .png files and save them
                    foreach (string symbol in symbolTextures)
                    {
                        string[] symbolfileparts = symbol.Split(Path.DirectorySeparatorChar);
                        string symbolFullPath = collectionPath + Path.DirectorySeparatorChar + symbolfileparts[^1];
                        File.Copy(symbol, symbolFullPath, true);
                    }
                }
            }

            MessageBox.Show("Copied " + sCollectionFolders.Count + " Wonderdraft symbol collections to the assets directory. Prepare the symbol collections for use by using the Create Symbol Collection tool accessed by the 'Assets > Create Symbol Collection' menu option for each collection.", "Assets Added",
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void LoadFrameAssets(List<string> frFolders)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string frameAssetsPath = AssetDirectory + Path.DirectorySeparatorChar + "Frames" + Path.DirectorySeparatorChar;

            // copy the frame bitmaps to the frame folder
            List<string> frameTextureFiles = [];
            List<string> frameFullPaths = [];

            List<MapFrame> mapFrames = [];

            foreach (string folder in frFolders)
            {
                string? wonderdraft9PatchFile = string.Empty;

                string[] frameFiles = Directory.GetFiles(Path.GetFullPath(folder), "*", SearchOption.AllDirectories);

                foreach (string file in frameFiles)
                {
                    if (file.StartsWith(folder) && file.Contains("\\textures\\frames\\") && file.EndsWith(".png"))
                    {
                        frameTextureFiles.Add(file);
                    }

                    if (file.StartsWith(folder) && file.Contains("\\textures\\frames\\") && file.EndsWith(".wonderdraft_9patch"))
                    {
                        wonderdraft9PatchFile = file;
                    }
                }

                foreach (string filename in frameTextureFiles)
                {
                    // copy the box .png files from Wonderdraft assets folder to the box assets folder
                    string[] fileparts = filename.Split(Path.DirectorySeparatorChar);
                    string textureFullPath = frameAssetsPath + fileparts[^1];
                    frameFullPaths.Add(textureFullPath);

                    if (!File.Exists(textureFullPath))
                    {
                        File.Copy(filename, textureFullPath, true);
                    }
                }

                string wd9PatchFullPath = Path.Combine(frameAssetsPath, ".wonderdraft_9patch");

                if (!string.IsNullOrEmpty(wonderdraft9PatchFile))
                {
                    File.Copy(wonderdraft9PatchFile, wd9PatchFullPath, true);
                }

                // load and parse the .wonderdraft_9patch file

                if (File.Exists(wd9PatchFullPath))
                {
                    string jsonString = File.ReadAllText(wd9PatchFullPath);

                    var jsonObject = JsonNode.Parse(jsonString).AsObject();

                    if (jsonObject != null)
                    {
                        foreach (KeyValuePair<string, JsonNode?> kvPair in jsonObject)
                        {
                            MapFrame frame = new();

                            frame.FrameName = kvPair.Key;

                            foreach (var kvp in kvPair.Value.AsObject().Where(kvp => kvp.Key == "margin"))
                            {
                                foreach (var marginkvp in kvp.Value.AsObject())
                                {
                                    if (marginkvp.Key == "top")
                                    {
                                        frame.FrameCenterTop = float.Parse(marginkvp.Value.ToString());
                                    }

                                    if (marginkvp.Key == "bottom")
                                    {
                                        frame.FrameCenterBottom = float.Parse(marginkvp.Value.ToString());
                                    }

                                    if (marginkvp.Key == "left")
                                    {
                                        frame.FrameCenterLeft = float.Parse(marginkvp.Value.ToString());
                                    }

                                    if (marginkvp.Key == "right")
                                    {
                                        frame.FrameCenterRight = float.Parse(marginkvp.Value.ToString());
                                    }
                                }
                            }


                            // set the frame bitmap path
                            string? fullpath = frameFullPaths.Find(x => x.EndsWith(frame.FrameName + ".png", StringComparison.OrdinalIgnoreCase));

                            if (fullpath != null)
                            {
                                frame.FrameBitmapPath = fullpath;
                            }

                            if (File.Exists(frame.FrameBitmapPath))
                            {
                                mapFrames.Add(frame);
                            }
                        }
                    }

                    // delete the 9 patch file
                    File.Delete(wd9PatchFullPath);
                }
            }

            // write the MapBox objects as XML to the box assets directory
            foreach (MapFrame f in mapFrames)
            {
                string xmlPath = frameAssetsPath + f.FrameName + ".xml";
                f.FrameXmlFilePath = xmlPath;
                MapFileMethods.SerializeFrameAsset(f);
            }

            MessageBox.Show("Frame assets have been copied from the Wonderdraft assets folder to the assets folder. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private void LoadBoxOutlineAssets(List<string> bFolders)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string boxAssetsPath = AssetDirectory + Path.DirectorySeparatorChar + "Boxes" + Path.DirectorySeparatorChar;

            // copy the box bitmaps to the box folder
            List<string> boxTextureFiles = [];
            List<string> boxFullPaths = [];

            List<MapBox> mapBoxes = [];

            foreach (string folder in bFolders)
            {
                string? wonderdraft9PatchFile = string.Empty;

                string[] boxFiles = Directory.GetFiles(Path.GetFullPath(folder), "*", SearchOption.AllDirectories);

                foreach (string file in boxFiles)
                {
                    if (file.StartsWith(folder) && file.Contains("\\textures\\boxes\\") && file.EndsWith(".png"))
                    {
                        boxTextureFiles.Add(file);
                    }

                    if (file.StartsWith(folder) && file.Contains("\\textures\\boxes\\") && file.EndsWith(".wonderdraft_9patch"))
                    {
                        wonderdraft9PatchFile = file;
                    }
                }

                foreach (string filename in boxTextureFiles)
                {
                    // copy the box .png files from Wonderdraft assets folder to the box assets folder
                    string[] fileparts = filename.Split(Path.DirectorySeparatorChar);
                    string textureFullPath = boxAssetsPath + fileparts[^1];
                    boxFullPaths.Add(textureFullPath);

                    if (!File.Exists(textureFullPath))
                    {
                        File.Copy(filename, textureFullPath, true);
                    }
                }

                string wd9PatchFullPath = Path.Combine(boxAssetsPath, ".wonderdraft_9patch");

                if (!string.IsNullOrEmpty(wonderdraft9PatchFile))
                {
                    File.Copy(wonderdraft9PatchFile, wd9PatchFullPath, true);
                }

                // load and parse the .wonderdraft_9patch file

                if (File.Exists(wd9PatchFullPath))
                {
                    string jsonString = File.ReadAllText(wd9PatchFullPath);

                    var jsonObject = JsonNode.Parse(jsonString).AsObject();

                    if (jsonObject != null)
                    {
                        foreach (KeyValuePair<string, JsonNode?> kvPair in jsonObject)
                        {
                            MapBox box = new();

                            box.BoxName = kvPair.Key;

                            foreach (var kvp in kvPair.Value.AsObject().Where(kvp => kvp.Key == "margin"))
                            {
                                foreach (var marginkvp in kvp.Value.AsObject())
                                {
                                    if (marginkvp.Key == "top")
                                    {
                                        box.BoxCenterTop = float.Parse(marginkvp.Value.ToString());
                                    }

                                    if (marginkvp.Key == "bottom")
                                    {
                                        box.BoxCenterBottom = float.Parse(marginkvp.Value.ToString());
                                    }

                                    if (marginkvp.Key == "left")
                                    {
                                        box.BoxCenterLeft = float.Parse(marginkvp.Value.ToString());
                                    }

                                    if (marginkvp.Key == "right")
                                    {
                                        box.BoxCenterRight = float.Parse(marginkvp.Value.ToString());
                                    }
                                }
                            }


                            // set the box bitmap path
                            string? fullpath = boxFullPaths.Find(x => x.EndsWith(box.BoxName + ".png", StringComparison.OrdinalIgnoreCase));

                            if (fullpath != null)
                            {
                                box.BoxBitmapPath = fullpath;
                            }

                            if (File.Exists(box.BoxBitmapPath))
                            {
                                mapBoxes.Add(box);
                            }
                        }
                    }

                    // delete the 9 patch file
                    File.Delete(wd9PatchFullPath);
                }
            }

            // write the MapBox objects as XML to the box assets directory
            foreach (MapBox box in mapBoxes)
            {
                string xmlPath = boxAssetsPath + box.BoxName + ".xml";
                box.BoxXmlFilePath = xmlPath;
                MapFileMethods.SerializeBoxAsset(box);
            }

            MessageBox.Show("Box assets have been copied from the Wonderdraft assets folder to the assets folder. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private void AddGroundTextures(List<string> gTextureFolders)
        {
            string groundTexturePath = AssetDirectory + Path.DirectorySeparatorChar + "Textures" + Path.DirectorySeparatorChar + "Land" + Path.DirectorySeparatorChar;

            List<string> groundTextureFiles = [];

            foreach (string folder in gTextureFolders)
            {
                string[] groundTextures = Directory.GetFiles(Path.GetFullPath(folder), "*", SearchOption.AllDirectories);
                foreach (string file in groundTextures)
                {
                    if (file.StartsWith(folder) && file.Contains("\\textures\\ground\\") && file.EndsWith(".png"))
                    {
                        groundTextureFiles.Add(file);
                    }
                }
            }

            foreach (string filename in groundTextureFiles)
            {
                string[] fileparts = filename.Split(Path.DirectorySeparatorChar);
                string textureFullPath = groundTexturePath + fileparts[^1];

                File.Copy(filename, textureFullPath, true);
            }

            MessageBox.Show("Copied " + groundTextureFiles.Count + " ground textures from Wonderdraft assets to assets directory. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void AddWaterTextures(List<string> wTextureFolders)
        {
            string waterTexturePath = AssetDirectory + Path.DirectorySeparatorChar + "Textures" + Path.DirectorySeparatorChar + "Water" + Path.DirectorySeparatorChar;

            List<string> waterTextureFiles = [];

            foreach (string folder in wTextureFolders)
            {
                string[] waterTextures = Directory.GetFiles(Path.GetFullPath(folder), "*", SearchOption.AllDirectories);
                foreach (string file in waterTextures)
                {
                    if (file.StartsWith(folder) && file.Contains("\\textures\\water\\") && file.EndsWith(".png"))
                    {
                        waterTextureFiles.Add(file);
                    }
                }
            }

            foreach (string filename in waterTextureFiles)
            {
                string[] fileparts = filename.Split(Path.DirectorySeparatorChar);
                string textureFullPath = waterTexturePath + fileparts[^1];

                File.Copy(filename, textureFullPath, true);
            }

            MessageBox.Show("Copied " + waterTextureFiles.Count + " water textures from Wonderdraft assets to assets directory. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
