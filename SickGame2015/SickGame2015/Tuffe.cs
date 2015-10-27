using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SickGame2015
{
    class Tuffe
    {
        public static Texture2D texture;
        public int Health { get; set; }
        public Rectangle Rectangle { get; set; }
        private static bool up;
        private bool down;
        private float speed = 1f;
        public Tuffe()
        {
            down = !up;
            up = !up;
            Rectangle = new Rectangle(250, (down) ? 400 : -50, 97*2, 136*2);
            Health = 4;
        }
        public void Hit()
        {
            Health -= 2;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rectangle, Color.White);
        }
        public void Update(GameTime gameTime)
        {
            Rectangle = new Rectangle(Rectangle.X, Rectangle.Y + (int)((down) ? -speed : speed), Rectangle.Width, Rectangle.Height);
        }
        public static void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("tuffe.png");

        }
    }
}
