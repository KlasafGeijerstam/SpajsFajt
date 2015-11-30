using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class Gold : GameObject
    {
        private float frameTime = 150;
        private float frameTimer = 0;
        private int frame = 1;
        public bool Collect { get; set; }
        public static Vector2 Target { get; set; }
        private new Vector2 velocity;
        public static Vector2 Offset { get; set; }

        public Gold(Vector2 position,int id):base ("coin1",id)
        {
            Position = position;
            CollisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, 7, 7);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, Position, TextureManager.GetRectangle("coin" + frame.ToString()), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.5f);
            //spriteBatch.DrawString(TextureManager.GameFont, frame.ToString(), Position + new Vector2(10, 10), Color.Yellow);
        }

        public override void Update(GameTime gameTime)
        {
            frameTimer += (float)gameTime.ElapsedGameTime.Milliseconds;
            if(Collect)
                position += Offset;
            velocity *= 500 / Vector2.Distance(Target, Position);
            
            Position += velocity;

            if (frameTimer > frameTime)
            {
                frameTimer = 0f;
                frame++;
                if (frame > 4)
                    frame = 1;
            }

            if (Collect)
            {
                var r = Math.Atan((Target.Y - Position.Y) / (Target.X - Position.X));
                velocity = -new Vector2((float)Math.Cos(r) * 1f, (float)Math.Sin(r)*1f);
                if (Vector2.Distance(Target,Position) < 30)
                    Dead = true;
            }
        }
    }
}
