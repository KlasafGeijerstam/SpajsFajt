using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;


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

                                if (world.Players[i].PowerLevel >= 10)
                                {
                                    world.Players[i].PowerLevel -= 10;

                                    var offset = new Vector2((float)Math.Cos(world.GameObjects[i].Rotation * 20), (float)Math.Sin(world.GameObjects[i].Rotation * 20));
                                    world.AddObject(new Projectile(NextID(), world.GameObjects[i].Rotation, world.GameObjects[i].Position + offset) { SenderID = i });
                                    netOut = netServer.CreateMessage();
                                    
                                    netOut.Write((int)GameMessageType.PowerUpdate);
                                    netOut.Write(world.Players[i].PowerLevel);
                                    netServer.SendMessage(netOut, ((Player)world.GameObjects[i]).Connection, NetDeliveryMethod.ReliableUnordered); 
                                }
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
            
            if (nextUpdate <= 0)
            {
                //Time to send updates
                
                foreach (var c in world.Players.Values)
                {
                    //Update game objects
                    if(powerTimer >= 1000 && c.PowerLevel < 70)
                    {
                        c.PowerLevel += 10;
                        var nO = netServer.CreateMessage();
                        nO.Write((int)GameMessageType.PowerUpdate);
                        nO.Write(c.PowerLevel);
                        netServer.SendMessage(nO, c.Connection, NetDeliveryMethod.ReliableUnordered);
                    }
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
                        c.Dead = true;
                        c.DeathSent = true;
                    }
                    //Respawn player after 5 seconds
                    if (c.Dead)
                        c.TimeDead += gameTime.ElapsedGameTime.Milliseconds;
                    if (c.TimeDead >= 5000)
                    {
                        c.Respawn();
                        var netOut = netServer.CreateMessage();
                        netOut.Write((int)GameMessageType.PlayerRespawn);
                        netOut.Write(c.ID);
                        netServer.SendToAll(netOut, NetDeliveryMethod.ReliableUnordered);
                    }
                    foreach (var o in world.GameObjects.Values.Where(x => x is Projectile))
                    {
                        
                        var p = (Projectile)o;
                        NetOutgoingMessage netOut;
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
                        if (p.Dead || p.Position.Y < 400 || p.Position.Y > 1500 || p.Position.X < 450 || p.Position.X > 1550)
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
                }
                world.GameObjects.Values.Where(x => x is Projectile).ToList().ForEach(x => ((Projectile)x).Move());
                world.GameObjects.Except(world.GameObjects.Where(x => x.Value is Projectile && ((Projectile)x.Value).Dead));

                nextUpdate = 10;
            }
            if (powerTimer >= 1000)
                powerTimer = 0;
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

