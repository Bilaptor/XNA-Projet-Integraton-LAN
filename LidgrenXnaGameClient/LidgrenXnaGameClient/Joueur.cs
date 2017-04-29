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
    public class Joueur : ObjetDeDémo
    {
        BasicEffect EffetDeBase { get; set; }
        public BoundingBox ZoneDeCollisionModel { get; set; }
        Vector3 DimensionModel { get; set; }
        protected float IntervalleMAJ { get; set; }


        public Joueur(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Vector3 dimentionModel)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            DimensionModel = dimentionModel;
        }

        public override void Initialize()
        {
            ZoneDeCollisionModel = new BoundingBox(Vector3.Zero, DimensionModel);
            base.Initialize();
        }

        /// <summary>
        /// méthode qui reconnaît les collisions entre les bounding box des plateformes et les bounding sphere
        /// des personnages.
        /// </summary>
        /// <param name="autreZoneCollison"></param>
        /// <returns></returns>
        public bool EstEnCollision(BoundingSphere autreZoneCollison)
        {
            return ZoneDeCollisionModel.Intersects(autreZoneCollison);
        }
    }
}
