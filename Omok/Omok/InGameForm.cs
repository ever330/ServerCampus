using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Omok.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Omok
{
    public partial class InGameForm : Form
    {
        private int _myLevel;
        private int _myExp;
        private int _myWinCount;
        private int _myLoseCount;

        public InGameForm()
        {
            InitializeComponent();
        }

        public void SetInGameData(string email, UserGameData userData)
        {
            emailTextBox.Text = email;
            _myLevel = userData.Level;
            _myExp = userData.Exp;
            _myWinCount = userData.WinCount;
            _myLoseCount = userData.LoseCount;

            RefreshUserData();
        }

        public void RefreshUserData()
        {
            levelTextBox.Text = _myLevel + $"({_myExp})";

            float winningRate = _myWinCount / (_myWinCount + _myLoseCount);
            winningRateTextBox.Text = winningRate + $"W:{_myWinCount}/L:{_myLoseCount}";
        }
    }
}
