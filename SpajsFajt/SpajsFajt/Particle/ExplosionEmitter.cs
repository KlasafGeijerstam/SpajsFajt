using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class ExplosionEmitter:ParticleEmitter
    {

        private float timeToLive = 1000;
        private float timeSinceLastParticle = 0; 
        public bool Dead { get; set; }
        private int particlesPerWave = 400;


        public ExplosionEmitter(Vector2 p):base(p,0f)
        {
            ParticleSpeed = 7f;
            ParticleLifeTime = 700;
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceLastParticle -= gameTime.ElapsedGameTime.Milliseconds;
            timeToLive -= gameTime.ElapsedGameTime.Milliseconds;
            
            if (timeToLive > 0 && timeSinceLastParticle <= 0)
            {
                timeToLive = 0;
                timeSinceLastParticle = 50;
                var angleOffset = (float)(Math.PI * 2) / particlesPerWave;
                
                for (int i = 0; i < particlesPerWave; i++)
                {
                    var speed = ParticleSpeed + random.Next(-4, 4);
                    particles.Add(new ExplosionParticle(angleOffset * i,(float)random.NextDouble(), Position, new Vector2((float)Math.Cos(angleOffset * i) *speed,
                        (float)Math.Sin(angleOffset * i) * speed),ParticleLifeTime + random.Next(-200*(int)speed,200/(int)speed),Color.Orange));
                }
            }

            if (timeToLive < 0 && particles.Count == 0)
                Dead = true;
                        
            base.Update(gameTime);  
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch); 
        }



    }
}
