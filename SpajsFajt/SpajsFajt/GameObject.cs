using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GameObject : IDrawable, ICollidable,IFocus
    {
        protected Rectangle collisionRectangle;
        protected string rectangleName = "error";
        protected Rectangle textureRectangle;
        protected Vector2 position;
        protected float rotation = 0f;
        protected Vector2 origin;
        protected float velocity = 0f;
        
        public float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public int ID { get; set; }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Rectangle CollisionRectangle
        {
            get{return collisionRectangle;}

            set{collisionRectangle = value; }
        }

        public Rectangle TextureRectangle
        { get { return textureRectangle; } }

        public string RectangleName
        {
            get{return rectangleName;}
        }


        public GameObject(string rectName,int id)
        {
            rectangleName = rectName;
            textureRectangle = TextureManager.GetRectangle(rectangleName);
            collisionRectangle = new Rectangle((int)Position.X,(int)position.Y,textureRectangle.Width,textureRectangle.Height);
            ID = id;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, position, textureRectangle, Color.White);
        }

        public virtual void Hit()
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
            collisionRectangle.X = (int)position.X;
            collisionRectangle.Y = (int)position.Y;
        }
    }
}
