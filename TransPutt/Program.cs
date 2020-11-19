using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TransPutt
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static int Launch(string id)
        {
            if (id == "marvelous")
            {
                if (!Directory.Exists(".\\marvelous\\"))
                {
                    MessageBox.Show("\"marvelous\" folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }

                if (Directory.GetDirectories(".\\marvelous\\").Length == 0)
                {
                    MessageBox.Show("\"marvelous\" folder does not contain language subdirectories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }

                Games.MarvelousForm app = new Games.MarvelousForm();
                app.ShowDialog();
                return 0;
            }
            return -1;
        }

        public static string Help(string id)
        {
            if (id == "marvelous")
            {
                return "Marvelous\n\nStuff.";
            }
            return "Invalid";
        }

        public static byte[] Subarray(byte[] array, int index, int size)
        {
            byte[] temp = new byte[size];

            for (int i = 0; i < size; i++)
                temp[i] = array[index + i];

            return temp;
        }

        public static bool isByteArrayEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return true;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return true;
            }

            return false;
        }
    }
}
