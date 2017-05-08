using System;
using Microsoft.Xna.Framework;



namespace XnaGameClient
{
    public class PlateformeVerticaleFlottante : Plateforme
    {
        const float INCRÉMENT_ANGLE_FLOTTAISON = MathHelper.Pi / 360;
        const float LIMITE_ANGLE_FLOTTAISON = MathHelper.Pi / 180;

        float TempsÉcouléDepuisMAJ { get; set; }
        float AngleFlottaison { get; set; }



        public PlateformeVerticaleFlottante(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                                   Vector3 dimension, float angleDeFlottaison, float incrémentAngleDeFlottaison, float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incrémentAngleDeFlottaison, intervalleMAJ)
        {
        }


        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        void GérerTranslationVertcicalePlateforme()
        {
            Translation = true;

            //if (AngleFlottaison < LIMITE_ANGLE_FLOTTAISON)
            //{
            //    Position += Vector3.Transform(Position, Matrix.CreateTranslation(0, (float)Math.Sin(AngleFlottaison), 0));
            //    AngleFlottaison += INCRÉMENT_ANGLE_FLOTTAISON;
            //}
            //else
            //{
            //    Position -= Vector3.Transform(Position, Matrix.CreateTranslation(0, (float)Math.Sin(AngleFlottaison), 0));
            //    AngleFlottaison += INCRÉMENT_ANGLE_FLOTTAISON;
            //}

            Position = PositionInitiale + new Vector3(0, (float)Math.Sin(AngleDeFlottaison), 0);
            AngleDeFlottaison += IncrémentAngleDeFlottaison;
            AngleDeFlottaison %= 2 * (float)Math.PI;

        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerTranslationVertcicalePlateforme();
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }
    }
}
