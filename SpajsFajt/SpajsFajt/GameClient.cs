using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GameClient
    {
        private NetClient netClient;
        private NetIncomingMessage netIn;
        private NetConnection connection;
        public NetConnection Connection
        { get { return connection; } set { connection = value; } }
        public IPEndPoint EndPoint { get; set; }
        public int ID { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public IFocus Focus { get { return (Player)world.GetObject(world.LocalPlayerID); } }

        private World world = new World();
        private Vector2 prevPos;
        private float prevRot;
        private float updateFrequency = 1/30, nextSendUpdate = 1/30;

        public GameClient()
        {
            var npcc = new NetPeerConfiguration("SpajsFajt");
            npcc.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            npcc.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            netClient = new NetClient(npcc);
            netClient.Start();
        }
        
        public void Connect(string host,int port)
        {
            if (netClient.ServerConnection == null)
            {
                netClient.DiscoverKnownPeer(host, port);
                world.Init();
            }
            else
                throw new Exception("Client already connected to server!");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            world.Draw(spriteBatch); 
        }
        internal void Update(GameTime gameTime)
        {
            while ((netIn = netClient.ReadMessage()) != null)
            {
                switch (netIn.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        if (netClient.ConnectionStatus == NetConnectionStatus.Connected)
                        {
                            var netOut = netClient.CreateMessage();
                            netOut.Write((int)GameMessageType.ClientReady);
                            netClient.SendMessage(netOut,NetDeliveryMethod.ReliableOrdered);
                        }
                        else if (netClient.ConnectionStatus == NetConnectionStatus.Disconnected)
                        {
                            Game1.ShouldExit = true;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        switch ((GameMessageType)netIn.ReadInt32())
                        {
                            case GameMessageType.ClientID:
                                ID = netIn.ReadInt32();
                                world.AddObject(new Player(ID));
                                world.LocalPlayerID = ID;
                                world.LocalPlayer = (Player)world.GameObjects[ID];
                                Focus.Position = new Vector2(1000, 1000);
                                Game1.Focus = Focus;
                                break;
                            case GameMessageType.ClientUpdate:
                                var id = netIn.ReadInt32();
                                var v = new Vector2(netIn.ReadFloat(), netIn.ReadFloat());
                                var r = netIn.ReadFloat();
                                var vel = netIn.ReadFloat();
                                world.DoUpdate(id, v, r,vel);
                                break;
                            case GameMessageType.ObjectUpdate:
                                id = netIn.ReadInt32();
                                v = new Vector2(netIn.ReadFloat(), netIn.ReadFloat());
                                r = netIn.ReadFloat();
                                vel = netIn.ReadFloat();
                                var type = netIn.ReadInt32();
                                world.DoUpdate(id, v, r, vel, type);
                                break;
                            case GameMessageType.ObjectDeleted:
                                id = netIn.ReadInt32();
                                world.GameObjects.Remove(id);
                                break;
                            case GameMessageType.HPUpdate:
                                world.LocalPlayer.Health = netIn.ReadInt32();
                                break;
                            case GameMessageType.PlayerDead:
                                ((Player)world.GameObjects[netIn.ReadInt32()]).Die();
                                break;
                            case GameMessageType.PlayerRespawn:
                                ((Player)world.GameObjects[netIn.ReadInt32()]).Respawn();
                                break;
                            case GameMessageType.PowerUpdate:
                                world.LocalPlayer.PowerLevel = netIn.ReadInt32();
                                break;
                        }
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        netClient.Connect(netIn.SenderEndPoint);
                        break;
                    
                }
            }
            world.Update(gameTime);
            nextSendUpdate -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSendUpdate <= 0 && world.LocalPlayerID != -1)
            {
                var p = world.LocalPlayer;
                var netOut = netClient.CreateMessage();
                netOut.Write((int)GameMessageType.ClientPosition);
                netOut.Write(p.ID);
                netOut.Write(p.Position.X);
                netOut.Write(p.Position.Y);
                netOut.Write(p.Rotation);
                netOut.Write(p.Velocity);
                netClient.SendMessage(netOut, NetDeliveryMethod.Unreliable);
                nextSendUpdate = updateFrequency;

                if (world.RequestedProjectiles > 0)
                {
                    netOut = netClient.CreateMessage();
                    netOut.Write((int)GameMessageType.ProjectileRequest);
                    netOut.Write(p.ID);
                    netOut.Write(world.RequestedProjectiles);
                    netClient.SendMessage(netOut, NetDeliveryMethod.Unreliable);
                    world.RequestedProjectiles = 0; 
                }
                
            }
            prevPos = Position;
            prevRot = Rotation;
        }

        internal void ShutDown()
        {
            netClient.Shutdown("exiting");
        }
    }
}
