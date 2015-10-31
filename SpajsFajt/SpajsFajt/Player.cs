using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;

namespace SpajsFajt
{
    class Player : GameObject
    {
        public NetConnection  Connection {get;set;}
        private float speed = 2f;
        public int Health { get; set; }
        public bool Dead { get; set; }
        public bool DeathSent { get; set; }
        public bool Boosting { get; set; }
        public bool LastBoostValue { get; set; }
        public bool BoostRequest { get; set; }
        public int LastPowerLevel { get; set; }

        public float LastDamageTaken
        {
            get; set;
        }

        private float rotSpeed = 0.05f;
        private float rotationOffset = (float)Math.PI / 2;
        private Rectangle playerRectangle = new Rectangle(0, 0, 50, 50);
        public Rectangle PlayerRectangle { get { return playerRectangle; } }
        private ShipEmitter emitter = new ShipEmitter();
        private float lastVelocity = 0f;
        private ExplosionEmitter explosionEmitter;
        public float TimeDead { get; set; }
        public int PowerLevel { get; set; }


        public Player(int id):
            base("shipPlayer",id)
        {
            Health = 70;
            origin = new Vector2(textureRectangle.Width / 2, textureRectangle.Height / 2);
            collisionRectangle.Width = 34; //39
            collisionRectangle.Height = 31; //36
            PowerLevel = 70;
        }

        public void Die()
        {
            explosionEmitter = new ExplosionEmitter(Position);
            textureRectangle = TextureManager.GetRectangle("none");
            Dead = true;
            velocity = 0f;
            rotation = 0;
            TimeDead = 0;
        }

        public void Respawn()
        {
            TimeDead = 0;
            Dead = false;
            textureRectangle = TextureManager.GetRectangle("shipPlayer");
            DeathSent = false;
            Health = 70;
            PowerLevel = 70;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (explosionEmitter != null)
                explosionEmitter.Draw(spriteBatch);

            spriteBatch.Draw(TextureManager.SpriteSheet, position, textureRectangle, Color.White, rotation + rotationOffset, origin, 1f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(TextureManager.GameFont, Boosting.ToString(), Position - new Vector2(50, 50), Color.Yellow);
            emitter.Draw(spriteBatch);
        }

        public void Input(GameTime gameTime)
        {

            if (!Dead)
            {
                var ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.W))
                {
                    velocity += 0.01f;
                    if (velocity > speed)
                        velocity = speed;
                    if (Boosting)
                    {
                        velocity *= 2;
                    }
                     
                }
                else
                {
                    velocity -= 0.01f;
                    if (velocity < 0.1f)
                        velocity = 0f;
                }

                if (ks.IsKeyDown(Keys.D))
                {
                    if (Boosting)
                        rotation += rotSpeed / 2;
                    else
                        rotation += rotSpeed;
                }

                if (ks.IsKeyDown(Keys.A))
                {
                    if (Boosting)
                        rotation -= rotSpeed / 2;
                    else
                        rotation -= rotSpeed;
                }
                if (ks.IsKeyDown(Keys.Z)) //&& textureRectangle != TextureManager.GetRectangle("none")
                    Die();
                if (ks.IsKeyDown(Keys.LeftShift))
                {
                    BoostRequest = true;
                }
                else
                    BoostRequest = false;

            }

            position += new Vector2((float)Math.Cos(rotation) * velocity, (float)Math.Sin(rotation) * velocity);
        }
        public override void Update(GameTime gameTime)
        {
            playerRectangle.X = (int)position.X;
            playerRectangle.Y = (int)position.Y;
            emitter.Position = new Vector2(position.X - (float)Math.Cos(rotation) * 20, position.Y -(float)Math.Sin(rotation) * 20);
            emitter.Rotation = rotation + (float)Math.PI;
            emitter.Update(gameTime);
            emitter.ParticleSpeed = ((velocity) < .5f) ? 1f: velocity*1.5f;
            emitter.Boosting = Boosting;
            if (Dead)
                TimeDead += gameTime.ElapsedGameTime.Milliseconds;

            if (explosionEmitter != null)
            {
                explosionEmitter.Update(gameTime);
                if (explosionEmitter.Dead)
                    explosionEmitter = null;

            }

            if (velocity >= lastVelocity && velocity != 0f)
                emitter.GenerateParticle(5);

            lastVelocity = velocity;

            collisionRectangle.X = (int)(position.X-collisionRectangle.Height/2);
            collisionRectangle.Y = (int)(position.Y -collisionRectangle.Width/2);
        }

    }
}
