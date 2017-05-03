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
        const float AUCUNE_HOMOTH�TIE = 1f;
        const float AVANT_PLAN = 0f;

        Vector3 Position { get; set; }
        Color Couleur { get; set; }
        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        int Compteur { get; set; }

        string Cha�neSCORE { get; set; }
        Vector2 PositionHautGauche { get; set; }
        Vector2 PositionCha�ne { get; set; }
        Vector2 Dimension { get; set; }
        Game Jeu { get; set; }

        BoundingBox ZoneModel { get; set; }
        BoundingBox ZoneCheckPoint { get; set; }
        Cam�ra Cam�raJeu { get; set; }


        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }


        public Score(Game game, string font, Color couleur, float intervalleMAJ, Vector3 positionCam�ra)
            : base(game)
        {
            Jeu = game;
            Couleur = couleur;
            Position = positionCam�ra;
        }

        public override void Initialize()
        {
            base.Initialize();
            Temps�coul�DepuisMAJ = 0;
            Compteur = 0;
            PositionHautGauche = new Vector2(Game.Window.ClientBounds.Width - MARGE_GAUCHE,
                                            Game.Window.ClientBounds.Height - MARGE_HAUT);
            foreach(Checkpoint T in Game.Components.Where(c => c is Checkpoint))
            {
                ZoneCheckPoint = new BoundingBox(T.PositionCheckpoint - new Vector3(3,3,3), T.PositionCheckpoint + new Vector3(3,3,3));
            }
            
            ZoneModel = new BoundingBox(Position - new Vector3(3,3,3), Position + new Vector3(3,3,3));
            

           
        }
        protected override void LoadContent()
        {
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            ArialFont = Game.Content.Load<SpriteFont>("Fonts/Arial");
            Cam�raJeu = Game.Services.GetService(typeof(Cam�ra)) as Cam�ra;
        }
        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                Position = Cam�raJeu.Position;
                ZoneModel = new BoundingBox(Position - new Vector3(3, 3, 3), Position + new Vector3(3, 3, 3));
                foreach (Checkpoint T in Game.Components.Where(c => c is Checkpoint))
                {
                    ZoneCheckPoint = new BoundingBox(T.PositionCheckpoint - new Vector3(3, 3, 3), T.PositionCheckpoint + new Vector3(3, 3, 3));
                }
                CalculerScore();
                Temps�coul�DepuisMAJ = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(ArialFont, Cha�neSCORE, PositionCha�ne, Couleur, AUCUNE_ROTATION,
                                      Vector2.Zero, AUCUNE_HOMOTH�TIE, SpriteEffects.None, AVANT_PLAN);
            GestionSprites.End();
        }

        void CalculerScore()
        {
            if (ZoneModel.Intersects(ZoneCheckPoint))
            {
                Compteur += 1;
                Cha�neSCORE = "Player 1:" + Compteur + "     Player 2:" + Compteur;
            }
            else
            {
                Cha�neSCORE = "Player 1:" + Compteur + "     Player 2:" + Compteur;
            }

            
        }
    }
}
