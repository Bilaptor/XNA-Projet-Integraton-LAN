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
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_UPDATE = 1f / 60f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        float TempsÉcouléDepuisMAJ { get; set; }
        int CptFrames { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch spriteBatch;
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        Caméra CaméraJeu { get; set; }
        
        Vector3[] Ptsommets { get; set; }

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        
        Texture2D[] Textures;
        Dictionary<long, Vector2> Positions = new Dictionary<long, Vector2>();
        NetClient client;
        
        public Game1()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
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
            Vector3[] tuile1 = new Vector3[4];
            tuile1[0] = new Vector3(0, 0, 0);
            tuile1[1] = new Vector3(250, 0, 0);
            tuile1[2] = new Vector3(0, 75, 0);
            tuile1[3] = new Vector3(250, 75, 0);
            Vector3[] tuile2 = new Vector3[4];
            tuile2[0] = new Vector3(250, 0, 0);
            tuile2[1] = new Vector3(250, 0, -250);
            tuile2[2] = new Vector3(250, 75, 0);
            tuile2[3] = new Vector3(250, 75, -250);
            Vector3[] tuile3 = new Vector3[4];
            tuile3[0] = new Vector3(0, 0, -250);
            tuile3[1] = new Vector3(0, 0, 0);
            tuile3[2] = new Vector3(0, 75, -250);
            tuile3[3] = new Vector3(0, 75, 0);
            Vector3[] tuile4 = new Vector3[4];
            tuile4[0] = new Vector3(250, 0, -250);
            tuile4[1] = new Vector3(0, 0, -250);
            tuile4[2] = new Vector3(250, 75, -250);
            tuile4[3] = new Vector3(0, 75, -250);

            Vector3 positionCaméra = new Vector3(0, 0, 0);
            Vector3 positionCibleCaméra = new Vector3(0, 0, -1);
            Vector3 positionDragon = new Vector3(0, 0, 0);
            Vector3 positionDragon2 = new Vector3(2, 0, 0);
            Vector3 positionDrapeau = new Vector3(125, 25, -125);

            Vector3 positionTuileDragon = new Vector3(0, 0, 0);
            Vector3 positionTuileChartreuse = new Vector3(-2, -2, -2);
            Vector3 positionTuileDrapeau = new Vector3(-2, -2, -20);

            client.DiscoverLocalPeers(14242);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);

            Components.Add(GestionInput);
            Components.Add(new ArrièrePlanDéroulant(this, "murderoche", INTERVALLE_UPDATE));
            CaméraJeu = new CaméraSubjective(this, positionCaméra, positionCibleCaméra, Vector3.Up, INTERVALLE_UPDATE);
            Components.Add(CaméraJeu);
            Components.Add(new Afficheur3D(this));
            
            Components.Add(new TuileTexturée(this, 1f, Vector3.Zero, positionTuileDragon, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile1));
            Components.Add(new TuileTexturée(this, 1f, Vector3.Zero, positionTuileDragon, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile2));
            Components.Add(new TuileTexturée(this, 1f, Vector3.Zero, positionTuileDragon, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile3));
            Components.Add(new TuileTexturée(this, 1f, Vector3.Zero, positionTuileDragon, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile4));
            Components.Add(new Drapeau(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), positionDrapeau, new Vector2(250, 250), new Vector2(100, 100), "DrapeauQuébec", 1, 1 / 60f, INTERVALLE_MAJ_STANDARD));
            Components.Add(new AfficheurFPS(this, "Arial20", Color.Gold, INTERVALLE_CALCUL_FPS));

            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Caméra), CaméraJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Textures = new Texture2D[5];
            for (int i = 0; i < 5; i++)
                Textures[i] = Content.Load<Texture2D>("c" + (i + 1));
        }

        protected override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ++CptFrames;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= INTERVALLE_UPDATE)
            {
                TempsÉcouléDepuisMAJ = 0;

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

            //GestionSprites.Begin();
            base.Draw(gameTime);
            //GestionSprites.End();

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

            // draw all players
            foreach (var kvp in Positions)
            {
                // use player unique identifier to choose an image
                int num = Math.Abs((int)kvp.Key) % Textures.Length;

                // draw player
                spriteBatch.Draw(Textures[num], kvp.Value, Color.White);
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
