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
        private static Shop shop;
        private static bool shopSet;
        private Rectangle mouseTextureRectangle;

        public Player LocalPlayer
        {
            get { return localPlayer; }
            set { localPlayer = value; }
        }

        public static Vector2 GetRandomBorderPosition()
        {
            var vector = new Vector2();
            switch (rnd.Next(0,5))
            {
                case 0:
                    //left
                    vector.X = -1500;
                    vector.Y = rnd.Next(-1500, 3500);
                    break;
                case 1:
                    //top
                    vector.Y = -1500;
                    vector.X = rnd.Next(-1500, 3500);
                    break;
                case 2:
                    //right
                    vector.X = 3500;
                    vector.Y = rnd.Next(-1500, 3500);
                    break;
                case 3:
                    //bot
                    vector.Y = 3500;
                    vector.X = rnd.Next(-1500, 3500);
                    break;
            }

            return vector;
        }

        public static bool InShop(Vector2 v)
        {
            if (shop != null)
            {
                return shop.InShop(v);
            }
            else return false;
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
            shop = new Shop();
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

        internal static void HideShop()
        {
        }

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

                if (Keyboard.GetState().IsKeyDown(Keys.E) && lastProjectile <= 0 && !shop.InShop(localPlayer.Position))
                {
                    RequestedProjectiles++;
                    lastProjectile = 300;
                }

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

        internal static void ShowShop()
        {
            Mouse.SetPosition(320, 200);
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
            shop.Draw(spriteBatch);
            if (localPlayer != null && localPlayer.Shopping)
            {
                var pos = Mouse.GetState().Position.ToVector2();
                pos += Game1.CameraPosition;
                pos -= new Vector2(320, 200);
                spriteBatch.Draw(TextureManager.SpriteSheet,pos, TextureManager.GetRectangle("shopGreen"), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
            }
            if(localPlayer != null)
                spriteBatch.DrawString(TextureManager.GameFont, shop.InShop(localPlayer.Position).ToString(), shop.Position, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.7f);
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
