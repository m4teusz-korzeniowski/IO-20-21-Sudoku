
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;


namespace Sudoku
{
    public partial class Form1 : Form
    {

        string path;
        string userName;

        string connectionString = "GPu5x3qbpzJ4kDSViR5RQ56TpgXx9zJNhmAoMm7l3GZwfvakkBZN2mMsIkcJkxEfarvzY+R973ltsAy9VZqgCg==";
        string storageAccountName = "sudoku10";

        int i = 360000;
        int ticktock = 0;
        class SudokuCell : Button
        {
            public int Value { get; set; }
            public bool IsLocked { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public void Clear()
            {
                this.Text = string.Empty;
                this.IsLocked = false;
            }
        }
        public Form1(string _path, string _userName)
        {
            InitializeComponent();

            createCells();

            startNewGame();

            radioButton1.Checked = true;
            path = _path;
            userName = _userName;

        }

        private void startNewGame()
        {
            loadValues();
            var hintsCount = 0;

            if (radioButton1.Checked)
                hintsCount = 45;
            else if (radioButton2.Checked)
                hintsCount = 30;
            else if (radioButton3.Checked)
                hintsCount = 15;

            showRandomValuesHints(hintsCount);
        }

        private void showRandomValuesHints(int hintsCount)
        {
            for (int i = 0; i < hintsCount; i++)
            {
                var rX = random.Next(9);
                var rY = random.Next(9);

                cells[rX, rY].Text = cells[rX, rY].Value.ToString();
                cells[rX, rY].ForeColor = Color.Black;
                cells[rX, rY].IsLocked = true;
            }
        }

        private void loadValues()
        {
            foreach (var cell in cells)
            {
                cell.Value = 0;
                cell.Clear();
            }

            findValueForNextCell(0, -1);
        }

        Random random = new Random();

        private bool findValueForNextCell(int i, int j)
        {
            if (++j > 8)
            {
                j = 0;

                if (++i > 8)
                    return true;
            }

            var value = 0;
            var numsLeft = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            do
            {
                if (numsLeft.Count < 1)
                {
                    cells[i, j].Value = 0;
                    return false;
                }

                value = numsLeft[random.Next(0, numsLeft.Count)];
                cells[i, j].Value = value;

                numsLeft.Remove(value);
            }
            while (!isValidNumber(value, i, j) || !findValueForNextCell(i, j));

            // Mozna odkomentowac aby sprawdzic czy losuje dobre liczby do komurek
            // cells[i, j].Text = value.ToString();

            return true;
        }

        private bool isValidNumber(int value, int x, int y)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i != y && cells[x, i].Value == value)
                    return false;

                if (i != x && cells[i, y].Value == value)
                    return false;
            }

            for (int i = x - (x % 3); i < x - (x % 3) + 3; i++)
            {
                for (int j = y - (y % 3); j < y - (y % 3) + 3; j++)
                {
                    if (i != x && j != y && cells[i, j].Value == value)
                        return false;
                }
            }

            return true;
        }


        SudokuCell[,] cells = new SudokuCell[9, 9];

        private void createCells()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j] = new SudokuCell();
                    cells[i, j].Font = new Font(SystemFonts.DefaultFont.FontFamily, 20);
                    cells[i, j].Size = new Size(60, 60);
                    cells[i, j].ForeColor = SystemColors.ControlDarkDark;
                    cells[i, j].Location = new Point(i * 50, j * 50);
                    cells[i, j].BackColor = ((i / 3) + (j / 3)) % 2 == 0 ? SystemColors.Control : Color.YellowGreen;
                    cells[i, j].FlatStyle = FlatStyle.Flat;
                    cells[i, j].FlatAppearance.BorderColor = Color.Black;
                    cells[i, j].X = i;
                    cells[i, j].Y = j;

                    cells[i, j].KeyPress += cell_keyPressed;
                    panel1.Controls.Add(cells[i, j]);
                }
            }
        }

        private void cell_keyPressed(object sender, KeyPressEventArgs e)
        {
            var cell = sender as SudokuCell;
            if (cell.IsLocked)
                return;

            int value;
            if (int.TryParse(e.KeyChar.ToString(), out value))
            {
                if (value == 0)
                    cell.Clear();
                else
                    cell.Text = value.ToString();

                cell.ForeColor = SystemColors.ControlDarkDark;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var wrongCells = new List<SudokuCell>();

            foreach (var cell in cells)
            {
                if (!string.Equals(cell.Value.ToString(), cell.Text))
                {
                    wrongCells.Add(cell);
                }
            }

            if (wrongCells.Any())
            {
                wrongCells.ForEach(x => x.ForeColor = Color.Red);
                i -= 3000;
                MessageBox.Show("Zle");
            }
            else
            {
                timer1.Enabled = false;
                MessageBox.Show("Wygrales");
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "0");
                }
                int bestResult = 0;
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        bestResult = Convert.ToInt32(sr.ReadLine());
                    }
                }

                if(i > bestResult && path.ElementAt(0)  == 'g')
                {
                    File.WriteAllText(path, i.ToString());
                    string containerName = "users";
                    string blobName = path;
                    string filePath = "global/" + userName + "/" + path;


                    var storageAccount = new CloudStorageAccount(
                        new StorageCredentials(storageAccountName, connectionString), true);
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    var containter = blobClient.GetContainerReference(containerName);
                    containter.CreateIfNotExists();
                    containter.SetPermissions(new BlobContainerPermissions()
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });

                    var blob = containter.GetBlockBlobReference(userName + "/highscore.txt");
                    blob.UploadFromFile(path);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startNewGame();
            i = 100000;
            timer1.Enabled = true;
        }

        private void Wyszysc_Click(object sender, EventArgs e)
        {
            foreach (var cell in cells)
            {
                if (cell.IsLocked == false)
                    cell.Clear();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            i--;
            label2.Text = i.ToString();
            
            
            
            if (i < 75000) { label2.ForeColor = Color.Gold; };
            if (i < 25000) {
                
                label2.ForeColor = Color.DarkOrange;
                if (ticktock == 0) {label2.Font = new Font("Tahoma", 24, FontStyle.Bold | FontStyle.Italic); };
                if (ticktock == 1) {label2.Font = new Font("Tahoma", 22, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X + 2, label2.Location.Y + 2); };
                if (ticktock == 2) {label2.Font = new Font("Tahoma", 20, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X + 2, label2.Location.Y + 2); };
                if (ticktock == 3) {label2.Font = new Font("Tahoma", 18, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X + 2, label2.Location.Y + 2); };
                if (ticktock == 4) {label2.Font = new Font("Tahoma", 16, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X + 2, label2.Location.Y + 2); };
                if (ticktock == 5) {label2.Font = new Font("Tahoma", 18, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X - 2, label2.Location.Y - 2); };
                if (ticktock == 6) {label2.Font = new Font("Tahoma", 20, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X - 2, label2.Location.Y - 2); };
                if (ticktock == 7) {label2.Font = new Font("Tahoma", 22, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X - 2, label2.Location.Y - 2); };
                if (ticktock == 8) {label2.Font = new Font("Tahoma", 24, FontStyle.Bold | FontStyle.Italic); label2.Location = new Point(label2.Location.X - 2, label2.Location.Y - 2); ticktock = 0; };
            };
            if (i < 0) { label2.ForeColor = Color.OrangeRed; };
           
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FontClock_Tick(object sender, EventArgs e)
        {
            if (i < 120000)
                ticktock++;

            if (radioButton1.Checked == true) { radioButton1.ForeColor = Color.YellowGreen; radioButton2.ForeColor = Color.WhiteSmoke; radioButton3.ForeColor = Color.WhiteSmoke; radioButton1.Font = new Font("Tahoma", 9, FontStyle.Bold); radioButton2.Font = new Font("Tahoma", 9); radioButton3.Font = new Font("Tahoma", 9); }
            if (radioButton2.Checked == true) { radioButton2.ForeColor = Color.Khaki; radioButton1.ForeColor = Color.WhiteSmoke; radioButton3.ForeColor = Color.WhiteSmoke; radioButton1.Font = new Font("Tahoma", 9); radioButton2.Font = new Font("Tahoma", 9, FontStyle.Bold); radioButton3.Font = new Font("Tahoma", 9); }
            if (radioButton3.Checked == true) { radioButton3.ForeColor = Color.OrangeRed; radioButton1.ForeColor = Color.WhiteSmoke; radioButton2.ForeColor = Color.WhiteSmoke; radioButton1.Font = new Font("Tahoma", 9); radioButton2.Font = new Font("Tahoma", 9); radioButton3.Font = new Font("Tahoma", 9, FontStyle.Bold); }
        }
    }
}


/*using IO_Sudoku.src.model;
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
    public partial class MainForm : Form
    {
        private Panel fieldsPanel = new Panel();
        private List<List<TextBox>> fields = new List<List<TextBox>>();
        private Board sudokuBoard = new Board();
        public MainForm()
        {
            InitializeComponent();
            initializeFields();
            addFieldsToPanel();

        }


        private void initializeFields()
        {
            int x = 40;
            int y = 60;

            for (int i = 0; i < 9; i++)
            {
                List<TextBox> tempFields = new List<TextBox>();
                for (int j = 0; j < 9; j++)
                {
                    TextBox newField = new TextBox();
                    newField.Size = new Size(30, 30);
                    newField.TextAlign = HorizontalAlignment.Center;
                    newField.Text = sudokuBoard.Fields.ElementAt(i).ElementAt(j).ToString();
                    newField.Location = new Point(x, y);
                    tempFields.Add(newField);
                    x += 50;
                }
                fields.Add(tempFields);
                x = 40;
                y += 50;
            }
        }

        private void addFieldsToPanel()
        {
            foreach(List<TextBox> sublist in fields)
            {
                foreach(TextBox textBox in sublist)
                {
                    Controls.Add(textBox);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sudokuBoard = new Board();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                   fields.ElementAt(i).ElementAt(j).Text = sudokuBoard.Fields.ElementAt(i).ElementAt(j).ToString();

                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var menu = new Menu(new User());
            menu.Show();
            this.Hide();
        }
    }
}*/