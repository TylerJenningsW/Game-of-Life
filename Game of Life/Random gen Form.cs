using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Random_gen_Form : Form
    {
        public int theSeed
        {
            get
            { return (int)numericUpDown1.Value; }
            set
            { numericUpDown1.Value = value; }
        }
        public Random_gen_Form()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Creates a random seed
            Random random = new Random();
            theSeed = random.Next();
        }
    }
}
