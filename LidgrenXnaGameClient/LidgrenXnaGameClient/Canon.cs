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
    public class Canon : ObjetDeBase
    {
        public Canon(Game jeu, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomMod�le ,�chelleInitiale, rotationInitiale, positionInitiale)
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
