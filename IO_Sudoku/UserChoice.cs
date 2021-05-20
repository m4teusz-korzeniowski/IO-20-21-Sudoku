using IO_Sudoku.src.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IO_Sudoku.src.exceptions;

namespace IO_Sudoku
{
    public partial class UserChoice : Form
    {
        List<User> users = new List<User>();

        public UserChoice()
        {
            InitializeComponent();
            centerPanel(panel1);
            centerPanel(panel2);
            centerPanel(panel3);
            panel3.AutoScroll = true;
            centerPanel(panel4);
            centerPanel(panel5);


            button3.Enabled = false; //tymczasowo wyłączone

            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;

            label1.Text = "";
            label2.Text = "";
            button7.Visible = false; // fake button do łatwego tworzenia następnych

            button6.Enabled = false; // globalny user tymczasowo wyłączony
            button9.Enabled = false; // globalny user tymczasowo wyłączony
        }

        private void centerPanel(Panel panel)
        {
            panel.Location = new Point(
                this.ClientSize.Width / 2 - panel.Size.Width / 2,
                this.ClientSize.Height / 2 - panel.Size.Height / 2
                );
            panel.Anchor = AnchorStyles.None;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //var menu = new Menu();
            //menu.Location = this.Location;
            //menu.Show();
            //this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel4.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
        }

        private void registerUser()
        {
            bool isUserAddedProperly = false;
            User u = new User();
            try
            {
                u.Name = textBox1.Text;
                u.Email = textBox2.Text;
                isUserAddedProperly = addUser(u);
                if (isUserAddedProperly)
                {
                    users.Add(u);
                    panel4.Visible = false;
                    panel1.Visible = true;
                }
            }
            catch (IncorrectEmailException ex)
            {
                label2.Text = ex.Message;
            }
            catch (InitCharIsNotALetterException ex)
            {
                label1.Text = ex.Message;
            }
            catch (EpmtyFieldException ex)
            {
                if(textBox1.Text == "")
                {
                    label1.Text = ex.Message;
                }
                if(textBox2.Text == "")
                {
                    label2.Text = ex.Message;
                }
            }
            
            

        }

        private bool addUser(User user)
        {
            return !users.Contains(user);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            registerUser();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel5.Visible = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            bool isEmpty = !users.Any();
            if (isEmpty)
            {
                button7.Visible = true;
            }
            else
            {
                button7.Visible = false;
                label3.Visible = false;
                createUserButtons();
            }
            panel5.Visible = false;
            panel3.Visible = true;
        }

        private void createUserButtons()
        {
            Point position = button7.Location;
            foreach (User user in users)
            {
                Button button = new Button();
                button.Location = position;
                button.Text = user.Name;
                button.Click += new EventHandler(userClick);

                panel3.Controls.Add(button);
                button.Visible = true;

                position.Y += 50;


            }
        }

        private void userClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            User user = users.First(u => u.Name == button.Text);
            var menu = new Menu(user);
            menu.Location = this.Location;
            menu.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            panel1.Visible = true;
        }
    }
}
