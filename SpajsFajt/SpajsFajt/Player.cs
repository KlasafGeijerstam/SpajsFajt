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
        public float LastDamageTaken
        {
            get; set;
        }

        private float rotSpeed = 0.1f;
        private float rotationOffset = (float)Math.PI / 2;
        private Rectangle playerRectangle = new Rectangle(0, 0, 50, 50);
        public Rectangle PlayerRectangle { get { return playerRectangle; } }
        private ShipEmitter emitter = new ShipEmitter();
        private float lastVelocity = 0f;

        public Player(int id):
            base("shipPlayer",id)
        {
            Health = 70;
            origin = new Vector2(textureRectangle.Width / 2, textureRectangle.Height / 2);
            collisionRectangle.Width = 39;
            collisionRectangle.Height = 36;
        }



        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, position, textureRectangle, Color.White, rotation + rotationOffset, origin, 1f, SpriteEffects.None, 1f);
            emitter.Draw(spriteBatch);
        }

        public void Input(GameTime gameTime)
        {
            
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.W))
            {
                velocity += 0.01f;
                if (velocity > speed)
                    velocity = speed;
            }
            else
            {
                velocity -= 0.01f;
                if (velocity < 0.1f)
                    velocity = 0f;
            }

            if (ks.IsKeyDown(Keys.D))
                rotation += rotSpeed;
            if (ks.IsKeyDown(Keys.A))
                rotation -= rotSpeed;

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
            
            if (velocity >= lastVelocity && velocity != 0f)
                emitter.GenerateParticle(5);

            lastVelocity = velocity;
            base.Update(gameTime);
        }

    }
}
