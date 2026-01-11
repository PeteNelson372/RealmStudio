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
using SkiaSharp.Views.Desktop;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
namespace RealmStudio
{
    [XmlInclude(typeof(MapLayer))]
    internal sealed class MapFileMethods
    {
        private static void Serializer_UnknownNode(object? sender, XmlNodeEventArgs e)
        {
            Program.LOGGER.Error("Exception on Load. Unknown Node: " + e.Name + "\t" + e.Text);
        }

        private static void Serializer_UnknownAttribute(object? sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Program.LOGGER.Error("Exception on Load. Unknown Attribute: " + attr.Name + "\t" + attr.Value);
        }

        internal static RealmStudioMapRoot? OpenMapRoot(string mapPath)
        {
            XmlSerializer? serializer = new(typeof(RealmStudioMapRoot));
            
            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);
            
            // A FileStream is needed to read the XML document.            
            FileStream fs = new(mapPath, FileMode.Open);
            using XmlReader reader = XmlReader.Create(fs);

            // Declares an object variable of the type to be deserialized.
            RealmStudioMapRoot? mapRoot;
            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                mapRoot = serializer.Deserialize(reader) as RealmStudioMapRoot;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception deserializing " + mapPath + ": " + ex.Message);
                mapRoot = null;
                throw;
            }
            finally
            {
                serializer = null;
                fs.Dispose();
            }

            return mapRoot;
        }

        internal static RealmStudioMap? OpenMap(string mapPath)
        {
            XmlSerializer? serializer = new(typeof(RealmStudioMap));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.            
            FileStream fs = new(mapPath, FileMode.Open);
            using XmlReader reader = XmlReader.Create(fs);

            // Declares an object variable of the type to be deserialized.
            RealmStudioMap? map;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                map = serializer.Deserialize(reader) as RealmStudioMap;

                map?.IsSaved = false;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception deserializing " + mapPath + ": " + ex.Message);
                map = null;
                throw;
            }
            finally
            {
                serializer = null;
                fs.Dispose();
            }

            return map;
        }

        internal static void SaveMap(RealmStudioMap map)
        {
            using TextWriter? writer = new StreamWriter(map.MapPath);
            XmlSerializer? serializer = new(typeof(RealmStudioMap));

            try
            {
                // Serializes the map and closes the TextWriter.
                serializer.Serialize(writer, map);
                map.IsSaved = true;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception serializing " + map.MapPath + " Message: " + ex.Message);
                throw;
            }
        }

        internal static MapTheme? ReadThemeFromXml(string path)
        {
            XmlSerializer? serializer = new(typeof(MapTheme));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);
            using XmlReader reader = XmlReader.Create(fs);

            // Declares an object variable of the type to be deserialized.            
            MapTheme? theme;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                theme = serializer.Deserialize(reader) as MapTheme;
                return theme;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception deserializing " + path + " Message: " + ex.Message);
                theme = null;
            }
            finally
            {
                serializer = null;
            }

            return null;
        }

        internal static void SerializeTheme(MapTheme theme)
        {
            if (theme.ThemeName != null && theme.ThemeName.Length > 0 && theme.ThemePath != null && theme.ThemePath.Length > 0)
            {
                TextWriter? writer = new StreamWriter(theme.ThemePath);
                XmlSerializer? serializer = new(typeof(MapTheme));

                try
                {
                    // Serializes the theme and closes the TextWriter.
                    serializer.Serialize(writer, theme);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error("Error saving theme: " + ex.Message);

                    MessageBox.Show("Error saving theme: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    throw;
                }
                finally
                {
                    writer?.Dispose();
                }
            }
        }

        internal static MapSymbolCollection? ReadCollectionFromXml(string path)
        {
            XmlSerializer? serializer = new(typeof(MapSymbolCollection));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.            
            FileStream fs = new(path, FileMode.Open);
            using XmlReader reader = XmlReader.Create(fs);

            // Declares an object variable of the type to be deserialized.            
            MapSymbolCollection? symbolCollection;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                symbolCollection = serializer.Deserialize(reader) as MapSymbolCollection;
                return symbolCollection;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception deserializing " + path + " Message: " + ex.Message);
                symbolCollection = null;
            }
            finally
            {
                serializer = null;
                fs.Dispose();
            }

            return null;
        }

        internal static void SerializeSymbolCollection(MapSymbolCollection collection)
        {
            if (collection.GetCollectionName().Length > 0 && collection.GetCollectionPath().Length > 0)
            {
                TextWriter? writer = new StreamWriter(collection.GetCollectionPath(), false);
                XmlSerializer? serializer = new(typeof(MapSymbolCollection));

                try
                {
                    // Serializes the collection and closes the TextWriter.
                    serializer.Serialize(writer, collection);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error("Error saving collection: " + ex.Message);
                    MessageBox.Show("Error saving collection: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    writer?.Dispose();
                }
            }
        }

        internal static List<MapSymbol> ReadMapSymbolAssets(string path)
        {
            List<MapSymbol> symbols = [];

            var symbolfiles = from file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Order()
                              where file.Contains(".png")
                                  || file.Contains(".jpg")
                                  || file.Contains(".bmp")
                                  || file.Contains(".svg")
                              select new
                              {
                                  File = file
                              };

            foreach (var f in symbolfiles)
            {
                MapSymbol symbol = new();

                if (Path.GetExtension(f.File) == ".png")
                {
                    symbol.SymbolFormat = SymbolFileFormat.PNG;
                }
                else if (Path.GetExtension(f.File) == ".jpg")
                {
                    symbol.SymbolFormat = SymbolFileFormat.JPG;
                }
                else if (Path.GetExtension(f.File) == ".bmp")
                {
                    symbol.SymbolFormat = SymbolFileFormat.BMP;
                }
                else if (Path.GetExtension(f.File) == ".svg")
                {
                    symbol.SymbolFormat = SymbolFileFormat.Vector;
                }

                symbol.SymbolFilePath = f.File;

                if (symbol.SymbolFormat == SymbolFileFormat.PNG
                    || symbol.SymbolFormat == SymbolFileFormat.JPG
                    || symbol.SymbolFormat == SymbolFileFormat.BMP)
                {
                    // create the symbol SKBitmap from the bitmap at the path
                    try
                    {
                        symbol.SetSymbolBitmapFromPath(f.File);
                        SymbolManager.AnalyzeSymbolBitmapColors(symbol);
                    }
                    catch { }

                }
                else if (symbol.SymbolFormat == SymbolFileFormat.Vector)
                {
                    try
                    {
                        string symbolSVG = File.ReadAllText(f.File);
                        symbol.SymbolSVG = symbolSVG;
                    }
                    catch { }
                }


                symbols.Add(symbol);
            }

            return symbols;
        }

        internal static MapFrame? ReadFrameAssetFromXml(string path)
        {
            XmlSerializer? serializer = new(typeof(MapFrame));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            MapFrame? frame;

            try
            {
                using XmlReader reader = XmlReader.Create(fs);

                // Uses the Deserialize method to restore the object's state
                // with data from the XML document.
                frame = serializer.Deserialize(reader) as MapFrame;
                return frame;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception deserializing frame XML at " + path + " Message: " + ex.Message);
                frame = null;
            }
            finally
            {
                serializer = null;
            }

            return null;
        }

        internal static void SerializeFrameAsset(MapFrame frame)
        {
            if (!string.IsNullOrEmpty(frame.FrameXmlFilePath))
            {
                TextWriter? writer = new StreamWriter(frame.FrameXmlFilePath);
                XmlSerializer? serializer = new(typeof(MapFrame));

                try
                {
                    // Serializes the frame and closes the TextWriter.
                    serializer.Serialize(writer, frame);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error("Error saving frame: " + ex.Message);

                    MessageBox.Show("Error saving frame: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    writer?.Dispose();
                }
            }
        }

        internal static MapBox? ReadBoxAssetFromXml(string path)
        {
            XmlSerializer? serializer = new(typeof(MapBox));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            MapBox? box;

            try
            {
                using XmlReader reader = XmlReader.Create(fs);

                // Uses the Deserialize method to restore the object's state
                // with data from the XML document.
                box = serializer.Deserialize(reader) as MapBox;
                return box;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception deserializing box XML at " + path + " Message: " + ex.Message);
                box = null;
            }
            finally
            {
                serializer = null;
            }

            return null;
        }

        internal static void SerializeBoxAsset(MapBox box)
        {
            if (!string.IsNullOrEmpty(box.BoxXmlFilePath))
            {
                TextWriter? writer = new StreamWriter(box.BoxXmlFilePath);
                XmlSerializer? serializer = new(typeof(MapBox));

                try
                {
                    // Serializes the box and closes the TextWriter.
                    serializer.Serialize(writer, box);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error("Error saving box: " + ex.Message);

                    MessageBox.Show("Error saving box: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    writer?.Dispose();
                }
            }
        }
        internal static void SerializeLabelPreset(LabelPreset preset)
        {
            if (!string.IsNullOrEmpty(preset.PresetXmlFilePath))
            {
                TextWriter? writer = new StreamWriter(preset.PresetXmlFilePath);
                XmlSerializer? serializer = new(typeof(LabelPreset));

                try
                {
                    // Serializes the label preset and closes the TextWriter.
                    serializer.Serialize(writer, preset);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error("Error saving label preset: " + ex.Message);
                    MessageBox.Show("Error saving label preset: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    writer?.Dispose();
                }
            }
        }

        internal static LabelPreset? ReadLabelPreset(string path)
        {
            XmlSerializer? serializer = new(typeof(LabelPreset));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            LabelPreset? labelPreset;

            try
            {
                using XmlReader reader = XmlReader.Create(fs);

                // Uses the Deserialize method to restore the object's state
                // with data from the XML document.
                labelPreset = serializer.Deserialize(reader) as LabelPreset;
                return labelPreset;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Exception deserializing label preset XML at " + path + " Message: " + ex.Message);
                labelPreset = null;
            }
            finally
            {
                serializer = null;
            }

            return null;
        }

        internal static LandformShapingFunction? ReadShapingFunction(string path)
        {
            LandformShapingFunction lsf = new();

            try
            {
                lsf.ShapingBitmap = ((Bitmap)(Image.FromFile(path))).ToSKBitmap();

                if (Path.GetFileNameWithoutExtension(path).Contains("Region"))
                {
                    lsf.LandformShapeType = GeneratedLandformType.Region;
                }
                else if (Path.GetFileNameWithoutExtension(path).Contains("Continent"))
                {
                    lsf.LandformShapeType = GeneratedLandformType.Continent;
                }
                else if (Path.GetFileNameWithoutExtension(path).Contains("Island"))
                {
                    lsf.LandformShapeType = GeneratedLandformType.Island;
                }
                else if (Path.GetFileNameWithoutExtension(path).Contains("Archipelago"))
                {
                    lsf.LandformShapeType = GeneratedLandformType.Archipelago;
                }
                else if (Path.GetFileNameWithoutExtension(path).Contains("Atoll"))
                {
                    lsf.LandformShapeType = GeneratedLandformType.Atoll;
                }
                else if (Path.GetFileNameWithoutExtension(path).Contains("World"))
                {
                    lsf.LandformShapeType = GeneratedLandformType.World;
                }
                else if (Path.GetFileNameWithoutExtension(path).Contains("Icecap"))
                {
                    lsf.LandformShapeType = GeneratedLandformType.Icecap;
                }
            }
            catch { }

            return lsf;
        }
    }
}
