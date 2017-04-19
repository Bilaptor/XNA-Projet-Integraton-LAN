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
        string NomSplineX { get; set; }
        string NomSplineZ { get; set; }

        string LigneLueSplineX { get; set; }
        string LigneLueSplineZ { get; set; }
        char[] séparateur = new char[] { ';' };
        string[] SplineX_Lue { get; set; }
        string[] SplineZ_Lue { get; set; }

        int SplineX_Nombres_Lue { get; set; }
        int SplineZ_Nombres_Lue { get; set; }

        public PlateformeSuivantUneSpline(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Color couleur,
                           Vector3 dimension, float angleDeFlottaison, float incrémentAngleDeFlottaison, float intervalleMAJ, string nomSplineX, string nomSplineZ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, couleur, dimension, angleDeFlottaison, incrémentAngleDeFlottaison, intervalleMAJ)
        {
            NomSplineX = nomSplineX;
            NomSplineZ = nomSplineZ;
        }

        public override void Initialize()
        {
            SplineX_Lue = new string[] { };
            SplineZ_Lue = new string[] { };
            base.Initialize();
        }

        void LireFichiersSplinesX()
        {
            if (File.Exists(NomSplineX))
            {
                StreamReader fichierLectureSplineX = new StreamReader(NomSplineX, System.Text.Encoding.UTF7);

                while (!fichierLectureSplineX.EndOfStream)
                {
                    LigneLueSplineX = fichierLectureSplineX.ReadLine();

                    SplineX_Lue = LigneLueSplineX.Split(séparateur);

                    SplineX_Nombres_Lue = int.Parse(SplineX_Lue[0]);
                }
            }
        }

        void LireFichiersSplinesZ()
        {
            if (File.Exists(NomSplineZ))
            {
                StreamReader fichierLectureSplineZ = new StreamReader(NomSplineZ, System.Text.Encoding.UTF7);

                while (!fichierLectureSplineZ.EndOfStream)
                {
                    LigneLueSplineZ = fichierLectureSplineZ.ReadLine();

                    SplineZ_Lue = LigneLueSplineZ.Split(séparateur);

                    SplineZ_Nombres_Lue = int.Parse(SplineZ_Lue[0]);
                }
            }
        }


        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
