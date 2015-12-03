using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GUIGold:GUIComponent
    {
       
        private Vector2 textOffset = new Vector2(30, 7);
        
        public GUIGold()
        {
            
            Offset = new Vector2(215, 5);
            
        }
        public new Rectangle TextureRectangle
        {
            get { return TextureManager.GetRectangle("guiGold"); }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, Position + Offset, TextureRectangle, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.71f);
            spriteBatch.DrawString(TextureManager.GameFont, Value.ToString() ,Position + Offset + textOffset,Color.Gold,0f,Vector2.Zero,1f,SpriteEffects.None,0.71f);
        }
        public new int Value { get; set; }
    }
}
