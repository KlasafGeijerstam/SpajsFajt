using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class GUI
    {
        public GUIHealth HealthGUI { get; private set; }
        public GUIPower PowerGUI { get; private set; }
        public Vector2 Position { get; set; }

        public GUI()
        {
            HealthGUI = new GUIHealth();
            PowerGUI = new GUIPower();
        }

        public void Update()
        {
            HealthGUI.Position = Position;
            PowerGUI.Position = Position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            HealthGUI.Draw(spriteBatch);
            PowerGUI.Draw(spriteBatch);
        }
    }
}
