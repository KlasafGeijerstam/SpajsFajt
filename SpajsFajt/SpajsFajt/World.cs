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
        private static Random rnd = new Random();
        private Vector2 prevCamPos;

        public Player LocalPlayer
        {
            get { return localPlayer; }
            set { localPlayer = value; }
        }

        public static Vector2 GetRandomBorderPosition()
        {
            var vector = new Vector2();
            if (rnd.Next(0, 2) == 1)
                vector.Y = -1500;
            else
                vector.Y = 3500;

            if (rnd.Next(0, 2) == 1)
                vector.X = -1500;
            else
                vector.X = 3500;

            return vector;
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
        int c = 300;
        public void Update(GameTime gameTime)
        {
            foreach (var o in gameObjects.Values.ToList())
            {
                o.Update(gameTime);
                if (o is Enemy)
                {
                    var e = (Enemy)o;
                    if (e.ProperDead)
                        gameObjects.Remove(e.ID);

                }

                if (o is Gold && o.Dead)
                {
                   var e = (Gold)o;
                    gameObjects.Remove(e.ID);

                    localPlayer.Gold++;
                }
                    
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
                if (Keyboard.GetState().IsKeyDown(Keys.O))
                    gameObjects.Add(c, new Gold(Game1.CameraPosition, c++) { Collect = false });

                if (localPlayer.Position.Y < -1500)
                    localPlayer.Position = new Vector2(localPlayer.Position.X, -1500);
                else if (localPlayer.Position.Y > 3500)
                    localPlayer.Position = new Vector2(localPlayer.Position.X, 3500);
                if (localPlayer.Position.X < -1500)
                    localPlayer.Position = new Vector2(-1500, localPlayer.Position.Y);
                else if (localPlayer.Position.X > 3500)
                    localPlayer.Position = new Vector2(3500, localPlayer.Position.Y);
            }
            if (Game1.Focus != null)
            {
                gui.Position = Game1.CameraPosition - new Vector2(Game1.ViewportSize.X / 2, Game1.ViewportSize.Y / 2);
                gui.HealthGUI.Value = LocalPlayer.Health / 10;
                gui.PowerGUI.Value = LocalPlayer.PowerLevel / 10;
                gui.GoldGUI.Value = LocalPlayer.Gold;
                prevCamPos = Game1.CameraPosition - prevCamPos;
                Gold.Offset = prevCamPos;
                Gold.Target = gui.GoldGUI.Position + gui.GoldGUI.Offset;

                prevCamPos = Game1.CameraPosition;
            }
            gui.Update();
            
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
                else if (type == 3)
                    gameObjects.Add(id, new Enemy(id) { Rotation = r, Velocity = vel, Position = v });
            }
        }


    }

}
