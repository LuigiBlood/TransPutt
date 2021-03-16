using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TransPutt.Games
{
    public partial class MarvelousForm : Form
    {
        struct lang
        {
            public string name;
            public byte[] font_chr;                    //font.bin - 0x00 to 0xEF
            public byte[] kanji_chr;                   //kanji.bin
            public byte[] icons_chr;                   //icons.bin
            public byte[] width_tbl;                   //width.tbl - Char Width Table
            public List<Tuple<string, string>> table;  //table.tbl - Table
            public string[] main_txt;                  //main.txt - Main Text File
            public string[] main_end;                  //Keeps all End Commands in mind
            public string[] notes;                     //notes.txt - Notes
            public string[] nl_tags;                   //Holds all new line tags so we don't have to keep finding them 
            public string[] end_tags;                  //Storing end command tags here
        }

        static byte[] other_list = { 0x00, 0x02, 0x04, 0x06, 0x08, 0x14, 0x0A, 0x54,
                                     0x0C, 0x0E, 0x10, 0x36, 0x44, 0x46, 0x48, 0x4A,
                                     0x4C, 0x4E, 0x50, 0x52, 0x1E, 0x20, 0x22, 0x24,
                                     0x26, 0x28, 0x2A, 0x2C, 0x2E, 0x30 };

        lang lang1;
        lang lang2;
        bool hasChanged;
        // added skipRefresh and maxWidthExceeded "FORM GLOBALS"
        bool skipRefresh = false;
        bool maxWidthExceeded = false;

        int curIndex = -1;

        public MarvelousForm()
        {
            InitializeComponent();
        }

        private void MarvelousForm_Load(object sender, EventArgs e)
        {
            //Get all languages available
            foreach (string path in Directory.GetDirectories(".\\marvelous\\"))
            {
                if (!File.Exists(path + "\\font.bin"))
                    continue;
                if (!File.Exists(path + "\\kanji.bin"))
                    continue;
                if (!File.Exists(path + "\\icons.bin"))
                    continue;
                if (!File.Exists(path + "\\width.tbl"))
                    continue;
                if (!File.Exists(path + "\\table.tbl"))
                    continue;
                if (!File.Exists(path + "\\main.txt"))
                    continue;
                string[] split = path.Split(Path.DirectorySeparatorChar);
                comboBoxLang1.Items.Add(split[split.Length - 1]);
                comboBoxLang2.Items.Add(split[split.Length - 1]);
            }

            comboBoxLang1.SelectedIndex = 0;
            comboBoxLang2.SelectedIndex = 0;
            comboBoxStyle1.SelectedIndex = 0;
        }

        private void comboBoxLang1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (curIndex != -1 && (hasChanged == true || isTextDifferent(textBoxText1, curIndex, lang1) || (lang1.notes[curIndex] != textBoxDesc1.Text.Replace("\r\n", "\\"))))
            {
                switch (MessageBox.Show("Would you like to quit & save the full main script of language \"" + lang1.name + "\"?", "Save?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        SaveText(textBoxText1, curIndex, lang1);
                        SaveNote(textBoxDesc1, curIndex, lang1);
                        SaveMainScript(lang1);
                        break;
                }
            }
            LoadLanguage((string)comboBoxLang1.SelectedItem, out lang1);
            UpdateTableTab(lang1);
            numericUpDownID1.Minimum = 0;
            numericUpDownID1.Maximum = lang1.main_txt.Length - 1;
            curIndex = (int)numericUpDownID1.Value;
            UpdateTextBox(textBoxText1, curIndex, lang1);
            UpdateNotesBox(textBoxDesc1, curIndex, lang1);
            UpdatePictureBox(pictureBoxPreview1, textBoxText1, lang1);
            UpdateSaveAllButton();
        }

        private void comboBoxLang2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLanguage((string)comboBoxLang2.SelectedItem, out lang2);
            UpdateTextBox(textBoxText2, curIndex, lang2);
            UpdateNotesBox(textBoxDesc2, curIndex, lang2);
            UpdatePictureBox(pictureBoxPreview2, textBoxText2, lang2);
        }

        private void textBoxText1_TextChanged(object sender, EventArgs e)
        {
            if (skipRefresh) return;  // skip this. we're busy doing some hacky stuff...

            UpdatePictureBox(pictureBoxPreview1, textBoxText1, lang1);
            UpdateSaveAllButton();
        }

        private void numericUpDownID1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownID1.Value = Math.Min(numericUpDownID1.Value, numericUpDownID1.Maximum);

            if (curIndex != -1 && curIndex < lang1.main_txt.Length)
            {
                if (isTextDifferent(textBoxText1, curIndex, lang1) || (lang1.notes[curIndex] != textBoxDesc1.Text.Replace("\r\n", "\\")))
                {
                    //if (MessageBox.Show("Do you want to save the text & notes?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SaveText(textBoxText1, curIndex, lang1);
                        SaveNote(textBoxDesc1, curIndex, lang1);
                    }
                }
            }
            curIndex = (int)numericUpDownID1.Value;
            UpdateTextBox(textBoxText1, curIndex, lang1);
            UpdateNotesBox(textBoxDesc1, curIndex, lang1);
            UpdateSaveAllButton();

            UpdateTextBox(textBoxText2, curIndex, lang2);
            UpdateNotesBox(textBoxDesc2, curIndex, lang2);
            UpdatePictureBox(pictureBoxPreview2, textBoxText2, lang2);
        }

        private void buttonSave1_Click(object sender, EventArgs e)
        {
            SaveText(textBoxText1, curIndex, lang1);
            SaveNote(textBoxDesc1, curIndex, lang1);
            UpdateSaveAllButton();
        }

        private void buttonSaveAll1_Click(object sender, EventArgs e)
        {
            SaveText(textBoxText1, curIndex, lang1);
            SaveNote(textBoxDesc1, curIndex, lang1);
            SaveMainScript(lang1);
            UpdateSaveAllButton();
        }

        private void MarvelousForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hasChanged == true || isTextDifferent(textBoxText1, curIndex, lang1) || (lang1.notes[curIndex] != textBoxDesc1.Text.Replace("\r\n", "\\")))
            {
                switch (MessageBox.Show("Would you like to close & save the full main script and notes of language \"" + lang1.name + "\"?\nCancel to not close the editor.", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        SaveText(textBoxText1, curIndex, lang1);
                        SaveNote(textBoxDesc1, curIndex, lang1);
                        SaveMainScript(lang1);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void comboBoxStyle1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePictureBox(pictureBoxPreview1, textBoxText1, lang1);
            UpdatePictureBox(pictureBoxPreview2, textBoxText2, lang2);
        }

        private void nextIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numericUpDownID1.Value == numericUpDownID1.Maximum)
            {
                numericUpDownID1.Value = numericUpDownID1.Minimum;
            }
            else
            {
                numericUpDownID1.Value++;
            }
        }

        private void previousIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numericUpDownID1.Value == numericUpDownID1.Minimum)
            {
                numericUpDownID1.Value = numericUpDownID1.Maximum;
            }
            else
            {
                numericUpDownID1.Value--;
            }
        }

        private void textBoxDesc1_TextChanged(object sender, EventArgs e)
        {
            UpdateSaveAllButton();
        }

        private void buttonRevert1_Click(object sender, EventArgs e)
        {
            UpdateTextBox(textBoxText1, curIndex, lang1);
            UpdateNotesBox(textBoxDesc1, curIndex, lang1);
            UpdateSaveAllButton();

            UpdateTextBox(textBoxText2, curIndex, lang2);
            UpdateNotesBox(textBoxDesc2, curIndex, lang2);
            UpdatePictureBox(pictureBoxPreview2, textBoxText2, lang2);

            toolStripStatusLabel1.Text = "Reverted ID " + curIndex;
        }

        private void line1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxText1.Paste(GetCommandString("F7", lang1));
        }

        private void line2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxText1.Paste(GetCommandString("F8", lang1));
        }

        private void line3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxText1.Paste(GetCommandString("F9", lang1));
        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxText1.Paste(GetCommandString("F6", lang1));
        }

        private void buttonSelectText_Click(object sender, EventArgs e)
        {
            int temp = Math.Min(GetSelectID(lang1, false), (int)numericUpDownID1.Maximum);
            if (temp >= 0)
                numericUpDownID1.Value = temp;
        }

        private void buttonSelectNotes_Click(object sender, EventArgs e)
        {
            int temp = Math.Min(GetSelectID(lang1, true), (int)numericUpDownID1.Maximum);
            if (temp >= 0)
                numericUpDownID1.Value = temp;
        }

        private void buttonSelectWindow2_Click(object sender, EventArgs e)
        {
            int temp = Math.Min(GetSelectID(lang2, false), (int)numericUpDownID1.Maximum);
            if (temp >= 0)
                numericUpDownID1.Value = temp;
        }

        private void buttonSelectNotes2_Click(object sender, EventArgs e)
        {
            int temp = Math.Min(GetSelectID(lang2, true), (int)numericUpDownID1.Maximum);
            if (temp >= 0)
                numericUpDownID1.Value = temp;
        }

        //--Text Functions
        private void LoadLanguage(string lang, out lang outlang)
        {
            outlang = new lang();
            outlang.name = lang;
            string fullpath = ".\\marvelous\\" + lang + "\\";
            outlang.font_chr = File.ReadAllBytes(fullpath + "font.bin");
            outlang.kanji_chr = File.ReadAllBytes(fullpath + "kanji.bin");
            outlang.icons_chr = File.ReadAllBytes(fullpath + "icons.bin");
            outlang.width_tbl = File.ReadAllBytes(fullpath + "width.tbl");
            PuttScript.GetDictFromFile(fullpath + "table.tbl", 1, out outlang.table);
            hasChanged = false;

            List<string> list_txt = new List<string>();
            List<string> list_end = new List<string>();
            string temp = File.ReadAllText(fullpath + "main.txt");

            //Find End Commands and new line tags
            string en1 = "";
            string en2 = "";
            string nl2 = "";
            string nl3 = "";
            string scl = "";
            string np = ""; //FE69
            for (int i = 0; i < outlang.table.Count; i++)
            {
                if (outlang.table[i].Item1 == "FA")
                    en1 = outlang.table[i].Item2;
                if (outlang.table[i].Item1 == "FB")
                    en2 = outlang.table[i].Item2;
                if (outlang.table[i].Item1 == "F8")  // nl2
                    nl2 = outlang.table[i].Item2;
                if (outlang.table[i].Item1 == "F9")  // nl3
                    nl3 = outlang.table[i].Item2;
                if (outlang.table[i].Item1 == "F6")  // scl
                    scl = outlang.table[i].Item2;
                if (outlang.table[i].Item1 == "FE69")  // np
                    np = outlang.table[i].Item2;
                if (en1 != "" && en2 != "" && nl2 != "" && nl3 != "" && scl != "" && np != "")
                    break;
            }
            outlang.end_tags = new string[] { en1, en2 }; // storing to negate future searching...
            outlang.nl_tags = new string[] { "", nl2, nl3, scl, np };  // stored in order

            //Put all script in string array, separated by end commands
            int lastidx = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                for (int e = 0; e < outlang.end_tags.Length; e++)
                {
                    if (((temp.Length - i) > outlang.end_tags[e].Length))
                    {
                        if (temp.Substring(i, outlang.end_tags[e].Length) == outlang.end_tags[e])
                        {
                            i--;
                            list_txt.Add(temp.Substring(lastidx, i - lastidx).Trim().Replace("\r\n", "\n"));
                            i += outlang.end_tags[e].Length;
                            list_end.Add(outlang.end_tags[e]);
                            lastidx = i + 1;
                        }
                    }
                }
            }

            outlang.main_txt = list_txt.ToArray();
            outlang.main_end = list_end.ToArray();

            outlang.notes = new string[list_txt.Count];
            string[] temp_notes;
            if (File.Exists(fullpath + "notes.txt"))
                temp_notes = File.ReadAllLines(fullpath + "notes.txt");
            else
                temp_notes = new string[0];

            for (int i = 0; i < outlang.notes.Length; i++)
            {
                if (i < temp_notes.Length)
                    outlang.notes[i] = temp_notes[i];
                else
                    outlang.notes[i] = "";
            }

            toolStripStatusLabel1.Text = "Loaded Language " + lang;
        }

        private bool isTextDifferent(TextBox textBox, int id, lang inlang)
        {
            if (id >= inlang.main_txt.Length)
                return false;

            byte[] encoded_orig;
            byte[] encoded_new;
            string errorout = "";
            PuttScript.Encode(inlang.table, inlang.main_txt[id], out encoded_orig, out errorout);
            PuttScript.Encode(inlang.table, textBox.Text, out encoded_new, out errorout);

            return Program.isByteArrayEqual(encoded_orig, encoded_new);
        }

        private void SaveNote(TextBox textBox, int id, lang inlang)
        {
            if (id >= inlang.main_txt.Length)
                return;

            if (inlang.notes[id] == textBox.Text.Replace("\r\n", "\\"))
                return;

            inlang.notes[id] = textBox.Text.Replace("\r\n", "\\");
            hasChanged = true;
            toolStripStatusLabel1.Text = "Saved Note ID " + id + " to memory.";
        }

        private void SaveText(TextBox textBox, int id, lang inlang)
        {
            if (!isTextDifferent(textBoxText1, curIndex, lang1))
                return;

            // remove all stored end command tags
            inlang.main_txt[id] = textBox.Text.Replace("\r\n", "\n");
            for (int e = 0; e < inlang.end_tags.Length; e++)
            {
                inlang.main_txt[id].Replace(inlang.end_tags[e], "");
            }

            hasChanged = true;
            toolStripStatusLabel1.Text = "Saved Text ID " + id + " to memory.";
        }

        private void SaveMainScript(lang inlang)
        {
            toolStripStatusLabel1.Text = "Saving Full Script and Notes...";
            string temp = "";
            for (int i = 0; i < inlang.main_txt.Length; i++)
                temp += "\n" + inlang.main_txt[i] + "\n" + inlang.main_end[i] + "\n";
            string fullpath = ".\\marvelous\\" + inlang.name + "\\";

            File.WriteAllText(fullpath + "main.txt", temp);
            File.WriteAllLines(fullpath + "notes.txt", inlang.notes);
            hasChanged = false;

            //MessageBox.Show(inlang.name + "\\main.txt and notes.txt have been updated.");
            toolStripStatusLabel1.Text = inlang.name + "\\main.txt and notes.txt have been updated.";
        }

        private void UpdateNotesBox(TextBox textBox, int id, lang inlang)
        {
            if (id < inlang.main_txt.Length)
                textBox.Text = inlang.notes[id].Replace("\\", "\r\n");
            else
                textBox.Text = "";
        }

        private void UpdateTextBox(TextBox textBox, int id, lang inlang)
        {
            if (id < inlang.main_txt.Length)
                textBox.Text = inlang.main_txt[id].Replace("\n", "\r\n");
            else
                textBox.Text = "";
        }

        private void UpdatePictureBox(PictureBox pictureBox, TextBox textBox, lang inlang)
        {
            pictureBox.Image = RenderText(textBox.Text, inlang, comboBoxStyle1.SelectedIndex);
            pictureBox.Width = pictureBox.Image.Width;
            pictureBox.Height = pictureBox.Image.Height;
        }

        private void UpdateSaveAllButton()
        {
            buttonSaveAll1.Enabled = hasChanged || isTextDifferent(textBoxText1, curIndex, lang1) || (lang1.notes[curIndex] != textBoxDesc1.Text.Replace("\r\n", "\\"));
            saveToFileToolStripMenuItem.Enabled = buttonSaveAll1.Enabled;
        }

        private void UpdateTableTab(lang curlang)
        {
            //Update Table Raw
            listBoxTableRaw.Items.Clear();
            List<Tuple<string, string>> tbl = new List<Tuple<string, string>>();
            tbl.AddRange(curlang.table);
            tbl.Sort();
            foreach (Tuple<string, string> s in tbl)
                listBoxTableRaw.Items.Add(s.Item1 + "=" + s.Item2);
            

            //Update Regular PictureBox
            //16 chars per line, from 00 to EF
            Bitmap regular = new Bitmap(16 * 16, 32 * 15);
            for (int i = 0; i < 0xF0; i++)
            {
                using (Graphics g = Graphics.FromImage(regular))
                {
                    g.DrawImageUnscaled(RenderChar(i, curlang, 3), (i % 16) * 16, (i / 16) * 32);
                    g.DrawLine(new Pen(Color.FromArgb(128, 255, 0, 0)), ((i % 16) * 16) + 15, (i / 16) * 32, ((i % 16) * 16) + 15, ((i / 16) * 32) + 32);
                    g.DrawLine(new Pen(Color.FromArgb(128, 255, 0,0 )), ((i % 16) * 16), ((i / 16) * 32) + 32, ((i % 16) * 16) + 15, ((i / 16) * 32) + 32);
                }
            }
            pictureBoxTable_Regular.Image = regular;
            pictureBoxTable_Regular.Width = regular.Width;
            pictureBoxTable_Regular.Height = regular.Height;

            //Update Kanji PictureBox
            //16 chars per line, from 0x0100 to 0x036F
            Bitmap kanji = new Bitmap(16 * 16, 32 * 39);
            for (int i = 0x100; i < 0x370; i++)
            {
                using (Graphics g = Graphics.FromImage(kanji))
                {
                    g.DrawImageUnscaled(RenderChar(i, curlang, 3), ((i - 0x100) % 16) * 16, ((i - 0x100) / 16) * 32);
                    g.DrawLine(new Pen(Color.FromArgb(128, 255, 0, 0)), (((i - 0x100) % 16) * 16) + 15, ((i - 0x100) / 16) * 32, (((i - 0x100) % 16) * 16) + 15, (((i - 0x100) / 16) * 32) + 32);
                    g.DrawLine(new Pen(Color.FromArgb(128, 255, 0, 0)), (((i - 0x100) % 16) * 16), (((i - 0x100) / 16) * 32) + 32, (((i - 0x100) % 16) * 16) + 15, (((i - 0x100) / 16) * 32) + 32);
                }
            }
            pictureBoxTable_Kanji.Image = kanji;
            pictureBoxTable_Kanji.Width = kanji.Width;
            pictureBoxTable_Kanji.Height = kanji.Height;

            //Update Other PictureBox
            //8 icons per line, defined by table
            Bitmap other = new Bitmap(8 * 32, 32 * (int)Math.Ceiling(other_list.Length / 8.0));
            for (int i = 0; i < other_list.Length; i++)
            {
                using (Graphics g = Graphics.FromImage(other))
                {
                    g.DrawImageUnscaled(RenderIcon(other_list[i], curlang, 3), (i % 8) * 32, (i / 8) * 32);
                }
            }

            pictureBoxTable_Other.Image = other;
            pictureBoxTable_Other.Width = other.Width;
            pictureBoxTable_Other.Height = other.Height;
        }

        private Bitmap RenderText(string text, lang curlang, int style)
        {
            maxWidthExceeded = false;
            //Styles: 1 = Regular Text Box (black on white), 2 = Small Text (white on black), 3 = With border (black border, white font)
            int maxwidth = 0;
            if (style == 0)
                maxwidth = 352;
            else if (style == 1)
                maxwidth = 384;
            else
                maxwidth = 256;
            int line = 0;
            int h_pixel = 0;

            byte[] encoded;
            string errorout = "";
            int error = PuttScript.Encode(curlang.table, text, out encoded, out errorout);
            int detectlines = 1;
            for (int i = 0; i < encoded.Length; i++)
            {
                if (encoded[i] >= 0xF6 && encoded[i] < 0xFC)
                    detectlines++;
            }
            Bitmap output = new Bitmap(maxwidth, 32 * Math.Max(detectlines, 3));
            using (Graphics g = Graphics.FromImage(output))
            {
                if (style == 0)
                    g.FillRectangle(Brushes.White, 0, 0, output.Width, output.Height);
                else if (style == 1)
                    g.FillRectangle(Brushes.Black, 0, 0, output.Width, output.Height);
                else
                    g.FillRectangle(Brushes.White, 0, 0, output.Width, output.Height);

                if (error != 0)
                    g.DrawString(errorout, new Font(FontFamily.GenericMonospace, 8), Brushes.Red, 0, 0);
            }
            if (error == 0)
            {
                for (int i = 0; i < encoded.Length; i++)
                {
                    Bitmap char_gfx = new Bitmap(32, 16);
                    if (encoded[i] < 0xF0)
                    {
                        char_gfx = RenderChar(encoded[i], curlang, style);
                    }
                    else if (encoded[i] == 0xF0)
                    {
                        char_gfx = new Bitmap(curlang.width_tbl[0xF0], 32);
                    }
                    else if (encoded[i] < 0xF4)
                    {
                        continue;
                    }
                    else if (encoded[i] == 0xF4)
                    {
                        i++;
                        char_gfx = RenderChar(0x300 + encoded[i], curlang, style);
                    }
                    else if (encoded[i] == 0xF5)
                    {
                        i += 2;
                        continue;
                    }
                    else if (encoded[i] < 0xFA)
                    {
                        line++;
                        h_pixel = 0;
                        continue;
                    }
                    else if (encoded[i] < 0xFC)
                    {
                        break;
                    }
                    else if (encoded[i] == 0xFC)
                    {
                        i++;
                        char_gfx = RenderChar(0x200 + encoded[i], curlang, style);
                    }
                    else if (encoded[i] == 0xFD)
                    {
                        i++;
                        char_gfx = RenderChar(0x100 + encoded[i], curlang, style);
                    }
                    else if (encoded[i] == 0xFE)
                    {
                        i++;
                        if (encoded[i] == 0x6D)
                        {
                            //Icon
                            i++;
                            if (encoded[i] == 0x18)
                            {
                                //Item Select
                                char_gfx = RenderItemSelect(curlang);
                                using (Graphics g = Graphics.FromImage(output))
                                {
                                    g.DrawImageUnscaled(char_gfx, 0, 32);
                                }
                                continue;
                            }
                            else if (encoded[i] == 0x1C)
                            {
                                //Leader Select
                                char_gfx = RenderLeaderSelect(curlang);
                                using (Graphics g = Graphics.FromImage(output))
                                {
                                    g.DrawImageUnscaled(char_gfx, 8 * 28, 0);
                                }
                                continue;
                            }
                            else
                            {
                                //Other Icons
                                char_gfx = RenderIcon(encoded[i], curlang, style);
                                if ((h_pixel % 16) != 0)
                                    h_pixel += 16 - (h_pixel % 16);
                            }
                        }
                        else if (encoded[i] == 0x68)
                        {
                            //2 Choices
                            continue;
                        }
                        else if (encoded[i] == 0x6A)
                        {
                            //???
                            continue;
                        }
                        else if (encoded[i] == 0x6B)
                        {
                            //Number of something
                            continue;
                        }
                        else if (encoded[i] == 0x6C)
                        {
                            //Amount of something
                            continue;
                        }
                        else if (encoded[i] == 0x6E)
                        {
                            //3 Choices
                            continue;
                        }
                        else if (encoded[i] == 0x6F)
                        {
                            //Robot Path
                            continue;
                        }
                        else if (encoded[i] == 0x70)
                        {
                            //Robot Rotation
                            continue;
                        }
                        else if (encoded[i] == 0x71)
                        {
                            //Item Select Manager
                            continue;
                        }
                        else if (encoded[i] == 0x78)
                        {
                            //??? Button Use
                            continue;
                        }
                        else if (encoded[i] == 0x79)
                        {
                            //??? Prep Button use?
                            continue;
                        }
                        else if (encoded[i] == 0x7B)
                        {
                            //??? Directions?
                            continue;
                        }
                        else if (encoded[i] == 0x7C)
                        {
                            //???
                            continue;
                        }
                        else if (encoded[i] == 0x7E)
                        {
                            //Button Use (Mash or Gameplay)
                            continue;
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else if (encoded[i] == 0xFF)
                    {
                        break;
                    }

                    using (Graphics g = Graphics.FromImage(output))
                    {
                        g.DrawImageUnscaled(char_gfx, h_pixel, line * 32);
                    }
                    h_pixel += char_gfx.Width;
                    // added a check to see if we exceeded our horizontal screen space
                    if (h_pixel > maxwidth) maxWidthExceeded = true;
                }
            }

            return output;
        }

        private Bitmap RenderItemSelect(lang curlang)
        {
            Color[] pal = { Color.Black, Color.Black, Color.White, Color.Red };
            Bitmap gfx = new Bitmap(16 * 4, 16 * 2 * 2);

            //Render
            using (Graphics g = Graphics.FromImage(gfx))
            {
                g.ScaleTransform(1f, 2f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);

                for (int y = 0; y < 2; y++)
                    for (int x = 0; x < 4; x++)
                    {
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x4040 + (0x10 * x) + (0x100 * y), 0x10), pal), 8 * x, 8 * y);
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x4060 + (0x10 * x) + (0x100 * y), 0x10), pal), 8 * (x + 4), 8 * y);
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x40A0 + (0x10 * x) + (0x100 * y), 0x10), pal), 8 * x, 8 * (y + 2));
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x40C0 + (0x10 * x) + (0x100 * y), 0x10), pal), 8 * (x + 4), 8 * (y + 2));
                    }
            }

            return gfx;
        }

        private Bitmap RenderLeaderSelect(lang curlang)
        {
            Color[] palhud = { Color.Black, Color.Black, Color.White, Color.Green };
            Color[] pal1 = { Color.Black, Color.Beige, Color.White, Color.Maroon };
            Color[] pal3 = { Color.Black, Color.Black, Color.White, Color.Blue };
            Color[] palskin = { Color.Black, Color.Black, Color.White, Color.Bisque };
            Bitmap gfx = new Bitmap(16 * 8, 16 * 2 * 2);

            //Render
            using (Graphics g = Graphics.FromImage(gfx))
            {
                g.ScaleTransform(1f, 2f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);

                for (int y = 0; y < 4; y++)
                    for (int x = 0; x < 2; x++)
                    {
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x5000 + (0x10 * x) + (0x100 * y), 0x10), palhud), 8 * x, 8 * y);
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x5020 + (0x10 * x) + (0x100 * y), 0x10), palhud), 8 * (x + 14), 8 * y);
                    }

                for (int y = 0; y < 2; y++)
                    for (int x = 0; x < 4; x++)
                    {
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x5040 + (0x10 * x) + (0x100 * y), 0x10), pal1), 8 * (x + 2), 8 * y);
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x5080 + (0x10 * x) + (0x100 * y), 0x10), pal1), 8 * (x + 6), 8 * y);
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x50C0 + (0x10 * x) + (0x100 * y), 0x10), pal3), 8 * (x + 10), 8 * y);

                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x5240 + (0x10 * x) + (0x100 * y), 0x10), palskin), 8 * (x + 2), 8 * (y + 2));
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x5280 + (0x10 * x) + (0x100 * y), 0x10), palskin), 8 * (x + 6), 8 * (y + 2));
                        g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, 0x52C0 + (0x10 * x) + (0x100 * y), 0x10), palskin), 8 * (x + 10), 8 * (y + 2));
                    }
            }

            return gfx;
        }

        private Bitmap RenderIcon(int id, lang curlang, int style)
        {
            Color[] pal = { Color.Transparent, Color.Black, Color.White, Color.Red };
            Bitmap gfx;

            if (id == 0x12 || id == 0x38 || id == 0x42)
            {
                gfx = new Bitmap(64, 64);
            }
            else if (id == 0x1A || id == 0x34 || id == 0x56)
            {
                gfx = new Bitmap(16 * 5, 32);
            }
            else
            {
                gfx = new Bitmap(32, 32);
            }

            //Color Set
            switch (id)
            {
                case 0x00:
                    pal[3] = Color.Green;
                    break;
                case 0x04:
                case 0x4C:
                case 0x4E:
                case 0x50:
                case 0x52:
                case 0x54:
                    pal[3] = Color.Yellow;
                    break;
                case 0x06:
                case 0x36:
                    pal[3] = Color.Blue;
                    break;
                case 0x08:
                case 0x0A:
                case 0x14:
                case 0x38:
                case 0x42:
                case 0x44:
                case 0x46:
                case 0x48:
                case 0x4A:
                    pal[3] = Color.Gray;
                    break;
            }

            //Offset
            int offset = 0;
            switch (id)
            {
                case 0x00:
                    offset = 0x4400;
                    break;
                case 0x02:
                    offset = 0x44C0;
                    break;
                case 0x04:
                    offset = 0x4480;
                    break;
                case 0x06:
                    offset = 0x4440;
                    break;
                case 0x08:
                    offset = 0x4200;
                    break;
                case 0x0A:
                    offset = 0x4240;
                    break;
                case 0x0C:
                    offset = 0x4280;
                    break;
                case 0x0E:
                    offset = 0x42C0;
                    break;
                case 0x10:
                    offset = 0x4800;
                    break;
                case 0x12:
                    offset = 0x5C80;
                    break;
                case 0x14:
                    offset = 0x4880;
                    break;
                case 0x16:
                    offset = 0x48C0;
                    break;
                case 0x18:
                    offset = 0x4040;
                    break;
                case 0x1E:
                    offset = 0x4A00;
                    break;
                case 0x20:
                    offset = 0x4A40;
                    break;
                case 0x22:
                    offset = 0x4A80;
                    break;
                case 0x24:
                    offset = 0x4AC0;
                    break;
                case 0x26:
                    offset = 0x4C00;
                    break;
                case 0x28:
                    offset = 0x4C40;
                    break;
                case 0x2A:
                    offset = 0x4C80;
                    break;
                case 0x2C:
                    offset = 0x4CC0;
                    break;
                case 0x2E:
                    offset = 0x4E00;
                    break;
                case 0x30:
                    offset = 0x4E40;
                    break;
                case 0x32:
                    offset = 0x4A40;
                    break;
                case 0x36:
                    offset = 0x4EC0;
                    break;
                case 0x38:
                    offset = 0x5400;
                    break;
                case 0x42:
                    offset = 0x5C00;
                    break;
                case 0x44:
                    offset = 0x4840;
                    break;
                case 0x46:
                    offset = 0x6680;
                    break;
                case 0x48:
                    offset = 0x66C0;
                    break;
                case 0x4A:
                    offset = 0x6880;
                    break;
                case 0x4C:
                    offset = 0x4600;
                    break;
                case 0x4E:
                    offset = 0x4640;
                    break;
                case 0x50:
                    offset = 0x4680;
                    break;
                case 0x52:
                    offset = 0x46C0;
                    break;
                case 0x54:
                    offset = 0x6A80;
                    break;
            }

            //Render
            using (Graphics g = Graphics.FromImage(gfx))
            {
                g.ScaleTransform(1f, 2f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);
                if (id == 0x12 || id == 0x38 || id == 0x42)
                {
                    for (int y = 0; y < 4; y++)
                        for (int x = 0; x < 8; x++)
                            g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, offset + (0x10 * x) + (0x100 * y), 0x10), pal), 8 * x, 8 * y);
                }
                else if (id == 0x1A || id == 0x34 || id == 0x56)
                {
                    g.FillRectangle(Brushes.Gray, 0, 0, 16 * 5, 32);
                }
                else
                {
                    for (int y = 0; y < 2; y++)
                        for (int x = 0; x < 4; x++)
                            g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(Program.Subarray(curlang.icons_chr, offset + (0x10 * x) + (0x100 * y), 0x10), pal), 8 * x, 8 * y);
                }
            }

            return gfx;
        }

        private Bitmap RenderChar(int id, lang curlang, int style)
        {
            Color[] pal = { Color.Transparent, Color.Transparent, Color.Black, Color.White };
            if (style > 0)
                pal[2] = Color.White;
            if (style == 2)
                pal[1] = Color.Black;
            if (style == 3)
            {
                pal[1] = Color.Black;
                pal[2] = Color.Gray;
                pal[3] = Color.White;
            }

            Bitmap gfx = new Bitmap(curlang.width_tbl[id], 32);

            byte[][] dat = new byte[4][];
            if (id < 0x100)
            {
                dat[0] = new byte[0x10];
                dat[1] = new byte[0x10];
                dat[2] = new byte[0x10];
                dat[3] = new byte[0x10];
                for (int i = 0; i < 0x40; i++)
                {
                    dat[i / 0x10][i % 0x10] = curlang.font_chr[(id * 0x40) + i];
                }
            }
            else
            {
                dat[0] = new byte[0x10];
                dat[1] = new byte[0x10];
                dat[2] = new byte[0x10];
                dat[3] = new byte[0x10];
                for (int i = 0; i < 0x40; i++)
                {
                    dat[i / 0x10][i % 0x10] = curlang.kanji_chr[((id - 0x100) * 0x40) + i];
                }
            }

            using (Graphics g = Graphics.FromImage(gfx))
            {
                g.ScaleTransform(1f, 2f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);
                g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(dat[0], pal), 0, 0);
                g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(dat[1], pal), 0, 8);
                g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(dat[2], pal), 8, 0);
                g.DrawImage(GraphicsRender.Nintendo.TileFrom2BPP(dat[3], pal), 8, 8);
            }
            return gfx;
        }

        private string GetCommandString(string key, lang inlang)
        {
            for (int i = 0; i < inlang.table.Count; i++)
            {
                if (inlang.table[i].Item1 == key)
                    return inlang.table[i].Item2;
            }
            return "";
        }

        private int GetSelectID(lang inlang, bool useNotes = false)
        {
            MarvelousFormSearchID idForm = new MarvelousFormSearchID();

            if (!useNotes)
                idForm.list = inlang.main_txt;
            else
                idForm.list = inlang.notes;

            if (idForm.ShowDialog() == DialogResult.OK)
                return idForm.id;
            else
                return -1;
        }

        private void reformatTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
             This reformats the currently displayed text and adds required linebreaks
             It's a little buggy, so dont go willy-nilly re-formatting everything all at once.
            */

            // Always back it up first...
            SaveText(textBoxText1, curIndex, lang1);

            // temp variable for the current text
            string tb_text = textBoxText1.Text;

            // No need to do anything if the box is blank
            if (tb_text.Length == 0) return;

            // check for the existing end-of-line tags and remove them and any line break characters
            var tb_lines = new List<string>();
            var rem_vals = new Dictionary<string, string>() { 
                { lang1.nl_tags[1], " " },
                { lang1.nl_tags[2], " " },
                { lang1.nl_tags[3], " " },
                { "\r", "" },
                { "\n", "" },
                { "\r\n", "" } 
            };
            foreach (KeyValuePair<string, string> kp in rem_vals)
            {
                tb_text = tb_text.Replace(kp.Key, kp.Value);
            }

            // initialize our loop vars... 
            // skipRefresh is turned ON (prevents the form from firing the text changed event for the text box)
            // clear the text box...
            int pos = 0;
            bool reading_var = false;
            string cur_line = "";
            skipRefresh = true;
            textBoxText1.Text = "";
            var nl = lang1.nl_tags;  // new line tags are now loaded with the lang struct

            // loop through each character in the stored text
            while (pos < tb_text.Length)
            {
                string next_line = "";
                bool overflow = false; // continue while we havent overflown the horizontal space, or the string hasnt ended
                while(!overflow && pos < tb_text.Length)
                {
                    char cur_char = tb_text[pos];  // the current character
                    cur_line += cur_char;
                    if (cur_char == '[') // the start of a tag
                    {
                        reading_var = true;
                    } 
                    else if (cur_char == ']') // end tag
                    {
                        reading_var = false;
                    }
                    textBoxText1.Text = cur_line;  // make the textbox only hold the current line

                    if (!reading_var)
                    {
                        UpdatePictureBox(pictureBoxPreview1, textBoxText1, lang1); 
                        // update the picture box and check the global (eww) variable we added that stores whether the horizontal space was exceeded
                        bool has_overflow = maxWidthExceeded;

                        if (nl[4].Length > 0 && cur_char == ' ' && cur_line.Contains(nl[4])) // np
                        {
                            has_overflow = true;  // force the next line if we hit a new page tag
                        }

                        if (has_overflow)
                        {
                            // get each "word" in the string...
                            var split_line = cur_line.Split(' ');
                            // the last word probably needs to continue on the next line
                            int last_index = split_line.Length - 1;
                            // temporarily store the next line
                            next_line = split_line[last_index];
                            // remove the last index from the array
                            split_line = split_line.Where((source, index) => index != last_index).ToArray();
                            //rebuild the text on the current line
                            textBoxText1.Text = String.Join(" ", split_line);
                            cur_line = next_line;
                            overflow = true;
                        }
                    }
                    pos += 1;
                }

                // save textbox text as a line
                if (textBoxText1.Text.Length > 0) tb_lines.Add(textBoxText1.Text);
                // if we reach the end of the text before we can save the rest normally-- add the next line to the list
                if (!(pos < tb_text.Length) && next_line.Length > 0) tb_lines.Add(next_line);
            }

            if (tb_lines.ToArray().Length > 0)
            {   // loop through the new list of lines...
                tb_text = "";
                String[] lines = tb_lines.ToArray();
                for (int c = 0; c < lines.Length; c++)
                {
                    // lines > 3 all have the same line break code
                    int nl_idx = (c < 3) ? c : 3;
                    // add tags to the required lines
                    tb_text += $"{((c > 0) ? "\r\n" : "")}{nl[nl_idx]}{lines[c]}";
                }
            }
            // turn back on the refresh event
            skipRefresh = false;
            // update the text box
            textBoxText1.Text = tb_text;
        }

        private void pictureBoxTable_Regular_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            int id = (me.X / 16) + ((me.Y / 32) * 16);
            textBox_SelectID.Text = id.ToString("X2");
            textBox_SelectChar.Text = "";
            foreach (var c in lang1.table)
            {
                if (c.Item1 == id.ToString("X2"))
                {
                    textBox_SelectChar.Text = c.Item2;
                    break;
                }
            }
        }

        private void pictureBoxTable_Kanji_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            int id = (me.X / 16) + ((me.Y / 32) * 16);
            string cid;
            if (id < 0x100)
                cid = "FD" + (id - 0x000).ToString("X2");
            else if (id < 0x200)
                cid = "FC" + (id - 0x100).ToString("X2");
            else
                cid = "F4" + (id - 0x200).ToString("X2");

            textBox_SelectID.Text = cid;
            textBox_SelectChar.Text = "";
            foreach (var c in lang1.table)
            {
                if (c.Item1 == cid)
                {
                    textBox_SelectChar.Text = c.Item2;
                    break;
                }
            }
        }

        private void pictureBoxTable_Other_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            int id = (me.X / 32) + ((me.Y / 32) * 8);

            if (id >= other_list.Length)
                return;

            string cid = "FE6D"+ other_list[id].ToString("X2");
            textBox_SelectID.Text = cid;
            textBox_SelectChar.Text = "";
            foreach (var c in lang1.table)
            {
                if (c.Item1 == cid)
                {
                    textBox_SelectChar.Text = c.Item2;
                    break;
                }
            }
        }

        private void buttonCopySelect_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox_SelectChar.Text);
        }
    }
}
