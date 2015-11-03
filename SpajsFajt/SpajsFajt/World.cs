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
        private Player localPlayer;
        private float lastProjectile = 0;
        private GUI gui = new GUI();
        private Vector2 cameraOffset = new Vector2(400, 300);
        private Enemy enemy;
        public Player LocalPlayer
        {
            get { return localPlayer; }
            set { localPlayer = value; }
        }

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
                    return localPlayer.Position;
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
            enemy = new Enemy(GameServer.NextID());
            enemy.Position = new Vector2(700, 700);
            enemy.Target = LocalPlayer;
            gameObjects.Add(enemy.ID, enemy);
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
                localPlayer.Input(gameTime);

                if (Keyboard.GetState().IsKeyDown(Keys.E) && lastProjectile <= 0)
                {
                    RequestedProjectiles++;
                    lastProjectile = 300;
                } 

                if (localPlayer.Position.Y < 400)
                    localPlayer.Position = new Vector2(localPlayer.Position.X, 400);
                else if (localPlayer.Position.Y > 1500)
                    localPlayer.Position = new Vector2(localPlayer.Position.X, 1500);
                if (localPlayer.Position.X < 450)
                    localPlayer.Position = new Vector2(450, localPlayer.Position.Y);
                else if (localPlayer.Position.X > 1550)
                    localPlayer.Position = new Vector2(1550, localPlayer.Position.Y);
            }
            if (Game1.Focus != null)
            {
                gui.Position = Game1.CameraPosition - new Vector2(Game1.ViewportSize.X / 2, Game1.ViewportSize.Y / 2);
                gui.HealthGUI.Value = LocalPlayer.Health / 10;
                gui.PowerGUI.Value = LocalPlayer.PowerLevel / 10;
            }
            gui.Update();
            enemy.Target = LocalPlayer;
            enemy.Move(gameTime);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var o in gameObjects.Values)
            {
                //spriteBatch.DrawString(TextureManager.GameFont, o.Position.ToString(), o.Position, Color.White);
                o.Draw(spriteBatch);
            }
            background.Draw(spriteBatch);
            gui.Draw(spriteBatch);
            enemy.Draw(spriteBatch);
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
