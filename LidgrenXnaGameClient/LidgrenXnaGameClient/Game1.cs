using System;
using Lidgren.Network;
using System.Linq;
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
        const float INTERVALLE_CALCUL_STANDARD = 1f / 60f;

        const float ÉCHELLE_OBJET = 1f;

        const float ANGLE_DE_FLOTTAISON = MathHelper.Pi / 360;

        const float DIMENSION_CHECKPOINT = 0.5f;


        //dimensions plateformes
        const int LARGEUR_PLATEFORME = 6;
        const int ÉPAISSEUR_PLATEFORME = 1;

        const int NB_DE_PLATEFORMES_HORIZONTALES = 25;
        const int NB_DE_PLATEFORMES_VERTICALES = 25;
        const int POSITION_Y_PLATEFORMES = 45;
        const int POSITION_Y_CHECKPOINT = 50;

        //limites qui définissent où tous les types de plateformes peuvent apparaître
        const int LIMITE_POSITION_X_DROITE_PLATEFORMES = 220;
        const int LIMITE_POSITION_X_GAUCHE_PLATEFORMES = 20;
        const int LIMITE_POSITION_Z_ARRIÈRE_PLATEFORMES = -230;
        const int LIMITE_POSITION_Z_AVANT_PLATEFORMES = -30;

        //limites des 9 sections de l'aire de jeu
        const int LIMITE_ZONES_1_4_7_X_DROITE = 80;
        const int LIMITE_ZONES_2_5_8_X_DROITE = 150;
        const int LIMITE_ZONES_1_2_3_Z_AVANT = -170;
        const int LIMITE_ZONES_4_5_6_Z_AVANT = -100;

        int[][] LimitesAireDeJeu { get; set; }
        int[] Limites_Zone_1 { get; set; }
        int[] Limites_Zone_2 { get; set; }
        int[] Limites_Zone_3 { get; set; }
        int[] Limites_Zone_4 { get; set; }
        int[] Limites_Zone_5 { get; set; }
        int[] Limites_Zone_6 { get; set; }
        int[] Limites_Zone_7 { get; set; }
        int[] Limites_Zone_8 { get; set; }
        int[] Limites_Zone_9 { get; set; }



        Adversaire Adversaire { get; set; }
        ArrièrePlanDéroulant ArrièrePlan { get; set; }
        Lave Lave { get; set; }
        ObjetDeDémo ObjDemo { get; set; }
        PlateformeHorizontaleFlottante PlateHori { get; set; }
        PlateformeSuivantUneSpline PlateSpline { get; set; }
        PlateformeVerticaleFlottante Plateverti { get; set; }
        TuileTexturée TuileTex1 { get; set; }
        TuileTexturée TuileTex2 { get; set; }
        TuileTexturée TuileTex3 { get; set; }
        TuileTexturée TuileTex4 { get; set; }


        int[] TableauCoordonnéesX_Spline { get; set; }
        int[] TableauCoordonnéesZ_Spline { get; set; }

        int IndiceTableauLimitesAireJeu { get; set; }
        int IndiceTableauAngleFlottaison { get; set; }


        int Position_X_plateformes { get; set; }
        int Position_Z_plateformes { get; set; }
        float[] IncrémementAngleDeFlottaison { get; set; }

        int Position_X_checkpoint { get; set; }
        int Position_Z_checkpoint { get; set; }


        float TempsÉcouléDepuisMAJ { get; set; }
        int CptFrames { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch spriteBatch;
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        Caméra CaméraJeu { get; set; }
        public MouseState OriginalMouseState;

        Vector3[] Ptsommets { get; set; }

        Random GénérateurAléatoire { get; set; }

        Vector3 PositionCaméra { get; set; }
        Vector3 PositionCibleCaméra { get; set; }
        Vector3 PositionOrigineMurRoche { get; set; }
        Vector3 PositionOrigineLave { get; set; }

        Vector3[] Tuile1 { get; set; }
        Vector3[] Tuile2 { get; set; }
        Vector3[] Tuile3 { get; set; }
        Vector3[] Tuile4 { get; set; }
        Vector3[] TableauPositionPlateformesHorizontales { get; set; }


        BoundingBox ZoneDeCollisionModel { get; set; }
        BoundingBox ZoneDeCollisionCheckPoint { get; set; }
       

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }

        Texture2D[] Textures;
        Dictionary<long, Vector2> Positions = new Dictionary<long, Vector2>();
        NetClient client;
        bool Pause { get; set; }

        //position de l'adversaire qui vas etre modifié par le serveur
        public Vector3 PositionAdversaireSelonServeur { get; set; }

        Menu2 LeMenu { get; set; }

        public Game1()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";

            //enlever le commentaire pour mettre le jeu en fullscreen
            //this.PériphériqueGraphique.IsFullScreen = true;

            NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();
        }

        protected override void Initialize()
        {
            Pause = true;
            InitialiserTableauxLimitesAireJeu();
            InitialiserTableauIncrémentationAngleFlottaison();
            InitialierTableauxCoordonnéesSpline();
            TableauPositionPlateformesHorizontales = new Vector3[NB_DE_PLATEFORMES_HORIZONTALES];

            PositionCaméra = new Vector3(125, 250, -125);
            PositionCibleCaméra = new Vector3(125, 0, -125);
            PositionOrigineMurRoche = new Vector3(0, 0, 0);
            PositionOrigineLave = new Vector3(125, 25, -125);


            Vector3 positionObjet = new Vector3(125, 45, -125);
            Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);


            client.DiscoverLocalPeers(14242);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);
            GénérateurAléatoire = new Random();

            //IsMouseVisible = true;
            //fait en sorte que la souris reste au milieu de l'écran
            Mouse.SetPosition(this.Window.ClientBounds.Width / 2, this.Window.ClientBounds.Height / 2);
            OriginalMouseState = Mouse.GetState();

            Components.Add(GestionInput);
            ArrièrePlan = new ArrièrePlanDéroulant(this, "MurDeRoche", INTERVALLE_UPDATE);
            Components.Add(ArrièrePlan);
            CaméraJeu = new CaméraSubjective(this, PositionCaméra, PositionCibleCaméra, new Vector3(0, 0, -126), OriginalMouseState, INTERVALLE_UPDATE);
            Components.Add(CaméraJeu);
            Components.Add(new Afficheur3D(this));
            CréerMursAireDeJeu();
            Lave = new Lave(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), PositionOrigineLave, new Vector2(250, 250), new Vector2(100, 100), "Lave", 1, 1 / 60f, INTERVALLE_MAJ_STANDARD);
            Components.Add(Lave);
            Components.Add(new AfficheurFPS(this, "Arial20", Color.Gold, INTERVALLE_CALCUL_FPS));
            Components.Add(new Score(this, "Arial20", Color.Red, INTERVALLE_CALCUL_FPS, ZoneDeCollisionModel, ZoneDeCollisionCheckPoint));

            GérerPositionsPlateformesHorizontales();
            GérerPositionsPlateformesVerticales();
            GérerPositionsPlateformesSuivantSpline();
            GérerPositionCheckpoint();

            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Caméra), CaméraJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            //ObjDemo = new ObjetDeDémo(this, "bonhommeFinal", ÉCHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_CALCUL_STANDARD);
            //Components.Add(ObjDemo);
            Adversaire = new Adversaire(this, "bonhommeFinal", ÉCHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_CALCUL_STANDARD);
            Components.Add(Adversaire);

            LeMenu = new Menu2(this, PériphériqueGraphique, Pause);

            Services.AddService(typeof(Menu2), LeMenu);
            Components.Add(LeMenu);

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
                MettreEnPause();

                //
                // Collect input
                //
                int xinput = 0;
                int yinput = 0;
                KeyboardState keyState = Keyboard.GetState();

                // exit game if escape or Back is pressed
                if (keyState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                //// use arrows or dpad to move avatar
                //if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyState.IsKeyDown(Keys.Left))
                //    xinput = -1;
                //if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyState.IsKeyDown(Keys.Right))
                //    xinput = 1;
                //if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || keyState.IsKeyDown(Keys.Up))
                //    yinput = -1;
                //if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || keyState.IsKeyDown(Keys.Down))
                //    yinput = 1;

                if (xinput != 0 || yinput != 0)
                {
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write((byte)PacketTypes.POSITIONJEU2D);
                    om.Write(xinput); // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity
                    om.Write(yinput);
                    client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                }

                // read messages
                NetIncomingMessage incomingMessage;
                while ((incomingMessage = client.ReadMessage()) != null)
                {
                    switch (incomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:

                            client.Connect(incomingMessage.SenderEndPoint);
                            break;

                        case NetIncomingMessageType.Data:
                            bool hasBeenRead = false;
                            if (!hasBeenRead && incomingMessage.ReadByte() == (byte)PacketTypes.POSITIONJEU2D)
                            {
                                long who = incomingMessage.ReadInt64();
                                int x = incomingMessage.ReadInt32();
                                int y = incomingMessage.ReadInt32();
                                Positions[who] = new Vector2(x, y);
                                hasBeenRead = true;
                            }
                            if (!hasBeenRead && incomingMessage.ReadByte() == (byte)PacketTypes.POSITION)
                            {
                                float positionX = incomingMessage.ReadInt32();
                                float positionY = incomingMessage.ReadInt32();
                                float positionZ = incomingMessage.ReadInt32();
                                
                                Adversaire.DonnerPosition(new Vector3(positionX, positionY, positionZ));
                                hasBeenRead = true;
                            }
                            break;

                        default:
                            // Should not happen and if happens, don't care
                            Console.WriteLine(incomingMessage.ReadString() + " Strange message");
                            break;

                    }
                }
                //case NetIncomingMessageType.Data:
                //    // server sent a position update
                //    long who = incomingMessage.ReadInt64();
                //    int x = incomingMessage.ReadInt32();
                //    int y = incomingMessage.ReadInt32();
                //    Positions[who] = new Vector2(x, y);
                //    break;
            }
            
            base.Update(gameTime);
        }

        void InitialiserTableauxLimitesAireJeu()
        {
            Limites_Zone_1 = new int[] { LIMITE_POSITION_X_GAUCHE_PLATEFORMES, LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_POSITION_Z_ARRIÈRE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT };
            Limites_Zone_2 = new int[] { LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_Z_ARRIÈRE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT };
            Limites_Zone_3 = new int[] { LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_X_DROITE_PLATEFORMES, LIMITE_POSITION_Z_ARRIÈRE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT };
            Limites_Zone_4 = new int[] { LIMITE_POSITION_X_GAUCHE_PLATEFORMES, LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_1_2_3_Z_AVANT, LIMITE_ZONES_4_5_6_Z_AVANT };
            Limites_Zone_5 = new int[] { LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_ZONES_1_2_3_Z_AVANT, LIMITE_ZONES_4_5_6_Z_AVANT };
            Limites_Zone_6 = new int[] { LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_X_DROITE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT, LIMITE_ZONES_4_5_6_Z_AVANT };
            Limites_Zone_7 = new int[] { LIMITE_POSITION_X_GAUCHE_PLATEFORMES, LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_4_5_6_Z_AVANT, LIMITE_POSITION_Z_AVANT_PLATEFORMES };
            Limites_Zone_8 = new int[] { LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_ZONES_4_5_6_Z_AVANT, LIMITE_POSITION_Z_AVANT_PLATEFORMES };
            Limites_Zone_9 = new int[] { LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_X_DROITE_PLATEFORMES, LIMITE_ZONES_4_5_6_Z_AVANT, LIMITE_POSITION_Z_AVANT_PLATEFORMES };

            LimitesAireDeJeu = new int[][] { Limites_Zone_1, Limites_Zone_2, Limites_Zone_3, Limites_Zone_4, Limites_Zone_5, Limites_Zone_6, Limites_Zone_7, Limites_Zone_8, Limites_Zone_9 };
        }

        void InitialiserTableauIncrémentationAngleFlottaison()
        {
            IncrémementAngleDeFlottaison = new float[] { MathHelper.Pi / 90, MathHelper.Pi / 105, MathHelper.Pi / 120, MathHelper.Pi / 135, MathHelper.Pi / 150, MathHelper.Pi / 165, MathHelper.Pi / 180 };
        }

        void InitialierTableauxCoordonnéesSpline()
        {
            TableauCoordonnéesX_Spline = new int[] { 20, 35, 50, 60, 55, 65, 80, 95, 105, 115, 105, 110, 110, 105, 90, 75, 55, 35 };
            TableauCoordonnéesZ_Spline = new int[] { -25, -45, -55, -80, -105, -120, -115, -100, -85, -65, -50, -35, -20, -15, -20, -15, -10, -15 };
        }

        void CréerMur1()
        {
            Tuile1 = new Vector3[4];
            Tuile1[0] = new Vector3(0, 0, 0);
            Tuile1[1] = new Vector3(250, 0, 0);
            Tuile1[2] = new Vector3(0, 75, 0);
            Tuile1[3] = new Vector3(250, 75, 0);
        }

        void CréerMur2()
        {
            Tuile2 = new Vector3[4];
            Tuile2[0] = new Vector3(250, 0, 0);
            Tuile2[1] = new Vector3(250, 0, -250);
            Tuile2[2] = new Vector3(250, 75, 0);
            Tuile2[3] = new Vector3(250, 75, -250);
        }

        void CréerMur3()
        {
            Tuile3 = new Vector3[4];
            Tuile3[0] = new Vector3(0, 0, -250);
            Tuile3[1] = new Vector3(0, 0, 0);
            Tuile3[2] = new Vector3(0, 75, -250);
            Tuile3[3] = new Vector3(0, 75, 0);
        }

        void CréerMur4()
        {
            Tuile4 = new Vector3[4];
            Tuile4[0] = new Vector3(250, 0, -250);
            Tuile4[1] = new Vector3(0, 0, -250);
            Tuile4[2] = new Vector3(250, 75, -250);
            Tuile4[3] = new Vector3(0, 75, -250);
        }

        void CréerMursAireDeJeu()
        {
            CréerMur1();
            CréerMur2();
            CréerMur3();
            CréerMur4();

            TuileTex1 = new TuileTexturée(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile1);
            Components.Add(TuileTex1);
            TuileTex2 = new TuileTexturée(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile2);
            Components.Add(TuileTex2);
            TuileTex3 = new TuileTexturée(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile3);
            Components.Add(TuileTex3);
            TuileTex4 = new TuileTexturée(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile4);
            Components.Add(TuileTex4);
        }

        void GérerPositionsPlateformesHorizontales()
        {

            for (int cpt = 0; cpt < NB_DE_PLATEFORMES_HORIZONTALES; ++cpt)
            {
                IndiceTableauLimitesAireJeu = GénérateurAléatoire.Next(0, LimitesAireDeJeu.Length);
                IndiceTableauAngleFlottaison = GénérateurAléatoire.Next(0, IncrémementAngleDeFlottaison.Length);
                Position_X_plateformes = GénérateurAléatoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][0], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][1] + 1);
                Position_Z_plateformes = GénérateurAléatoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][2], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][3] + 1);
                TableauPositionPlateformesHorizontales[cpt] = new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes);
                PlateHori = new PlateformeHorizontaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, ÉPAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, IncrémementAngleDeFlottaison[IndiceTableauAngleFlottaison], INTERVALLE_MAJ_STANDARD);
                Components.Add(PlateHori);
            }
        }

        void GérerPositionsPlateformesVerticales()
        {
            for (int cpt = 0; cpt < NB_DE_PLATEFORMES_VERTICALES; ++cpt)
            {
                IndiceTableauLimitesAireJeu = GénérateurAléatoire.Next(0, LimitesAireDeJeu.Length);
                IndiceTableauAngleFlottaison = GénérateurAléatoire.Next(0, IncrémementAngleDeFlottaison.Length);
                Position_X_plateformes = GénérateurAléatoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][0], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][1] + 1);
                Position_Z_plateformes = GénérateurAléatoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][2], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][3] + 1);

                Plateverti = new PlateformeVerticaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, ÉPAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, IncrémementAngleDeFlottaison[IndiceTableauAngleFlottaison], INTERVALLE_MAJ_STANDARD);
                Components.Add(Plateverti);
            }
        }

        void GérerPositionsPlateformesSuivantSpline()
        {
            for(int cpt = 0; cpt < TableauCoordonnéesX_Spline.Length; ++cpt)
            {
                PlateSpline = new PlateformeSuivantUneSpline(this, 1f, Vector3.Zero, new Vector3(TableauCoordonnéesX_Spline[cpt], 23, TableauCoordonnéesZ_Spline[cpt]), Color.GreenYellow, new Vector3(LARGEUR_PLATEFORME, ÉPAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), INTERVALLE_MAJ_STANDARD, ANGLE_DE_FLOTTAISON, 0, "SplineX.txt", "SplineZ.txt");
                Components.Add(PlateSpline);
            }
        }

        void GérerPositionCheckpoint()
        {
            IndiceTableauLimitesAireJeu = GénérateurAléatoire.Next(0, LimitesAireDeJeu.Length);
            IndiceTableauAngleFlottaison = GénérateurAléatoire.Next(0, IncrémementAngleDeFlottaison.Length);
            Position_X_checkpoint = GénérateurAléatoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][0], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][1] + 1);
            Position_Z_checkpoint = GénérateurAléatoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][2], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][3] + 1);

            Components.Add(new Plateforme(this, 1f, Vector3.Zero, new Vector3(Position_X_checkpoint/2, 23, Position_Z_checkpoint/2), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, ÉPAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, 0, INTERVALLE_MAJ_STANDARD));
            Components.Add(new CheckpointAnimé(this, 5f, new Vector3(MathHelper.Pi, 0, 0), new Vector3(Position_X_checkpoint+5, POSITION_Y_CHECKPOINT, Position_Z_checkpoint+5), Color.Yellow, new Vector3(DIMENSION_CHECKPOINT,DIMENSION_CHECKPOINT,DIMENSION_CHECKPOINT) ,INTERVALLE_MAJ_STANDARD));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
          
            base.Draw(gameTime);

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
        protected void MettreEnPause()
        {
            Adversaire.Enabled = !LeMenu.Pause;
            ArrièrePlan.Enabled = !LeMenu.Pause;
            Lave.Enabled = !LeMenu.Pause;
            //ObjDemo.Enabled = !LeMenu.Pause;
            CaméraJeu.Enabled = !LeMenu.Pause;
            PlateHori.Enabled = !LeMenu.Pause;
            PlateSpline.Enabled = !LeMenu.Pause;
            Plateverti.Enabled = !LeMenu.Pause;
            TuileTex1.Enabled = !LeMenu.Pause;
            TuileTex2.Enabled = !LeMenu.Pause;
            TuileTex3.Enabled = !LeMenu.Pause;
            TuileTex4.Enabled = !LeMenu.Pause;

        }
    }
    enum PacketTypes
    {
        LOGIN,
        POSITION,
        POSITIONJEU2D
    }
}
