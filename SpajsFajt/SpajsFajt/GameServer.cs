using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;


namespace SpajsFajt
{
    enum GameMessageType { ClientID = 0, ClientReady = 1, ClientUpdate = 2, ClientPosition = 3, ProjectileRequest = 4, ObjectUpdate = 5, ObjectDeleted = 6, HPUpdate = 7 };
    class GameServer
    {
        private NetServer netServer;
        private NetIncomingMessage netIn;
        private List<GameObject> gameObjects = new List<GameObject>();
        private float nextUpdate = 300;
        private World world = new World();

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
                                var offset = new Vector2((float)Math.Cos(world.GameObjects[i].Rotation * 20), (float)Math.Sin(world.GameObjects[i].Rotation * 20));
                                world.AddObject(new Projectile(NextID(), world.GameObjects[i].Rotation, world.GameObjects[i].Position + offset) { SenderID = i });
                                System.Diagnostics.Debug.WriteLine("Spawned projectile at: " + world.GameObjects[i].Position.ToString());
                                break;
                            default:
                                //Got unknown message type
                                break;
                        }
                        break;
                }
            }

            

            nextUpdate -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextUpdate <= 0)
            {
                //Time to send updates
                
                foreach (var c in world.Players.Values)
                {
                    //Update game objects
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
                    foreach (var o in world.GameObjects.Values.Where(x => x is Projectile))
                    {
                        
                        var p = (Projectile)o;
                        NetOutgoingMessage netOut;
                        //Collision
                        if (p.CollisionRectangle.Intersects(c.CollisionRectangle) && p.SenderID != c.ID && c.LastDamageTaken >= 300)
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

                nextUpdate = 1/30;
            }
        }

        private static int idCounter = 0;
        public static int NextID()
        {
            return ++idCounter;
        }

    }

}

