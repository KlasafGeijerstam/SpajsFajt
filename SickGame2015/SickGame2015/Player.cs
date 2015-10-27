using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SickGame2015
{
    class Player
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed = 1f;
        private Rectangle bjornRectangle;
        private float rotation;
        List<Projectile> projectiles = new List<Projectile>();
        private float timeSinceLastShot = 0;
        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("bjorn.jpg");
        }
        public void Update(GameTime gameTime,List<Tuffe> tuffe)
        {
     
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
                rotation += 0.2f;
            if (ks.IsKeyDown(Keys.Down))
                rotation -= 0.2f;
            if (ks.IsKeyDown(Keys.Space) && timeSinceLastShot <= 0)
            {
                projectiles.Add(new Projectile(position, rotation));
                timeSinceLastShot = 1000;
            }
            timeSinceLastShot -= gameTime.ElapsedGameTime.Milliseconds;
            position += new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            projectiles.ForEach(x => x.Update(gameTime));
            foreach (var p in projectiles.ToList())
            {
                foreach (var item in tuffe)
                {
                    if (p.rect.Intersects(item.Rectangle))
                    { item.Hit();
                        projectiles.Remove(p);
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null,Color.White,rotation,new Vector2(50,50),1f,SpriteEffects.None,1f);
            projectiles.ForEach(x => x.Draw(spriteBatch));
            
        }
    }
}
