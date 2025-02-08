

namespace GeoLib.XData
{
    using GeoLib.XData.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ZwSoft.ZwCAD.DatabaseServices;

    public static class XDataSerializer
    {
        private static Dictionary<Type, SerializableTypeData> serializableTypesByType = new Dictionary<Type, SerializableTypeData>();
        private static Dictionary<string, SerializableTypeData> serializableTypesByName = new Dictionary<string, SerializableTypeData>();

        static XDataSerializer()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                object[] customAttributes = type.GetCustomAttributes(typeof(XDataNameAttribute), true);
                if (customAttributes.Length != 0)
                {
                    if (customAttributes.Length > 1)
                    {
                        throw new InvalidOperationException();
                    }
                    string name = (customAttributes.FirstOrDefault<object>() as XDataNameAttribute).Name;
                    XDataVersionAttribute attribute = type.GetCustomAttributes(typeof(XDataVersionAttribute), true).FirstOrDefault<object>() as XDataVersionAttribute;
                    SerializableTypeData data = new SerializableTypeData
                    {
                        Type = type,
                        Name = name,
                        Version = (attribute != null) ? attribute.Version : 0
                    };
                    serializableTypesByType.Add(type, data);
                    serializableTypesByName.Add(name, data);
                }
            }
        }

        public static object Deserialize(ResultBuffer buffer)
        {
            SerializableTypeData data;
            XDataPacket packet = XDataPacket.Deserialize(buffer);
            if (!serializableTypesByName.TryGetValue(packet.Name, out data))
            {
                throw new ArgumentException();
            }
            return JsonConvert.DeserializeObject(packet.Data, data.Type);
        }

        public static ResultBuffer Serialize(object obj)
        {
            SerializableTypeData data;
            Type key = obj.GetType();
            if (!serializableTypesByType.TryGetValue(key, out data))
            {
                throw new ArgumentException();
            }
            XDataPacket packet1 = new XDataPacket();
            packet1.Name = data.Name;
            packet1.Version = data.Version;
            packet1.Data = JsonConvert.SerializeObject(obj);
            return XDataPacket.Serialize(packet1);
        }

        private class SerializableTypeData
        {
            public System.Type Type { get; set; }

            public string Name { get; set; }

            public int Version { get; set; }
        }
    }
}

