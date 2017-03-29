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
    public class ArrièrePlanDéroulant : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const float vitesseDéplacement = 0.5f;
        Vector2 Scale { get; set; }
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        string NomImage { get; set; }
        Vector2 ZoneAffichage1 { get; set; }
        Vector2 ZoneAffichage2 { get; set; }
        Texture2D ImageDeFond { get; set; }
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        int CptFrames { get; set; }
        Vector2 Déplacement { get; set; }

        public ArrièrePlanDéroulant(Game jeu, string nomImage, float intervalMAJ)
            : base(jeu)
        {
            NomImage = nomImage;
            IntervalleMAJ = intervalMAJ;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Scale = new Vector2(Game.Window.ClientBounds.Width / (float)ImageDeFond.Width, Game.Window.ClientBounds.Height / (float)ImageDeFond.Height);
            Déplacement = new Vector2(1, 0);
            ZoneAffichage1 = new Vector2(0, 0);
            ZoneAffichage2 = new Vector2(ZoneAffichage1.X +ImageDeFond.Width * Scale.X, ZoneAffichage1.Y);
        }

        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            ImageDeFond = GestionnaireDeTextures.Find(NomImage);
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ++CptFrames;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                DéplacerBackground();
                TempsÉcouléDepuisMAJ = 0;
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.Draw(ImageDeFond, ZoneAffichage1, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            GestionSprites.Draw(ImageDeFond, ZoneAffichage2, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            GestionSprites.End();
        }

        private void DéplacerBackground()
        {
            CptFrames = 0;
            if(ZoneAffichage1.X < -ImageDeFond.Width * Scale.X || ZoneAffichage1.X > ImageDeFond.Width * Scale.X)
            {
                ZoneAffichage1 = new Vector2(vitesseDéplacement, 0);
            }
            else
            {
                ZoneAffichage1 = new Vector2(ZoneAffichage1.X + vitesseDéplacement, ZoneAffichage1.Y);
                ZoneAffichage2 = new Vector2(ZoneAffichage1.X - ImageDeFond.Width * Scale.X, ZoneAffichage1.Y);
            }
        }
    }
}
