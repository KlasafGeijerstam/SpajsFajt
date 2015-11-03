using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class Enemy : GameObject
    {
        public Player Target { get; set; }
        private ShipEmitter emitter;
        private float lastVelocity;
        public Enemy(int id):base("shipPlayer",id)
        {
            emitter = new ShipEmitter();
            origin = new Vector2(textureRectangle.Width / 2, textureRectangle.Height / 2);
            velocity = 1.5f;
        }

        public override void Update(GameTime gameTime)
        {
            emitter.Position = new Vector2(position.X - (float)Math.Cos(rotation) * 20, position.Y - (float)Math.Sin(rotation) * 20);
            emitter.Rotation = rotation + (float)Math.PI;
            emitter.Update(gameTime);
            emitter.ParticleSpeed = ((velocity) < .5f) ? 1f : velocity * 1.5f;
            if (velocity >= lastVelocity && velocity != 0f)
                emitter.GenerateParticle(5);
            lastVelocity = velocity;
        }

        public void Move(GameTime gameTime)
        {
            if (Target != null)
            {
                Rotation = (float)Math.Atan((Position.Y - Target.Position.Y) / (Position.X - Target.Position.X));
                //Rotation = (float)Math.Asin((Position.Y - Target.Position.Y) / Vector2.Distance(Position, Target.Position));
                if (Target.Position.X < Position.X)
                    Rotation += (float)Math.PI;

                position += new Vector2((float)Math.Cos(Rotation) * velocity, (float)Math.Sin(Rotation) * velocity);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        { 
            spriteBatch.Draw(TextureManager.SpriteSheet, position, textureRectangle, Color.White, rotation + (float)Math.PI/2, origin, 1f, SpriteEffects.None, 0.5f);
            emitter.Draw(spriteBatch);
        }

    }
}
