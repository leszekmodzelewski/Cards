namespace GeoLib.XData.Attributes
{
    using System;

    public sealed class XDataVersionAttribute : Attribute
    {
        private int version;

        public XDataVersionAttribute(int version)
        {
            this.version = version;
        }

        public int Version =>
            this.version;
    }
}

