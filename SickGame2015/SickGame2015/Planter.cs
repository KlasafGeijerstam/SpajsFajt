using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SickGame2015
{
    class Planter
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed = 1f;
        private Rectangle bjornRectangle;
        private float rotation;
        List<Projectile2> projectiles = new List<Projectile2>();
        private float timeSinceLastShot = 0;
        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("bullla.png");
        }
        public void Update(GameTime gameTime,List<Tuffe> tuffe)
        {

            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.W))
                rotation += 0.2f;
            if (ks.IsKeyDown(Keys.S))
                rotation -= 0.2f;
            if (ks.IsKeyDown(Keys.F) && timeSinceLastShot <= 0)
            {
                projectiles.Add(new Projectile2(position, rotation));
                timeSinceLastShot = 0;
            }
            timeSinceLastShot -= gameTime.ElapsedGameTime.Milliseconds;
            position += new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            projectiles.ForEach(x => x.Update(gameTime));
            foreach (var p in projectiles.ToList())
            {
                foreach (var item in tuffe)
                {
                    if (p.rect.Intersects(item.Rectangle))
                    {
                        item.Hit();
                        projectiles.Remove(p);
                    }  
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, new Vector2(50, 50), 1f, SpriteEffects.None, 1f);
            projectiles.ForEach(x => x.Draw(spriteBatch));

        }
    }
}
