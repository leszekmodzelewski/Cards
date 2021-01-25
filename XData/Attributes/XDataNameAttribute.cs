namespace GeoLib.XData.Attributes
{
    using System;

    public sealed class XDataNameAttribute : Attribute
    {
        private readonly string name;

        public XDataNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name =>
            this.name;
    }
}

