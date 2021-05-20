using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IO_Sudoku.src.model;

namespace IO_Sudoku
{
    public partial class Menu : Form
    {
        private User _loggedUser;
        public Menu(User user)
        {
            InitializeComponent();
            _loggedUser = user;
            centerPanel(panel1);
            centerPanel(panel2);

            panel2.Visible = false;
        }

        private void centerPanel(Panel panel)
        {
            panel.Location = new Point(
                this.ClientSize.Width / 2 - panel.Size.Width / 2,
                this.ClientSize.Height / 2 - panel.Size.Height / 2
                );
            panel.Anchor = AnchorStyles.None;
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

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;

            label1.Text = _loggedUser.Name;
            label2.Text = _loggedUser.Email;
            label3.Text = _loggedUser.TimePlayed.ToString();
            label4.Text = _loggedUser.JoinDate.ToString();
            panel2.Visible = true;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Visible = true;
        }
    }
}
