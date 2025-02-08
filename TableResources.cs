

namespace GeoLib
{
    using System.Runtime.InteropServices;
    using ZwSoft.ZwCAD.DatabaseServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TableResources
    {
        public ObjectId TopLeft;
        public ObjectId TopRight;
        public ObjectId BottomLeft;
        public ObjectId BottomRight;
        public ObjectId Frame;
        public ObjectId Point;
    }
    public struct TableResources2
    {
        public ObjectId Point;
    }
}

