using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WSATools.Libs;

namespace WSATools
{
    public partial class WSAList : Form
    {
        public WSAList()
        {
            InitializeComponent();
        }
        private void WSAList_Load(object sender, EventArgs e)
        {

        }
        private async void button1_Click(object sender, EventArgs e)
        {
            var list = await AppX.GetFilePath();
            if (list != null && list.Count > 0)
            {

            }
            else
            {

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}