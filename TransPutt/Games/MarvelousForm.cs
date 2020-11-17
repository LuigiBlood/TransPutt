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
            public byte[] font_chr;                    //font.bin - 0x00 to 0xEF
            public byte[] kanji_chr;                   //kanji.bin
            public byte[] width_tbl;                   //width.tbl - Char Width Table
            public List<Tuple<string, string>> table;  //table.tbl - Table
            public string[] main_txt;                  //main.txt - Main Text File
        }

        lang lang1;
        lang lang2;

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
                if (!File.Exists(path + "\\width.tbl"))
                    continue;
                if (!File.Exists(path + "\\table.tbl"))
                    continue;
                if (!File.Exists(path + "\\main.txt"))
                    continue;
                string[] split = path.Split(Path.DirectorySeparatorChar);
                comboBoxLang1.Items.Add(split[split.Length - 1]);
            }

            comboBoxLang1.SelectedIndex = 0;
        }

        private int LoadLanguage(string lang, out lang outlang)
        {
            outlang = new lang();
            string fullpath = ".\\marvelous\\" + lang + "\\";
            outlang.font_chr = File.ReadAllBytes(fullpath + "font.bin");
            outlang.kanji_chr = File.ReadAllBytes(fullpath + "kanji.bin");
            outlang.width_tbl = File.ReadAllBytes(fullpath + "width.tbl");
            PuttScript.GetDictFromFile(fullpath + "table.tbl", 1, out outlang.table);
            return 0;
        }

        private Bitmap RenderText(string text, lang curlang, int style)
        {
            //Styles: 1 = Regular Text Box (black on white), 2 = Small Text (white on black), 3 = With border (black border, white font)
            int maxwidth = 380;
            int line = 0;
            int h_pixel = 0;
            Bitmap output = new Bitmap(maxwidth + 16, 32 * 3);

            using (Graphics g = Graphics.FromImage(output))
            {
                g.FillRectangle(Brushes.White, 0, 0, output.Width, output.Height);
            }

            byte[] encoded;
            PuttScript.Encode(curlang.table, text, out encoded);

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
                        i++;
                    h_pixel = (h_pixel + 16) & ~0xF;
                    char_gfx = new Bitmap(16, 32);
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

        private void comboBoxLang1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLanguage((string)comboBoxLang1.SelectedItem, out lang1);
        }

        private void buttonSave1_Click(object sender, EventArgs e)
        {
            pictureBoxPreview1.Image = RenderText(textBoxText1.Text, lang1, 0);
        }
    }
}
