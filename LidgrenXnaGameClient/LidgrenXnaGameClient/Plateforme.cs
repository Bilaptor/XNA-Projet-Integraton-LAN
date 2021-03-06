﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace XnaGameClient
{
    public class Plateforme : PrimitiveDeBaseAniméePourPlateforme, IPhysique
    {
        const int NB_SOMMETS = 14;
        const int NB_TRIANGLES = 12;
        const int LARGEUR_PLATEFORME = 6;
        Color Couleur { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3 Origine { get; set; }
        float DeltaX { get; set; }
        float DeltaY { get; set; }
        float DeltaZ { get; set; }
        BasicEffect EffetDeBase { get; set; }

        public Vector3 PositionPlateforme { get; set; }
        public BoundingBox ZoneDeCollisionPlateforme { get; set; }
        Vector3 DimensionPlateforme { get; set; }

        protected float IntervalleMAJ { get; set; }
        string NomTexturePlateforme { get; set; }

        protected float AngleDeFlottaison { get; set; }
        protected float IncrémentAngleDeFlottaison { get; set; }


        public Plateforme(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                           Vector3 dimension, float angleDeFlottaison, float incrémentAngleDeFlottaison,float intervalleMAJ)
         : base(game, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            DeltaX = dimension.X;
            DeltaY = dimension.Y;
            DeltaZ = dimension.Z;
            Origine = new Vector3(DeltaX / 2, DeltaY / 2, DeltaZ / 2);
            Couleur = couleur;
            AngleDeFlottaison = angleDeFlottaison;
            IncrémentAngleDeFlottaison = incrémentAngleDeFlottaison;
            IntervalleMAJ = intervalleMAJ;
            DimensionPlateforme = dimension;
            PositionPlateforme = positionInitiale;
        }

        public override void Initialize()
        {
            Sommets = new VertexPositionColor[NB_SOMMETS];
            ZoneDeCollisionPlateforme = new BoundingBox(Vector3.Zero, DimensionPlateforme);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.VertexColorEnabled = true;
            base.LoadContent();
        }

        protected override void InitialiserSommets()
        {
            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(Origine.X, Origine.Y + DeltaY, Origine.Z);
            points[1] = new Vector3(Origine.X, Origine.Y + DeltaY, Origine.Z + DeltaZ);
            points[2] = new Vector3(Origine.X + DeltaX, Origine.Y + DeltaY, Origine.Z + DeltaZ);
            points[3] = new Vector3(Origine.X + DeltaX, Origine.Y + DeltaY, Origine.Z);
            points[4] = Origine;
            points[5] = new Vector3(Origine.X, Origine.Y, Origine.Z + DeltaZ);
            points[6] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z + DeltaZ);
            points[7] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z);

            Sommets[0] = new VertexPositionColor(points[1], Couleur);
            Sommets[1] = new VertexPositionColor(points[2], Couleur);
            Sommets[2] = new VertexPositionColor(points[5], Couleur);

            Sommets[3] = new VertexPositionColor(points[6], Couleur);
            Sommets[4] = new VertexPositionColor(points[7], Couleur);
            Sommets[5] = new VertexPositionColor(points[2], Couleur);

            Sommets[6] = new VertexPositionColor(points[3], Couleur);
            Sommets[7] = new VertexPositionColor(points[1], Couleur);
            Sommets[8] = new VertexPositionColor(points[0], Couleur);

            Sommets[9] = new VertexPositionColor(points[5], Couleur);
            Sommets[10] = new VertexPositionColor(points[4], Couleur);
            Sommets[11] = new VertexPositionColor(points[7], Couleur);

            Sommets[12] = new VertexPositionColor(points[0], Couleur);
            Sommets[13] = new VertexPositionColor(points[3], Couleur);

        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
            }
        }

        //Modifie la position du volume en modifiant les deux coins le définissant en fonction de la position envoyé
        private void SetPositionVolume(Vector3 position)
        {
            ZoneDeCollisionPlateforme = new BoundingBox(new Vector3(position.X + LARGEUR_PLATEFORME, position.Y, position.Z + LARGEUR_PLATEFORME) - DimensionPlateforme / 2, new Vector3(position.X + LARGEUR_PLATEFORME, position.Y, position.Z + LARGEUR_PLATEFORME) + DimensionPlateforme / 2);
        }

        public BoundingBox GetVolume()
        {
            SetPositionVolume(this.Position);
            return ZoneDeCollisionPlateforme;
        }

        public void SetEnCollision(bool enCollision, IPhysique autre)
        { }
    }
}