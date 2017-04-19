using System;
using Microsoft.Xna.Framework;



namespace XnaGameClient
{
    public class PlateformeHorizontaleFlottante : Plateforme
    {
        const float LIMITE_ANGLE_FLOTTAISON = MathHelper.Pi / 180;

        float TempsÉcouléDepuisMAJ { get; set; }
       

        public PlateformeHorizontaleFlottante(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                                   Vector3 dimension, float angleDeFlottaison,float incrémentAngleDeFlottaison,float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incrémentAngleDeFlottaison, intervalleMAJ)
        {

        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        void GérerTranslationHorizontalePLateforme()
        {
            Translation = true;
            
            if (AngleDeFlottaison < LIMITE_ANGLE_FLOTTAISON)
            {
                Position += Vector3.Transform(Position, Matrix.CreateTranslation((float)Math.Sin(AngleDeFlottaison), 0, 0));
                AngleDeFlottaison += IncrémentAngleDeFlottaison;
            }
            else
            {
                Position -= Vector3.Transform(Position, Matrix.CreateTranslation((float)Math.Sin(AngleDeFlottaison), 0, 0));
                AngleDeFlottaison += IncrémentAngleDeFlottaison;
            }
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerTranslationHorizontalePLateforme();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
    }
}
