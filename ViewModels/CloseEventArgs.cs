using System;
using System.Windows.Forms;

namespace GeoLib.ViewModels
{
    public class CloseEventArgs : EventArgs
    {
        public CloseEventArgs(DialogResult result)
        {
            Result = result;
        }

        public DialogResult Result { get; }
    }
}