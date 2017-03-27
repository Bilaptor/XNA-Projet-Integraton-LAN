﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace XnaGameClient
{
    public class AfficheurFPS : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int MARGE_BAS = 10;
        const int MARGE_DROITE = 15;
        const float AUCUNE_ROTATION = 0f;
        const float AUCUNE_HOMOTHÉTIE = 1f;
        const float AVANT_PLAN = 0f;

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        int CptFrames { get; set; }
        float ValFPS { get; set; }

        string ChaîneFPS { get; set; }
        Vector2 PositionDroiteBas { get; set; }
        Vector2 PositionChaîne { get; set; }
        Vector2 Dimension { get; set; }

        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }

        public AfficheurFPS(Game game, string font, Color couleur,float intervalleMAJ)
           : base(game)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            ValFPS = 0;
            CptFrames = 0;
            ChaîneFPS = "";
            PositionDroiteBas = new Vector2(Game.Window.ClientBounds.Width - MARGE_DROITE,
                                            Game.Window.ClientBounds.Height - MARGE_BAS);
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
            ++CptFrames;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                CalculerFPS();
                TempsÉcouléDepuisMAJ = 0;
            }
        }

        void CalculerFPS()
        {
            float oldValFPS = ValFPS;
            ValFPS = CptFrames / TempsÉcouléDepuisMAJ;
            if (oldValFPS != ValFPS)
            {
                ChaîneFPS = ValFPS.ToString("0");
                Dimension = ArialFont.MeasureString(ChaîneFPS);
                PositionChaîne = PositionDroiteBas - Dimension;
                Game.Window.Title = ChaîneFPS;
            }
            CptFrames = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.DrawString(ArialFont, ChaîneFPS, PositionChaîne, Color.Chartreuse, AUCUNE_ROTATION,
                                      Vector2.Zero, AUCUNE_HOMOTHÉTIE, SpriteEffects.None, AVANT_PLAN);
        }
    }
}