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
        public int defaultIndex = -1;
        public string lastFilter = "";
        
        private System.Threading.Timer timer;

        public MarvelousFormSearchID(string filter, int selectedIndex)
        {
            InitializeComponent();
            timer = new System.Threading.Timer((c) => FilterUpdateListBox(), null, Timeout.Infinite, Timeout.Infinite);
            if (filter.Length > 0)
                textBox1.Text = filter;

            if (selectedIndex > -1)
                defaultIndex = selectedIndex;
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
            if (listBox1.SelectedIndex > -1)
                id = GetIDFromList(listBox1.SelectedIndex);
            defaultIndex = listBox1.SelectedIndex;
            buttonOK.Enabled = listBox1.Items.Count > 0 && listBox1.SelectedIndex > -1;
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
                lastFilter = textBox1.Text;
                //Search via Filter
                List<string> newlist = new List<string>();
                int idx = -1;

                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].Contains(lastFilter))
                    {
                        newlist.Add(i + ": " + list[i]);
                        if (i == id) idx = newlist.Count - 1;
                    }
                }

                //Update ListBox
                listBox1.SuspendLayout();
                listBox1.Items.Clear();

                listBox1.Items.AddRange(newlist.ToArray());

                if (defaultIndex != -1)
                {
                    if (listBox1.Items.Count >= defaultIndex + 1)
                    {
                        listBox1.SelectedIndex = defaultIndex;
                        idx = -1;
                    }
                }
                if (idx != -1) listBox1.SelectedIndex = idx;
                buttonOK.Enabled = listBox1.Items.Count > 0 && listBox1.SelectedIndex > -1;
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
