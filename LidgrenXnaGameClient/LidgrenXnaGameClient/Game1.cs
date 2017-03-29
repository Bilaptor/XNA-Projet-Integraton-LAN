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
        float Temps�coul�DepuisMAJ { get; set; }
        int CptFrames { get; set; }

        Texture2D[] textures;
        Dictionary<long, Vector2> Positions = new Dictionary<long, Vector2>();
        NetClient client;
        
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        Cam�ra Cam�raJeu { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeMod�les { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();
        }

        protected override void Initialize()
        {
            Vector3 positionCam�ra = new Vector3(1, 1, 10);
            Vector3 positionTuileDragon = new Vector3(-2, -2, -10);
            Vector3 positionTuileChartreuse = new Vector3(-2, -2, -2);
            Vector3 positionTuileDrapeau = new Vector3(-2, -2, -20);

            client.DiscoverLocalPeers(14242);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeMod�les = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);

            Components.Add(GestionInput);
            Components.Add(new Arri�rePlanD�roulant(this, "murderoche", INTERVALLE_UPDATE));
            Cam�raJeu = new Cam�raSubjective(this, positionCam�ra, positionTuileDragon, Vector3.Up, INTERVALLE_UPDATE);
            Components.Add(Cam�raJeu);
            
            Components.Add(new Afficheur3D(this));
            Components.Add(new AfficheurFPS(this, "Arial", Color.Tomato, INTERVALLE_CALCUL_FPS));
            Components.Add(new TuileColor�e(this, 1f, Vector3.Zero, positionTuileChartreuse, new Vector2(2, 2), Color.Gold, INTERVALLE_UPDATE));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, positionTuileDrapeau, new Vector2(6, 4), "DrapeauQu�bec", INTERVALLE_UPDATE));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, positionTuileDragon, new Vector2(2, 2), "Dragon", INTERVALLE_UPDATE));
            //Components.Add(new AfficheurFPS(this, "Arial", Color.Tomato, INTERVALLE_CALCUL_FPS));


            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);

            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeMod�les);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Cam�ra), Cam�raJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
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
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ++CptFrames;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ >= INTERVALLE_UPDATE)
            {
                Temps�coul�DepuisMAJ = 0;
                
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

            GestionSprites.Begin();
            base.Draw(gameTime);
            GestionSprites.End();

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
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Shutdown("bye");

            base.OnExiting(sender, args);
        }
    }
}
