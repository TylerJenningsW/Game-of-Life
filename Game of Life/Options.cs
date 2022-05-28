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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }
        public int UniverseWidth
        {
            get
            {
                // get the value from the numeric up/down universe width window
                return (int)WidthUpDown.Value;
            }
            set
            {
                // set the value from the numeric up/down universe width window equal to a value
                WidthUpDown.Value = value;
            }
        }
        public int UniverseHeight
        {
            get
            {
                // get the value from the numeric up/down universe height window
                return (int)HeightUpDown.Value;
            }
            set
            {
                // set the value from the numeric up/down universe height window equal to a value
                HeightUpDown.Value = value;
            }
        }
        public int TimeInterval
        {
            get
            {
                // get the value from the numeric up/down timer interval window
                return (int)IntervalUpDown.Value;
            }
            set
            {
                // set the value from the numeric up/down timer interval window equal to a value
                IntervalUpDown.Value = value;
            }
        }
    }
}
