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
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var userChoice = new UserChoice();
            userChoice.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var board = new MainForm();
            board.Show();
            this.Hide();
        }
    }
}
