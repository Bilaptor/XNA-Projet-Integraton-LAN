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
    public class PlateformeSuivantUneSpline : Plateforme
    {
        public PlateformeSuivantUneSpline(Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                           Vector3 dimension, float angleDeFlottaison, float incr�mentAngleDeFlottaison, float intervalleMAJ)
            : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incr�mentAngleDeFlottaison, intervalleMAJ)
        {
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
