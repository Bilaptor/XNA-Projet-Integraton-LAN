using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;

namespace XnaGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;
            config.MaximumConnections = 2;
            float XInput = 0;
            float YInput = 0;
            float ZInput = 0;



            // create and start server
            NetServer server = new NetServer(config);
            server.Start();

            // schedule initial sending of position updates
            double nextSendUpdates = NetTime.Now;

            // run until escape is pressed
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            //
                            // Server received a discovery request from a client; send a discovery response (with no extra data attached)
                            //
                            server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //
                            // Just print diagnostic messages to console
                            //
                            Console.WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                //
                                // A new player just connected!
                                //
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");

                                // randomize his position and store in connection tag
                                msg.SenderConnection.Tag = new int[] {
                                    NetRandom.Instance.Next(10, 100),
                                    NetRandom.Instance.Next(10, 100)
                                };
                            }

                            break;
                        case NetIncomingMessageType.Data:
                            bool hasBeenRead = false;
                            if(!hasBeenRead)
                            {
                                if (msg.ReadByte() == (byte)PacketTypes.POSITIONJEU2D)
                                {
                                    int xInput2D = msg.ReadInt32();
                                    int yInput2D = msg.ReadInt32();

                                    int[] pos = msg.SenderConnection.Tag as int[];
                                    
                                    pos[0] += xInput2D;
                                    pos[1] += yInput2D;
                                    hasBeenRead = true;
                                }
                            }
                            if(!hasBeenRead)
                            {
                                if (msg.ReadByte() == (byte)PacketTypes.POSITION)
                                {
                                    XInput = msg.ReadInt32();
                                    YInput = msg.ReadInt32();
                                    ZInput = msg.ReadInt32();
                                    hasBeenRead = true;
                                }
                            }
                            break;
                        default:
                            // Should not happen and if happens, don't care
                            Console.WriteLine(msg.ReadString() + " Strange message");
                            break;
                    }

                    //
                    // send position updates 60 times per second
                    //
                    double now = NetTime.Now;
                    if (now > nextSendUpdates)
                    {
                        // Yes, it's time to send position updates

                        // for each player...
                        foreach (NetConnection player in server.Connections)
                        {
                            // ... send information about every other player (actually including self)
                            foreach (NetConnection otherPlayer in server.Connections)
                            {
                                NetOutgoingMessage omPosition = server.CreateMessage();
                                omPosition.Write((byte)PacketTypes.POSITION);

                                omPosition.Write(XInput);
                                omPosition.Write(YInput);
                                omPosition.Write(ZInput);
                                server.SendMessage(omPosition, player, NetDeliveryMethod.ReliableOrdered);

                                // send position update about 'otherPlayer' to 'player'
                                NetOutgoingMessage om = server.CreateMessage();

                                om.Write((byte)PacketTypes.POSITIONJEU2D);


                                // write who this position is for
                                om.Write(otherPlayer.RemoteUniqueIdentifier);

                                if (otherPlayer.Tag == null)
                                {
                                    otherPlayer.Tag = new int[2];
                                }

                                int[] pos = otherPlayer.Tag as int[];

                                om.Write(pos[0]);
                                om.Write(pos[1]);

                                // send message
                                server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered);
                            }
                        }

                        // schedule next update
                        nextSendUpdates += (1.0 / 60.0);
                    }
                }

                // sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            server.Shutdown("app exiting");
        }
    }
    enum PacketTypes
    {
        LOGIN,
        POSITION,
        POSITIONJEU2D
    }
}
