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
        const int NB_PLATEFORMES = 50;

        const float �CHELLE_OBJET = 1f;

        const float ANGLE_DE_FLOTTAISON = MathHelper.Pi / 360;

        //dimensions plateformes
        const int LARGEUR_PLATEFORME = 6;
        const int �PAISSEUR_PLATEFORME = 1;
        const int NB_DE_PLATEFORMES_HORIZONTALES = 25;
        const int NB_DE_PLATEFORMES_VERTICALES = 25;
        const int POSITION_Y_PLATEFORMES = 45;
        const int POSITION_Y_CHECKPOINT = 50;

        //limites qui d�finissent o� tous les types de plateformes peuvent appara�tre
        const int LIMITE_POSITION_X_DROITE_PLATEFORMES = 220;
        const int LIMITE_POSITION_X_GAUCHE_PLATEFORMES = 20;
        const int LIMITE_POSITION_Z_ARRI�RE_PLATEFORMES = -230;
        const int LIMITE_POSITION_Z_AVANT_PLATEFORMES = -30;

        //limites positions canons
        const int LIMITE_POSITION_X_DROITE_CANONS = 250;
        const int LIMITE_POSITION_X_GAUCHE_CANONS = 0;
        const int LIMITE_POSITION_Z_ARRI�RE_CANONS = -250;
        const int LIMITE_POSITION_Z_AVANT_CANONS = 0;
        const int POSITION_Y_CANONS = 75;

        const int NB_CANONS_PAR_MURS = 5;
        const float �CHELLE_CANONS = 0.02f;

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



        Adversaire AutreJoueur { get; set; }
        Arri�rePlanD�roulant Arri�rePlan { get; set; }
        Lave Lave { get; set; }
        ObjetDeD�mo ObjDemo { get; set; }
        PlateformeHorizontaleFlottante PlateHori { get; set; }
        PlateformeSuivantUneSpline PlateSpline { get; set; }
        PlateformeVerticaleFlottante Plateverti { get; set; }
        TuileTextur�e TuileTex1 { get; set; }
        TuileTextur�e TuileTex2 { get; set; }
        TuileTextur�e TuileTex3 { get; set; }
        TuileTextur�e TuileTex4 { get; set; }


        int[] TableauCoordonn�esX_Spline { get; set; }
        int[] TableauCoordonn�esZ_Spline { get; set; }

        int IndiceTableauLimitesAireJeu { get; set; }
        int IndiceTableauAngleFlottaison { get; set; }


        int Position_X_plateformes { get; set; }
        int Position_Z_plateformes { get; set; }
        float[] Incr�mementAngleDeFlottaison { get; set; }

        int Position_X_checkpoint { get; set; }
        int Position_Z_checkpoint { get; set; }

        int Position_X_canons { get; set; }
        int Position_Z_canons { get; set; }


        float Temps�coul�DepuisMAJ { get; set; }
        int CptFrames { get; set; }
        GraphicsDeviceManager P�riph�riqueGraphique { get; set; }
        SpriteBatch spriteBatch;
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        Cam�ra Cam�raJeu { get; set; }
        public MouseState OriginalMouseState;

        Vector3[] Ptsommets { get; set; }

        Random G�n�rateurAl�atoire { get; set; }

        Vector3 PositionCam�ra { get; set; }
        Vector3 PositionCibleCam�ra { get; set; }
        Vector3 PositionOrigineMurRoche { get; set; }
        Vector3 PositionOrigineLave { get; set; }

        Vector3[] Tuile1 { get; set; }
        Vector3[] Tuile2 { get; set; }
        Vector3[] Tuile3 { get; set; }
        Vector3[] Tuile4 { get; set; }

        Vector3[] TableauPositionPlateformesHorizontales { get; set; }
        Vector3[] TableauPositionPlateformesVerticales { get; set; }

        Vector3 DimensionCheckpoint { get; set; }
        Vector3 DimensionModel { get; set; }
        BoundingBox ZoneDeCollisionModel { get; set; }
        BoundingBox ZoneDeCollisionCheckPoint { get; set; }

        Vector3 PositionBalle { get; set; }


        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeMod�les { get; set; }

        Texture2D[] Textures;
        Dictionary<long, Vector2> Positions = new Dictionary<long, Vector2>();
        public NetClient client;
        bool Pause { get; set; }
        public ControllerNet ControleurNet;

        ControllerNet controleurNet;



        //position de l'adversaire qui vas etre modifi� par le serveur
        public Vector3 PositionAdversaireSelonServeur { get; set; }

        Vector3[] PositionDesPlateformes = new Vector3[NB_PLATEFORMES];

        Menu2 LeMenu { get; set; }

        public Game1()
        {
            P�riph�riqueGraphique = new GraphicsDeviceManager(this);
            P�riph�riqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";

            //enlever le commentaire pour mettre le jeu en fullscreen
            //this.P�riph�riqueGraphique.IsFullScreen = true;

            NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();
            //client.Connect(host: "127.0.0.1", port: (14242));
        }

        protected override void Initialize()
        {

            Pause = true;
            DimensionCheckpoint = new Vector3(2.5f, 2.5f, 2.5f);
            DimensionModel = new Vector3(1, 1, 1);

            InitialiserTableauxLimitesAireJeu();
            InitialiserTableauIncr�mentationAngleFlottaison();
            InitialierTableauxCoordonn�esSpline();
            TableauPositionPlateformesHorizontales = new Vector3[NB_DE_PLATEFORMES_HORIZONTALES];
            TableauPositionPlateformesVerticales = new Vector3[NB_DE_PLATEFORMES_VERTICALES];

            PositionCam�ra = new Vector3(125, 250, -125);
            PositionCibleCam�ra = new Vector3(125, 0, -125);
            PositionOrigineMurRoche = new Vector3(0, 0, 0);
            PositionOrigineLave = new Vector3(125, 25, -125);

            Vector3 positionObjet = new Vector3(125, 45, -125);
            Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);


            client.DiscoverLocalPeers(14242);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeMod�les = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);
            G�n�rateurAl�atoire = new Random();
            controleurNet = new ControllerNet(this);
            //IsMouseVisible = true;
            //fait en sorte que la souris reste au milieu de l'�cran
            Mouse.SetPosition(this.Window.ClientBounds.Width / 2, this.Window.ClientBounds.Height / 2);
            OriginalMouseState = Mouse.GetState();

            Components.Add(GestionInput);
            Arri�rePlan = new Arri�rePlanD�roulant(this, "MurDeRoche", INTERVALLE_UPDATE);
            Components.Add(Arri�rePlan);
            Cam�raJeu = new Cam�raSubjective(this, PositionCam�ra, PositionCibleCam�ra, new Vector3(0, 0, -126), OriginalMouseState, INTERVALLE_UPDATE);
            Components.Add(Cam�raJeu);
            Joueur j = new Joueur(this, new Vector3(166, 120, -224), OriginalMouseState, INTERVALLE_CALCUL_STANDARD, new Vector3(2, 4, 2));
            Components.Add(j);
            Components.Add(new Afficheur3D(this));
            Cr�erMursAireDeJeu();
            Lave = new Lave(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), PositionOrigineLave, new Vector2(250, 250), new Vector2(100, 100), "Lave", 1, 1 / 60f, INTERVALLE_MAJ_STANDARD);
            Components.Add(Lave);
            Components.Add(new AfficheurFPS(this, "Arial20", Color.Gold, INTERVALLE_CALCUL_FPS));
            Components.Add(new Score(this, "Arial20", Color.Chartreuse, INTERVALLE_CALCUL_FPS, Cam�raJeu.Position));
            Components.Add(new Balle(this, 1f, rotationObjet, j.Position, INTERVALLE_MAJ_STANDARD, new Vector3(2, 2, 2), Color.Blue));

            G�rerPositionsPlateformesHorizontales();
            G�rerPositionsPlateformesVerticales();
            G�rerPositionsPlateformesSuivantSpline();
            G�rerPositionCheckpoint();
            G�rerPositionsCanons();


            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeMod�les);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Cam�ra), j);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            //ObjDemo = new ObjetDeD�mo(this, "bonhommeFinal", �CHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_CALCUL_STANDARD);
            //Components.Add(ObjDemo);
            AutreJoueur = new Adversaire(this, "bonhommeFinal", �CHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_CALCUL_STANDARD);
            Components.Add(AutreJoueur);


            LeMenu = new Menu2(this, P�riph�riqueGraphique, Pause);

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
            //base.LoadContent();
        }


        protected override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ++CptFrames;
            Temps�coul�DepuisMAJ += temps�coul�;
            PositionBalle = Cam�raJeu.Position;
           

            if (Temps�coul�DepuisMAJ >= INTERVALLE_UPDATE)
            {
                Temps�coul�DepuisMAJ = 0;
                
                for (int i = 0; i < Components.Count; ++i)
                    for (int j = i + 1; j < Components.Count; ++j)
                        if (!LeMenu.Pause && Components[i] is IPhysique && Components[j] is IPhysique)
                        {
                            
                            
                            IPhysique A = Components[i] as IPhysique;
                            IPhysique B = Components[j] as IPhysique;
                            bool enCollision = A.GetVolume().Intersects(B.GetVolume());
                            (Components[i] as IPhysique).SetEnCollision(enCollision, B);
                            (Components[j] as IPhysique).SetEnCollision(enCollision, A);
                        }

                KeyboardState keyState = Keyboard.GetState();

                if (keyState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();


                LireMessages();
            }

            base.Update(gameTime);
        }

        

        Vector3 Position;
        int Identifiant = 0;
        public void LireMessages()
        {
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
                        TraiterMessage(incomingMessage);
                        break;

                    default:
                        // Should not happen and if happens, don't care
                        Console.WriteLine(incomingMessage.ReadString() + " Strange message client");
                        break;
                }
            }
        }

        public void TraiterMessage(NetIncomingMessage msg)
        {
            bool hasBeenRead = false;
            byte TypeMessage = msg.ReadByte();
            long UniqueId = 0;
            UniqueId = msg.ReadInt64();

            if (!hasBeenRead)
            {
                switch ((PacketTypes)TypeMessage)
                {
                    case PacketTypes.CONNECTIONNUMBER:

                        hasBeenRead = true;
                        break;

                    case PacketTypes.POSITION:
                        Position = LireVector3(msg);
                        this.Window.Title = Position.ToString();
                        AutreJoueur.SetPosition(Position);
                        hasBeenRead = true;
                        break;
                    case PacketTypes.POSITIONMAP:
                        for(int i = 0; i < NB_PLATEFORMES; ++i)
                        {
                            PositionDesPlateformes[i] = LireVector3(msg);
                        }
                        break;

                }
            }
        }

        Vector3 LireVector3(NetIncomingMessage msg)
        {
            return new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }
        Vector2 LireVector2(NetIncomingMessage msg)
        {
            return new Vector2(msg.ReadFloat(), msg.ReadFloat());
        }

        void InitialiserTableauxLimitesAireJeu()
        {
            Limites_Zone_1 = new int[] { LIMITE_POSITION_X_GAUCHE_PLATEFORMES, LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_POSITION_Z_ARRI�RE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT };
            Limites_Zone_2 = new int[] { LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_Z_ARRI�RE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT };
            Limites_Zone_3 = new int[] { LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_X_DROITE_PLATEFORMES, LIMITE_POSITION_Z_ARRI�RE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT };
            Limites_Zone_4 = new int[] { LIMITE_POSITION_X_GAUCHE_PLATEFORMES, LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_1_2_3_Z_AVANT, LIMITE_ZONES_4_5_6_Z_AVANT };
            Limites_Zone_5 = new int[] { LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_ZONES_1_2_3_Z_AVANT, LIMITE_ZONES_4_5_6_Z_AVANT };
            Limites_Zone_6 = new int[] { LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_X_DROITE_PLATEFORMES, LIMITE_ZONES_1_2_3_Z_AVANT, LIMITE_ZONES_4_5_6_Z_AVANT };
            Limites_Zone_7 = new int[] { LIMITE_POSITION_X_GAUCHE_PLATEFORMES, LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_4_5_6_Z_AVANT, LIMITE_POSITION_Z_AVANT_PLATEFORMES };
            Limites_Zone_8 = new int[] { LIMITE_ZONES_1_4_7_X_DROITE, LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_ZONES_4_5_6_Z_AVANT, LIMITE_POSITION_Z_AVANT_PLATEFORMES };
            Limites_Zone_9 = new int[] { LIMITE_ZONES_2_5_8_X_DROITE, LIMITE_POSITION_X_DROITE_PLATEFORMES, LIMITE_ZONES_4_5_6_Z_AVANT, LIMITE_POSITION_Z_AVANT_PLATEFORMES };

            LimitesAireDeJeu = new int[][] { Limites_Zone_1, Limites_Zone_2, Limites_Zone_3, Limites_Zone_4, Limites_Zone_5, Limites_Zone_6, Limites_Zone_7, Limites_Zone_8, Limites_Zone_9 };
        }

        void InitialiserTableauIncr�mentationAngleFlottaison()
        {
            Incr�mementAngleDeFlottaison = new float[] { MathHelper.Pi / 90, MathHelper.Pi / 105, MathHelper.Pi / 120, MathHelper.Pi / 135, MathHelper.Pi / 150, MathHelper.Pi / 165, MathHelper.Pi / 180 };
        }

        void InitialierTableauxCoordonn�esSpline()
        {
            TableauCoordonn�esX_Spline = new int[] { 40, 70, 100, 120, 110, 130, 160, 190, 210, 230, 210, 220, 220, 210, 180, 150, 110, 70 };
            TableauCoordonn�esZ_Spline = new int[] { -50, -90, -110, -160, -210, -240, -230, -200, -170, -130, -100, -70, -40, -30, -40, -30, -20, -30 };
        }

        void Cr�erMur1()
        {
            Tuile1 = new Vector3[4];
            Tuile1[0] = new Vector3(0, 0, 0);
            Tuile1[1] = new Vector3(250, 0, 0);
            Tuile1[2] = new Vector3(0, 75, 0);
            Tuile1[3] = new Vector3(250, 75, 0);
        }

        void Cr�erMur2()
        {
            Tuile2 = new Vector3[4];
            Tuile2[0] = new Vector3(250, 0, 0);
            Tuile2[1] = new Vector3(250, 0, -250);
            Tuile2[2] = new Vector3(250, 75, 0);
            Tuile2[3] = new Vector3(250, 75, -250);
        }

        void Cr�erMur3()
        {
            Tuile3 = new Vector3[4];
            Tuile3[0] = new Vector3(0, 0, -250);
            Tuile3[1] = new Vector3(0, 0, 0);
            Tuile3[2] = new Vector3(0, 75, -250);
            Tuile3[3] = new Vector3(0, 75, 0);
        }

        void Cr�erMur4()
        {
            Tuile4 = new Vector3[4];
            Tuile4[0] = new Vector3(250, 0, -250);
            Tuile4[1] = new Vector3(0, 0, -250);
            Tuile4[2] = new Vector3(250, 75, -250);
            Tuile4[3] = new Vector3(0, 75, -250);
        }

        void Cr�erMursAireDeJeu()
        {
            Cr�erMur1();
            Cr�erMur2();
            Cr�erMur3();
            Cr�erMur4();

            TuileTex1 = new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile1);
            Components.Add(TuileTex1);
            TuileTex2 = new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile2);
            Components.Add(TuileTex2);
            TuileTex3 = new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile3);
            Components.Add(TuileTex3);
            TuileTex4 = new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile4);
            Components.Add(TuileTex4);
        }

        void G�rerPositionsPlateformesHorizontales()
        {

            for (int cpt = 0; cpt < NB_DE_PLATEFORMES_HORIZONTALES; ++cpt)
            {
                IndiceTableauLimitesAireJeu = G�n�rateurAl�atoire.Next(0, LimitesAireDeJeu.Length);
                IndiceTableauAngleFlottaison = G�n�rateurAl�atoire.Next(0, Incr�mementAngleDeFlottaison.Length);
                Position_X_plateformes = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][0], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][1] + 1);
                Position_Z_plateformes = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][2], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][3] + 1);
                TableauPositionPlateformesHorizontales[cpt] = new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes);
                PlateHori = new PlateformeHorizontaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, Incr�mementAngleDeFlottaison[IndiceTableauAngleFlottaison], INTERVALLE_MAJ_STANDARD);
                Components.Add(PlateHori);
            }
        }

        void G�rerPositionsPlateformesVerticales()
        {
            for (int cpt = 0; cpt < NB_DE_PLATEFORMES_VERTICALES; ++cpt)
            {
                IndiceTableauLimitesAireJeu = G�n�rateurAl�atoire.Next(0, LimitesAireDeJeu.Length);
                IndiceTableauAngleFlottaison = G�n�rateurAl�atoire.Next(0, Incr�mementAngleDeFlottaison.Length);
                Position_X_plateformes = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][0], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][1] + 1);
                Position_Z_plateformes = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][2], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][3] + 1);
                TableauPositionPlateformesVerticales[cpt] = new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes);

                Plateverti = new PlateformeVerticaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, Incr�mementAngleDeFlottaison[IndiceTableauAngleFlottaison], INTERVALLE_MAJ_STANDARD);
                Components.Add(Plateverti);
            }
        }

        void G�rerPositionsPlateformesSuivantSpline()
        {
            for (int cpt = 0; cpt < TableauCoordonn�esX_Spline.Length; ++cpt)
            {
                PlateSpline = new PlateformeSuivantUneSpline(this, 1f, Vector3.Zero, new Vector3(TableauCoordonn�esX_Spline[cpt], POSITION_Y_PLATEFORMES, TableauCoordonn�esZ_Spline[cpt]), Color.GreenYellow, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), INTERVALLE_MAJ_STANDARD, ANGLE_DE_FLOTTAISON, MathHelper.Pi / 360, "SplineX.txt", "SplineZ.txt");
                Components.Add(PlateSpline);
            }
        }

        void G�rerPositionCheckpoint()
        {
            IndiceTableauLimitesAireJeu = G�n�rateurAl�atoire.Next(0, LimitesAireDeJeu.Length);
            IndiceTableauAngleFlottaison = G�n�rateurAl�atoire.Next(0, Incr�mementAngleDeFlottaison.Length);
            Position_X_checkpoint = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][0], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][1] + 1);
            Position_Z_checkpoint = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][2], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][3] + 1);

            Components.Add(new Plateforme(this, 1f, Vector3.Zero, new Vector3(Position_X_checkpoint, POSITION_Y_PLATEFORMES, Position_Z_checkpoint), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, 0, INTERVALLE_MAJ_STANDARD));
            Components.Add(new CheckpointAnim�(this, 1f, new Vector3(MathHelper.Pi, 0, 0), new Vector3(Position_X_checkpoint + LARGEUR_PLATEFORME, POSITION_Y_CHECKPOINT, Position_Z_checkpoint + LARGEUR_PLATEFORME), Color.Yellow, DimensionCheckpoint, INTERVALLE_MAJ_STANDARD, Cam�raJeu.Position));

        }

        void G�rerPositionsCanonsMurGauche()
        {
            Position_X_canons = LIMITE_POSITION_X_GAUCHE_CANONS;

            for (int cpt = 0; cpt < NB_CANONS_PAR_MURS; ++cpt)
            {
                Position_Z_canons = G�n�rateurAl�atoire.Next(LIMITE_POSITION_Z_ARRI�RE_CANONS, LIMITE_POSITION_Z_AVANT_CANONS);
                Components.Add(new Canon(this, "canon", �CHELLE_CANONS, Vector3.Zero, new Vector3(Position_X_canons, POSITION_Y_CANONS, Position_Z_canons)));
            }
        }

        void G�rerPositionCanonsMurDroite()
        {
            Position_X_canons = LIMITE_POSITION_X_DROITE_CANONS;

            for (int cpt = 0; cpt < NB_CANONS_PAR_MURS; ++cpt)
            {
                Position_Z_canons = G�n�rateurAl�atoire.Next(LIMITE_POSITION_Z_ARRI�RE_CANONS, LIMITE_POSITION_Z_AVANT_CANONS);
                Components.Add(new Canon(this, "canon", �CHELLE_CANONS, new Vector3(0, MathHelper.Pi, 0), new Vector3(Position_X_canons, POSITION_Y_CANONS, Position_Z_canons)));
            }
        }

        void G�rerPositionsCanonsMurAvant()
        {
            Position_Z_canons = LIMITE_POSITION_Z_AVANT_CANONS;

            for (int cpt = 0; cpt < NB_CANONS_PAR_MURS; ++cpt)
            {
                Position_X_canons = G�n�rateurAl�atoire.Next(LIMITE_POSITION_X_GAUCHE_CANONS, LIMITE_POSITION_X_DROITE_CANONS);
                Components.Add(new Canon(this, "canon", �CHELLE_CANONS, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(Position_X_canons, POSITION_Y_CANONS, Position_Z_canons)));
            }
        }

        void G�rerPositionsCanonsMurArri�re()
        {
            Position_Z_canons = LIMITE_POSITION_Z_ARRI�RE_CANONS;

            for (int cpt = 0; cpt < NB_CANONS_PAR_MURS; ++cpt)
            {
                Position_X_canons = G�n�rateurAl�atoire.Next(LIMITE_POSITION_X_GAUCHE_CANONS, LIMITE_POSITION_X_DROITE_CANONS);
                Components.Add(new Canon(this, "canon", �CHELLE_CANONS, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(Position_X_canons, POSITION_Y_CANONS, Position_Z_canons)));
            }
        }

        void G�rerPositionsCanons()
        {
            G�rerPositionCanonsMurDroite();
            G�rerPositionsCanonsMurGauche();
            G�rerPositionsCanonsMurAvant();
            G�rerPositionsCanonsMurArri�re();
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

    }
    enum PacketTypes
    {
        CONNECTIONNUMBER,
        POSITION,
        POSITIONMAP
    }
}
