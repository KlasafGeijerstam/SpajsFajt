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
        private float rotationSpeed = 0.07f;
        private bool slowDown { get; set; }
        private float timeSinceLastProjectile = 0;
        public bool FireProjectile { get; set; }
        public int Health { get; set; }
        public float TimeSinceLastDamage { get; set; }

        public Enemy(int id):base("shipPlayer",id)
        {
            Position = new Vector2(400, 300);
            emitter = new ShipEmitter();
            origin = new Vector2(textureRectangle.Width / 2, textureRectangle.Height / 2);
            velocity = 1.5f;
            Health = 30;
            collisionRectangle = new Rectangle(0, 0, 36, 35);
        }

        public override void Update(GameTime gameTime)
        {
            emitter.Position = new Vector2(position.X - (float)Math.Cos(rotation) * 20, position.Y - (float)Math.Sin(rotation) * 20);
            emitter.Rotation = rotation + (float)Math.PI;
            emitter.Update(gameTime);
            emitter.ParticleSpeed = ((velocity) < .5f) ? 1f : velocity * 1.5f;
            if (velocity >= lastVelocity && velocity != 0f && !slowDown)
                emitter.GenerateParticle(5);
            lastVelocity = velocity;
            
        }
        public void Fire()
        {
            timeSinceLastProjectile = 0;
            FireProjectile = false;
        }
        public void Move(GameTime gameTime)
        {
            if (Target != null)
            {
                timeSinceLastProjectile += gameTime.ElapsedGameTime.Milliseconds;
                TimeSinceLastDamage -= gameTime.ElapsedGameTime.Milliseconds;
                if (TimeSinceLastDamage <= 0)
                    TimeSinceLastDamage = 0;

                slowDown = false;
                var rot = (float)Math.Atan((Position.Y - Target.Position.Y) / (Position.X - Target.Position.X));
                if (Target.Position.X < Position.X)
                    rot += (float)Math.PI;
                if (rot != Rotation)
                {
                    Rotation += Math.Min((rot-Rotation) - rotationSpeed, (rot-Rotation)+ rotationSpeed);
                }
                //Rotation = (float)Math.Asin((Position.Y - Target.Position.Y) / Vector2.Distance(Position, Target.Position));

                if (Vector2.Distance(Position, Target.Position) > 50)
                    position += new Vector2((float)Math.Cos(Rotation) * velocity, (float)Math.Sin(Rotation) * velocity);
                else
                {
                    slowDown = true;
                }
                if (timeSinceLastProjectile >= 1000 && Vector2.Distance(Position,Target.Position) < 400)
                {
                    FireProjectile = true;
                    timeSinceLastProjectile = 0;
                }

            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        { 
            spriteBatch.Draw(TextureManager.SpriteSheet, position, textureRectangle, Color.White, rotation + (float)Math.PI/2, origin, 1f, SpriteEffects.None, 0.5f);
            emitter.Draw(spriteBatch);
        }

    }
}
