using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO_Sudoku
{
    public partial class UserChoice : Form
    {
        public UserChoice()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var menu = new Menu();
            //menu.Location = this.Location;
            menu.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
