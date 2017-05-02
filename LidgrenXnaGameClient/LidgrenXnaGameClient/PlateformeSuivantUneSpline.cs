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
using System.IO;


namespace XnaGameClient
{
    public class PlateformeSuivantUneSpline : Plateforme
    {
        const int VITESSE = 5;
        

        float Temps�coul�DepuisMAJ { get; set; }

        string NomFichierSplineX { get; set; }
        string NomFichierSplineZ { get; set; }

        string LigneLueSplineX { get; set; }
        string LigneLueSplineZ { get; set; }

        string[] SplineX_Lue { get; set; }
        string[] SplineZ_Lue { get; set; }

        int SplineX_Nombres_Lue { get; set; }
        int SplineZ_Nombres_Lue { get; set; }

        List<int> Liste_SplineX { get; set; }
        List<int> Liste_SplineZ { get; set; }

        int[] Tableau_SplineX { get; set; }
        int[] Tableau_SplineZ { get; set; }

        double Position_X { get; set; }
        double Position_Z { get; set; }

        public PlateformeSuivantUneSpline(Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                           Vector3 dimension, float angleDeFlottaison, float incr�mentAngleDeFlottaison, float intervalleMAJ, string nomSplineX, string nomSplineZ)
            : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incr�mentAngleDeFlottaison, intervalleMAJ)
        {
            NomFichierSplineX = nomSplineX;
            NomFichierSplineZ = nomSplineZ;
        }

        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = 0;

            SplineX_Lue = new string[] { };
            SplineZ_Lue = new string[] { };
            Liste_SplineX = new List<int>();
            Liste_SplineZ = new List<int>();
            Tableau_SplineX = new int[] { };
            Tableau_SplineZ = new int[] { };

            Position_X = Position.X;
            Position_Z = Position.Z;

            base.Initialize();
        }


        void LireFichiersSplineX()
        {
            if (File.Exists(NomFichierSplineX))
            {
                char[] s�parateur = new char[] { ';' };
                StreamReader fichierLectureSplineX = new StreamReader(NomFichierSplineX, System.Text.Encoding.UTF7);

                while (!fichierLectureSplineX.EndOfStream)
                {
                    LigneLueSplineX = fichierLectureSplineX.ReadLine();

                    SplineX_Lue = LigneLueSplineX.Split(s�parateur);

                    SplineX_Nombres_Lue = int.Parse(SplineX_Lue[0]);

                    AjouterValeursAuSplineX(SplineX_Nombres_Lue);
                }
                fichierLectureSplineX.Close();
            }
        }

        void LireFichiersSplineZ()
        {
            if (File.Exists(NomFichierSplineZ))
            {
                char[] s�parateur = new char[] { ';' };
                StreamReader fichierLectureSplineZ = new StreamReader(NomFichierSplineZ, System.Text.Encoding.UTF7);

                while (!fichierLectureSplineZ.EndOfStream)
                {
                    LigneLueSplineZ = fichierLectureSplineZ.ReadLine();

                    SplineZ_Lue = LigneLueSplineZ.Split(s�parateur);

                    SplineZ_Nombres_Lue = int.Parse(SplineZ_Lue[0]);

                    AjouterValeursAuSplineZ(SplineZ_Nombres_Lue);
                }
                fichierLectureSplineZ.Close();
            }
        }

        void AjouterValeursAuSplineX(int valeur)
        {
            Liste_SplineX.Add(valeur);
        }

        void AjouterValeursAuSplineZ(int valeur)
        {
            Liste_SplineZ.Add(valeur);
        }

        void Cr�erTableauxSpline()
        {
            LireFichiersSplineX();
            LireFichiersSplineZ();
            Tableau_SplineX = Liste_SplineX.ToArray();
            Tableau_SplineZ = Liste_SplineZ.ToArray();
        }

        void D�placerPlateforme()
        {
            Cr�erTableauxSpline();

            for (int cpt = 0; cpt < Liste_SplineX.Count - 1; ++cpt)
            {
                Position_X = Tableau_SplineX[cpt] + Tableau_SplineX[cpt + 1] * Position.X + Tableau_SplineX[cpt + 2] * (Math.Pow(Position.X, 2)) + Tableau_SplineX[cpt + 3] * (Math.Pow(Position.X, 3));
                Position_Z = Tableau_SplineZ[cpt] + Tableau_SplineZ[cpt + 1] * Position.Z + Tableau_SplineZ[cpt + 2] * (Math.Pow(Position.Z, 2)) + Tableau_SplineZ[cpt + 3] * (Math.Pow(Position.Z, 3));
                Position += Vector3.Transform(Position, Matrix.CreateTranslation(new Vector3((float)Position_X, 0, (float)Position_Z)));
            }
        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;

            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                D�placerPlateforme();
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
    }
}

//const float VITESSE = 5f;

//List<Vector3> ListeDeCoordonn�es { get; set; }
//float[] Lengths { get; set; }
//Vector3[] Directions { get; set; }
//float StagePos { get; set; }
//int StageIndex { get; set; }


//        public PlateformeSuivantUneSpline(Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
//                           Vector3 dimension, float angleDeFlottaison, float incr�mentAngleDeFlottaison, float intervalleMAJ, List<Vector3> listeDeCoordonn�es)
//            : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incr�mentAngleDeFlottaison, intervalleMAJ)
//        {
//            ListeDeCoordonn�es = listeDeCoordonn�es;
//        }

//        public override void Initialize()
//        {
//            Temps�coul�DepuisMAJ = 0;
//            base.Initialize();
//        }


//        void Build()
//        {
//            Lengths = new float[ListeDeCoordonn�es.Count - 1];
//            Directions = new Vector3[ListeDeCoordonn�es.Count - 1];
//            for (int i = 0; i < ListeDeCoordonn�es.Count - 1; i++)
//            {
//                Directions[i] = ListeDeCoordonn�es[i + 1] - ListeDeCoordonn�es[i];
//                Lengths[i] = Directions[i].Length();
//                Directions[i].Normalize();
//            }
//        }

//        void D�placerPlateforme(float temps)
//        {
//            Build();

//            if (StageIndex != ListeDeCoordonn�es.Count - 1)
//            {
//                StagePos += VITESSE * temps;
//                while (StagePos > Lengths[StageIndex])
//                {
//                    StagePos -= Lengths[StageIndex];
//                    StageIndex++;
//                    if (StageIndex == ListeDeCoordonn�es.Count - 1)
//                    {
//                        Position = ListeDeCoordonn�es[StageIndex];
//                    }
//                }
//                Position = ListeDeCoordonn�es[StageIndex] + Directions[StageIndex] * StagePos;
//            }
//        }

//        public override void Update(GameTime gameTime)
//        {
//            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
//            Temps�coul�DepuisMAJ += Temps�coul�;

//            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
//            {
//                D�placerPlateforme(Temps�coul�DepuisMAJ);
//                Temps�coul�DepuisMAJ = 0;
//            }
//            base.Update(gameTime);
//        }
//    }
//}

