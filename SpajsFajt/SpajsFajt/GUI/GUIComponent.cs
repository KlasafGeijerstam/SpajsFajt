using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GUIComponent:IDrawable
    {
        public object Value { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; }

        public string RectangleName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Rectangle TextureRectangle
        {
            get
            {
                return new Rectangle();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
           
        }
    }
}
