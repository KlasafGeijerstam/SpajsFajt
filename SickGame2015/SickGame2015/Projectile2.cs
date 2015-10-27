using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SickGame2015
{
    internal class Projectile2
    {
        private static Texture2D texture;
        private Vector2 position;
        private float rotation = 90;
        private static float speed = 2f;
        public Rectangle rect { get; set; }
        public Projectile2(Vector2 pos, float rotation)
        {
            position = pos;
            this.rotation = rotation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.Purple, rotation, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        }
        public void Update(GameTime gameTime)
        {
            position += new Vector2((float)Math.Cos(rotation) * speed, (float)Math.Sin(rotation) * speed);
            rect = new Rectangle((int)position.X, (int)position.Y, 8, 4);
        }
        public static void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("projectile.png");
        }
    }
}