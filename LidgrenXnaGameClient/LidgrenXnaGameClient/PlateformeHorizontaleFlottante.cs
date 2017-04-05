using System;
using Microsoft.Xna.Framework;



namespace XnaGameClient
{
    public class PlateformeHorizontaleFlottante : Plateforme
    {
        const float INCRÉMENT_ANGLE_FLOTTAISON = MathHelper.Pi / 360;
        const float LIMITE_ANGLE_FLOTTAISON = MathHelper.Pi / 180;

        float TempsÉcouléDepuisMAJ { get; set; }
        float AngleFlottaison { get; set; }


        public PlateformeHorizontaleFlottante(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, string nomTextureCube,
                                   Vector3 dimension, float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, nomTextureCube, dimension, intervalleMAJ)
        {

        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            AngleFlottaison = MathHelper.Pi / 360;
            base.Initialize();
        }

        void GérerTranslationHorizontalePLateforme()
        {
            Translation = true;
            
            if (AngleFlottaison < LIMITE_ANGLE_FLOTTAISON)
            {
                Position += Vector3.Transform(Position, Matrix.CreateTranslation((float)Math.Sin(AngleFlottaison), 0, 0));
                AngleFlottaison += INCRÉMENT_ANGLE_FLOTTAISON;
            }
            else
            {
                Position -= Vector3.Transform(Position, Matrix.CreateTranslation((float)Math.Sin(AngleFlottaison), 0, 0));
                AngleFlottaison += INCRÉMENT_ANGLE_FLOTTAISON;
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
