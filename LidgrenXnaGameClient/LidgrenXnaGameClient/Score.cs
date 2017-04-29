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
    public class Score : AfficheurFPS 
    {
        const int MARGE_HAUT = 10;
        const int MARGE_GAUCHE = 15;
        const float AUCUNE_ROTATION = 0f;
        const float AUCUNE_HOMOTH�TIE = 1f;
        const float AVANT_PLAN = 0f;

        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        int CptFrames { get; set; }
        float ValFPS { get; set; }

        string Cha�neSCORE { get; set; }
        Vector2 PositionHautGauche { get; set; }
        Vector2 PositionCha�ne { get; set; }
        Vector2 Dimension { get; set; }

        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }
        public Score(Game game, string font, Color couleur, float intervalleMAJ)
            : base(game, font, couleur, intervalleMAJ)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = 0;
            ValFPS = 0;
            CptFrames = 0;
            Cha�neSCORE = "123";
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
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ++CptFrames;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                CalculerScore();
                Temps�coul�DepuisMAJ = 0;
            }
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(ArialFont, Cha�neSCORE, PositionCha�ne, Color.Chartreuse, AUCUNE_ROTATION,
                                      Vector2.Zero, AUCUNE_HOMOTH�TIE, SpriteEffects.None, AVANT_PLAN);
            GestionSprites.End();
        }

        void CalculerScore()
        {
            float oldValFPS = ValFPS;
            ValFPS = CptFrames / Temps�coul�DepuisMAJ;
            if (oldValFPS != ValFPS)
            {
                Cha�neSCORE = ValFPS.ToString("0");
                Dimension = ArialFont.MeasureString(Cha�neSCORE);
                PositionCha�ne = PositionHautGauche - Dimension;
            }
            CptFrames = 0;
        }
    }
}
