﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piskvorky
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
            string[] defaultSymbols = { "❌", "⭕", "⭐", "🍀", "🔥" };
            comboBox1.Items.AddRange(defaultSymbols);
            comboBox2.Items.AddRange(defaultSymbols);

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
        }

        public int WinLength 
        {
            get 
            { 
                return (int)numUpDown_winLenght.Value; 
            }
            set
            {
                numUpDown_winLenght.Value = value;
            }
       
        }
        public int BoardSize
        {
            get
            {
                return (int)numUpDown_boardSize.Value;
            }
            set
            {
                numUpDown_boardSize.Value = value;
            }

        }
        public int GameLength
        {
            get
            {
                return (int)numUpDown_gameLenght.Value; 
            }
            set
            {
                numUpDown_gameLenght.Value = value;
            }

        }
        public string Player1Symbol
        {
            get { return comboBox1.SelectedItem.ToString(); }
            set { comboBox1.SelectedItem = value; }
        }

        public string Player2Symbol
        {
            get { return comboBox2.SelectedItem.ToString(); }
            set { comboBox2.SelectedItem = value; }
        }

        public bool IsWithAI
        {
            get { return IsWithAI_checkBox.Checked; }
            set { IsWithAI_checkBox.Checked = value;}
        }

        public string AIDifficulty()
        {
            if (radioButton1.Checked) return "lehká";
            if (radioButton2.Checked) return "střední";
            return "těžká";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Player1Symbol == Player2Symbol)
            {
                MessageBox.Show("Symboly hráčů musí být různé!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }
        }
    }
}
