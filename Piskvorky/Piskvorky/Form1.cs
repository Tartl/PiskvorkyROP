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
    public partial class Form1 : Form
    {
        int fieldSize;
        int width = 0,
            height  = 0;
        public Form1()
        {
            InitializeComponent();
            fieldSize = playingBoard1.FieldSize;
        }
        public void BoardRedraw()
        {
            if (width != Width || height != Height)
            {
                if (Math.Abs(Width / Height) < 2)
                {
                    int currentAvg = Width + Height / 2;
                    int minAvg = this.MinimumSize.Width + this.MinimumSize.Height / 2;
                    playingBoard1.FieldSize = (int)(fieldSize * (float)currentAvg / minAvg);
                    width = Width;
                    height = Height;
                }

            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            BoardRedraw();
        }
    }
}
