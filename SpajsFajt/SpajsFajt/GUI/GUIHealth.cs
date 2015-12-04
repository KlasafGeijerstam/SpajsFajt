using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GUIHealth:GUIComponent
    {
        public GUIHealth()
        {
            Value = 7;
            Offset = new Vector2(2,5);
        }

        public new int Value { get; set; }
        
        public new Rectangle TextureRectangle
        {
            get { return TextureManager.GetRectangle("health" + Value); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, Position +Offset, TextureRectangle, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.91f);
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
