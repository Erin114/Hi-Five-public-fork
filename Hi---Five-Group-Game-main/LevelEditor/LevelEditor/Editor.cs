using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;


namespace LevelEditor
{
    public partial class Editor : Form
    {
        /*
         * Erin Ford
         * 2/21/2021
         * IGME 106 HW 2
         */
        string selectedColor;
        Screen activeScreen;
        int activeScreenPos;
        bool isDrag;
        List<string> tileSets = new List<string>();
        List<string> tileSetNames = new List<string>();
        Screen[] level;
        bool changes = false;
        //not reccommended to set width over 15 due to the sheer number of objects
        /// <summary>
        /// Instantiates all game assets and creates a new set of 15x15 screens.
        /// </summary>
        /// <param name="screenNumber"></param>
        public Editor(int screenNumber)
        {

            InitializeComponent();
            level = new Screen[screenNumber];
            label1.Text = $"Screen: 1/{screenNumber}";
            for (int i = 0; i < screenNumber; i++)
            {
                level[i] = new Screen();
                level[i].Visible = false;
            }

            level[0].Visible = true;
            activeScreenPos = 0;
            for (int i = 0; i < level.Length; i++)
            {
                for (int x = 0; x < 15; x++)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        //clears old objects from controls if they exist
                        this.Controls.Add(level[i].Cell(x, y));
                        level[i].Cell(x, y).MouseDown += new System.Windows.Forms.MouseEventHandler(this.square_Click);
                        level[i].Cell(x, y).MouseEnter += new System.EventHandler(this.DragTileColor);
                        level[i].Cell(x, y).MouseUp += new System.Windows.Forms.MouseEventHandler(this.DisengageDrag);
                    }
                }
            }
            string currentDir = Environment.CurrentDirectory;
            string projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;
            MessageBox.Show(projectDir);
            tileSetNames.Add("Eraser");
            tileSets.Add("");
            tileSetNames.Add("Player");
            tileSets.Add(projectDir + @"\Hi FIve Game\Content\playerStatic.png");
            tileSetNames.Add("EnemyCharger");
            tileSets.Add(projectDir + @"\Hi FIve Game\Content\chargerStatic.png");
            tileSetNames.Add("EnemyShooter");
            tileSets.Add(projectDir + @"\Hi FIve Game\Content\shooterStatic.png");
            // tileSetNames.Add("PlatformCenter");
            // tileSetNames.Add("PlatformLeft");
            // tileSetNames.Add("PlatformRight");
            // Static Sprites =======================================
            // Collectibles -----------------------------------------
            tileSetNames.Add("Rocket Ammo");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\ammo_Rocket.png");
            tileSetNames.Add("Shotgun Ammo");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\ammo_Shotgun.png");
            tileSetNames.Add("Health PickUp");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\pickup_Health.png");
            tileSetNames.Add("Level Clear Door");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\pickup_LevelDoor.png");
            // Platforms --------------------------------------------
            // Normal Platforms =====================================
            tileSetNames.Add("Left Float Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_LeftNormal.png");
            tileSetNames.Add("Middle Float Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_MidNormal.png");
            tileSetNames.Add("Right Float Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_RightNormal.png");

            tileSetNames.Add("Left Ground Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_LeftNormal.png");
            tileSetNames.Add("Middle Ground Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_MidNormal.png");
            tileSetNames.Add("Right Ground Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_RightNormal.png");

            tileSetNames.Add("Left Wall Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_LeftNormal.png");
            tileSetNames.Add("Middle Wall Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_MidNormal.png");
            tileSetNames.Add("Right Wall Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_RightNormal.png");

            // Destructable Platforms ===============================
            tileSetNames.Add("Left Float Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_LeftDestruct.png");
            tileSetNames.Add("Middle Float Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_MidDestruct.png");
            tileSetNames.Add("Right Float Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_RightDestruct.png");

            tileSetNames.Add("Left Ground Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_LeftDestruct.png");
            tileSetNames.Add("Middle Ground Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_MidDestruct.png");
            tileSetNames.Add("Right Ground Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_RightDestruct.png");

            tileSetNames.Add("Left Wall Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_LeftDestruct.png");
            tileSetNames.Add("Middle Wall Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_MidDestruct.png");
            tileSetNames.Add("Right Wall Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_RightDestruct.png");

            // Jump Through Platforms ===============================
            tileSetNames.Add("Left Float Jump Thru");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_LeftJumpThru.png");
            tileSetNames.Add("Middle Float Jump Thru");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_MidJumpThru.png");
            tileSetNames.Add("Right Float Jump Thru");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_RightJumpThru.png");

            // Lava Platform ========================================
            tileSetNames.Add("Spikes");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\spikes.png");

            listBox1.DataSource = tileSetNames;
        }
        public Editor() //init patch for load level button on the original editorinit form to 
        {               //make my life easier and save on redundant code

            InitializeComponent();
            string currentDir = Environment.CurrentDirectory;
            string projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;
            MessageBox.Show(projectDir);
            tileSetNames.Add("Eraser");
            tileSets.Add("");
            tileSetNames.Add("Player");
            tileSets.Add(projectDir + @"\Hi FIve Game\Content\playerStatic.png");
            tileSetNames.Add("EnemyCharger");
            tileSets.Add(projectDir + @"\Hi FIve Game\Content\chargerStatic.png");
            tileSetNames.Add("EnemyShooter");
            tileSets.Add(projectDir + @"\Hi FIve Game\Content\shooterStatic.png");
            // tileSetNames.Add("PlatformCenter");
            // tileSetNames.Add("PlatformLeft");
            // tileSetNames.Add("PlatformRight");
            // Static Sprites =======================================
            // Collectibles -----------------------------------------
            tileSetNames.Add("Rocket Ammo");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\ammo_Rocket.png");
            tileSetNames.Add("Shotgun Ammo");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\ammo_Shotgun.png");
            tileSetNames.Add("Health PickUp");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\pickup_Health.png");
            tileSetNames.Add("Level Clear Door");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\pickup_LevelDoor.png");
            // Platforms --------------------------------------------
            // Normal Platforms =====================================
            tileSetNames.Add("Left Float Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_LeftNormal.png");
            tileSetNames.Add("Middle Float Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_MidNormal.png");
            tileSetNames.Add("Right Float Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_RightNormal.png");

            tileSetNames.Add("Left Ground Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_LeftNormal.png");
            tileSetNames.Add("Middle Ground Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_MidNormal.png");
            tileSetNames.Add("Right Ground Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_RightNormal.png");

            tileSetNames.Add("Left Wall Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_LeftNormal.png");
            tileSetNames.Add("Middle Wall Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_MidNormal.png");
            tileSetNames.Add("Right Wall Normal");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_RightNormal.png");

            // Destructable Platforms ===============================
            tileSetNames.Add("Left Float Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_LeftDestruct.png");
            tileSetNames.Add("Middle Float Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_MidDestruct.png");
            tileSetNames.Add("Right Float Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_RightDestruct.png");

            tileSetNames.Add("Left Ground Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_LeftDestruct.png");
            tileSetNames.Add("Middle Ground Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_MidDestruct.png");
            tileSetNames.Add("Right Ground Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_ground_RightDestruct.png");

            tileSetNames.Add("Left Wall Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_LeftDestruct.png");
            tileSetNames.Add("Middle Wall Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_MidDestruct.png");
            tileSetNames.Add("Right Wall Destruct");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_wall_RightDestruct.png");

            // Jump Through Platforms ===============================
            tileSetNames.Add("Left Float Jump Thru");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_LeftJumpThru.png");
            tileSetNames.Add("Middle Float Jump Thru");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_MidJumpThru.png");
            tileSetNames.Add("Right Float Jump Thru");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\platform_float_RightJumpThru.png");

            // Lava Platform ========================================
            tileSetNames.Add("Lava");
            tileSets.Add(projectDir +
                @"\Hi FIve Game\Content\" +
                @"LevelEditor Sprites\tempLava.jpg");

            listBox1.DataSource = tileSetNames;
            load_Click(button8, null);
        }
        /// <summary>
        /// Changes the selected image preview to show what asset is being used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void swatch_Click(object sender, EventArgs e)
        {
            int ind = listBox1.SelectedIndex;
            if (tileSetNames[ind] != "Eraser")
            {
                selectedColor = tileSets[ind];
                //selectedColor = Image.FromFile(@"C:\Users\Erin\Pictures\hqdefault.jpg");
                CurrentColor.ImageLocation = selectedColor;
            }
            else
            {
                CurrentColor.Image = null;
                selectedColor = default;
            }


        }
        /// <summary>
        /// Changes the image location a specific picturebox pulls from, alongside starting the drag function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void square_Click(object sender, MouseEventArgs e)
        {
            //checks to see if there is a selected color and that said color is different than the clicked picture before changing
            if (selectedColor != default && e.Button == MouseButtons.Left)
            {
                ((PictureBox)sender).ImageLocation = selectedColor;
                ((PictureBox)sender).Capture = false;
                isDrag = true;
                if (!changes)
                {
                    changes = true;
                    this.Text = this.Text + "*";
                }
            }
            else
            {

                ((PictureBox)sender).Image = default;
            }
        }
        /// <summary>
        /// Essentially a continuation of square_click that allows the user to continue dragging to paint objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragTileColor(object sender, EventArgs e)
        {
            // Checks is the object being hovered over is a
            // Picture Box, as well as if they are currently
            // dragging the mouse
            if (sender is PictureBox && isDrag)
            {
                ((PictureBox)sender).ImageLocation = selectedColor;
            }
        }
        /// <summary>
        /// Self Explanatory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisengageDrag(object sender, EventArgs e)
        {
            isDrag = false;
        }
        /// <summary>
        /// Disables ActiveControl to allow the user to drag their cursor like a paintbrush
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paint_funct(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }
        /// <summary>
        /// Disables the visibility of the current screen and scrolls to the previous screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switchScreensLeft_Click(object sender, EventArgs e)
        {
            if (activeScreenPos - 1 < 0)
            {
                return;
            }
            else
            {

                level[activeScreenPos].Visible = false;
                activeScreenPos--;
                label1.Text = $"Screen {activeScreenPos + 1}/{level.Length}";
                level[activeScreenPos].Visible = true;
            }
        }
        /// <summary>
        /// Disables the visibility of the current screen and scrolls to the next screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switchScreensRight_Click(object sender, EventArgs e)
        {
            if (activeScreenPos + 1 >= level.Length)
            {
                return;
            }
            else
            {
                level[activeScreenPos].Visible = false;
                activeScreenPos++;
                label1.Text = $"Screen {activeScreenPos + 1}/{level.Length}";
                level[activeScreenPos].Visible = true;
            }
        }
        /// <summary>
        /// Saves the level length in cells alongside the index of each asset in an array.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_Click(object sender, EventArgs e)
        {
            //opens save dialog to allow user input of folders and file name
            SaveFileDialog save = new SaveFileDialog();
            save.DefaultExt = ".level";
            save.InitialDirectory = "../../";
            save.ShowDialog();

            FileStream outStream = new FileStream(save.FileName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(outStream);
            writer.Write(level.Length);
            //important for save and load features to be indexed in the same way to avoid OOB or incorrect renders
            for (int i = 0; i < level.Length; i++)
            {
                for (int x = 0; x < 15; x++)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        if (level[i].Cell(x, y).Image != default)
                        {
                            for (int l = 0; l < tileSets.Count; l++)
                            {
                                if (level[i].Cell(x, y).ImageLocation == tileSets[l])
                                {
                                    writer.Write(l);
                                }
                            }
                        }
                        else
                        {
                            writer.Write(0);
                        }

                    }
                }
            }
            writer.Close();
            changes = false;
            this.Text = $"Editor - {save.FileName} ";
        }
        /// <summary>
        /// Loads from a given file, essentially the inverse of the save class. Simply gets the indices of each cell and assigns it an image accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void load_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            //filters to only show the format used in saving (.level)
            open.Filter = "Level Files (.level)|*.level";
            open.InitialDirectory = "../../";
            open.ShowDialog();
            if (open.FileName != "")
            {
                FileStream inStream = File.OpenRead(open.FileName);
                BinaryReader reader = new BinaryReader(inStream);

                //resets window size to accomodate loaded width and height


                if (level != null) //avoids nullreference exception and OOB
                {
                    for (int i = 0; i < level.Length; i++)
                    {
                        for (int x = 0; x < 15; x++)
                        {
                            for (int y = 0; y < 15; y++)
                            {
                                //clears old objects from controls if they exist
                                this.Controls.Remove(level[i].Cell(x, y));
                            }
                        }
                    }
                }
                level = new Screen[reader.ReadInt32()];
                for (int i = 0; i < level.Length; i++)
                {

                    level[i] = new Screen();
                    level[i].Visible = false;
                    for (int x = 0; x < 15; x++)
                    {
                        for (int y = 0; y < 15; y++)
                        {
                            //exact same as the init loading code but assigns the color values according to the loaded file
                            level[i].Cell(x, y).ImageLocation = tileSets[reader.ReadInt32()];
                            this.Controls.Add(level[i].Cell(x, y));
                            level[i].Cell(x, y).MouseDown += new System.Windows.Forms.MouseEventHandler(this.square_Click);
                            level[i].Cell(x, y).MouseEnter += new System.EventHandler(this.DragTileColor);
                            level[i].Cell(x, y).MouseUp += new System.Windows.Forms.MouseEventHandler(this.DisengageDrag);

                        }
                    }
                }

                reader.Close();
                //resetting parameters to ensure everything loads properly
                changes = false;
                activeScreen = level[0];
                activeScreen.Visible = true;
                activeScreenPos = 0;
                label1.Text = $"Screen {activeScreenPos + 1}/{level.Length}";
                this.Text = $"Editor - {open.FileName}";
                MessageBox.Show("File loaded successfully.",
                             "Success");
            }
            else
            {
                MessageBox.Show("Invalid file.",
                             "Error");
                this.Close();
                return;
            }
        }


        private void Editor_Closing(object sender, FormClosingEventArgs e)
        {
            if (changes)
            {

                if (MessageBox.Show("There are Unsaved Changes, would you still like to exit?",
                                "Exit?",
                                MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }

            }

        }
    }
}
