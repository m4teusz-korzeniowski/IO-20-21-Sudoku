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
            var menu = new Menu();
            menu.Show();
            this.Hide();
        }
    }
}
