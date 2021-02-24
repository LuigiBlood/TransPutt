using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransPutt.Games
{
    public partial class MarvelousFormSearchID : Form
    {
        public string[] list;
        public int id;

        private System.Threading.Timer timer;

        public MarvelousFormSearchID()
        {
            InitializeComponent();
            timer = new System.Threading.Timer((c) => FilterUpdateListBox(), null, Timeout.Infinite, Timeout.Infinite);
        }

        private void MarvelousFormSearchID_Load(object sender, EventArgs e)
        {
            FilterUpdateListBox();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Delay ListBox Update so it doesn't stutter
            timer.Change(250, Timeout.Infinite);
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


        private void FilterUpdateListBox()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(FilterUpdateListBox));
            }
            else
            {
                //Search via Filter
                string filter = textBox1.Text;
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

                //Update ListBox
                listBox1.SuspendLayout();
                listBox1.Items.Clear();

                listBox1.Items.AddRange(newlist.ToArray());

                if (idx != -1) listBox1.SelectedIndex = idx;
                listBox1.ResumeLayout();
            }
        }

        private int GetIDFromList(int i)
        {
            string s = ((string)listBox1.Items[i]).Split(':')[0];

            return Int32.Parse(s);
        }
    }
}
