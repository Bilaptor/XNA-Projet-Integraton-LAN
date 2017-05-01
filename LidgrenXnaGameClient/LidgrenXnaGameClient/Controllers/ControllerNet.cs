using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace XnaGameClient
{
    class ControllerNet : Microsoft.Xna.Framework.GameComponent, IController
    {
        Vector3 Direction { get; set; }
        Vector3 Position { get; set; }
        int Identifiant;
        //NetClient client;

        public Vector3 GetPosition()
        {
            return Position;
        }

        public Vector3 GetDirection()
        {
            return Vector3.Zero;
        }

        public Vector3 GetDirectionVu()
        {
            return Vector3.Zero;
        }

        public ControllerNet(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            Position = new Vector3(0, 0, 0);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void LireMessages()
        {
            // read messages
            NetIncomingMessage incomingMessage;
            while ((incomingMessage = (Game as Game1).client.ReadMessage()) != null)
            {
                switch (incomingMessage.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:

                        (Game as Game1).client.Connect(incomingMessage.SenderEndPoint);
                        break;

                    case NetIncomingMessageType.Data:
                        bool hasBeenRead = false;

                        if (!hasBeenRead && incomingMessage.ReadByte() == (byte)PacketTypes.CONNECTIONNUMBER)
                        {
                            Identifiant = incomingMessage.ReadInt32();
                            hasBeenRead = true;
                        }
                        if (!hasBeenRead && incomingMessage.ReadByte() == (byte)PacketTypes.POSITIONJEU2D)
                        {
                            if (incomingMessage.ReadInt32() != Identifiant)
                            {
                                long who = incomingMessage.ReadInt64();
                                int x = incomingMessage.ReadInt32();
                                int y = incomingMessage.ReadInt32();
                                //Positions[who] = new Vector2(x, y);
                                hasBeenRead = true;
                            }
                        }
                        if (!hasBeenRead && incomingMessage.ReadByte() == (byte)PacketTypes.POSITION)
                        {
                            if (incomingMessage.ReadInt32() != Identifiant)
                            {


                                float positionX = incomingMessage.ReadInt32();
                                float positionY = incomingMessage.ReadInt32();
                                float positionZ = incomingMessage.ReadInt32();

                                Position = new Vector3(positionX, positionY, positionZ);

                                //Adversaire.DonnerPosition(new Vector3(positionX, positionY, positionZ));
                                hasBeenRead = true;
                            }
                        }
                        break;

                    default:
                        // Should not happen and if happens, don't care
                        Console.WriteLine(incomingMessage.ReadString() + " Strange message");
                        break;

                }
            }


        }
    }
}
