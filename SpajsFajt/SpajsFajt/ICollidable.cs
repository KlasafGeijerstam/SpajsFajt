using Microsoft.Xna.Framework;

namespace SpajsFajt
{
    interface ICollidable
    {
        Rectangle CollisionRectangle { get; set; }
        void Hit();
    }
}
