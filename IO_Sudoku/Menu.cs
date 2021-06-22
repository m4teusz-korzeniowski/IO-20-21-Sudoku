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
using System.IO;

namespace IO_Sudoku
{
    public partial class Menu : Form
    {
        private User _loggedUser;
        private string path;
        public Menu(User user)
        {
            InitializeComponent();
            _loggedUser = user;
            centerPanel(panel1);
            centerPanel(panel2);
            centerPanel(panel3);

            label12.ForeColor = Color.Gold;
            label13.ForeColor = Color.Silver;
            label10.ForeColor = Color.SandyBrown;

            panel2.Visible = false;
            panel3.Visible = false;

            if(user.Password != null)
            {
                path = "global/" + user.Name + "/highscore.txt";
                createHighscores("global");
            }
            else
            {
                path = "local/" + user.Name + "/highscore.txt";
                createHighscores("local");
            }
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    label5.Text = "Najlepszy wynik: " + sr.ReadLine();
                }
            }
            else
            {
                label5.Text = "Najlepszy wynik: 0";
            }          
        }

        private void createHighscores(string userType)
        {
            DirectoryInfo localDirectory = new DirectoryInfo(userType);
            DirectoryInfo[] localUsers = localDirectory.GetDirectories();

            Dictionary<string, int> userAndHighscores = new Dictionary<string, int>();
            foreach (DirectoryInfo dir in localUsers)
            {
                FileInfo[] files = dir.GetFiles("*.txt");
                string name = dir.Name;
                foreach (FileInfo file in files)
                {
                    string value = File.ReadAllText(file.FullName);
                    userAndHighscores.Add(name, Convert.ToInt32(value));
                }
            }

            if(userAndHighscores.Count == 0)
            {
                label12.Text = " - ";
                label13.Text = " - ";
                label10.Text = " - ";
            }
            if (userAndHighscores.Count == 1)
            {
                label13.Text = " - ";
                label10.Text = " - ";
            }
            if (userAndHighscores.Count == 2)
            {
                label10.Text = " - ";
            }

            int i = 0;
            foreach (var item in userAndHighscores.OrderByDescending(key => key.Value))
            {
                if( i == 0)
                {
                    label12.Text = item.Key + " - " + item.Value;
                }
                else if( i == 1)
                {
                    label13.Text = item.Key + " - " + item.Value;
                }
                else if( i == 2)
                {
                    label10.Text = item.Key + " - " + item.Value;
                }
                else
                {
                    break;
                }
                i++;
            }
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
            var board = new Sudoku.Form1(path,_loggedUser.Name);
            board.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;

            label1.Text = _loggedUser.Name;
            label2.Text = _loggedUser.Email;
            label4.Text = _loggedUser.JoinDate.ToString();
            panel2.Visible = true;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Visible = true;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;

            /*label12.Text = _loggedUser.Name;
            label13.Text = _loggedUser.Email;
            label10.Text = _loggedUser.JoinDate.ToString();*/
            panel3.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            panel1.Visible = true;
        }
    }
}
