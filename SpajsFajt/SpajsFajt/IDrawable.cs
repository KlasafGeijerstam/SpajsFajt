using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
        string RectangleName { get; }
        Rectangle TextureRectangle { get; }
        Vector2 Position { get; }
    }
}
