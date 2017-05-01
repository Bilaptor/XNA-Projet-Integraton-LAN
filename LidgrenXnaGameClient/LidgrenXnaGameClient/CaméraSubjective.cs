using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaGameClient
{
    public class CaméraSubjective : Caméra
    {
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float ACCÉLÉRATION = 0.001f;
        const float VITESSE_INITIALE_ROTATION = 0.1f;
        const float VITESSE_INITIALE_ROTATION_MANETTE = 4f;
        const float VITESSE_INITIALE_TRANSLATION = 1.5f;
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
        const float RAYON_COLLISION = 1f;

        Vector3 Direction { get; set; }
        Vector3 Latéral { get; set; }
        float VitesseTranslation { get; set; }
        float VitesseRotation { get; set; }
        float VitesseRotationManette { get; set; }

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }
        MouseState OriginalMouseState { get; set; }

        NetClient client;

        IController controller;

        bool estEnZoom;
        bool EstEnZoom
        {
            get { return estEnZoom; }
            set
            {
                float ratioAffichage = Game.GraphicsDevice.Viewport.AspectRatio;
                estEnZoom = value;
                if (estEnZoom)
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
                else
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
            }
        }

        public CaméraSubjective(Game jeu, Vector3 positionCaméra, Vector3 cible, Vector3 orientation, MouseState originalMouseState, float intervalleMAJ)
           : base(jeu)
        {
            OriginalMouseState = originalMouseState;
            IntervalleMAJ = intervalleMAJ;
            CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            CréerPointDeVue(positionCaméra, cible, orientation);
            EstEnZoom = false;
        }

        public override void Initialize()
        {
            client = (Game as Game1).client;
            controller = new ControllerClavier(Game, OriginalMouseState);

            VitesseRotationManette = VITESSE_INITIALE_ROTATION_MANETTE;
            VitesseRotation = VITESSE_INITIALE_ROTATION;
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        protected override void CréerPointDeVue()
        {
            // Méthode appelée s'il est nécessaire de recalculer la matrice de vue.
            // Calcul et normalisation de certains vecteurs
            // (à compléter)
            Direction = Vector3.Normalize(Direction);
            OrientationVerticale = Vector3.Normalize(OrientationVerticale);
            Latéral = Vector3.Normalize(Latéral);

            Vue = Matrix.CreateLookAt(Position, Position + Direction, OrientationVerticale);
            GénérerFrustum();
        }

        protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
        {
            Position = position;
            Cible = cible;
            Direction = Cible - Position;

            Latéral = Vector3.Cross(Direction, orientation);
            OrientationVerticale = Vector3.Cross(Latéral, Direction);

            //Création de la matrice de vue (point de vue)
            CréerPointDeVue();
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            GestionClavier();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerSouris();
                GérerAccélération();
                GérerDéplacement();
                //GérerRotation();
                CréerPointDeVue();

                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void GérerSouris()
        {
            Game.Window.Title = Direction.ToString();

            //MouseState currentMouseState = Mouse.GetState();
            //if (currentMouseState != OriginalMouseState)
            //{
            //   float déplacementX = currentMouseState.X - OriginalMouseState.X;
            //   float déplacementY = currentMouseState.Y - OriginalMouseState.Y;

            //   angleHorizontal += déplacementX * 0.01f;
            //   angleHorizontal %= (float)(Math.PI * 2);

            //   angleVertical += déplacementY * 0.01f;
            //   if (angleVertical > (Math.PI / 2))
            //      angleVertical = (float)(Math.PI / 2 - 0.01);
            //   if (angleVertical < -(Math.PI / 2))
            //      angleVertical = (float)(-Math.PI / 2 + 0.01);

            //Direction = new Vector3((float)Math.Cos(angleHorizontal), -(float)Math.Tan(angleVertical), (float)Math.Sin(angleHorizontal));
            Direction = controller.GetDirectionVu();
            //Direction.Normalize();
            Latéral = Vector3.Cross(Direction, OrientationVerticale);
            OrientationVerticale = Vector3.Up;
            //}
        }

        private int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }

        private void GérerAccélération()
        {
            int valAccélération = (GérerTouche(Keys.Subtract) + GérerTouche(Keys.OemMinus)) - (GérerTouche(Keys.Add) + GérerTouche(Keys.OemPlus));
            if (valAccélération != 0)
            {
                IntervalleMAJ += ACCÉLÉRATION * valAccélération;
                IntervalleMAJ = MathHelper.Max(INTERVALLE_MAJ_STANDARD, IntervalleMAJ);
            }
        }

        private void GérerDéplacement()
        {

            Vector3 AnciennePosition = Position;
            float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
            float déplacementLatéral = (GérerTouche(Keys.A) - GérerTouche(Keys.D)) * VitesseTranslation;

            if (déplacementDirection == 0)
            {
                déplacementDirection = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * VitesseTranslation;
            }

            if (déplacementLatéral == 0)
            {
                déplacementLatéral = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * VitesseTranslation;
            }

            Position = Position + déplacementDirection * Direction;
            Position = Position - déplacementLatéral * Latéral;

            if(AnciennePosition != Position)
            {
            // envoie la position au serveur
            NetOutgoingMessage om = client.CreateMessage();
            om.Write((byte)PacketTypes.POSITION);
            om.Write(Position.X);
            om.Write(Position.Y);
            om.Write(Position.Z);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            }
        }

        //private void GérerRotation()
        //{
        //    GérerLacet();
        //    GérerTangage();
        //    GérerRoulis();
        //}

        //private void GérerLacet()
        //{
        //    float lacet = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * VitesseRotationManette * DELTA_LACET;
            
        //    Matrix mLacet = Matrix.CreateFromAxisAngle(OrientationVerticale, lacet);
        //    Direction = Vector3.Transform(Direction, mLacet);
        //    Direction = Vector3.Normalize(Direction);
        //    Latéral = Vector3.Cross(Direction, OrientationVerticale);
        //}

        //private void GérerTangage()
        //{
        //    float tangage = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * VitesseRotationManette * DELTA_TANGAGE;

        //    Matrix mTangageLatéral = Matrix.CreateFromAxisAngle(Latéral, tangage);
        //    OrientationVerticale = Vector3.Transform(OrientationVerticale, mTangageLatéral);
        //    OrientationVerticale = Vector3.Normalize(OrientationVerticale);

        //    Matrix mTangageDirection = Matrix.CreateFromAxisAngle(Latéral, tangage);
        //    Direction = Vector3.Transform(Direction, mTangageDirection);
        //    Direction = Vector3.Normalize(Direction);
        //    Latéral = Vector3.Cross(Direction, OrientationVerticale);
        //}

        //private void GérerRoulis()
        //{
        //    float roulis = (GérerTouche(Keys.PageUp) - GérerTouche(Keys.PageDown)) * VitesseRotationManette * DELTA_ROULIS;

        //    Matrix mRoulis = Matrix.CreateFromAxisAngle(Direction, roulis);
        //    OrientationVerticale = Vector3.Transform(OrientationVerticale, mRoulis);
        //    OrientationVerticale = Vector3.Normalize(OrientationVerticale);
        //}

        private void GestionClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Z))
            {
                EstEnZoom = !EstEnZoom;
            }
        }
    }
}
