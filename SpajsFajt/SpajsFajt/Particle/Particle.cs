using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class Particle
    {
        protected Vector2 position;
        protected float rotation;
        protected Vector2 velocity;
        protected float rotationVelocity;
        protected Rectangle sourceRectangle;
        protected float timeToLive;
        public bool IsDead { get; internal set; }
        protected static Vector2 particleOrigin = new Vector2(1, 1);
        protected Color particleColor;
        protected float particleScale;

        public Particle(float rot, float rotVel,Vector2 pos,Rectangle src,float ttl,Vector2 vel)
        {
            position = pos;
            rotation = rot;
            rotationVelocity = rotVel;
            sourceRectangle = src;
            timeToLive = ttl;
            IsDead = false;
            velocity = vel;
            particleColor = Color.White;
            particleScale = 1;
        }

        public virtual void Update(GameTime gameTime)
        {
            timeToLive -= gameTime.ElapsedGameTime.Milliseconds;
            if (timeToLive <= 0)
                IsDead = true;
            rotation += rotationVelocity;
            position += velocity;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, position, sourceRectangle, particleColor,rotation,particleOrigin,particleScale,SpriteEffects.None,0.5f);
        }
    }
}
