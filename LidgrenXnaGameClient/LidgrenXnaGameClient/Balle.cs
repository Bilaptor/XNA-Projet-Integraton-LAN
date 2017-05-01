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
    public class Balle : PrimitiveDeBaseAnimée
    {
        const int NB_SOMMETS = 14;
        const int NB_TRIANGLES = 12;

        Color Couleur { get; set; }
        float Homothétie { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3 Position { get; set; }
        float IntervalleMAJ { get; set; }
        protected InputManager GestionInput { get; private set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IncrémentAngleRotation { get; set; }
        BasicEffect EffetDeBase { get; set; }
        Vector3 Origine { get; set; }
        float DeltaX { get; set; }
        float DeltaY { get; set; }
        float DeltaZ { get; set; }
        Vector3 DimensionBalle { get; set; }
        Game Jeu { get; set; }
        bool EstFeu { get; set; }
        Vector3 PositionBalle { get; set; }
        Caméra CameraJeu { get; set; }

        public Balle(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Vector3 dimension, Color couleur)
            : base(jeu,  homothétieInitiale,  rotationInitiale,  positionInitiale,  intervalleMAJ)
        {
            Jeu = jeu;
            DimensionBalle = dimension;
            Couleur = couleur;
            DeltaX = dimension.X;
            DeltaY = dimension.Y;
            DeltaZ = dimension.Z;
            Origine = new Vector3(DeltaX / 2, DeltaY / 2, DeltaZ / 2);
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            EstFeu = false;
            Sommets = new VertexPositionColor[NB_SOMMETS];
            Homothétie = HomothétieInitiale;
            
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            IncrémentAngleRotation = MathHelper.Pi * IntervalleMAJ / 2;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            CameraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;
            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.VertexColorEnabled = true;
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
           
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                PositionBalle = CameraJeu.Position;
                GérerClavier();
                EffectuerMiseÀJour();
                TempsÉcouléDepuisMAJ -= IntervalleMAJ;
            }
            base.Update(gameTime);
        }

        protected override void GérerClavier()
        {
            if (GestionInput.EstEnfoncée(Keys.Q))
            {
                EstFeu = true;
            }

        }
       

        protected override void InitialiserSommets()
        {
            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(Origine.X, Origine.Y + DeltaY, Origine.Z);
            points[1] = new Vector3(Origine.X, Origine.Y + DeltaY, Origine.Z + DeltaZ);
            points[2] = new Vector3(Origine.X + DeltaX, Origine.Y + DeltaY, Origine.Z + DeltaZ);
            points[3] = new Vector3(Origine.X + DeltaX, Origine.Y + DeltaY, Origine.Z);
            points[4] = Origine;
            points[5] = new Vector3(Origine.X, Origine.Y, Origine.Z + DeltaZ);
            points[6] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z + DeltaZ);
            points[7] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z);

            Sommets[0] = new VertexPositionColor(points[1], Couleur);
            Sommets[1] = new VertexPositionColor(points[2], Couleur);
            Sommets[2] = new VertexPositionColor(points[5], Couleur);

            Sommets[3] = new VertexPositionColor(points[6], Couleur);
            Sommets[4] = new VertexPositionColor(points[7], Couleur);
            Sommets[5] = new VertexPositionColor(points[2], Couleur);

            Sommets[6] = new VertexPositionColor(points[3], Couleur);
            Sommets[7] = new VertexPositionColor(points[1], Couleur);
            Sommets[8] = new VertexPositionColor(points[0], Couleur);

            Sommets[9] = new VertexPositionColor(points[5], Couleur);
            Sommets[10] = new VertexPositionColor(points[4], Couleur);
            Sommets[11] = new VertexPositionColor(points[7], Couleur);

            Sommets[12] = new VertexPositionColor(points[0], Couleur);
            Sommets[13] = new VertexPositionColor(points[3], Couleur);

        }

        public override void Draw(GameTime gameTime)
        {
            if (EstFeu == true)
            {
                EffetDeBase.World = GetMonde();
                EffetDeBase.View = CaméraJeu.Vue;
                EffetDeBase.Projection = CaméraJeu.Projection;

                foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
                {
                    passeEffet.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
                }
            }
           
        }
    }
}
