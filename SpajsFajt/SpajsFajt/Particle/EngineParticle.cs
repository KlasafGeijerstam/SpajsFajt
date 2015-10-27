using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class EngineParticle:Particle
    {

        public EngineParticle(float rot,float rotVel, Vector2 pos, Vector2 vel,Color customColor,float ttl)
            : base(rot, rotVel, pos, TextureManager.GetRectangle("particleWhite"), ttl, vel)
        {
            particleColor = customColor;
            particleScale = 1f;
        }




    }
}
