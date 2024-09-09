using SkiaSharp;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    class Landform : MapComponent, IXmlSerializable
    {
        public string LandformName { get; set; } = string.Empty;
        public Guid LandformGuid { get; } = Guid.NewGuid();

        public SKPath ContourPath { get; set; } = new SKPath();

        public SKPath DrawPath { get; set; } = new SKPath();

        // inner paths are used to paint the gradient shading around the inside of the landform
        public SKPath InnerPath1 { get; set; } = new SKPath();
        public SKPath InnerPath2 { get; set; } = new SKPath();
        public SKPath InnerPath3 { get; set; } = new SKPath();
        public SKPath InnerPath4 { get; set; } = new SKPath();
        public SKPath InnerPath5 { get; set; } = new SKPath();
        public SKPath InnerPath6 { get; set; } = new SKPath();
        public SKPath InnerPath7 { get; set; } = new SKPath();
        public SKPath InnerPath8 { get; set; } = new SKPath();

        // outer paths are used to paint the coastline effect around the outside of the landform
        public SKPath OuterPath1 { get; set; } = new SKPath();
        public SKPath OuterPath2 { get; set; } = new SKPath();
        public SKPath OuterPath3 { get; set; } = new SKPath();
        public SKPath OuterPath4 { get; set; } = new SKPath();
        public SKPath OuterPath5 { get; set; } = new SKPath();
        public SKPath OuterPath6 { get; set; } = new SKPath();
        public SKPath OuterPath7 { get; set; } = new SKPath();
        public SKPath OuterPath8 { get; set; } = new SKPath();


        public Landform()
        {
        }

        public override void Render(SKCanvas canvas)
        {
            throw new NotImplementedException();
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
