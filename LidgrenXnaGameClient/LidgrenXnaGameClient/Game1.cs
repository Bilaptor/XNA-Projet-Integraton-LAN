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

        const float �CHELLE_OBJET = 1f;

        const float ANGLE_DE_FLOTTAISON = MathHelper.Pi / 360;


        //dimensions plateformes
        const int LARGEUR_PLATEFORME = 6;
        const int �PAISSEUR_PLATEFORME = 1;

        const int NB_DE_PLATEFORMES_HORIZONTALES = 22;
        const int NB_DE_PLATEFORMES_VERTICALES = 22;
        const int NB_DE_PLATEFORMES_POUR_PARCOURS_POSSIBLES = 12;
        const int POSITION_Y_PLATEFORMES = 45;

        //limites qui d�finissent o� tous les types de plateformes peuvent appara�tre
        const int LIMITE_POSITION_X_DROITE_PLATEFORMES = 220;
        const int LIMITE_POSITION_X_GAUCHE_PLATEFORMES = 20;
        const int LIMITE_POSITION_Z_ARRI�RE_PLATEFORMES = -230;
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

        int IndiceTableauLimitesAireJeu { get; set; }
        int IndiceTableauAngleFlottaison { get; set; }


        int Position_X_plateformes { get; set; }
        int Position_Z_plateformes { get; set; }
        float[] Incr�mementAngleDeFlottaison { get; set; }


        float Temps�coul�DepuisMAJ { get; set; }
        int CptFrames { get; set; }
        GraphicsDeviceManager P�riph�riqueGraphique { get; set; }
        SpriteBatch spriteBatch;
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        Cam�ra Cam�raJeu { get; set; }

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
            InitialiserTableauxLimitesAireJeu();
            InitialiserTableauIncr�mentationAngleFlottaison();

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

            Components.Add(GestionInput);
            Components.Add(new Arri�rePlanD�roulant(this, "MurDeRoche", INTERVALLE_UPDATE));
            Cam�raJeu = new Cam�raSubjective(this, PositionCam�ra, PositionCibleCam�ra, new Vector3(0, 0, -126), INTERVALLE_UPDATE);
            Components.Add(Cam�raJeu);
            Components.Add(new Afficheur3D(this));

            Cr�erMursAireDeJeu();

            Components.Add(new Lave(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), PositionOrigineLave, new Vector2(250, 250), new Vector2(100, 100), "Lave", 1, 1 / 60f, INTERVALLE_MAJ_STANDARD));
            Components.Add(new AfficheurFPS(this, "Arial20", Color.Gold, INTERVALLE_CALCUL_FPS));

            G�rerPositionsPlateformesHorizontales();
            G�rerPositionsPlateformesVerticales();

            Components.Add(new PlateformeSuivantUneSpline(this, 1f, Vector3.Zero, new Vector3(40, POSITION_Y_PLATEFORMES, -50), Color.GreenYellow, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), INTERVALLE_MAJ_STANDARD, ANGLE_DE_FLOTTAISON, 0,"SplineX.txt", "SplineZ.txt"));
        

            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeMod�les);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Cam�ra), Cam�raJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            Components.Add(new ObjetDeD�mo(this, "bonhommeFinal", �CHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_CALCUL_STANDARD));
            Components.Add(new Adversaire(this, "bonhommeFinal", �CHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_CALCUL_STANDARD));

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
                    client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
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

            LimitesAireDeJeu = new int[][] {Limites_Zone_1, Limites_Zone_2,Limites_Zone_3,Limites_Zone_4,Limites_Zone_5,Limites_Zone_6,Limites_Zone_7,Limites_Zone_8,Limites_Zone_9 };
        }

        void InitialiserTableauIncr�mentationAngleFlottaison()
        {
            Incr�mementAngleDeFlottaison = new float[] { MathHelper.Pi / 90, MathHelper.Pi / 105, MathHelper.Pi / 120, MathHelper.Pi / 135 ,MathHelper.Pi / 150, MathHelper.Pi / 165 ,MathHelper.Pi / 180 };
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

            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile1));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile2));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile3));
            Components.Add(new TuileTextur�e(this, 1f, Vector3.Zero, PositionOrigineMurRoche, new Vector2(2, 2), "MurAireDeJeu", INTERVALLE_MAJ_STANDARD, Tuile4));
        }

        void G�rerPositionsPlateformesHorizontales()
        {
            for( int cpt = 0; cpt < NB_DE_PLATEFORMES_HORIZONTALES; ++cpt)
            {
                IndiceTableauLimitesAireJeu = G�n�rateurAl�atoire.Next(0, LimitesAireDeJeu.Length);
                IndiceTableauAngleFlottaison = G�n�rateurAl�atoire.Next(0, Incr�mementAngleDeFlottaison.Length);
                Position_X_plateformes = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][0], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][1] + 1);
                Position_Z_plateformes = G�n�rateurAl�atoire.Next(LimitesAireDeJeu[IndiceTableauLimitesAireJeu][2], LimitesAireDeJeu[IndiceTableauLimitesAireJeu][3] + 1);

                Components.Add(new PlateformeHorizontaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, Incr�mementAngleDeFlottaison[IndiceTableauAngleFlottaison], INTERVALLE_MAJ_STANDARD));
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

                Components.Add(new PlateformeVerticaleFlottante(this, 1f, Vector3.Zero, new Vector3(Position_X_plateformes, POSITION_Y_PLATEFORMES, Position_Z_plateformes), Color.WhiteSmoke, new Vector3(LARGEUR_PLATEFORME, �PAISSEUR_PLATEFORME, LARGEUR_PLATEFORME), ANGLE_DE_FLOTTAISON, Incr�mementAngleDeFlottaison[IndiceTableauAngleFlottaison], INTERVALLE_MAJ_STANDARD));
            }
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
}
