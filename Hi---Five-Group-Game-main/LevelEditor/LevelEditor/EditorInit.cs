using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class EditorInit : Form
    {
        /*
         * Erin Ford
         * 2/21/2021
         * IGME 106 HW 2
         */
        public EditorInit()
        {
            InitializeComponent();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox2.Text, out int width);
            //shows a popup whenever there is a parse failure or the parameters are too small
            if (width <= 0)
            {
                MessageBox.Show("Error: Invalid level length");
            }
            else
            {
                Editor editor = new Editor(width);
                editor.ShowDialog();
            }
            
        }

        private void load_Click(object sender, EventArgs e)
        {
            //creates a new instance of the editor with no parameters
            // so that it automatically creates the load file popup
            Editor editor = new Editor();
            if (editor.IsDisposed != true)
            {
                editor.ShowDialog();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
