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

        const float LIMITE_ANGLE_DE_FLOTTAISON_MAX = MathHelper.Pi / 360;
        const float LIMITE_ANGLE_DE_FLOTTAISON_MIN = MathHelper.Pi / 180;

        const int LARGEUR_PLATEFORME = 6;
        const int �PAISSEUR_PLATEFORME = 1;
        const int NB_DE_PLATEFORMES_Al�ATOIRE = 15;
        const int NB_DE_PLATEFORMES_POUR_PARCOURS_POSSIBLES = 12;
        const int POSITION_Y_PLATEFORMES = 45;
        const int LIMITE_POSITION_X_PLATEFORMES_DROITE = 50;
        const int LIMITE_POSITION_X_PLATEFORMES_GAUCHE = 200;
        const int LIMITE_POSITION_Z_PLATEFORMES = 200;

        const int CHANGEMENT_POSITION = 6;


        int Position_X_plateformes { get; set; }
        int Position_Z_plateformes { get; set; }
        float AngleDeFlottaison { get; set; }


        float Temps�coul�DepuisMAJ { get; set; }
        int CptFrames { get; set; }
        GraphicsDeviceManager P�riph�riqueGraphique { get; set; }
        SpriteBatch spriteBatch;
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        Cam�ra Cam�raJeu { get; set; }

        Vector3[] Ptsommets { get; set; }

        Random G�n�rateurAl�atoirePourPlateformesAl�atoire { get; set; }
        Random G�n�rateurAl�atoirePourParcoursPossibles { get; set; }

        int[] Incr�mentEnXPourCheminsPossibles { get; set; }
        int[] Incr�mentEnZPourCheminsPossibles { get; set; }
        int Incr�ment_X_Al�atoire { get; set; }
        int Incr�ment_Z_Al�atoire { get; set; }


        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeMod�les { get; set; }
        
        Texture2D[] Textures;
        Dictionary<long, Vector2> Positions = new Dictionary<long, Vector2>();
        NetClient client;
        
        public Game1()
        {
            P�riph�riqueGraphique = new GraphicsDeviceManager(this);
            P�riph�riqueGraphique.SynchronizeWithVerticalRetrace = false;
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
            InitialiserTableauxIncr�mentationPourCheminsPossibles();

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

            Vector3 positionCam�ra = new Vector3(0, 0, 0);
            Vector3 positionCibleCam�ra = new Vector3(0, 0, -1);
            Vector3 positionOrigineMurRoche = new Vector3(0, 0, 0);
            Vector3 positionOrigineLave = new Vector3(125, 25, -125);


            client.DiscoverLocalPeers(14242);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeMod�les = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);
            G�n�rateurAl�atoirePourPlateformesAl�atoire = new Random();
            G�n�rateurAl�atoirePourParcoursPossibles = new Random();

            Components.Add(GestionInput);
            Components.Add(new Arri�rePlanD�roulant(this, "murderoche", INTERVALLE_UPDATE));
            Cam�raJeu = new Cam�raSubjective(this, positionCam�ra, positionCibleCam�ra, Vector3.Up, INTERVALLE_UPDATE);
            Components.Add(Cam�raJeu);
            Components.Add(new Afficheur3D(this));
            
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, positionOrigineMurRoche, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile1));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, positionOrigineMurRoche, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile2));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, positionOrigineMurRoche, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile3));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, positionOrigineMurRoche, new Vector2(2, 2), "Dragon", INTERVALLE_MAJ_STANDARD, tuile4));
            Components.Add(new Drapeau(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), positionOrigineLave, new Vector2(250, 250), new Vector2(100, 100), "DrapeauQu�bec", 1, 1 / 60f, INTERVALLE_MAJ_STANDARD));
            Components.Add(new AfficheurFPS(this, "Arial20", Color.Gold, INTERVALLE_CALCUL_FPS));

            Cr�erPlateformesAvecPositionsAl�atoires();
            Cr�erParcoursPossibles();
           
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeMod�les);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Cam�ra), Cam�raJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);

            base.Initialize();
        }

        void InitialiserTableauxIncr�mentationPourCheminsPossibles()
        {
            Incr�mentEnXPourCheminsPossibles = new int[CHANGEMENT_POSITION] { 8, 9, 10, -8, -9, -10 };
            Incr�mentEnZPourCheminsPossibles = new int[CHANGEMENT_POSITION] { -8, -9, -10, 5, 6, 7 };
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
                NetIncomingMessage incomingMessage;
                while ((incomingMessage = client.ReadMessage()) != null)
                {
                    switch (incomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            // just connect to first server discovered
                            client.Connect(incomingMessage.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.Data:
                            // server sent a position update
                            long who = incomingMessage.ReadInt64();
                            int x = incomingMessage.ReadInt32();
                            int y = incomingMessage.ReadInt32();
                            Positions[who] = new Vector2(x, y);
                            break;
                    }
                }
            }

            base.Update(gameTime);
        }

        void Cr�erPlateformesAvecPositionsAl�atoires()
        {
            for (int cpt = 0; cpt < NB_DE_PLATEFORMES_Al�ATOIRE; ++cpt)
            {
                Position_X_plateformes = G�n�rateurAl�atoirePourPlateformesAl�atoire.Next(LIMITE_POSITION_X_PLATEFORMES_DROITE, LIMITE_POSITION_X_PLATEFORMES_GAUCHE);
                Position_Z_plateformes = G�n�rateurAl�atoirePourPlateformesAl�atoire.Next(-LIMITE_POSITION_Z_PLATEFORMES, 0);
                AngleDeFlottaison = LIMITE_ANGLE_DE_FLOTTAISON_MAX;

                Components.Add(new PlateformeVerticaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), AngleDeFlottaison ,INTERVALLE_MAJ_STANDARD));
            }
        }

        void Cr�erParcoursPossibles()
        {
            Position_X_plateformes = G�n�rateurAl�atoirePourParcoursPossibles.Next(LIMITE_POSITION_X_PLATEFORMES_DROITE, LIMITE_POSITION_X_PLATEFORMES_GAUCHE);
            Position_Z_plateformes = G�n�rateurAl�atoirePourParcoursPossibles.Next(-LIMITE_POSITION_Z_PLATEFORMES, 0);

            for (int cpt = 0; cpt < NB_DE_PLATEFORMES_POUR_PARCOURS_POSSIBLES; ++cpt)
            {

                Incr�ment_X_Al�atoire = G�n�rateurAl�atoirePourParcoursPossibles.Next(0, CHANGEMENT_POSITION);
                Incr�ment_Z_Al�atoire = G�n�rateurAl�atoirePourParcoursPossibles.Next(0, CHANGEMENT_POSITION);
                Position_X_plateformes += Incr�mentEnXPourCheminsPossibles[Incr�ment_X_Al�atoire];
                Position_Z_plateformes += Incr�mentEnZPourCheminsPossibles[Incr�ment_Z_Al�atoire];
                AngleDeFlottaison = LIMITE_ANGLE_DE_FLOTTAISON_MAX;

                if (Position_X_plateformes != Position_Z_plateformes)
                {
                    Components.Add(new PlateformeHorizontaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.GreenYellow, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), AngleDeFlottaison, INTERVALLE_MAJ_STANDARD));
                }

                   
            }
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
