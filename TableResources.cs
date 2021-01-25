

namespace GeoLib
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TableResources
    {
        public ObjectId TopLeft;
        public ObjectId TopRight;
        public ObjectId BottomLeft;
        public ObjectId BottomRight;
        public ObjectId Frame;
    }
}

