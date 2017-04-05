using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace XnaGameClient
{
    public abstract class PrimitiveDeBaseAnimÈePourPlateforme : PrimitiveDeBase
    {
        protected float HomothÈtie { get; set; }
        protected Vector3 Position { get; set; }
        float IntervalleMAJ { get; set; }
        float Temps…coulÈDepuisMAJ { get; set; }
        protected InputManager GestionInput { get; private set; }
        float IncrÈmentAngleRotation { get; set; }
        protected bool Translation { get; set; }
        protected bool Monde¿Recalculer { get; set; }
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
                    angleLacet += IncrÈmentAngleRotation;
                    angleLacet = MathHelper.WrapAngle(angleLacet);
                }
                return angleLacet;
            }
            set { angleLacet = value; }
        }

        protected PrimitiveDeBaseAnimÈePourPlateforme(Game jeu, float homothÈtieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
           :base(jeu, homothÈtieInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public PrimitiveDeBaseAnimÈePourPlateforme(Game jeu, float homothÈtieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, float intervalleMAJ1) : this(jeu, homothÈtieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            this.intervalleMAJ1 = intervalleMAJ1;
        }

        public override void Initialize()
        {
            HomothÈtie = HomothÈtieInitiale;
            InitialiserRotations();
            Position = PositionInitiale;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            IncrÈmentAngleRotation = 0;
            Temps…coulÈDepuisMAJ = 0;
            base.Initialize();
        }

        protected override void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(HomothÈtie) *
                    Matrix.CreateFromYawPitchRoll(AngleLacet, AngleTangage, AngleRoulis) *
                    Matrix.CreateTranslation(PositionInitiale + Position);
        }

        public override void Update(GameTime gameTime)
        {
            GÈrerClavier();
            float Temps…coulÈ = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps…coulÈDepuisMAJ += Temps…coulÈ;
            if (Temps…coulÈDepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMise¿Jour();
                Temps…coulÈDepuisMAJ = 0;
            }
        }

        protected virtual void EffectuerMise¿Jour()
        {
            if (Monde¿Recalculer)
            {
                CalculerMatriceMonde();
                Monde¿Recalculer = false;
            }
        }

        private void InitialiserRotations()
        {
            AngleLacet = RotationInitiale.Y;
        }

        protected virtual void GÈrerClavier()
        {
            if (GestionInput.EstEnfoncÈe(Keys.LeftControl) || GestionInput.EstEnfoncÈe(Keys.RightControl))
            {
                if (GestionInput.EstNouvelleTouche(Keys.D1) || GestionInput.EstNouvelleTouche(Keys.NumPad1))
                {
                    Translation = !Translation;
                }
            }
            Monde¿Recalculer = Monde¿Recalculer || Translation;
        }
    }
}