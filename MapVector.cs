using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RealmStudio
{
    internal class MapVector
    {
        public MapVector()
        {
            VectorName = string.Empty;
            VectorPath = string.Empty;
        }

        public MapVector(string name, string path)
        {
            VectorName = name;
            VectorPath = path;
        }

        public string VectorName { get; set; }

        public string VectorPath { get; set; }

        public float ViewBoxSizeWidth { get; set; }

        public float ViewBoxSizeHeight { get; set; }

        [XmlIgnore]
        public string? VectorSvg { get; set; }

        [XmlIgnore]
        public SKPath? VectorSkPath { get; set; }

        [XmlIgnore]
        public SKPath? ScaledPath { get; set; }
    }
}
