using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media.Media3D;


namespace GeoLib.Winforms
{
    public partial class ImportedFilesWindow : Form
    {


        public ImportedFilesWindow()
        {
            InitializeComponent();
        }



        public void FillListBox(List<Point3D> points)
        {
            this.label1.Text = $@"Imported {points.Count} points count";
            this.listBox1.Items.Clear();

            foreach (var point3D in points)
            {
                this.listBox1.Items.Add(point3D);
            }
        }
    }


}
