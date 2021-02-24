using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransPutt.Games
{
    public partial class MarvelousFormSearchID : Form
    {
        public string[] list;
        public int id;

        public MarvelousFormSearchID()
        {
            InitializeComponent();
        }

        private void MarvelousFormSearchID_Load(object sender, EventArgs e)
        {
            FilterUpdateListBox();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterUpdateListBox(textBox1.Text);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            id = GetIDFromList(listBox1.SelectedIndex);
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }


        private void FilterUpdateListBox(string filter = "")
        {
            List<string> newlist = new List<string>();
            int idx = -1;

            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Contains(filter))
                {
                    newlist.Add(i + ": " + list[i]);
                    if (i == id) idx = newlist.Count - 1;
                }
            }

            listBox1.SuspendLayout();
            listBox1.Items.Clear();

            foreach (string s in newlist)
            {
                listBox1.Items.Add(s);
            }

            if (idx != -1) listBox1.SelectedIndex = idx;
            listBox1.ResumeLayout();
        }

        private int GetIDFromList(int i)
        {
            string s = ((string)listBox1.Items[i]).Split(':')[0];

            return Int32.Parse(s);
        }
    }
}
