using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    public interface IFocus
    {
        Vector2 Position { get; set; }
    }
    class Camera2D
    {
        private float zoom;
        private Vector2 position;
        private float rotation;
        public float Zoom { get { return zoom; } set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public float Rotation { get { return rotation; } set { rotation = value; } }
        public IFocus Focus { get; set; }
        private Point viewportSize;

        public Camera2D(Point viewps)
        {
            viewportSize = viewps;
        }
        public void Init()
        {
            Zoom = 1f;
            Rotation = 0f;
        }
        public Matrix TransformationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * 
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(zoom) * 
                    Matrix.CreateTranslation(new Vector3(viewportSize.X * 0.5f, viewportSize.Y * 0.5f, 0f));

            }
        }
        public Matrix WeirdMatrix
        {
            get {
                return Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                  Matrix.CreateRotationX(rotation) * Matrix.CreateScale(zoom) *
                  Matrix.CreateTranslation(new Vector3(viewportSize.X * 0.5f, viewportSize.Y * 0.5f, 0f));
            }
        }
        public void Update(GameTime gameTime)
        {
            if (Focus != null)
                position = new Vector2(Focus.Position.X - 50,Focus.Position.Y -25);

            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
                //rotation += 0.002f;
            if (Keyboard.GetState().IsKeyDown(Keys.G))
                Zoom -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.H))
                Zoom += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                Zoom = 1f;
                rotation = 0;
            }
            
        }
    }
}
