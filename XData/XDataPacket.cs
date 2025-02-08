

namespace GeoLib.XData
{
    using ZwSoft.ZwCAD.DatabaseServices;

    public class XDataPacket
    {
        public static XDataPacket Deserialize(ResultBuffer buffer)
        {
            TypedValue[] valueArray = buffer.AsArray();
            int num = 0;
            string text1 = (string)valueArray[num++].Value;
            XDataPacket packet1 = new XDataPacket();
            packet1.Name = (string)valueArray[num++].Value;
            packet1.Version = (int)valueArray[num++].Value;
            packet1.Data = (string)valueArray[num++].Value;
            return packet1;
        }

        public static ResultBuffer Serialize(XDataPacket data)
        {
            TypedValue[] values = new TypedValue[] { new TypedValue(0x3e9, "GEOLIB"), new TypedValue(0x3e8, data.Name), new TypedValue(0x42f, data.Version), new TypedValue(0x3e8, data.Data) };
            return new ResultBuffer(values);
        }

        public string Name { get; set; }

        public int Version { get; set; }

        public string Data { get; set; }
    }
}

