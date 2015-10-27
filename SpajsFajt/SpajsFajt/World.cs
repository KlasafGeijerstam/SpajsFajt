using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class World
    {
        private Dictionary<int, GameObject> gameObjects = new Dictionary<int, GameObject>();
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private Background background;
        public int RequestedProjectiles { get; set; }
        public int LocalPlayerID { get; set; }
        private float lastProjectile = 0;

        public Dictionary<int, GameObject> GameObjects
        {
            get { return gameObjects; }
        }
        public Dictionary<int,Player> Players
        {
            get { return players; }
        }
        public Vector2 LocalPosition
        {
            get
            {
                if (LocalPlayerID != -1)
                    return gameObjects[LocalPlayerID].Position;
                else return Vector2.Zero;
            }

        }
        public World()
        {
            LocalPlayerID = -1;
            
        }
        public void Init()
        {
            background = new Background(new Vector2(0, 0));
        }
        public void AddObject(GameObject g)
        {
            gameObjects.Add(g.ID, g);
            
        }
        public void AddPlayer(Player p)
        {
            players.Add(p.ID, p);
            gameObjects.Add(p.ID, p);
        }
        public void Update(GameTime gameTime)
        {
            foreach (var o in gameObjects.Values)
            {
                o.Update(gameTime);
            }
            lastProjectile -= gameTime.ElapsedGameTime.Milliseconds;
            if (LocalPlayerID != -1)
            {
                var p = (Player)gameObjects[LocalPlayerID];
                p.Input(gameTime);

                if (Keyboard.GetState().IsKeyDown(Keys.E) && lastProjectile <= 0)
                {
                    RequestedProjectiles++;
                    lastProjectile = 300;
                } 

                if (p.Position.Y < 400)
                    p.Position = new Vector2(p.Position.X, 400);
                else if (p.Position.Y > 1500)
                    p.Position = new Vector2(p.Position.X, 1500);
                if (p.Position.X < 450)
                    p.Position = new Vector2(450, p.Position.Y);
                else if (p.Position.X > 1550)
                    p.Position = new Vector2(1550, p.Position.Y);
            }
            
            
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var o in gameObjects.Values)
            {
                //spriteBatch.DrawString(TextureManager.GameFont, o.Position.ToString(), o.Position, Color.White);
                o.Draw(spriteBatch);
                
            }
            background.Draw(spriteBatch);
            
        }
        public GameObject GetObject(int id)
        {
            return gameObjects[id];
        }
        
        internal void DoUpdate(int id, Vector2 v, float r,float vel,int type = 1)
        {
            if (gameObjects.ContainsKey(id))
            {
                gameObjects[id].Rotation = r;
                gameObjects[id].Position = v;
                gameObjects[id].Velocity = vel;
            }
            else
            {
                if (type == 1)
                    gameObjects.Add(id, new Player(id) { Rotation = r, Position = v, Velocity = vel });
                else if (type == 2)
                    gameObjects.Add(id, new Projectile(id, r, v));
            }
        }

    }

}
