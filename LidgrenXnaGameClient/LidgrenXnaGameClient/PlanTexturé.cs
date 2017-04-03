using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaGameClient
{
   class PlanTexturé : Plan
   {
        // à compléter

        RessourcesManager<Texture2D> gestionnaireDeTextures;
        Texture2D texturePlan { get; set; }
        protected VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        string NomTexturePlan { get; set; }
        BlendState GestionAlpha { get; set; }

        public PlanTexturé(Game jeu, float homothétieInitiale, Vector3 rotationInitiale,Vector3 positionInitiale, Vector2 étendue, Vector2 charpente,string nomTexture, float intervalleMAJ)
         : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, intervalleMAJ)
      {
            NomTexturePlan = nomTexture;
        }

        protected override void CréerTableauSommets()
        {
            PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
            Sommets = new VertexPositionTexture[NbSommets];
            CréerTableauPointsTexture();
        }

        private void CréerTableauPointsTexture()
        {

            for (int j = 0; j <= NbRangées; j++)
            {
                for (int i = 0; i <= NbColonnes; i++)
                {
                    PtsTexture[i,j] = new Vector2((float)i/NbColonnes, (float)(NbRangées-j)/NbRangées);
                }
            }
          
        }

        protected override void InitialiserSommets() // Est appelée par base.Initialize()
        {
            int NoSommet = -1;
            for (int j = 0; j < NbRangées; ++j)
            {
                for (int i = 0; i < NbColonnes+1; ++i)
                {
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                }
            }
        }

        protected override void LoadContent()
        {

            gestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            texturePlan = gestionnaireDeTextures.Find(NomTexturePlan);
            base.LoadContent();
        }

        protected override void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = texturePlan;
            GestionAlpha = BlendState.AlphaBlend;
        }

        public override void Draw(GameTime gameTime)
        {
            BlendState oldBlendState = GraphicsDevice.BlendState;
            GraphicsDevice.BlendState = GestionAlpha;
            base.Draw(gameTime);
            GraphicsDevice.BlendState = oldBlendState;
        }

        protected override void DessinerTriangleStrip(int noStrip)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, noStrip * (NbTrianglesParStrip + 2), NbTrianglesParStrip);
        }
    }
}
