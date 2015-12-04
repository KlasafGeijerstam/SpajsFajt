using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SpajsFajt
{
    
    class GameServer
    {
        private NetServer netServer;
        private NetIncomingMessage netIn;
        private List<GameObject> gameObjects = new List<GameObject>();
        private float nextUpdate = 300;
        private World world = new World();
        private float powerTimer = 0;
        private float boostTimer = 0;
        private int maxEnemies = 20;
        private int enemyCount = 0;
        private float timeSinceLastEnemy = 0;
        private float timeUNE = 1000;
        private Random rnd = new Random();

        public GameServer(int port)
        {
            //Configuration
            var npc = new NetPeerConfiguration("SpajsFajt");
            npc.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            npc.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            npc.Port = port;
            netServer = new NetServer(npc);
            netServer.Start();
            
        }

        public void Update(GameTime gameTime)
        {
            //Loop trough all incoming messages
            while ((netIn = netServer.ReadMessage()) != null)
            {
                switch (netIn.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        netServer.SendDiscoveryResponse(netServer.CreateMessage("hej"), netIn.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.Data:
                        switch ((GameMessageType)netIn.ReadInt32())
                        {
                            case GameMessageType.ClientID:
                                break;
                            case GameMessageType.ClientReady:
                                var i = NextID();
                                world.AddPlayer(new Player(i) { Connection = netIn.SenderConnection });
                                var netOut = netServer.CreateMessage();
                                netOut.Write((int)GameMessageType.ClientID);
                                netOut.Write(i);
                                netServer.SendMessage(netOut, netIn.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                System.Diagnostics.Debug.WriteLine("SERVER: Client" + idCounter.ToString() + " connected");
                                break;
                            case GameMessageType.ClientPosition:
                                i = netIn.ReadInt32();
                                var v = new Vector2(netIn.ReadFloat(), netIn.ReadFloat());
                                var r = netIn.ReadFloat();
                                var vel = netIn.ReadFloat();
                                world.DoUpdate(i, v, r,vel);
                                break;
                            case GameMessageType.ProjectileRequest:
                                i = netIn.ReadInt32();
                                var player = world.Players[i];
                                if (player.PowerLevel >= 10)
                                {
                                    player.PowerLevel -= 10;

                                    var offset = new Vector2((float)Math.Cos(player.Rotation * 20), (float)Math.Sin(player.Rotation * 20));
                                    var velAddition = new Vector2((float)Math.Cos(player.Rotation * player.Velocity));
                                    netOut = netServer.CreateMessage();

                                    player.Modifiers.GetProjectiles(player.Position + offset,player.Rotation).ForEach(x => world.AddObject(x));

                                    netOut.Write((int)GameMessageType.PowerUpdate);
                                    netOut.Write(player.PowerLevel);
                                    netServer.SendMessage(netOut, player.Connection, NetDeliveryMethod.ReliableUnordered); 
                                }
                                break;
                            case GameMessageType.BoostRequest:
                                i = netIn.ReadInt32();
                                world.Players[i].Boosting = netIn.ReadBoolean();
                                break;
                            case GameMessageType.GoldUpdate:
                                world.Players[netIn.ReadInt32()].Gold = netIn.ReadInt32();
                                break;
                            case GameMessageType.ModificationAdded:
                                world.Players[netIn.ReadInt32()].Modifiers.Modify(netIn.ReadInt32());
                                break;
                            default:
                                //Got unknown message type
                                break;
                        }
                        break;
                }
            }
            
            nextUpdate -= gameTime.ElapsedGameTime.Milliseconds;
            powerTimer += gameTime.ElapsedGameTime.Milliseconds;
            boostTimer += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastEnemy += gameTime.ElapsedGameTime.Milliseconds;

            if (timeSinceLastEnemy >= timeUNE && enemyCount < maxEnemies)
            {
                var id = NextID();
                var enemy = new Enemy(id) { Roaming = true };
                world.AddObject(enemy);
                enemyCount++;
                timeSinceLastEnemy = 0;
            }
            if (nextUpdate <= 0)
            {
                //Time to send updates
                var projectiles = world.GameObjects.Values.Where(x => x is Projectile).Select(x => (Projectile)x).ToList();
                var coins = world.GameObjects.Values.Where(x => x is Gold).Select(x => (Gold)x).ToList();

                foreach (var c in world.Players.Values)
                {
                    //Update game objects
                    if(powerTimer >= 1000 && c.PowerLevel < 70)
                    {
                        c.PowerLevel += 10;
                    }
                    if (c.Boosting)
                    {
                        if (boostTimer >= 300)
                            c.PowerLevel -= 10;

                        if (c.PowerLevel <= 0)
                        {
                            c.PowerLevel = 0;
                            c.Boosting = false;
                        }
                    }
                    if (c.LastPowerLevel != c.PowerLevel || c.LastBoostValue != c.Boosting)
                    {
                        var nO = netServer.CreateMessage();
                        nO.Write((int)GameMessageType.PowerUpdate);
                        nO.Write(c.PowerLevel);
                        nO.Write(c.Boosting);
                        netServer.SendMessage(nO, c.Connection, NetDeliveryMethod.ReliableUnordered);
                    }

                    c.LastPowerLevel = c.PowerLevel;

                    foreach (var c2 in world.Players.Values)
                    {
                        if (c != c2)
                        {
                            var netOut = netServer.CreateMessage();
                            netOut.Write((int)GameMessageType.ClientUpdate);
                            netOut.Write(c.ID);
                            netOut.Write(c.Position.X);
                            netOut.Write(c.Position.Y);
                            netOut.Write(c.Rotation);
                            netOut.Write(c.Velocity);
                            netOut.Write(c.Boosting);
                            netServer.SendMessage(netOut, c2.Connection, NetDeliveryMethod.Unreliable);
                        }
                    }
                    c.LastDamageTaken += gameTime.ElapsedGameTime.Milliseconds;
                    

                    if (c.LastDamageTaken > 300)
                        c.LastDamageTaken = 300;

                    if (c.Health == 0 && !c.DeathSent)
                    {
                        var netOut = netServer.CreateMessage();
                        netOut.Write((int)GameMessageType.PlayerDead);
                        netOut.Write(c.ID);
                        netServer.SendToAll(netOut,NetDeliveryMethod.ReliableUnordered);
                        

                        //Drop coins
                        for (int i = 0; i < c.Gold; i++)
                        {
                            var d = NextID();
                            var g = new Gold(c.Position - new Vector2(rnd.Next(-50, 50), rnd.Next(-50, 50)), d);
                            world.GameObjects.Add(d,g);

                            netOut = netServer.CreateMessage();
                            netOut.Write((int)GameMessageType.CoinAdded);
                            netOut.Write(d);
                            netOut.Write(g.Position.X);
                            netOut.Write(g.Position.Y);
                            netServer.SendToAll(netOut, NetDeliveryMethod.ReliableUnordered);

                        }
                        c.Gold = 0;
                        c.Dead = true;
                        c.DeathSent = true;
                    }
                    //Respawn player after 5 seconds
                    if (c.Dead)
                        c.TimeDead += gameTime.ElapsedGameTime.Milliseconds;

                    if (c.TimeDead >= 2500)
                    {
                        c.Respawn();
                        var netOut = netServer.CreateMessage();
                        netOut.Write((int)GameMessageType.PlayerRespawn);
                        netOut.Write(c.ID);
                        netServer.SendToAll(netOut, NetDeliveryMethod.ReliableUnordered);
                    }
                    foreach (var o in projectiles)
                    {
                        
                        var p = o;
                        NetOutgoingMessage netOut;

                        p.UpdateTime(gameTime);
                        //Collision
                        if (p.CollisionRectangle.Intersects(c.CollisionRectangle) && p.SenderID != c.ID && c.LastDamageTaken >= 300 && !c.Dead)
                        {
                            c.LastDamageTaken = 0;
                            c.Health -= 10;
                            p.Dead = true;
                            if (c.Health < 0)
                            {
                                c.Health = 0;
                                goto cont;
                            }
                                
                            netOut = netServer.CreateMessage();
                            netOut.Write((int)GameMessageType.HPUpdate);
                            netOut.Write(c.Health);
                            netServer.SendMessage(netOut, c.Connection, NetDeliveryMethod.ReliableUnordered);
                        }
                        cont:
                        netOut = netServer.CreateMessage();
                        if (p.Dead || p.Position.Y < -1500 || p.Position.Y > 3500 || p.Position.X < -1500 || p.Position.X > 3500 || World.InShop(p.Position))
                        {
                            netOut.Write((int)GameMessageType.ObjectDeleted);
                            netOut.Write(p.ID);
                            netServer.SendMessage(netOut, c.Connection, NetDeliveryMethod.ReliableUnordered);
                            p.Dead = true;
                            continue;
                        }
                        
                        netOut.Write((int)GameMessageType.ObjectUpdate);
                        netOut.Write(p.ID);
                        netOut.Write(p.Position.X);
                        netOut.Write(p.Position.Y);
                        netOut.Write(p.Rotation);
                        netOut.Write(p.Velocity);
                        netOut.Write(2);
                        netServer.SendMessage(netOut, c.Connection, NetDeliveryMethod.Unreliable);
                    }
                    c.LastBoostValue = c.Boosting;


                    //Coins

                    foreach (var coin in coins)
                    {
                        if( ! c.Dead && c.CollisionRectangle.Intersects(coin.CollisionRectangle))
                        {
                            var netOut = netServer.CreateMessage();
                            netOut.Write((int)GameMessageType.CoinPickedUp);
                            netOut.Write(coin.ID);
                            netServer.SendMessage(netOut, c.Connection, NetDeliveryMethod.ReliableUnordered);
                            c.Gold++;

                            foreach (var client in world.Players)
                            {
                                if (client.Value == c)
                                    continue;

                                netOut = netServer.CreateMessage();
                                netOut.Write((int)GameMessageType.ObjectDeleted);
                                netOut.Write(coin.ID);
                                netServer.SendMessage(netOut, client.Value.Connection, NetDeliveryMethod.ReliableUnordered);
                            }

                            coin.Dead = true;
                        }
                    }

                }
                
                //Projectiles

                foreach (var item in projectiles)
                {
                    item.Move();
                    if (item.Dead)
                        world.GameObjects.Remove(item.ID);
                }

                //Enemies

                foreach (var enemy in world.GameObjects.Values.Where(x => x is Enemy).Select(z => (Enemy)z).ToList())
                {
                    NetOutgoingMessage netOut;
                    enemy.Move(gameTime);
                    if (enemy.FireProjectile)
                    {
                        var offset = new Vector2((float)Math.Cos(enemy.Rotation * 20), (float)Math.Sin(enemy.Rotation * 20));
                        world.AddObject(new Projectile(NextID(), enemy.Rotation, enemy.Position + offset) { SenderID = enemy.ID,FiredByAI = true });
                        enemy.Fire();
                    }

                    if (World.InShop(enemy.Position))
                    {
                        enemy.Dead = true;
                        netOut = netServer.CreateMessage();
                        netOut.Write((int)GameMessageType.EnemyDeleted);
                        netOut.Write(enemy.ID);
                        netServer.SendToAll(netOut, NetDeliveryMethod.ReliableUnordered);

                        world.GameObjects.Remove(enemy.ID);
                        enemyCount--;
                    }

                    if (enemy.Roaming)
                    {
                        foreach (var pl in world.Players.Values)
                        {
                            if (!pl.Dead && !World.InShop(pl.Position) && enemy.InRadius(pl.Position))
                            {
                                enemy.Roaming = false;
                                enemy.Target = pl;
                                break;
                            }
                        }
                    }
                    else if (World.InShop(enemy.Target.Position))
                    {
                        enemy.Roaming = true;
                        enemy.Target = null;
                    } 
                    

                    netOut = netServer.CreateMessage();
                    netOut.Write((int)GameMessageType.ObjectUpdate);
                    netOut.Write(enemy.ID);
                    netOut.Write(enemy.Position.X);
                    netOut.Write(enemy.Position.Y);
                    netOut.Write(enemy.Rotation);
                    netOut.Write(enemy.Velocity);
                    netOut.Write(3);
                    netServer.SendToAll(netOut, NetDeliveryMethod.Unreliable);


                    foreach (var proj in projectiles)
                    {
                        if (!enemy.Dead &&  !proj.FiredByAI && proj.CollisionRectangle.Intersects(enemy.CollisionRectangle) && proj.SenderID != enemy.ID && enemy.TimeSinceLastDamage <= 0)
                        {
                            enemy.Health -= 10;
                            enemy.TimeSinceLastDamage = 20;
                            if (enemy.Health <= 0)
                            {
                                enemy.Dead = true;
                                proj.Dead = true;
                                netOut = netServer.CreateMessage();
                                netOut.Write((int)GameMessageType.EnemyDeleted);
                                netOut.Write(enemy.ID);
                                netServer.SendToAll(netOut, NetDeliveryMethod.ReliableUnordered);
                                var id = NextID();
                                world.GameObjects.Add(id, new Gold(enemy.Position, id));
                                netOut = netServer.CreateMessage();

                                //Add coin
                                netOut.Write((int)GameMessageType.CoinAdded);
                                netOut.Write(id);
                                netOut.Write(enemy.Position.X);
                                netOut.Write(enemy.Position.Y);
                                netServer.SendToAll(netOut, NetDeliveryMethod.ReliableUnordered);

                                world.GameObjects.Remove(enemy.ID);
                                enemyCount--;
                                
                            }
                        }
                    }
                    coins.ForEach(x => {
                    if (x.Dead)
                        world.GameObjects.Remove(x.ID);
                    });
                }


                nextUpdate = 20;
            }
            if (powerTimer >= 1000)
            {
                powerTimer = 0;
            }
            if(boostTimer > 300)
            {
                boostTimer = 0;
            }
        }

        internal void ShutDown()
        {
            netServer.Shutdown("exiting");
        }

        private static int idCounter = 0;
        public static int NextID()
        {
            return ++idCounter;
        }

    }

}

