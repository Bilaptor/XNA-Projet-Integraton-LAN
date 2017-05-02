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
            long UniqueIdentifier = 0;


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
                        #region Recoit message
                        //Console.WriteLine("Reading message " + msg.ToString() + " from " + msg.SenderConnection.RemoteUniqueIdentifier);

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

                                //Lorsqu'un joueur se connecte:
                                if (status == NetConnectionStatus.Connected)
                                {
                                    Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");

                                    //On lui envoie une confirmation + son numero unique.
                                    foreach (NetConnection player in server.Connections)
                                    {
                                        NetOutgoingMessage omIdentifier = server.CreateMessage();
                                        omIdentifier.Write((byte)PacketTypes.CONNECTIONNUMBER);
                                        omIdentifier.Write(player.RemoteUniqueIdentifier);
                                        server.SendMessage(omIdentifier, player, NetDeliveryMethod.ReliableOrdered);
                                    }
                                }
                                break;

                            case NetIncomingMessageType.Data:
                                bool hasBeenRead = false;
                                byte TypeMessage = msg.ReadByte();

                                if (!hasBeenRead)
                                    switch ((PacketTypes)TypeMessage)
                                    {
                                        case PacketTypes.POSITION:
                                            Console.WriteLine(msg.SenderConnection.RemoteUniqueIdentifier.ToString());
                                            UniqueIdentifier = msg.SenderConnection.RemoteUniqueIdentifier;
                                            XInput = msg.ReadFloat();
                                            YInput = msg.ReadFloat();
                                            ZInput = msg.ReadFloat();
                                            Console.WriteLine(XInput + " " + YInput + " " + ZInput);
                                            hasBeenRead = true;
                                            break;
                                    }
                                break;

                            default:
                                // Should not happen and if happens, don't care
                                Console.WriteLine(msg.ReadString() + " Strange message server");
                                break;
                        }

                        #endregion recoit message

                        #region Envoie de message
                        //
                        // send position updates 30 times per second
                        //
                        double now = NetTime.Now;
                        if (now > nextSendUpdates && msg.SenderConnection != null)
                        {
                            // for each player...
                            //foreach (NetConnection player in server.Connections)
                            //{
                                // ... send information about every other player (actually including self)
                                foreach (NetConnection player in server.Connections.Where(x => x.RemoteUniqueIdentifier.ToString() != UniqueIdentifier.ToString()))
                                {
                                    Console.WriteLine(msg.SenderConnection.RemoteUniqueIdentifier.ToString());
                                    NetOutgoingMessage omPosition = server.CreateMessage();
                                    omPosition.Write((byte)PacketTypes.POSITION);
                                    omPosition.Write(UniqueIdentifier);
                                    omPosition.Write((float)XInput);
                                    omPosition.Write((float)YInput);
                                    omPosition.Write((float)ZInput);
                                    server.SendMessage(omPosition, player, NetDeliveryMethod.ReliableOrdered);
                                }
                            //}
                            #endregion

                            // schedule next update
                            nextSendUpdates += (1.0 / 30.0);
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
