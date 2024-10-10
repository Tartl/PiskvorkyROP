using System;
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
        public Form1()
        {
            InitializeComponent();
            ScreenSize(Width, Height);
        }
        public void ScreenSize(int width, int height)
        {
            int activeFormWidth = width;
            int activeFormHeight = height;
            string screenSize = activeFormWidth.ToString() + "x" + activeFormHeight.ToString();
            MessageBox.Show(screenSize);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            ScreenSize(Width, Height);
        }
    }
}
