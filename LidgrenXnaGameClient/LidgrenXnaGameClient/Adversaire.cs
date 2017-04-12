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
             
            : base(jeu,nomModèle,échelleInitiale,rotationInitiale,positionInitiale,intervalleMAJ)
        {

            Jeu = jeu;
        }

        public override void Initialize()
        {
           
                base.Initialize();
            //foreach (GameComponent c in Game.Components)
            //{
            //    compteur += 1;

            //}
            foreach(CaméraSubjective T in Game.Components.Where(c => c is CaméraSubjective))
            {
                PositionASuivre = T.Position;
            }
            
            TempsÉcouléDepuisMAJ = 0;


        }
        protected override void LoadContent()
        {
            base.LoadContent();
            CaméraJeu = Game.Services.GetService(typeof(CaméraSubjective)) as CaméraSubjective;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleVariation)
            {
                Position  = CaméraJeu.Position + new Vector3(0,-1 ,-1);
                TempsÉcouléDepuisMAJ = 0;
                Monde = GetMonde();
            }
            Jeu.Window.Title = "PosX:  " + Position.X + "   PosY:  " + Position.Y + "   PosZ:  " + Position.Z;

            base.Update(gameTime);
        }


    }
}
