using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hi_FIve_Game
{
    class EnemyPistol : Pistol
    {
        /// <summary>
        /// Creates a pistol that the Enemy can shoot
        /// </summary>
        /// <param name="x">X position of the pistol</param>
        /// <param name="y">Y position of the pistol</param>
        /// <param name="width">Width of the pistol</param>
        /// <param name="height">Height of the pistol</param>
        /// <param name="tex">Pistol's texture</param>
        /// <param name="b">Bullet that the pistol shoots</param>
        public EnemyPistol(int x, int y, int width, int height, Texture2D tex, Bullet b) :
            base(x, y, width, height, tex, b)
        {
            fireRate = 25;
        }
    }
}
