using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace XnaGameClient
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_UPDATE = 1f / 60f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch spriteBatch;
        float Temps…coulÈDepuisMAJ { get; set; }
        int CptFrames { get; set; }

        Texture2D[] textures;
        Dictionary<long, Vector2> Positions = new Dictionary<long, Vector2>();
        NetClient client;
        
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";

            NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();
        }

        protected override void Initialize()
        {
            client.DiscoverLocalPeers(14242);

            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "texture");
            Components.Add(GestionInput);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Components.Add(new AfficheurFPS(this, "Arial", Color.Tomato, INTERVALLE_CALCUL_FPS));
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(SpriteBatch), GestionSprites);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            textures = new Texture2D[5];
            for (int i = 0; i < 5; i++)
                textures[i] = Content.Load<Texture2D>("c" + (i + 1));
        }

        protected override void Update(GameTime gameTime)
        {
            float temps…coulÈ = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ++CptFrames;
            Temps…coulÈDepuisMAJ += temps…coulÈ;
            if (Temps…coulÈDepuisMAJ >= INTERVALLE_UPDATE)
            {
                Temps…coulÈDepuisMAJ = 0;
                
                //
                // Collect input
                //
                int xinput = 0;
                int yinput = 0;
                KeyboardState keyState = Keyboard.GetState();

                // exit game if escape or Back is pressed
                if (keyState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // use arrows or dpad to move avatar
                if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyState.IsKeyDown(Keys.Left))
                    xinput = -1;
                if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyState.IsKeyDown(Keys.Right))
                    xinput = 1;
                if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || keyState.IsKeyDown(Keys.Up))
                    yinput = -1;
                if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || keyState.IsKeyDown(Keys.Down))
                    yinput = 1;

                if (xinput != 0 || yinput != 0)
                {
                    //
                    // If there's input; send it to server
                    //
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write(xinput); // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity
                    om.Write(yinput);
                    client.SendMessage(om, NetDeliveryMethod.Unreliable);
                }

                // read messages
                NetIncomingMessage msg;
                while ((msg = client.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            // just connect to first server discovered
                            client.Connect(msg.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.Data:
                            // server sent a position update
                            long who = msg.ReadInt64();
                            int x = msg.ReadInt32();
                            int y = msg.ReadInt32();
                            Positions[who] = new Vector2(x, y);
                            break;
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

            // draw all players
            foreach (var kvp in Positions)
            {
                // use player unique identifier to choose an image
                int num = Math.Abs((int)kvp.Key) % textures.Length;

                // draw player
                spriteBatch.Draw(textures[num], kvp.Value, Color.White);
            }

            spriteBatch.End();
            GestionSprites.Begin();
            base.Draw(gameTime);
            GestionSprites.End();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Shutdown("bye");

            base.OnExiting(sender, args);
        }
    }
}
