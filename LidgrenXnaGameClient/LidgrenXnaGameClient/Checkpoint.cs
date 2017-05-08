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
    public class Checkpoint : PrimitiveDeBaseAnim�e
    {
        const int NB_DE_PLATEFORMES_UN_TYPE = 25;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const int LARGEUR_PLATEFORME = 6;
        const int DIFF�RENCE_ENTRE_HAUTEUR_CHECKPOINT_ET_HAUTEUR_PLATEFORMES = 5;
        const int NB_SOMMETS = 18;
        const int NB_TRIANGLES = 6;
        
        Random g�n�rateurAl�atoire = new Random();
        Vector3 Dimension { get; set; }
        Color Couleur { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3 Origine { get; set; }
        float DeltaX { get; set; }
        float DeltaY { get; set; }
        float DeltaZ { get; set; }
        BasicEffect EffetDeBase { get; set; }

        Vector3[][] TableauPositionPlateformes { get; set; }
        Vector3[] TableauPositionsPlateformesHorizontales { get; set;}
        Vector3[] TableauPositionsPlateformesVerticales { get; set; }

        public Vector3 PositionCheckpoint { get; set; }
        Vector3 PositionCam�ra { get; set; }

        Plateforme Plateforme { get; set; }
        
        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }

        BoundingBox ZoneDeCollisionCheckPoint { get; set; }
        BoundingBox ZoneModel { get; set; }

        public Checkpoint(Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                          Vector3 dimension, float intervalleMAJ, Vector3 positionCam�ra)
         : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Couleur = couleur;
            DeltaX = dimension.X;
            DeltaY = dimension.Y;
            DeltaZ = dimension.Z;
            Origine = new Vector3(-DeltaX / 2, -DeltaY / 2, -DeltaZ / 2);
            PositionCheckpoint = positionInitiale;
            PositionCam�ra = positionCam�ra;
        }

        public override void Initialize()
        {
            Dimension = new Vector3(2.5f, 2.5f, 2.5f);
            TableauPositionsPlateformesHorizontales = new Vector3[NB_DE_PLATEFORMES_UN_TYPE];
            TableauPositionsPlateformesVerticales = new Vector3[NB_DE_PLATEFORMES_UN_TYPE];
            Sommets = new VertexPositionColor[NB_SOMMETS];
            ZoneDeCollisionCheckPoint = new BoundingBox(PositionCheckpoint - new Vector3(LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2), PositionCheckpoint + new Vector3(LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2));
            ZoneModel = new BoundingBox(PositionCam�ra - new Vector3(LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2), PositionCam�ra + new Vector3(LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2));
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
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(Origine.X + DeltaX / 2, Origine.Y + DeltaY, Origine.Z + DeltaZ / 2);
            points[1] = Origine;
            points[2] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z);
            points[3] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z + DeltaZ);
            points[4] = new Vector3(Origine.X, Origine.Y, Origine.Z + DeltaZ);

            Sommets[0] = new VertexPositionColor(points[4], Couleur);
            Sommets[1] = new VertexPositionColor(points[0], Couleur);
            Sommets[2] = new VertexPositionColor(points[3], Couleur);

            Sommets[3] = new VertexPositionColor(points[3], Couleur);
            Sommets[4] = new VertexPositionColor(points[0], Couleur);
            Sommets[5] = new VertexPositionColor(points[2], Couleur);

            Sommets[6] = new VertexPositionColor(points[2], Couleur);
            Sommets[7] = new VertexPositionColor(points[0], Couleur);
            Sommets[8] = new VertexPositionColor(points[1], Couleur);

            Sommets[9] = new VertexPositionColor(points[1], Couleur);
            Sommets[10] = new VertexPositionColor(points[0], Couleur);
            Sommets[11] = new VertexPositionColor(points[4], Couleur);

            Sommets[12] = new VertexPositionColor(points[2], Couleur);
            Sommets[13] = new VertexPositionColor(points[1], Couleur);
            Sommets[14] = new VertexPositionColor(points[3], Couleur);

            Sommets[15] = new VertexPositionColor(points[1], Couleur);
            Sommets[16] = new VertexPositionColor(points[4], Couleur);
            Sommets[17] = new VertexPositionColor(points[3], Couleur);

        }


        void G�rerDisparitionEtNouvelleApparitionCheckpoint()
        {
            if(ZoneDeCollisionCheckPoint.Intersects(ZoneModel))
            {
                for (int i = Game.Components.Count - 1; i >= 0; --i)
                {
                    if (Game.Components[i] is Checkpoint)
                    {
                        Game.Components.RemoveAt(i);
                    }
                }
                AllerChercherNouvellePositionCheckpoint();
                Game.Components.Add(new CheckpointAnim�(Game, 1f, new Vector3(MathHelper.Pi,0,0), PositionCheckpoint, Color.Yellow, Dimension, INTERVALLE_MAJ_STANDARD, Cam�raJeu.Position));
            }
        }

        void AllerChercherNouvellePositionCheckpoint()
        {
            ChercherPositionsDesPlateformes();

            TableauPositionPlateformes = new Vector3[][] { TableauPositionsPlateformesHorizontales, TableauPositionsPlateformesVerticales };
            PositionCheckpoint = TableauPositionPlateformes[g�n�rateurAl�atoire.Next(0, 2)][g�n�rateurAl�atoire.Next(0, NB_DE_PLATEFORMES_UN_TYPE)] + new Vector3(LARGEUR_PLATEFORME, DIFF�RENCE_ENTRE_HAUTEUR_CHECKPOINT_ET_HAUTEUR_PLATEFORMES, LARGEUR_PLATEFORME);
            ZoneDeCollisionCheckPoint = new BoundingBox(PositionCheckpoint - new Vector3(LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2), PositionCheckpoint + new Vector3(LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2, LARGEUR_PLATEFORME / 2));
        }

        void ChercherPositionsDesPlateformes()
        {
            foreach (PlateformeHorizontaleFlottante T in Game.Components.Where(c => c is PlateformeHorizontaleFlottante))
            {
                for (int cpt = 0; cpt < NB_DE_PLATEFORMES_UN_TYPE; ++cpt)
                {
                    TableauPositionsPlateformesHorizontales[cpt] = T.PositionsPlateformesHorizontales;
                }
            }

            foreach (PlateformeVerticaleFlottante T in Game.Components.Where(c => c is PlateformeVerticaleFlottante))
            {
                for (int cpt = 0; cpt < NB_DE_PLATEFORMES_UN_TYPE; ++cpt)
                {
                    TableauPositionsPlateformesVerticales[cpt] = T.PositionsPlateformesVerticales;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;

            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                PositionCam�ra = Cam�raJeu.Position;
                ZoneModel = new BoundingBox(PositionCam�ra - new Vector3(LARGEUR_PLATEFORME/2, LARGEUR_PLATEFORME/2, LARGEUR_PLATEFORME/2), PositionCam�ra + new Vector3(LARGEUR_PLATEFORME/2, LARGEUR_PLATEFORME/2, LARGEUR_PLATEFORME/2));
                G�rerDisparitionEtNouvelleApparitionCheckpoint();
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
    }
}
