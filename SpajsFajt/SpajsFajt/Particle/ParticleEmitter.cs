using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class ParticleEmitter
    {
        public float Rotation { get; set; }
        public Vector2 Position { get; set; }
        protected List<Particle> particles = new List<Particle>();
        public float ParticleSpeed { get; set; }
        protected Random random;
        public int ParticleCount { get { return particles.Count; } }
        public int ParticleLifeTime { get; set; }
        protected int particleAngleBound;
        protected float particleAngleDivisor;
        protected bool randomColor = false;
        protected Color upperBoundColor;
        protected Color lowerBoundColor;

        public ParticleEmitter(Vector2 pos,float rot)
        {
            Position = pos;
            Rotation = rot;
            ParticleSpeed = 2f;
            random = new Random();
            ParticleLifeTime = 6000;
            particleAngleBound = 100;
            particleAngleDivisor = 200f;
        }

        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
                if (particles[i].IsDead)
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var p in particles)
            {
                p.Draw(spriteBatch);
            }
        }
        public virtual void GenerateParticle(int amount = 1)
        {
            
            for (int i = 0; i < amount; i++)
            {
                float r = (random.Next(-particleAngleBound, particleAngleBound))/particleAngleDivisor;
               

                float rotvel = (random.Next(2) > 0) ? (float)random.NextDouble()/100: (float)-random.NextDouble()/100 ;
                particles.Add(new Particle(Rotation, rotvel, Position, TextureManager.GetParticle(), ParticleLifeTime, new Vector2((float)Math.Cos(Rotation+r)*ParticleSpeed,(float)Math.Sin(Rotation+r)*ParticleSpeed)));
            }
        }
    }
}
