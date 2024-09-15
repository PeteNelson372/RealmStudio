using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
