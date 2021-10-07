

namespace GeoLib
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using GeoLib.XData.Attributes;
    using System;

    [XDataName("APP")]
    public class XDataApp
    {
        private int version;
        private double defaultTableSize;

        public XDataApp()
        {
            this.version = 1;
            this.defaultTableSize = 1500.0;
        }

        public XDataApp(ResultBuffer buffer)
        {
            this.version = 1;
            this.defaultTableSize = 1500.0;
            TypedValue[] valueArray = buffer.AsArray();
            int num = 0;
            string text1 = (string) valueArray[num++].Value;
            this.version = (int) valueArray[num++].Value;
            this.defaultTableSize = (double) valueArray[num++].Value;
        }

        public ResultBuffer AsBuffer()
        {
            TypedValue[] values = new TypedValue[] { new TypedValue(0x3e9, "GEOLIB"), new TypedValue(0x42f, this.version), new TypedValue(0x410, this.defaultTableSize) };
            return new ResultBuffer(values);
        }

        public int Version
        {
            get => 
                this.version;
            set { this.version = value; }
        }

        public double DefaultTableSize
        {
            get => 
                this.defaultTableSize;
            set { this.defaultTableSize = value; }
        }
    }
}

