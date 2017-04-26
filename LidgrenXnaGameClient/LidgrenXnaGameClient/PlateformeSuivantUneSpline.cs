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

        float TempsÉcouléDepuisMAJ { get; set; }

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

        public PlateformeSuivantUneSpline(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                           Vector3 dimension, float angleDeFlottaison, float incrémentAngleDeFlottaison, float intervalleMAJ, string nomSplineX, string nomSplineZ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incrémentAngleDeFlottaison, intervalleMAJ)
        {
            NomFichierSplineX = nomSplineX;
            NomFichierSplineZ = nomSplineZ;
            Position_X = positionInitiale.X;
            Position_Z = positionInitiale.Z;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;

            SplineX_Lue = new string[] { };
            SplineZ_Lue = new string[] { };
            Liste_SplineX = new List<int>();
            Liste_SplineZ = new List<int>();
            Tableau_SplineX = new int[] { };
            Tableau_SplineZ = new int[] { };

            base.Initialize();
        }


        void LireFichiersSplineX()
        {
            if (File.Exists(NomFichierSplineX))
            {
                char[] séparateur = new char[] { ';' };
                StreamReader fichierLectureSplineX = new StreamReader(NomFichierSplineX, System.Text.Encoding.UTF7);

                while (!fichierLectureSplineX.EndOfStream)
                {
                    LigneLueSplineX = fichierLectureSplineX.ReadLine();

                    SplineX_Lue = LigneLueSplineX.Split(séparateur);

                    SplineX_Nombres_Lue = int.Parse(SplineX_Lue[0]);

                    AjouterValeursAuSplineX(SplineX_Nombres_Lue);
                }
            }
        }

        void LireFichiersSplineZ()
        {
            if (File.Exists(NomFichierSplineZ))
            {
                char[] séparateur = new char[] { ';' };
                StreamReader fichierLectureSplineZ = new StreamReader(NomFichierSplineZ, System.Text.Encoding.UTF7);

                while (!fichierLectureSplineZ.EndOfStream)
                {
                    LigneLueSplineZ = fichierLectureSplineZ.ReadLine();

                    SplineZ_Lue = LigneLueSplineZ.Split(séparateur);

                    SplineZ_Nombres_Lue = int.Parse(SplineZ_Lue[0]);

                    AjouterValeursAuSplineZ(SplineZ_Nombres_Lue);
                }
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

        void CréerTableauxSpline()
        {
            Tableau_SplineX = Liste_SplineX.ToArray();
            Tableau_SplineZ = Liste_SplineZ.ToArray();
        }

        void DéplacerPlateforme()
        {
            for(int cpt = 0; cpt < Liste_SplineX.Count; ++cpt)
            {
                //Position_X = Tableau_SplineX[cpt] + Tableau_SplineX[cpt + 1] * Position_X + Tableau_SplineX[cpt + 2] * (Math.Pow(Position_X, 2)) + Tableau_SplineX[cpt + 3] * (Math.Pow(Position_X, 3));
                //Position_Z = Tableau_SplineZ[cpt] + Tableau_SplineZ[cpt + 1] * Position_Z + Tableau_SplineZ[cpt + 2] * (Math.Pow(Position_Z, 2)) + Tableau_SplineZ[cpt + 3] * (Math.Pow(Position_Z, 3));
                //Position += Vector3.Transform(Position, Matrix.CreateTranslation(new Vector3((float)Position_X, 0, (float)Position_Z)));
            }
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                DéplacerPlateforme();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
    }
}
