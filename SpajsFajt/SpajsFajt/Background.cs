using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SpajsFajt
{
    class Background
    {
        private Rectangle textureRect;
        private Rectangle rectangle;
        private List<Border> borders = new List<Border>();

        public Background(Vector2 p)
        {
            if (textureRect == Rectangle.Empty)
                textureRect = TextureManager.GetRectangle("background1");
            rectangle = new Rectangle((int)p.X, (int)p.Y, 2000, 2000);
            borders.Add(new Border(new Rectangle(450, 400, 1100, 4),false));//top
            borders.Add(new Border(new Rectangle(450, 1500, 1100, 4),false));//bot
            borders.Add(new Border(new Rectangle(450, 400, 1100, 4), true));//left
            borders.Add(new Border(new Rectangle(1550, 400, 1100, 4), true));//right
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, rectangle, textureRect, Color.White);
            borders.ForEach(x => x.Draw(spriteBatch));
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public bool TouchLeft(Rectangle r)
        {
            return borders[3].Intersects(r);
        }
        public bool TouchRight(Rectangle r)
        {
            return borders[2].Intersects(r);
        }
        public bool TouchTop(Rectangle r)
        {
            return borders[0].Intersects(r);
        }
        public bool TouchBottom(Rectangle r)
        {
            return borders[1].Intersects(r);
        }
    }
    class Border
    {
        private Rectangle rectangle;
        private float rotation;
        private Rectangle sourceRectangle = TextureManager.GetRectangle("border");
        public Border(Rectangle r,bool rotated)
        {
            rotation = (rotated) ? (float)Math.PI / 2 : 0f;
            rectangle = r;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, rectangle, sourceRectangle, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0.5f);
        }
        public bool Intersects(Rectangle or)
        {
            return or.Intersects(rectangle);
        }
        
    }
}