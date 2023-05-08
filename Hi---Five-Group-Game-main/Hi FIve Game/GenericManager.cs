using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hi_FIve_Game
{
    class GenericManager <T>
    {
        // Fields ---------------------------------------------------
        private List<T> objectList;

        // Constructor ----------------------------------------------
        public GenericManager()
        {
            objectList = new List<T>();
        }

        // Methods --------------------------------------------------

        public void Update()
        {
            // First update any platforms

        }


        public void Draw(SpriteBatch sb)
        {
            // Draws all on screen objects in the list
            foreach (T item in objectList)
            {
                //
            }
        }

        /*
        public void Add(T newObject)
        {
            // Adds a new object to the list
        }
        public T Remove(T objectToRemove)
        {
            // Removes an object from the list
            // and returns it to the user
            return default;
        }
        */
    }
}
