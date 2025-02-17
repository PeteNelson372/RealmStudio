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
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace RealmStudio
{
    public partial class WonderdraftAssetImportDialog : Form
    {
        private readonly string AssetDirectory = Settings.Default.MapAssetDirectory;

        private string ZipFilePath = string.Empty;

        private readonly List<string> symbolCollectionFolders = [];
        private readonly List<string> frameFolders = [];
        private readonly List<string> boxFolders = [];
        private readonly List<string> groundTextureFolders = [];
        private readonly List<string> waterTextureFolders = [];

        private readonly List<string> ZipFiles = [];

        public WonderdraftAssetImportDialog()
        {
            InitializeComponent();
        }

        private void AssetsImportCloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChooseZipFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Choose Wonderdraft Asset Zip File",
                    DefaultExt = "zip",
                    Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*",
                    CheckFileExists = true,
                    Multiselect = false,
                    RestoreDirectory = true,
                    ShowHelp = false,
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(ofd.FileName))
                    {
                        ZipFilePathLabel.Text = ofd.FileName;
                        ZipFilePath = ofd.FileName;

                        FilePreviewTree.Nodes.Clear();
                    }
                }
            }
            catch { }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            FilePreviewTree.Nodes.Clear();
            ZipFiles.Clear();

            if (!string.IsNullOrEmpty(ZipFilePath))
            {
                using ZipArchive archive = ZipFile.OpenRead(ZipFilePath);

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    ZipFiles.Add(entry.FullName);

                    string[] pathEntries = entry.FullName.Split(['/', Path.DirectorySeparatorChar]);

                    TreeNodeCollection nodes = FilePreviewTree.Nodes;

                    for (int i = 0; i < pathEntries.Length; i++)
                    {
                        string pathEntry = pathEntries[i];

                        if (!string.IsNullOrEmpty(pathEntry))
                        {
                            if (!nodes.ContainsKey(pathEntry))
                            {
                                TreeNode t = new(pathEntry)
                                {
                                    Text = pathEntry,
                                    Name = pathEntry,
                                    Tag = entry.FullName,
                                };

                                nodes.Add(t);
                                nodes = t.Nodes;
                            }
                            else
                            {
                                TreeNode? t = nodes[pathEntry];

                                if (t != null)
                                {
                                    nodes = t.Nodes;
                                }
                            }
                        }
                    }
                }
            }

            FilePreviewTree.ExpandAll();
            LocateAssetFolders();
        }

        private void AddCollectionButton_Click(object sender, EventArgs e)
        {
            List<string> selectedSymbolFolders = [];
            List<string> selectedFrameFolders = [];
            List<string> selectedBoxFolders = [];
            List<string> selectedGroundFolders = [];
            List<string> selectedWaterFolders = [];

            foreach (string folder in symbolCollectionFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    if (collectionNode.Checked)
                    {
                        selectedSymbolFolders.Add(folder);
                    }

                }
            }

            foreach (string folder in frameFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    if (collectionNode.Checked)
                    {
                        selectedFrameFolders.Add(folder);
                    }

                }
            }

            foreach (string folder in boxFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    if (collectionNode.Checked)
                    {
                        selectedBoxFolders.Add(folder);
                    }

                }
            }

            foreach (string folder in groundTextureFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    if (collectionNode.Checked)
                    {
                        selectedGroundFolders.Add(folder);
                    }

                }
            }

            foreach (string folder in waterTextureFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    if (collectionNode.Checked)
                    {
                        selectedWaterFolders.Add(folder);
                    }

                }
            }

            if (selectedSymbolFolders.Count > 0
                || selectedFrameFolders.Count > 0
                || selectedBoxFolders.Count > 0
                || selectedGroundFolders.Count > 0
                || selectedWaterFolders.Count > 0)
            {
                string message = "Creating or adding:";

                if (selectedSymbolFolders.Count > 0)
                {
                    message += "\nSymbol collections: " + selectedSymbolFolders.Count;
                }

                if (selectedFrameFolders.Count > 0)
                {
                    message += "\nFrame sets: " + selectedFrameFolders.Count;
                }

                if (selectedBoxFolders.Count > 0)
                {
                    message += "\nBox outline sets: " + selectedBoxFolders.Count;
                }

                if (selectedGroundFolders.Count > 0)
                {
                    message += "\nGround texture sets: " + selectedGroundFolders.Count;
                }

                if (selectedWaterFolders.Count > 0)
                {
                    message += "\nWater texture sets: " + selectedWaterFolders.Count;
                }

                message += "\nContinue?";

                DialogResult result = MessageBox.Show(message, "Add Assets?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    if (selectedSymbolFolders.Count > 0)
                    {
                        CreateSymbolCollections(selectedSymbolFolders);
                    }

                    if (selectedFrameFolders.Count > 0)
                    {
                        LoadFrameAssets(selectedFrameFolders);
                    }

                    if (selectedBoxFolders.Count > 0)
                    {
                        LoadBoxOutlineAssets(selectedBoxFolders);
                    }

                    if (selectedGroundFolders.Count > 0)
                    {
                        AddGroundTextures(selectedGroundFolders);
                    }

                    if (selectedWaterFolders.Count > 0)
                    {
                        AddWaterTextures(selectedWaterFolders);
                    }
                }
            }
            else
            {
                MessageBox.Show("No selected asset folders found.", "No Selected Asset Folders", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void LocateAssetFolders()
        {
            // criteria for a symbol collection
            // ../sprites/[trees|mountains|symbols]/<collection name>/
            //      .wonderdraft_symbols
            //      (png asset files)

            // criteria for frames
            // ../textures/frames/
            //      .wonderdraft_9patch (if no .wonderdraft_9patch file, can one be generated from the .png file?)
            //      (png asset files)

            // criteria for boxes
            // ../textures/boxes/
            //      .wonderdraft_9patch
            //      (png asset files)

            // criteria for textures
            // ../textures/[ground|water]/
            //      (png asset files)

            for (int i = 0; i < ZipFiles.Count - 1; i++)
            {
                if ((ZipFiles[i].Contains("/sprites/trees/") || ZipFiles[i].Contains("/sprites/mountains/") || ZipFiles[i].Contains("/sprites/symbols/"))
                    && !ZipFiles[i].EndsWith("/sprites/trees/")
                    && !ZipFiles[i].EndsWith("/sprites/mountains/")
                    && !ZipFiles[i].EndsWith("/sprites/symbols/")
                    && !ZipFiles[i + 1].EndsWith(".wonderdraft_symbols")
                    && !ZipFiles[i + 1].EndsWith(".png"))
                {
                    symbolCollectionFolders.Add(ZipFiles[i]);
                }

                if (ZipFiles[i].EndsWith(".wonderdraft_symbols"))
                {
                    symbolCollectionFolders.Add(ZipFiles[i - 1]);
                }

                if (ZipFiles[i].EndsWith("textures/frames/.wonderdraft_9patch"))
                {
                    frameFolders.Add(ZipFiles[i - 1]);
                }

                if (ZipFiles[i].EndsWith("textures/boxes/.wonderdraft_9patch"))
                {
                    boxFolders.Add(ZipFiles[i - 1]);
                }

                if (ZipFiles[i].EndsWith("textures/ground/"))
                {
                    groundTextureFolders.Add(ZipFiles[i]);
                }

                if (ZipFiles[i].EndsWith("textures/water/"))
                {
                    waterTextureFolders.Add(ZipFiles[i]);
                }
            }

            foreach (string folder in symbolCollectionFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    collectionNode.BackColor = Color.LightCyan;
                    collectionNode.Checked = true;

                }
            }

            foreach (string folder in frameFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    collectionNode.BackColor = Color.LightGreen;
                    collectionNode.Checked = true;
                }
            }

            foreach (string folder in boxFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    collectionNode.BackColor = Color.LightSteelBlue;
                    collectionNode.Checked = true;
                }
            }

            foreach (string folder in groundTextureFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    collectionNode.BackColor = Color.Khaki;
                    collectionNode.Checked = true;
                }
            }

            foreach (string folder in waterTextureFolders)
            {
                TreeNode? collectionNode = null;

                foreach (TreeNode node in FilePreviewTree.Nodes)
                {
                    collectionNode = FromTag(folder, node);

                    if (collectionNode != null)
                    {
                        break;
                    }
                }

                if (collectionNode != null)
                {
                    collectionNode.BackColor = Color.LightSeaGreen;
                    collectionNode.Checked = true;
                }
            }

        }

        public static TreeNode? FromTag(string tag, TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                if (((string)node.Tag) == tag)
                {
                    return node;
                }

                TreeNode? next = FromTag(tag, node);
                if (next != null)
                {
                    return next;
                }
            }

            return null;
        }

        private void CreateSymbolCollections(List<string> selectedSymbolFolders)
        {
            string symbolAssetPath = AssetDirectory + Path.DirectorySeparatorChar + "Symbols" + Path.DirectorySeparatorChar;

            List<string> symbolFiles = [];

            foreach (string filename in selectedSymbolFolders)
            {
                string[] fileparts = filename.Split('/');
                string collectionName = fileparts[^1];

                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = fileparts[^2];
                }

                string collectionPath = symbolAssetPath + collectionName;

                List<string> symbolTextures = ZipFiles.FindAll(x => x.StartsWith(filename) && x.EndsWith(".png"));
                string? wdSymbolFile = ZipFiles.Find(x => x.StartsWith(filename) && x.EndsWith(".wonderdraft_symbols"));

                // only create collection if a .wonderdraft_symbols file can be located
                if (wdSymbolFile != null)
                {
                    // create the collection directory if it doesn't exist
                    if (!Directory.Exists(collectionPath))
                    {
                        Directory.CreateDirectory(collectionPath);
                    }

                    string[] wdfileparts = wdSymbolFile.Split('/');

                    string wdFullPath = collectionPath + Path.DirectorySeparatorChar + wdfileparts[^1];

                    // extract the .wonderdraft_symbols file and save it to the collection directory
                    ExtractAndSaveFile(wdSymbolFile, wdFullPath);

                    // extract all of the .png files and save them
                    foreach (string symbol in symbolTextures)
                    {
                        string[] symbolfileparts = symbol.Split('/');
                        string symbolFullPath = collectionPath + Path.DirectorySeparatorChar + symbolfileparts[^1];
                        ExtractAndSaveFile(symbol, symbolFullPath);
                    }
                }


            }

            MessageBox.Show("The selected symbol collections have been copied to the assets directory. Prepare the symbol collections for use by using the Create Symbol Collection tool accessed by the 'Assets > Create Symbol Collection' menu option for each collection.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void LoadFrameAssets(List<string> selectedFrameFolders)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string frameAssetsPath = AssetDirectory + Path.DirectorySeparatorChar + "Frames" + Path.DirectorySeparatorChar;

            // copy the frame bitmaps to the frame folder
            List<string> frameTextureFiles = [];
            List<string> frameFullPaths = [];

            List<MapFrame> mapFrames = [];

            foreach (string filename in selectedFrameFolders)
            {
                List<string> frameTextures = ZipFiles.FindAll(x => x.StartsWith(filename) && x.EndsWith(".png"));
                frameTextureFiles.AddRange(frameTextures);
            }

            foreach (string filename in frameTextureFiles)
            {
                // extract the frame .png files from the archive and save them to the frame assets folder
                string[] fileparts = filename.Split('/');
                string textureFullPath = frameAssetsPath + fileparts[^1];
                frameFullPaths.Add(textureFullPath);

                ExtractAndSaveFile(filename, textureFullPath);
            }

            foreach (string filepath in selectedFrameFolders)
            {
                // get the .wonderdraft_9patch file
                string? wonderdraft9PatchFile = ZipFiles.Find(x => x.StartsWith(filepath) && x.EndsWith(".wonderdraft_9patch"));
                string wd9PatchFullPath = Path.Combine(frameAssetsPath, ".wonderdraft_9patch");

                if (!string.IsNullOrEmpty(wonderdraft9PatchFile))
                {
                    ExtractAndSaveFile(wonderdraft9PatchFile, wd9PatchFullPath);
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
                            MapFrame frame = new()
                            {
                                FrameName = kvPair.Key
                            };

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

            // write the MapFrame objects as XML to the frame assets directory
            foreach (MapFrame frame in mapFrames)
            {
                string xmlPath = frameAssetsPath + frame.FrameName + ".xml";
                frame.FrameXmlFilePath = xmlPath;
                MapFileMethods.SerializeFrameAsset(frame);
            }

            MessageBox.Show("Frame assets have been loaded from the selected archive. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private void LoadBoxOutlineAssets(List<string> selectedBoxFolders)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string boxAssetsPath = AssetDirectory + Path.DirectorySeparatorChar + "Boxes" + Path.DirectorySeparatorChar;

            // copy the box bitmaps to the box folder
            List<string> boxTextureFiles = [];
            List<string> boxFullPaths = [];

            List<MapBox> mapBoxes = [];

            foreach (string filename in selectedBoxFolders)
            {
                List<string> boxTextures = ZipFiles.FindAll(x => x.StartsWith(filename) && x.EndsWith(".png"));
                boxTextureFiles.AddRange(boxTextures);
            }

            foreach (string filename in boxTextureFiles)
            {
                // extract the frame .png files from the archive and save them to the box assets folder
                string[] fileparts = filename.Split('/');
                string textureFullPath = boxAssetsPath + fileparts[^1];
                boxFullPaths.Add(textureFullPath);

                ExtractAndSaveFile(filename, textureFullPath);
            }

            foreach (string filepath in selectedBoxFolders)
            {
                // get the .wonderdraft_9patch file
                string? wonderdraft9PatchFile = ZipFiles.Find(x => x.StartsWith(filepath) && x.EndsWith(".wonderdraft_9patch"));
                string wd9PatchFullPath = Path.Combine(boxAssetsPath, ".wonderdraft_9patch");

                if (!string.IsNullOrEmpty(wonderdraft9PatchFile))
                {
                    ExtractAndSaveFile(wonderdraft9PatchFile, wd9PatchFullPath);
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
                            MapBox box = new()
                            {
                                BoxName = kvPair.Key
                            };

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

            MessageBox.Show("Box assets have been loaded from the selected archive. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private void AddGroundTextures(List<string> selectedGroundFolders)
        {
            string landTexturePath = AssetDirectory + Path.DirectorySeparatorChar + "Textures" + Path.DirectorySeparatorChar + "Land" + Path.DirectorySeparatorChar;

            List<string> landTextureFiles = [];

            foreach (string filename in selectedGroundFolders)
            {
                List<string> landTextures = ZipFiles.FindAll(x => x.StartsWith(filename) && x.EndsWith(".png"));
                landTextureFiles.AddRange(landTextures);
            }

            foreach (string filename in landTextureFiles)
            {
                string[] fileparts = filename.Split('/');
                string textureFullPath = landTexturePath + fileparts[^1];

                ExtractAndSaveFile(filename, textureFullPath);
            }

            MessageBox.Show("Land textures from zip file added to assets directory. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

        }

        private void AddWaterTextures(List<string> selectedWaterFolders)
        {
            string waterTexturePath = AssetDirectory + Path.DirectorySeparatorChar + "Textures" + Path.DirectorySeparatorChar + "Water" + Path.DirectorySeparatorChar;

            List<string> waterTextureFiles = [];

            foreach (string filename in selectedWaterFolders)
            {
                List<string> waterTextures = ZipFiles.FindAll(x => x.StartsWith(filename) && x.EndsWith(".png"));
                waterTextureFiles.AddRange(waterTextures);
            }

            foreach (string filename in waterTextureFiles)
            {
                string[] fileparts = filename.Split('/');
                string textureFullPath = waterTexturePath + fileparts[^1];

                ExtractAndSaveFile(filename, textureFullPath);
            }

            MessageBox.Show("Water textures from zip file added to assets directory. Reload assets using the 'Assets > Reload All Assets' menu option or restart Map Creator to use them.", "Assets Added", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void ExtractAndSaveFile(string zipPath, string fullPath)
        {
            using ZipArchive archive = ZipFile.OpenRead(ZipFilePath);
            ZipArchiveEntry? entry = archive.GetEntry(zipPath);

            if (entry != null)
            {
                try
                {
                    if (!File.Exists(fullPath))
                    {
                        entry.ExtractToFile(fullPath, true);
                    }
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("Failed to copy " + zipPath + " to " + fullPath, "File Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }
    }
}
