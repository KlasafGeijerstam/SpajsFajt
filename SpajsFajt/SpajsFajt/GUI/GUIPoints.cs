using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GUIPoints: GUIComponent
    {

        private Vector2 textOffset = new Vector2(5, 7);

        public GUIPoints()
        {

            Offset = new Vector2(297, 5);

        }
        public new Rectangle TextureRectangle
        {
            get { return TextureManager.GetRectangle("guiPoints"); }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, Position + Offset, TextureRectangle, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.91f);
            spriteBatch.DrawString(TextureManager.GameFont, Value.ToString(), Position + Offset + textOffset, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.92f);
        }
        public new int Value { get; set; }
    }
}
