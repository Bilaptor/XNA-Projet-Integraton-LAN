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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Score : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int MARGE_HAUT = 10;
        const int MARGE_GAUCHE = 15;
        const float AUCUNE_ROTATION = 0f;
        const float AUCUNE_HOMOTHÉTIE = 1f;
        const float AVANT_PLAN = 0f;

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        int Compteur { get; set; }

        string ChaîneSCORE { get; set; }
        Vector2 PositionHautGauche { get; set; }
        Vector2 PositionChaîne { get; set; }
        Vector2 Dimension { get; set; }
        Game Jeu { get; set; }

        BoundingBox ZoneModel { get; set; }
        BoundingBox ZoneCheckPoint { get; set; }
        Vector3 PositionModel { get; set; }
        Vector3 PositionCheckpoint { get; set; }


        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }


        public Score(Game game, string font, Color couleur, float intervalleMAJ, Vector3 positionModel, Vector3 positionCheckpoint, BoundingBox zoneModel, BoundingBox zoneCheckPoint)
            : base(game)
        {
            Jeu = game;
            ZoneCheckPoint = zoneCheckPoint;
            ZoneModel = zoneModel;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            Compteur = 0;
            PositionHautGauche = new Vector2(Game.Window.ClientBounds.Width - MARGE_GAUCHE,
                                            Game.Window.ClientBounds.Height - MARGE_HAUT);

            base.Initialize();
        }
        protected override void LoadContent()
        {
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            ArialFont = Game.Content.Load<SpriteFont>("Fonts/Arial");
        }
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                CalculerScore();
                TempsÉcouléDepuisMAJ = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(ArialFont, ChaîneSCORE, PositionChaîne, Color.Chartreuse, AUCUNE_ROTATION,
                                      Vector2.Zero, AUCUNE_HOMOTHÉTIE, SpriteEffects.None, AVANT_PLAN);
            GestionSprites.End();
        }

        void CalculerScore()
        {
            if(ZoneModel.Intersects(ZoneCheckPoint))
            {
                Compteur += 1;
                ChaîneSCORE = "Player 1:" + Compteur + "     Player 2:" + Compteur;
            }
        }
    }
}
