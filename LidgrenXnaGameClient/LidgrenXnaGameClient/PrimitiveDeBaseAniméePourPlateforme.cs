using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace XnaGameClient
{
    public abstract class PrimitiveDeBaseAnim�ePourPlateforme : PrimitiveDeBase
    {
        protected float Homoth�tie { get; set; }
        protected Vector3 Position { get; set; }
        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        protected InputManager GestionInput { get; private set; }
        float Incr�mentAngleRotation { get; set; }
        protected bool Translation { get; set; }
        protected bool Monde�Recalculer { get; set; }
        protected float AngleTangage = 0;
        protected float AngleRoulis = 0;

        float angleLacet;
        private float intervalleMAJ1;

        protected float AngleLacet
        {
            get
            {
                if (Translation)
                {
                    angleLacet += Incr�mentAngleRotation;
                    angleLacet = MathHelper.WrapAngle(angleLacet);
                }
                return angleLacet;
            }
            set { angleLacet = value; }
        }

        protected PrimitiveDeBaseAnim�ePourPlateforme(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
           :base(jeu, homoth�tieInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public PrimitiveDeBaseAnim�ePourPlateforme(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, float intervalleMAJ1) : this(jeu, homoth�tieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            this.intervalleMAJ1 = intervalleMAJ1;
        }

        public override void Initialize()
        {
            Homoth�tie = Homoth�tieInitiale;
            InitialiserRotations();
            Position = PositionInitiale;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Incr�mentAngleRotation = 0;
            Temps�coul�DepuisMAJ = 0;
            base.Initialize();
        }

        protected override void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Homoth�tie) *
                    Matrix.CreateFromYawPitchRoll(AngleLacet, AngleTangage, AngleRoulis) *
                    Matrix.CreateTranslation(PositionInitiale + Position);
        }

        public override void Update(GameTime gameTime)
        {
            G�rerClavier();
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMise�Jour();
                Temps�coul�DepuisMAJ = 0;
            }
        }

        protected virtual void EffectuerMise�Jour()
        {
            if (Monde�Recalculer)
            {
                CalculerMatriceMonde();
                Monde�Recalculer = false;
            }
        }

        private void InitialiserRotations()
        {
            AngleLacet = RotationInitiale.Y;
        }

        protected virtual void G�rerClavier()
        {
            if (GestionInput.EstEnfonc�e(Keys.LeftControl) || GestionInput.EstEnfonc�e(Keys.RightControl))
            {
                if (GestionInput.EstNouvelleTouche(Keys.D1) || GestionInput.EstNouvelleTouche(Keys.NumPad1))
                {
                    Translation = !Translation;
                }
            }
            Monde�Recalculer = Monde�Recalculer || Translation;
        }
    }
}