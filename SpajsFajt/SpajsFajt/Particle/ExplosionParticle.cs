using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class ExplosionParticle:Particle
    {
        
        public ExplosionParticle(float r,float rv,Vector2 p,Vector2 v,float ttl,Color c):base(r,rv,p,TextureManager.GetParticle(),ttl,v)
        {
            particleColor = c;
            particleScale = 1.3f;
        }

        public override void Update(GameTime gameTime)
        {
            velocity /= 1.01f;

            base.Update(gameTime);
        }
    }
}
