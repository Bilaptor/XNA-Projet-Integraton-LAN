using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace XnaGameClient
{
    public class CheckpointAnimé : Checkpoint
    {
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }

        public CheckpointAnimé(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                               Vector3 dimension, float intervalleMAJ, Vector3 positionCaméra)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, couleur, dimension, intervalleMAJ, positionCaméra)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        void GérerRotationLacet()
        {
            Lacet = true;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerRotationLacet();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
    }
}
