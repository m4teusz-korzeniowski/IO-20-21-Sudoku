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
using System.IO;
using System.Text.Json;
using System.Net.Mail;
using System.Net;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage;

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;


namespace IO_Sudoku
{
    public partial class UserChoice : Form
    {
        List<User> users = new List<User>();
        List<User> globalUsers = new List<User>();
        SmtpClient smtpClient;

        private bool isPositionCorrected = false;

        string connectionString = "GPu5x3qbpzJ4kDSViR5RQ56TpgXx9zJNhmAoMm7l3GZwfvakkBZN2mMsIkcJkxEfarvzY+R973ltsAy9VZqgCg==";
        string storageAccountName = "sudoku10";

        Button backButton = new Button();

        public UserChoice()
        {
            InitializeComponent();
            
            centerPanel(panel1);
            centerPanel(panel2);
            centerPanel(panel3);
            panel3.AutoScroll = true;
            centerPanel(panel4);
            centerPanel(panel5);
            centerPanel(panel6);
            centerPanel(panel7);
            centerPanel(panel8);
            Directory.CreateDirectory("local");
            Directory.CreateDirectory("global");

            loadUsers();
            loadUsersGlobalUsers();

            //GuestButton.Enabled = false; //tymczasowo wyłączone

            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
            //panel8.Visible = false;

            label1.Text = "Nazwa";
            label2.Text = "E-mail";
            label3.Text = "Nie ma stworzonych użytkowników";
            label4.Text = "Nazwa";
            label6.Text = "E-mail";
            label5.Text = "E-mail";
            label7.Text = "Hasło";


            textBox6.PasswordChar = '*';
            button7.Visible = false; // fake button do łatwego tworzenia następnych

            //button6.Enabled = false; // globalny user tymczasowo wyłączony
            //button9.Enabled = false; // globalny user tymczasowo wyłączony


            smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("io.sudoku.2021@gmail.com", "Pork21Boar"),
                EnableSsl = true,
            };
            //smtpClient.Send("io.sudoku.2021@gmail.com", "vilhjajmur@gmail.com", "Test", "1234321");
            //smtpClient.Send("io.sudoku.2021@gmail.com", "vilhjajmur@gmail.com", "Test", generateLoginToken());
        }

        private string generateLoginToken()
        {
            Random r = new Random();
            const string chars = "1234567890!@#$%^&*()-_=+" +
                "QWERTYUIOPADFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm,./<>?;:[{}]";
            return new string(Enumerable.Repeat(chars, 20).Select(s => s[r.Next(s.Length)]).ToArray());
        }

        private void loadUsers()
        {
            DirectoryInfo localDirectory = new DirectoryInfo("local");
            DirectoryInfo[] localUsers = localDirectory.GetDirectories();
            foreach(DirectoryInfo dir in localUsers)
            {
                FileInfo[] files = dir.GetFiles("*.json");
                foreach (FileInfo file in files)
                {
                    User u = new User();
                    string userInfo = File.ReadAllText(file.FullName);
                    u = JsonSerializer.Deserialize<User>(userInfo);

                    bool isUserAddedProperly = addUser(u);
                    if (isUserAddedProperly)
                    {
                        users.Add(u);
                    }
                }
            }  
        }

        private void loadUsersGlobalUsers()
        {
            DirectoryInfo localDirectory = new DirectoryInfo("global");

            var storageAccount = new CloudStorageAccount(
                        new StorageCredentials(storageAccountName, connectionString), true);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var containter = blobClient.GetContainerReference("users");
            containter.CreateIfNotExists();

            var blobs = containter.ListBlobs();

            downloadBlobs(blobs, "");



            DirectoryInfo[] localUsers = localDirectory.GetDirectories();


            foreach (DirectoryInfo dir in localUsers)
            {
                FileInfo[] files = dir.GetFiles("*.json");
                foreach (FileInfo file in files)
                {
                    User u = new User();
                    string userInfo = File.ReadAllText(file.FullName);
                    u = JsonSerializer.Deserialize<User>(userInfo);

                    bool isUserAddedProperly = addUser(u);
                    if (isUserAddedProperly)
                    {
                        globalUsers.Add(u);
                    }
                }
            }
        }

        private void downloadBlobs(IEnumerable<IListBlobItem> blobs, string dir)
        {
            foreach(var blob in blobs)
            {
                if(blob is CloudBlockBlob blockBlob)
                {
                    blockBlob.DownloadToFile(dir +  blockBlob.Name, FileMode.Create);
                }
                else if(blob is CloudBlobDirectory blobDirectory)
                {
                    Directory.CreateDirectory("global/" + blobDirectory.Prefix);
                    downloadBlobs(blobDirectory.ListBlobs(), "global/");
                }
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

        private void button3_Click(object sender, EventArgs e)
        {
            //var menu = new Menu();
            //menu.Location = this.Location;
            //menu.Show();
            //this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                    Directory.CreateDirectory("local//" + u.Name);
                    string userInfo = JsonSerializer.Serialize(u);
                    File.WriteAllText("local//" + u.Name + "//" + u.Name + ".json", userInfo);
                    
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

        private void registerGlobalUser()
        {
            bool isUserAddedProperly = false;
            User u = new User();

            
            try
            {
                u.Name = textBox3.Text;
                u.Email = textBox4.Text;
                isUserAddedProperly = addGlobalUser(u);
                if (isUserAddedProperly)
                {
                    string pass = generateLoginToken();
                    u.Password = pass;
                    globalUsers.Add(u);
                    smtpClient.Send("io.sudoku.2021@gmail.com",
                        u.Email, "Test", pass);
                    
                    panel6.Visible = false;
                    panel1.Visible = true;
                    Directory.CreateDirectory("global//" + u.Name);
                    string userInfo = JsonSerializer.Serialize(u);
                    File.WriteAllText("global//" + u.Name + "//" + u.Name + ".json", userInfo);

                    string containerName = "users";
                    string blobName = u.Name + ".json";
                    string filePath = "global//" + u.Name + "//" + u.Name + ".json";


                    var storageAccount = new CloudStorageAccount(
                        new StorageCredentials(storageAccountName, connectionString), true);
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    var containter = blobClient.GetContainerReference(containerName);
                    containter.CreateIfNotExists();
                    containter.SetPermissions(new BlobContainerPermissions()
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });

                    var blob = containter.GetBlockBlobReference(u.Name + "/" + u.Name + ".json");
                    blob.UploadFromFile(filePath);


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
                if (textBox1.Text == "")
                {
                    label1.Text = ex.Message;
                }
                if (textBox2.Text == "")
                {
                    label2.Text = ex.Message;
                }
            }
        }

        private bool addUser(User user)
        {
            return !users.Contains(user);
        }

        private bool addGlobalUser(User user)
        {
            return !globalUsers.Contains(user);
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
                Point position = button7.Location;          
                Point textPosition = label3.Location;
                

                if(isPositionCorrected == false)
                {
                    position.X += 60;
                    textPosition.X += 60;
                    isPositionCorrected = true;
                }
                label3.Location = textPosition;
                button7.Location = position;
                button7.Visible = true;
                button7.Size = new Size(192, 58);
                //createUserButtons();
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
            if(isPositionCorrected == false)
            {
                position.X += 60;
            }

            foreach (User user in users)
            {
                Button button = new Button();
                button.Location = position;
                button.Text = user.Name;
                button.BackColor = Color.FromArgb(56, 140, 0);
                button.Size = new Size (192, 58);
                button.FlatStyle = 0;
                button.FlatAppearance.MouseOverBackColor = Color.YellowGreen;
                button.FlatAppearance.BorderColor = Color.Black;
                button.FlatAppearance.BorderSize = 1;
                button.Font = new Font("Tahoma", 10, FontStyle.Bold);
                button.ForeColor = Color.WhiteSmoke;
                //button.Location = new Point(button.Location.X + 20);
                panel8.Controls.Add(button);
                button.Visible = true;

                button.Click += new EventHandler(userClick);

                position.Y += 75;

            }

            backButton.Location = position;
            backButton.Text = "POWRÓT";
            backButton.BackColor = Color.FromArgb(56, 140, 0);
            backButton.Size = new Size(192, 58);
            backButton.FlatStyle = 0;
            backButton.FlatAppearance.MouseOverBackColor = Color.YellowGreen;
            backButton.FlatAppearance.BorderColor = Color.Black;
            backButton.FlatAppearance.BorderSize = 1;
            backButton.Font = new Font("Tahoma", 10, FontStyle.Bold);
            backButton.ForeColor = Color.WhiteSmoke;
            //button.Location = new Point(button.Location.X + 20);
            panel8.Controls.Add(backButton);
            backButton.Visible = true;

            backButton.Click += button7_Click;



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

        private void button6_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel6.Visible = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            label5.Text = "";
            label4.Text = "";
            registerGlobalUser();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            panel7.Visible = false;
            panel1.Visible = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            panel7.Visible = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            label6.Text = "";
            //label7.Text = "";

            foreach (User user in globalUsers)
            {
                if(user.Email == textBox5.Text && user.Password == textBox6.Text)
                {
                    var menu = new Menu(user);
                    menu.Location = this.Location;
                    menu.Show();
                    this.Hide();
                }
            }  
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void UserChoice_Load(object sender, EventArgs e)
        {

        }

        private void ExitUserSelect_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Visible = true;
        }

        private void ExitUserLogin_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            panel1.Visible = true;
        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            panel6.Visible = false;
            panel1.Visible = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            panel4.Visible = false;
            panel1.Visible = true;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            panel3.Visible = false;
            panel5.Visible = true;
        }
    }
}
