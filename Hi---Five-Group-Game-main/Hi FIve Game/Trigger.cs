using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    /// <summary>
    /// Used to store Actions that can be run in update
    /// </summary>
    delegate void TriggerUpdateDelegate();

    delegate void TriggerDrawDelegate(SpriteBatch sb);

    class Trigger : GameObject
    {
        //---Fields----------------------------------------------------------------------
        private Vector2 textPosition;

        private SpriteFont spriteFont;

        private string message;

        /// <summary>
        /// Creates a hit box that performs certain actions when collided with
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Trigger(int x, int y, int width, int height, SpriteFont font, string msg, Vector2 textPos) :
            base(x, y, width, height, default(Texture2D))
        {
            // Handled by Base

            spriteFont = font;
            message = msg;
            textPosition = textPos;

            OnCollision += CollisionResolver.PlayerTrigger; 
        }

        public Trigger(int x, int y, int width, int height, SpriteFont font, string msg) :
            this(x, y, width, height, font, msg, new Vector2(x + width / 2, y + height / 2))
        {
            // Handled by Base
        }

        /// <summary>
        /// Draws the Trigger's message to the screen
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            // Only draw when there's a collision
            if (Colliding)
            {
                sb.DrawString(
                    spriteFont,
                    message,
                    textPosition,
                    Color.Black
                    );

                // Reset the collision bool
                isColliding = false;
            }
        }


    }
}
