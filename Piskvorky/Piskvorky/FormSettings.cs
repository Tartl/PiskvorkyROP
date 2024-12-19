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
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
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

    }
}
