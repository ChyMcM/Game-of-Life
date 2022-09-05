using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL4
{
    public partial class unisettings : Form
    {
        public bool accept = false;
        public unisettings()
        {
            InitializeComponent();
        }

        public void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown size = new NumericUpDown();
        }

        public void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown seedran = new NumericUpDown();
        }

        public void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown speed = new NumericUpDown();
        }
        public int sizeofuni()
        {
            return Convert.ToInt32(numericUpDown1.Value);
        }
        public int randseed()
        {
            return (int)numericUpDown2.Value;
        }
        public int speedofuni()
        {
            return (int)numericUpDown3.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            accept = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
