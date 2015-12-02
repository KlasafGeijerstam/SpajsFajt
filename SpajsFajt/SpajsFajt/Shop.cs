using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class Shop : IDrawable
    {
        public Vector2 Position { get; set; }
        
        public string RectangleName { get; set; }
        
        public Rectangle TextureRectangle { get; set; }
        private Vector2 auraOffset = new Vector2(210, 210);
        private Vector2 offset = new Vector2(41, 25);
        private float radius = 210;
        private Rectangle textureSource;
        private Rectangle auraTextureSource;

        public Shop()
        {
            auraTextureSource = TextureManager.GetRectangle("auraShop");
            textureSource = TextureManager.GetRectangle("shop");
            Position = new Vector2(1000, 1000);
        }
        public bool InShop(Vector2 v)
        {
            return Vector2.Distance(v, Position + auraOffset) < radius;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, Position, auraTextureSource, Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(TextureManager.SpriteSheet, Position + auraOffset, textureSource, Color.White, 0f, offset, 1f, SpriteEffects.None, 0.55f);
        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}
