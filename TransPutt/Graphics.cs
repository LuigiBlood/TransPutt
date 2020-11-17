using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransPutt
{
    public class GraphicsRender
    {
        public class Nintendo
        {
            public static Bitmap TileFrom2BPP(byte[] dat, Color[] pal)
            {
                Bitmap tile = new Bitmap(8, 8);

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int colorID = (((dat[(y * 2)] & (1 << (7 - x))) << x) >> 7)
                            | (((dat[(y * 2) + 1] & (1 << (7 - x))) << x) >> 6);

                        int xt = x;
                        int yt = y;

                        tile.SetPixel(xt, yt, pal[colorID]);
                    }
                }

                return tile;
            }
        }
    }
}
