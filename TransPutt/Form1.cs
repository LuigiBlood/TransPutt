using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransPutt
{
    public partial class Form1 : Form
    {
        static int selectedGame = 0;

        public Form1()
        {
            InitializeComponent();
            treeView1.SelectedNode = treeView1.Nodes[selectedGame];
        }

        private void updateSelectedLabel()
        {
            label1.Text = "Selected: " + treeView1.Nodes[selectedGame].Text;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedGame = e.Node.Index;
            updateSelectedLabel();
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Program.Help((string)treeView1.SelectedNode.Tag), "Help (" + treeView1.SelectedNode.Text + ")", MessageBoxButtons.OK);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Program.Launch((string)treeView1.SelectedNode.Tag);
        }
    }
}
