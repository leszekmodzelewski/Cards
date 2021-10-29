using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoLib.Winforms
{
    public partial class CalculateDataWindow : Form
    {
        public CalculateDataWindow()
        {
            InitializeComponent();
            this.textBoxFileToOpen.Text = @"D:\Temp\TestsFileWithCoords.txt";
            this.textBoxScaleFactor.Text = "1500";
        }

        private void LoadFile_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            this.textBoxFileToOpen.Text = this.openFileDialog1.FileName;
        }

        public string FileName => this.textBoxFileToOpen.Text;

        public int ScaleFactor
        {
            get
            {
                if (int.TryParse(this.textBoxScaleFactor.Text, out var scaleFactor))
                {
                    return scaleFactor;
                }

                MessageBox.Show("You Idiot!!!! now it is 1");
                return 1;
            }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
