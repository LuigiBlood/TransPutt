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
        }

        lang lang1;
        lang lang2;
        public bool hasChanged;

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

            numericUpDownID1.Maximum = lang1.main_txt.Length;
            curIndex = (int)numericUpDownID1.Value;
            UpdateTextBox(textBoxText1, curIndex, lang1);
            UpdateNotesBox(textBoxDesc1, curIndex, lang1);
            UpdatePictureBox(pictureBoxPreview1, textBoxText1, lang1);
            UpdateSaveAllButton(buttonSaveAll1);
        }

        private void comboBoxLang2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLanguage((string)comboBoxLang2.SelectedItem, out lang2);
            UpdateTextBox(textBoxText2, curIndex, lang2);
            UpdateNotesBox(textBoxDesc2, curIndex, lang2);
            UpdatePictureBox(pictureBoxPreview2, textBoxText2, lang2);
        }

        private void buttonSave1_Click(object sender, EventArgs e)
        {
            SaveText(textBoxText1, curIndex, lang1);
            SaveNote(textBoxDesc1, curIndex, lang1);
            UpdateSaveAllButton(buttonSaveAll1);
        }

        private void textBoxText1_TextChanged(object sender, EventArgs e)
        {
            UpdatePictureBox(pictureBoxPreview1, textBoxText1, lang1);
            UpdateSaveAllButton(buttonSaveAll1);
        }

        private void numericUpDownID1_ValueChanged(object sender, EventArgs e)
        {
            if (curIndex != -1)
            {
                if (isTextDifferent(textBoxText1, curIndex, lang1) || (lang1.notes[curIndex] != textBoxDesc1.Text.Replace("\r\n", "\\")))
                {
                    if (MessageBox.Show("Do you want to save the text & notes?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SaveText(textBoxText1, curIndex, lang1);
                        SaveNote(textBoxDesc1, curIndex, lang1);
                    }
                }
            }
            curIndex = (int)numericUpDownID1.Value;
            UpdateTextBox(textBoxText1, curIndex, lang1);
            UpdateNotesBox(textBoxDesc1, curIndex, lang1);
            UpdateSaveAllButton(buttonSaveAll1);

            UpdateTextBox(textBoxText2, curIndex, lang2);
            UpdateNotesBox(textBoxDesc2, curIndex, lang2);
            UpdatePictureBox(pictureBoxPreview2, textBoxText2, lang2);
        }

        private void buttonSaveAll1_Click(object sender, EventArgs e)
        {
            SaveText(textBoxText1, curIndex, lang1);
            SaveNote(textBoxDesc1, curIndex, lang1);
            SaveMainScript(lang1);
            UpdateSaveAllButton(buttonSaveAll1);
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

            //Find End Commands
            string en1 = "";
            string en2 = "";
            for (int i = 0; i < outlang.table.Count; i++)
            {
                if (outlang.table[i].Item1 == "FA")
                    en1 = outlang.table[i].Item2;
                if (outlang.table[i].Item1 == "FB")
                    en2 = outlang.table[i].Item2;
                if (en1 != "" && en2 != "")
                    break;
            }

            //Put all script in string array, seperated by end commands
            int lastidx = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (((temp.Length - i) > en1.Length))
                {
                    if (temp.Substring(i, en1.Length) == en1)
                    {
                        i--;
                        list_txt.Add(temp.Substring(lastidx, i - lastidx).Trim().Replace("\r\n", "\n"));
                        i += en1.Length;
                        list_end.Add(en1);
                        lastidx = i + 1;
                    }
                }
                if (((temp.Length - i) > en2.Length))
                {
                    if (temp.Substring(i, en2.Length) == en2)
                    {
                        i--;
                        list_txt.Add(temp.Substring(lastidx, i - lastidx).Trim().Replace("\r\n", "\n"));
                        i += en2.Length;
                        list_end.Add(en2);
                        lastidx = i + 1;
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
        }

        private bool isTextDifferent(TextBox textBox, int id, lang inlang)
        {
            byte[] encoded_orig;
            byte[] encoded_new;
            PuttScript.Encode(inlang.table, inlang.main_txt[id], out encoded_orig);
            PuttScript.Encode(inlang.table, textBox.Text, out encoded_new);

            return Program.isByteArrayEqual(encoded_orig, encoded_new);
        }

        private void SaveNote(TextBox textBox, int id, lang inlang)
        {
            if (inlang.notes[id] == textBox.Text.Replace("\r\n", "\\"))
                return;

            inlang.notes[id] = textBox.Text.Replace("\r\n", "\\");
            hasChanged = true;
        }

        private void SaveText(TextBox textBox, int id, lang inlang)
        {
            if (!isTextDifferent(textBoxText1, curIndex, lang1))
                return;

            //Find End Commands
            string en1 = "";
            string en2 = "";
            for (int i = 0; i < inlang.table.Count; i++)
            {
                if (inlang.table[i].Item1 == "FA")
                    en1 = inlang.table[i].Item2;
                if (inlang.table[i].Item1 == "FB")
                    en2 = inlang.table[i].Item2;
                if (en1 != "" && en2 != "")
                    break;
            }

            inlang.main_txt[id] = textBox.Text.Replace(en1, "").Replace(en2, "").Replace("\r\n", "\n");
            hasChanged = true;
        }

        private void SaveMainScript(lang inlang)
        {
            string temp = "";
            for (int i = 0; i < inlang.main_txt.Length; i++)
                temp += "\n" + inlang.main_txt[i] + "\n" + inlang.main_end[i] + "\n";
            string fullpath = ".\\marvelous\\" + inlang.name + "\\";

            File.WriteAllText(fullpath + "main.txt", temp);
            File.WriteAllLines(fullpath + "notes.txt", inlang.notes);
            hasChanged = false;

            MessageBox.Show(inlang.name + "\\main.txt and notes.txt have been updated.");
        }

        private void UpdateNotesBox(TextBox textBox, int id, lang inlang)
        {
            textBox.Text = inlang.notes[id].Replace("\\", "\r\n");
        }

        private void UpdateTextBox(TextBox textBox, int id, lang inlang)
        {
            textBox.Text = inlang.main_txt[id].Replace("\n", "\r\n");
        }

        private void UpdatePictureBox(PictureBox pictureBox, TextBox textBox, lang inlang)
        {
            pictureBox.Image = RenderText(textBox.Text, inlang, comboBoxStyle1.SelectedIndex);
            pictureBox.Width = pictureBox.Image.Width;
            pictureBox.Height = pictureBox.Image.Height;
        }

        private void UpdateSaveAllButton(Button button)
        {
            button.Enabled = hasChanged || isTextDifferent(textBoxText1, curIndex, lang1);
        }

        private Bitmap RenderText(string text, lang curlang, int style)
        {
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
            PuttScript.Encode(curlang.table, text, out encoded);
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
            }

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
                        i++;
                        char_gfx = RenderIcon(encoded[i], curlang, style);
                    }
                    else
                    {
                        char_gfx = new Bitmap(16, 32);
                    }
                    if ((h_pixel % 16) != 0)
                        h_pixel += 16 - (h_pixel % 16);

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
            }

            return output;
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
                    gfx = new Bitmap(16 * 5, 32);
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
    }
}
