using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaGameClient
{
    public class ObjetDeDémo : ObjetDeBase, IPausable
    {
        const float MAX_GROSSISSEMENT = 1F;
        const float MIN_GROSSISSEMENT = 0.005F;
        const float ÉCHELLEGRANDISSEMENT = 0.0008f;
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        const float CONSTANTE_ROTATION = MathHelper.Pi / 120;
        InputManager GestionInput { get; set; }
        Vector3 VecteurModifiant { get; set; }
        Vector3 RotationIni { get; set; }


        public ObjetDeDémo(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
            RotationIni = rotationInitiale;
        }

        public override void Initialize()
        {
            base.Initialize();
            Vector3 vecteurModifiant = new Vector3(0, 0, 0);

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            GérerClavier();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                Rotation = new Vector3(Rotation.X + VecteurModifiant.X, Rotation.Y + VecteurModifiant.Y, Rotation.Z + VecteurModifiant.Z);
                TempsÉcouléDepuisMAJ = 0;
                CalculerGrand();
            }
        }

        public void GérerPause(bool enPause)
        {
            this.Enabled = !enPause;
        }

        private void CalculerGrand()
        {
            if (GestionInput.EstEnfoncée(Keys.Subtract))
            {
                Échelle -= ÉCHELLEGRANDISSEMENT;
                if (Échelle < MIN_GROSSISSEMENT)
                {
                    Échelle = MIN_GROSSISSEMENT;
                }
            }
            else if (GestionInput.EstEnfoncée(Keys.Add))
            {
                Échelle += ÉCHELLEGRANDISSEMENT;
                if (Échelle > MAX_GROSSISSEMENT)
                {
                    Échelle = MAX_GROSSISSEMENT;
                }
            }
        }

        public override Matrix GetMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
            return Monde;
        }




        private void GérerClavier()
        {
            if (GestionInput.EstClavierActivé)
            {

                if (GestionInput.EstNouvelleTouche(Keys.D1))
                {
                    if (VecteurModifiant.X == 0)
                    {
                        VecteurModifiant = new Vector3(CONSTANTE_ROTATION, VecteurModifiant.Y, VecteurModifiant.Z);
                    }
                    else
                    {
                        VecteurModifiant = new Vector3(0, VecteurModifiant.Y, VecteurModifiant.Z);
                    }
                }


                if (GestionInput.EstNouvelleTouche(Keys.D2))
                {
                    if (VecteurModifiant.Y == 0)
                    {
                        VecteurModifiant = new Vector3(VecteurModifiant.X, CONSTANTE_ROTATION, VecteurModifiant.Z);
                    }
                    else
                    {
                        VecteurModifiant = new Vector3(VecteurModifiant.X, 0, VecteurModifiant.Z);
                    }
                }


                if (GestionInput.EstNouvelleTouche(Keys.D3))
                {
                    if (VecteurModifiant.Z == 0)
                    {
                        VecteurModifiant = new Vector3(VecteurModifiant.X, VecteurModifiant.Y, CONSTANTE_ROTATION);
                    }
                    else
                    {
                        VecteurModifiant = new Vector3(VecteurModifiant.X, VecteurModifiant.Y, 0);
                    }
                }


                if (GestionInput.EstNouvelleTouche(Keys.Space))
                {
                    Rotation = RotationIni;
                }

            }
        }
    }
}
