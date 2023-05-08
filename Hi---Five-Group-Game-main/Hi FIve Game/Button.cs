using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hi_FIve_Game
{
    // Name: Troy Corrington
    // Date: 3/22/21
    // Purpose: A button used for menus and
    // to allow the user to change game states
    class Button
    {
        // Fields ---------------------------------------------------
        Texture2D image;
        Texture2D hoverImage;
        SpriteFont buttonFont;
        string buttonText;
        Rectangle imageRectangle;

        // Properties -----------------------------------------------

        public int CenterX
        {
            get { return imageRectangle.X + imageRectangle.Width / 2; }
            set { imageRectangle.X = value; }
        }

        public int CenterY
        {
            get { return imageRectangle.Y + imageRectangle.Height / 2; }
            set { imageRectangle.Y = value; }
        }

        public string Text
        {
            get { return buttonText; }
            set { buttonText = value; }
        }

        // Constructor ----------------------------------------------
        /// <summary>
        /// A constructor that takes a rectangle parameter
        /// </summary>
        /// <param name="image">The default image for the button</param>
        /// <param name="secondImage">The image for when the button is hovered over</param>
        /// <param name="imageRectangle">The rectangle the image will conform to</param>
        public Button(Texture2D image, SpriteFont font, 
            string text, Rectangle imageRectangle)
        {
            this.image = image;
            this.buttonFont = font;
            this.buttonText = text;
            this.imageRectangle = imageRectangle;
        }

        /// <summary>
        /// Dummy constructor to be deleted later
        /// </summary>
        /// <param name="font">Font of the text</param>
        /// <param name="text">The text of the button</param>
        /// <param name="imageRectangle">The rectangle the text conforms to</param>
        public Button(SpriteFont font, string text, 
            Rectangle imageRectangle)
        {
            this.buttonFont = font;
            this.buttonText = text;
            this.imageRectangle = imageRectangle;
        }

        /// <summary>
        /// A constructor that takes position and height parameters
        /// </summary>
        /// <param name="image">The default image for the button</param>
        /// <param name="secondImage">The image for when the button is hovered over</param>
        /// <param name="width">The width of the button</param>
        /// <param name="height">The height of the button</param>
        /// <param name="x">The x position of the button</param>
        /// <param name="y">The y position of the button</param>
        public Button(Texture2D image, Texture2D secondImage,
            SpriteFont font, string text,
            int width, int height, int x, int y) :
            this(image, font, text, new Rectangle(x, y, width, height))
        { }

        // Methods --------------------------------------------------
        /// <summary>
        /// Changes the image of the button depending on if the mouse
        /// is currently hovered over it or not
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            // Checks if mouse is hovering above the button
            if (Mouse.GetState().Position.X >= this.imageRectangle.X &&
                Mouse.GetState().Position.X <=
                    (this.imageRectangle.X + this.imageRectangle.Width)
                &&
                Mouse.GetState().Position.Y >= this.imageRectangle.Y &&
                Mouse.GetState().Position.Y <=
                    (this.imageRectangle.Y + this.imageRectangle.Height))
            {
                //draws the button white around the text if hovered over           
                sb.Draw(image, imageRectangle, Color.White);
                // If it is, make the text gray
                sb.DrawString(buttonFont, buttonText,
                    new Vector2(imageRectangle.X, imageRectangle.Y),
                    Color.Gray);
            }
            else
            {
                //draws the button gray if not hovered over
                sb.Draw(image, imageRectangle, Color.Gray);
                // If it isn't, make the text black
                sb.DrawString(buttonFont, buttonText,
                    new Vector2(imageRectangle.X, imageRectangle.Y),
                    Color.Black);
            }
        }


        public bool isClicked(MouseState currentMouseState,
            MouseState previousMouseState)
        {
            // First check if the mouse is over
            // the button
            if (Mouse.GetState().Position.X >= this.imageRectangle.X &&
                Mouse.GetState().Position.X <=
                    (this.imageRectangle.X + this.imageRectangle.Width)
                &&
                Mouse.GetState().Position.Y >= this.imageRectangle.Y &&
                Mouse.GetState().Position.Y <=
                    (this.imageRectangle.Y + this.imageRectangle.Height))
            {
                // Check if mouse button was just
                // clicked
                if (currentMouseState.LeftButton == ButtonState.Pressed
                    && previousMouseState.LeftButton == ButtonState.Released)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
