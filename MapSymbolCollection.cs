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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using System.Xml.Serialization;

namespace RealmStudio
{
    [XmlRoot("MapSymbolCollection", Namespace = "RealmStudio", IsNullable = false)]
    public class MapSymbolCollection
    {
        [XmlIgnore]
        public bool IsModified { get; set; } = false;

        [XmlElement]
        public string CollectionFileVersion = "0.1";

        [XmlElement]
        public string CollectionName = "";

        [XmlElement]
        public Guid CollectionGuid = Guid.NewGuid();

        [XmlElement]
        public string CollectionLicense = "";

        [XmlElement]
        public string CollectionPath = "";

        [XmlArray]
        public List<MapSymbol> CollectionMapSymbols = [];

        public string GetCollectionName()
        {
            return CollectionName;
        }

        public void SetCollectionName(string name)
        {
            CollectionName = name;
        }

        public Guid GetCollectionGuid()
        {
            return CollectionGuid;
        }

        public string GetCollectionLicense()
        {
            return CollectionLicense;
        }

        public void SetCollectionLicense(string license)
        {
            CollectionLicense = license;
        }

        public string GetCollectionPath()
        {
            return CollectionPath;
        }

        public void SetCollectionPath(string path)
        {
            CollectionPath = path;
        }

        public List<MapSymbol> GetCollectionMapSymbols()
        {
            return CollectionMapSymbols;
        }

        public void AddCollectionMapSymbol(MapSymbol symbol)
        {
            CollectionMapSymbols.Add(symbol);
        }

        public int GetNumberOfTaggedSymbols()
        {
            int numberOfTaggedSymbols = 0;

            foreach (MapSymbol m in CollectionMapSymbols)
            {
                if (m.SymbolTags.Count > 0)
                {
                    numberOfTaggedSymbols++;
                }
            }

            return numberOfTaggedSymbols;
        }
    }
}
