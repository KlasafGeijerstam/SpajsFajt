using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GUIPower:GUIComponent
    {
        public new int Value { get; set; }

        public GUIPower()
        {
            Value = 7;
            Offset = new Vector2(110,5);
        }

        public new Rectangle TextureRectangle
        {
            get { return TextureManager.GetRectangle("power" + Value); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, Position + Offset, TextureRectangle, Color.White, 0f, Vector2.Zero,2f, SpriteEffects.None, 0.5f);
        }

    }
    

}
