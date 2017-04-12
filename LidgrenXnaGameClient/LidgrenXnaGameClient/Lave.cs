using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaGameClient
{
    class Lave : PlanTexturé
    {
        //à compléter
        double TempsÉcouléDepuisMAJ { get; set; }
        float MaxVariation { get; set; }
        float NombreOnde { get; set; }
        float FréquenceAngulaire { get; set; }
        float IntervalleVariation{ get; set; }
        float Angle { get; set; }

        public Lave(Game jeu, float homothétieInitiale, Vector3 rotationInitiale,Vector3 positionInitiale, Vector2 étendue, Vector2 charpente,string nomTexture, float maxVariation, float intervalleVariation,float intervalleMAJ)
            :base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, nomTexture, intervalleMAJ)
        {
            MaxVariation = maxVariation;
            IntervalleVariation = intervalleVariation;
        }

        public override void Initialize()
        {
            Angle = 0;
            TempsÉcouléDepuisMAJ = 0;
          
            base.Initialize();
            NombreOnde = (float)2 * MathHelper.Pi / (PtsSommets[NbColonnes, 0].X - PtsSommets[0, 0].X);
            FréquenceAngulaire = (float)(MathHelper.Pi);
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState premier = GraphicsDevice.RasterizerState;
            RasterizerState nouveau = new RasterizerState();
            nouveau.CullMode = CullMode.CullClockwiseFace;
            nouveau.FillMode = premier.FillMode;
            GraphicsDevice.RasterizerState = nouveau;
            base.Draw(gameTime);
            GraphicsDevice.RasterizerState = premier;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleVariation)
            {
                MouvementDrapeau(gameTime);
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public void MouvementDrapeau(GameTime gameTime)
        {
            for (int i = 0; i < NbColonnes + 1; ++i)
            {
                //Angle += MathHelper.PiOver4/15;
                for (int j = 0; j < NbRangées + 1; ++j)
                {
                    PtsSommets[i, j].Z = (1.3f*(float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * FréquenceAngulaire + NombreOnde * PtsSommets[i, j].X));
                }
            }
            InitialiserSommets();
        }


    }
}
