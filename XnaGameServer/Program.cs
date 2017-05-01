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
            config.MaximumConnections = 200;
            float XInput = 0;
            float YInput = 0;
            float ZInput = 0;
            int UniqueIdentifier = 0;


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
                    if (msg.SenderConnection != null)
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
                                    ++UniqueIdentifier;
                                    Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");

                                    foreach (NetConnection player in server.Connections)
                                    {
                                        NetOutgoingMessage omIdentifier = server.CreateMessage();
                                        omIdentifier.Write((byte)PacketTypes.CONNECTIONNUMBER);
                                        omIdentifier.Write(UniqueIdentifier);
                                        server.SendMessage(omIdentifier, player, NetDeliveryMethod.ReliableOrdered);
                                    }

                                    // randomize his position and store in connection tag
                                    msg.SenderConnection.Tag = new float[]
                                    {
                                    NetRandom.Instance.Next(10, 100),
                                    NetRandom.Instance.Next(10, 100),
                                    NetRandom.Instance.Next(10, 100)
                                    };
                                }

                                break;
                            case NetIncomingMessageType.Data:
                                bool hasBeenRead = false;
                                if (!hasBeenRead && msg.ReadByte() == (byte)PacketTypes.POSITIONJEU2D)
                                {

                                    // server sent a position update
                                    //long who = msg.ReadInt64();
                                    float xInput2D = msg.ReadInt32();
                                    float yInput2D = msg.ReadInt32();

                                    float[] pos = msg.SenderConnection.Tag as float[];

                                    //fancy movement logic goes here; we just append input to position
                                    pos[0] += xInput2D;
                                    pos[1] += yInput2D;
                                }
                                if (!hasBeenRead && msg.ReadByte() == (byte)PacketTypes.POSITION)
                                {
                                    Console.WriteLine(msg.SenderConnection.RemoteUniqueIdentifier.ToString());
                                    XInput = msg.ReadInt32();
                                    YInput = msg.ReadInt32();
                                    ZInput = msg.ReadInt32();
                                    hasBeenRead = true;
                                }
                                break;
                            default:
                                // Should not happen and if happens, don't care
                                Console.WriteLine(msg.ReadString() + " Strange message server");
                                break;
                        }

                        //
                        // send position updates 30 times per second
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
                                    Console.WriteLine(msg.SenderConnection.RemoteUniqueIdentifier.ToString());
                                    if (otherPlayer.RemoteUniqueIdentifier != msg.SenderConnection.RemoteUniqueIdentifier)
                                    {
                                        NetOutgoingMessage omPosition = server.CreateMessage();
                                        omPosition.Write((byte)PacketTypes.POSITION);
                                        //.Write(UniqueIdentifier);
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
                                            otherPlayer.Tag = new float[2];
                                        }

                                        float[] pos = otherPlayer.Tag as float[];

                                        om.Write(pos[0]);
                                        om.Write(pos[1]);

                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered);
                                    }
                                }
                            }

                            // schedule next update
                            nextSendUpdates += (1.0 / 1.0);
                        }
                    }

                    // sleep to allow other processes to run smoothly
                    Thread.Sleep(1);
                }
            }

            server.Shutdown("app exiting");
        }
    }
    enum PacketTypes
    {
        CONNECTIONNUMBER,
        POSITION,
        POSITIONJEU2D
    }
}
