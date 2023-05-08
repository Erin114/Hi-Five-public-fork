using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor
{
    class Screen
    {
        PictureBox[,] screen;

        bool visible = true;
        public bool Visible
        {
            get { return visible; }
            set { visible = value;
                for (int x = 0; x < 15; x++)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        screen[x, y].Visible = value;
                    }
                }
            }
        }
        public Screen()
        {
            screen = new PictureBox[15, 15];
            //populate screen
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    screen[x, y] = new PictureBox();
                    screen[x, y].SizeMode = PictureBoxSizeMode.StretchImage;
                    screen[x, y].BackColor = Color.Lavender;
                    //sets each square to be proportional to the window height
                    screen[x, y].Size = new Size((450 - 10) / 15,
                                                   (450 - 10) / 15);
                    //evenly spaces the picturebox objects                               
                    screen[x, y].Location = new Point(160 + ((450 - 10) / 15) * x,
                                                        10 + ((450 - 10) / 15) * y);
                    screen[x, y].Visible = true;
                    screen[x, y].BringToFront();
                    //won't actually show any of the pictureboxes without adding them all to controls
                    
                }
            }
        }

        public PictureBox Cell(int x, int y)
        { 
            //check to see if the given cell is in bounds of the screen
            if (x < 15 && y < 15 && x >= 0 && y >= 0)
            {
                return screen[x, y];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
