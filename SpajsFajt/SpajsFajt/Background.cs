using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SpajsFajt
{
    class Background
    {
        private List<Border> borders = new List<Border>();
        private List<Backdrop> backdrops = new List<Backdrop>();
        
        public Background(Vector2 p)
        {


            //borders.Add(new Border(new Rectangle(450, 400, 1100, 4),false));//top
            //borders.Add(new Border(new Rectangle(450, 1500, 1100, 4),false));//bot
            //borders.Add(new Border(new Rectangle(450, 400, 1100, 4), true));//left
            //borders.Add(new Border(new Rectangle(1550, 400, 1100, 4), true));//right

            borders.Add(new Border(new Rectangle(-1500, -1500, 5000, 4),false));//top
            borders.Add(new Border(new Rectangle(-1500, 3500, 5000, 4),false));//bot
            borders.Add(new Border(new Rectangle(-1500, -1500,5000, 4), true));//left
            borders.Add(new Border(new Rectangle(3500, -1500, 5000, 4), true));//right

            backdrops.Add(new Backdrop(new Rectangle(-1500, -1500, 5000, 5000),0.3f));

            for (int i = 0; i < 3; i++)
            {
                for (int c = 0; c < 3; c++)
                {
                    backdrops.Add(new Backdrop(new Rectangle(-2000 + (i * 2000), -2000 + (c * 2000), 2000, 2000)));
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            backdrops.ForEach(x => x.Draw(spriteBatch));
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
    class Backdrop
    {
        private Rectangle rectangle { get; set; }
        private static Rectangle source;
        private float depth;
        public Backdrop(Rectangle r, float depth = 0.4f)
        {
            this.depth = depth;
            rectangle = r;
            if (source == Rectangle.Empty)
                source = TextureManager.GetRectangle("background1");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, rectangle, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }
    }
}