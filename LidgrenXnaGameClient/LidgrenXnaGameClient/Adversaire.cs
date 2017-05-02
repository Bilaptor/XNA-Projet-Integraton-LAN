using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace XnaGameClient
{
   class Adversaire : ObjetDeDémo
   {
      double TempsÉcouléDepuisMAJ { get; set; }
      float IntervalleVariation { get; set; }
      Vector3 PositionASuivre { get; set; }
      Vector3 PositionCamera { get; set; }
      RessourcesManager<Model> GestionnaireDeModèles { get; set; }
      Caméra CaméraJeu { get; set; }
      Game Jeu { get; set; }


      public Adversaire(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
          : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
      {
         Jeu = jeu;
      }

      public override void Initialize()
      {
         base.Initialize();
      }

      public void SetPosition(Vector3 position)
      {
         this.Position = position;
      }

      protected override void LoadContent()
      {
         base.LoadContent();
      }
   }
}
