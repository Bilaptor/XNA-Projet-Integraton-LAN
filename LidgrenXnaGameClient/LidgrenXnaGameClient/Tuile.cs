using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaGameClient
{
    public abstract class Tuile : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES = 2;
        protected Vector3[,] PtsSommets { get; private set; }
        Vector3 Origine { get; set; }
        Vector2 Delta { get; set; }
        Vector3[] CréerTableauP { get; set; }
        protected BasicEffect EffetDeBase { get; private set; }


        public Tuile(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, float intervalleMAJ, Vector3[] créerTableauP)
           : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Delta = new Vector2(étendue.X, étendue.Y);
            Origine = new Vector3(-Delta.X / 2, -Delta.Y / 2, 0); //pour centrer la primitive au point (0,0,0)
            CréerTableauP = créerTableauP;
        }

        public override void Initialize()
        {
            NbSommets = NB_TRIANGLES + 2;
            PtsSommets = new Vector3[2, 2];
            CréerTableauSommets();
            CréerTableauPoints();
            base.Initialize();
        }

        protected abstract void CréerTableauSommets();

        protected override void LoadContent()
        {
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
            base.LoadContent();
        }

        protected abstract void InitialiserParamètresEffetDeBase();

        private void CréerTableauPoints()
        {
            PtsSommets[0, 0] = CréerTableauP[0];
            PtsSommets[1, 0] = CréerTableauP[1];
            PtsSommets[0, 1] = CréerTableauP[2];
            PtsSommets[1, 1] = CréerTableauP[3];
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                DessinerTriangleStrip();
            }
        }



        protected abstract void DessinerTriangleStrip();
    }
}

