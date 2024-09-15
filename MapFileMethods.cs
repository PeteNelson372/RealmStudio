﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RealmStudio
{
    // are these two includes needed? If so, does MapSymbol need to be included to get IXmlSerializable implementation to work for it?
    [XmlInclude(typeof(MapLayer))]
    internal class MapFileMethods
    {
        protected static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Program.LOGGER.Error("Exception on Load. Unknown Node: " + e.Name + "\t" + e.Text);
        }

        protected static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Program.LOGGER.Error("Exception on Load. Unknown Attribute: " + attr.Name + "\t" + attr.Value);
        }

        internal static RealmStudioMap OpenMap(string mapPath)
        {
            XmlSerializer? serializer = new(typeof(RealmStudioMap));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // A FileStream is needed to read the XML document.            
            FileStream fs = new(mapPath, FileMode.Open);

            // Declares an object variable of the type to be deserialized.
            RealmStudioMap? map;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                map = serializer.Deserialize(fs) as RealmStudioMap;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                map.IsSaved = false;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            MapTheme? theme;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                theme = serializer.Deserialize(fs) as MapTheme;
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
                }
                finally
                {
                    writer?.Dispose();
                }
            }
        }

        internal static MapSymbolCollection? ReadCollectionFromXml(string path)
        {
            XmlSerializer? serializer = new XmlSerializer(typeof(MapSymbolCollection));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // A FileStream is needed to read the XML document.            
            FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            MapSymbolCollection? symbolCollection;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                symbolCollection = serializer.Deserialize(fs) as MapSymbolCollection;
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
                TextWriter? writer = new StreamWriter(collection.GetCollectionPath());
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
                    symbol.SymbolFormat = SymbolFormatEnum.PNG;
                }
                else if (Path.GetExtension(f.File) == ".jpg")
                {
                    symbol.SymbolFormat = SymbolFormatEnum.JPG;
                }
                else if (Path.GetExtension(f.File) == ".bmp")
                {
                    symbol.SymbolFormat = SymbolFormatEnum.BMP;
                }
                else if (Path.GetExtension(f.File) == ".svg")
                {
                    symbol.SymbolFormat = SymbolFormatEnum.Vector;
                }

                symbol.SymbolFilePath = f.File;

                if (symbol.SymbolFormat == SymbolFormatEnum.PNG
                    || symbol.SymbolFormat == SymbolFormatEnum.JPG
                    || symbol.SymbolFormat == SymbolFormatEnum.BMP)
                {
                    // create the symbol SKBitmap from the bitmap at the path
                    try
                    {
                        symbol.SetSymbolBitmapFromPath(f.File);
                        SymbolMethods.AnalyzeSymbolBitmapColors(symbol);
                    }
                    catch { }

                }
                else if (symbol.SymbolFormat == SymbolFormatEnum.Vector)
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
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            MapFrame? frame;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                frame = serializer.Deserialize(fs) as MapFrame;
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
            XmlSerializer? serializer = new(typeof(MapFrame));

            // If the XML document has been altered with unknown
            // nodes or attributes, handle them with the
            // UnknownNode and UnknownAttribute events.
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            MapBox? box;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                box = serializer.Deserialize(fs) as MapBox;
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
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // A FileStream is needed to read the XML document.            
            using FileStream fs = new(path, FileMode.Open);

            // Declares an object variable of the type to be deserialized.            
            LabelPreset? labelPreset;

            try
            {
                // Uses the Deserialize method to restore the object's state
                // with data from the XML document. */
                labelPreset = serializer.Deserialize(fs) as LabelPreset;
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
    }
}
