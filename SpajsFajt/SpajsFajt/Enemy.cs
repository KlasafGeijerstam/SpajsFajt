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
        private float rotationSpeed = 0.1f;
        private bool slowDown { get; set; }
        private float timeSinceLastProjectile = 0;
        public bool FireProjectile { get; set; }
        public int Health { get; set; }
        public float TimeSinceLastDamage { get; set; }
        private ExplosionEmitter explosionEmitter;

        public bool ProperDead { get; set; }
        private float searchRadius = 200;
        public bool Roaming { get; set; }
        private Player targetPlayer = new Player(-4);
        private static Random rnd = new Random();

        public Enemy(int id):base("shipPlayer",id)
        {
            Position = World.GetRandomBorderPosition();
            emitter = new ShipEmitter();
            origin = new Vector2(textureRectangle.Width / 2, textureRectangle.Height / 2);
            velocity = 2f;
            Health = 40;
            collisionRectangle = new Rectangle(0, 0, 36, 35);
        }

        public bool InRadius(Vector2 v)
        {
            if (v.X <= position.X + searchRadius && v.X >= position.X - searchRadius && v.Y <= position.Y + searchRadius && v.Y >= position.Y - searchRadius)
            {
                return true;
            }
            else
                return false;
        }

        private void GetRoam()
        {
            targetPlayer.Position = new Vector2(rnd.Next(450, 1550), rnd.Next(400, 1500));
            Target = targetPlayer;
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

            if (explosionEmitter != null)
            {
                explosionEmitter.Update(gameTime);

                if (explosionEmitter.ParticleCount == 0)
                {
                    ProperDead = true;
                }
            }
            
        }
        public void Die()
        {
            explosionEmitter = new ExplosionEmitter(Position);
            Dead = true;
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

                var rot = (float)Math.Atan((Position.Y - Target.Position.Y) / (Position.X - Target.Position.X));
                if (Target.Position.X < Position.X)
                    rot += (float)Math.PI;

                //Fix rotation from  -2pi to 2pi
                if (Rotation > MathHelper.Pi)
                    Rotation -= MathHelper.TwoPi;
                if (rot > MathHelper.Pi) rot -= MathHelper.TwoPi;
                if (Rotation < -MathHelper.Pi)
                    Rotation = MathHelper.TwoPi - Rotation;
                if (rot < -MathHelper.Pi) rot = MathHelper.TwoPi - rot;

                bool incAngle = false;

                slowDown = false;

                if (rotation >= 0 && rot >= 0)
                {
                    if (rotation < rot)
                        incAngle = true;
                }
                else if (rotation < 0 && rot < 0)
                {
                    if (rotation < rot)
                        incAngle = true;
                }
                else
                {
                    if (Math.Abs(rotation - rot) < MathHelper.Pi)
                    {
                        if (rotation < 0)
                            incAngle = true;
                    }
                    else
                    {
                        if (rotation > 0)
                            incAngle = true;
                    }
                }

                if (Math.Abs(rotation - rot) < rotationSpeed)
                    rotation = rot;
                else
                    rotation += (incAngle) ? rotationSpeed : -rotationSpeed;

                if (Vector2.Distance(Position, Target.Position) > 50)
                    position += new Vector2((float)Math.Cos(Rotation) * velocity, (float)Math.Sin(Rotation) * velocity);
                else
                {
                    slowDown = true;
                }
                if (!Roaming && timeSinceLastProjectile >= 1000 && Vector2.Distance(Position, Target.Position) < 400)
                {
                    FireProjectile = true;
                    timeSinceLastProjectile = 0;
                }
                
                if (Roaming && Vector2.Distance(Target.Position, Position) <= 50)
                    GetRoam();

                if (Target.Dead)
                {
                    Roaming = true;
                    Target = null;
                }
            }
            else if (Target == null && Roaming)
            {
                GetRoam();
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Dead)
            {
                spriteBatch.Draw(TextureManager.SpriteSheet, position, textureRectangle, Color.White, rotation + (float)Math.PI / 2, origin, 1f, SpriteEffects.None, 0.5f);
                emitter.Draw(spriteBatch);
            }
            else if (explosionEmitter != null)
            {
                explosionEmitter.Draw(spriteBatch);
            }

        }

    }
}
