using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace XnaGameClient
{
    public class Plateforme : PrimitiveDeBaseAniméePourPlateforme, ICollisionable
    {
        protected const int NB_SOMMETS = 16;
        protected const int NB_TRIANGLES = 14;
        protected string NomTexture { get; set; }
        RessourcesManager<Texture2D> gestionnaireDeTextures;
        VertexPositionTexture[] Sommets { get; set; }
        BlendState GestionAlpha { get; set; }
        protected Vector3[] PtsSommets { get; set; }
        protected Vector2[,] PtsTexture { get; set; }
        BoundingSphere ICollisionable.SphèreDeCollision { get;}
        Texture2D texturePlanTexturé;
        protected Vector3 Origine { get; set; }
        protected float DeltaX { get; set; }
        protected float DeltaY { get; set; }
        protected float DeltaZ { get; set; }
        BasicEffect EffetDeBase { get; set; }
        protected float IntervalleMAJ { get; set; }
        string NomTextureCube { get; set; }
        


        public Plateforme(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, string nomTextureCube, 
                           Vector3 dimension, float intervalleMAJ)
         : base(game, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            DeltaX = dimension.X;
            DeltaY = dimension.Y; 
            DeltaZ = dimension.Z;
            Origine = new Vector3(DeltaX / 2, DeltaY / 2, DeltaZ / 2);
            NomTextureCube = nomTextureCube;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            Sommets = new VertexPositionTexture[NB_SOMMETS];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            texturePlanTexturé = gestionnaireDeTextures.Find(NomTextureCube);
            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = texturePlanTexturé;
            base.LoadContent();
        }

        private void CréerTableauPointsSommets()
        {
            PtsSommets = new Vector3[8];

            PtsSommets[0] = Origine;
            PtsSommets[1] = new Vector3(Origine.X, Origine.Y - DeltaY, Origine.Z);
            PtsSommets[2] = new Vector3(Origine.X - DeltaX, Origine.Y, Origine.Z);
            PtsSommets[3] = new Vector3(Origine.X - DeltaX, Origine.Y - DeltaY, Origine.Z);
            PtsSommets[4] = new Vector3(Origine.X - DeltaX, Origine.Y, Origine.Z - DeltaZ);
            PtsSommets[5] = new Vector3(Origine.X - DeltaX, Origine.Y - DeltaY, Origine.Z - DeltaZ);
            PtsSommets[6] = new Vector3(Origine.X, Origine.Y, Origine.Z - DeltaZ);
            PtsSommets[7] = new Vector3(Origine.X, Origine.Y - DeltaY, Origine.Z - DeltaZ);
        }

        private void CréerTableauPointsTexture()
        {
            PtsTexture = new Vector2[2, 2];

            PtsTexture[0, 0] = new Vector2(0, 1);
            PtsTexture[1, 0] = new Vector2(1 / 3.0000f, 1);
            PtsTexture[0, 1] = new Vector2(0, 0);
            PtsTexture[1, 1] = new Vector2(1 / 3.00000f, 0);
        }

        protected override void InitialiserSommets()
        {
            CréerTableauPointsSommets();
            CréerTableauPointsTexture();

            Sommets[0] = new VertexPositionTexture(PtsSommets[7], PtsTexture[0,0]);
            Sommets[1] = new VertexPositionTexture(PtsSommets[6], PtsTexture[0,1]);
            Sommets[2] = new VertexPositionTexture(PtsSommets[5], PtsTexture[1,0]);
            Sommets[3] = new VertexPositionTexture(PtsSommets[4], PtsTexture[1,1]);

            Sommets[4] = new VertexPositionTexture(PtsSommets[3], PtsTexture[0,0]);
            Sommets[5] = new VertexPositionTexture(PtsSommets[2], PtsTexture[0,1]);
            Sommets[6] = new VertexPositionTexture(PtsSommets[1], PtsTexture[1,0]);
            Sommets[7] = new VertexPositionTexture(PtsSommets[0], PtsTexture[1,1]);

            Sommets[8] = new VertexPositionTexture(PtsSommets[2], PtsTexture[0,0]);
            Sommets[9] = new VertexPositionTexture(PtsSommets[4], PtsTexture[0,1]);
            Sommets[10] = new VertexPositionTexture(PtsSommets[0], PtsTexture[1,0]);
            Sommets[11] = new VertexPositionTexture(PtsSommets[6], PtsTexture[1,1]);

            Sommets[12] = new VertexPositionTexture(PtsSommets[1], PtsTexture[0,0]);
            Sommets[13] = new VertexPositionTexture(PtsSommets[7], PtsTexture[0,1]);
            Sommets[14] = new VertexPositionTexture(PtsSommets[3], PtsTexture[1,0]);
            Sommets[15] = new VertexPositionTexture(PtsSommets[5], PtsTexture[1,1]);
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
            }
        }

        public virtual bool EstEnCollision(object autreObjet)
        {
            return false;
        }
        
    }
}