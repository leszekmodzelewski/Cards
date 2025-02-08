using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace GeoLib.Winforms
{
    public partial class GenericWinFormForWpf : Form
    {
        private ElementHost ctrlHost;
        public GenericWinFormForWpf()
        {
            InitializeComponent();
        }

        public GenericWinFormForWpf(System.Windows.UIElement control) : this()
        {
            ctrlHost = new ElementHost { Dock = DockStyle.Fill };
            this.Controls.Add(ctrlHost);
            ctrlHost.Child = control;
        }
    }
}
