using System;
using Microsoft.Xna.Framework;



namespace XnaGameClient
{
    public class PlateformeHorizontaleFlottante : Plateforme
    {
        const float LIMITE_ANGLE_FLOTTAISON = MathHelper.Pi / 180;

        float TempsÉcouléDepuisMAJ { get; set; }
        public Vector3 PositionsPlateformesHorizontales { get; set;}


        public PlateformeHorizontaleFlottante(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                                   Vector3 dimension, float angleDeFlottaison,float incrémentAngleDeFlottaison,float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incrémentAngleDeFlottaison, intervalleMAJ)
        {
            PositionsPlateformesHorizontales = positionInitiale;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        void GérerTranslationHorizontalePlateforme()
        {
            Translation = true;

            //if (AngleDeFlottaison < LIMITE_ANGLE_FLOTTAISON)
            //{
            //   Position += Vector3.Transform(Position, Matrix.CreateTranslation((float)Math.Sin(AngleDeFlottaison), 0, 0));
            //   AngleDeFlottaison += IncrémentAngleDeFlottaison;
            //}
            //else
            //{
            //   Position -= Vector3.Transform(Position, Matrix.CreateTranslation((float)Math.Sin(AngleDeFlottaison), 0, 0));
            //   AngleDeFlottaison += IncrémentAngleDeFlottaison;
            //}

            Position = PositionInitiale + new Vector3((float)Math.Cos(AngleDeFlottaison), 0, (float)Math.Sin(AngleDeFlottaison));
            AngleDeFlottaison += IncrémentAngleDeFlottaison;
            AngleDeFlottaison %= 2 * (float)Math.PI;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerTranslationHorizontalePlateforme();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
    }
}
