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
using Lidgren.Network;

namespace XnaGameClient
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Joueur : CaméraSubjective, IPhysique
    {
        float TempsDepuisDerniereMAJ;
        float IntervalMAJ;

        bool EnCollision { get; set; }
        BasicEffect EffetDeBase { get; set; }
        public BoundingBox ZoneDeCollisionModel { get; set; }
        Vector3 DimensionModel { get; set; }
        protected float IntervalleMAJ { get; set; }
        NetClient client;

        Vector3 Vitesse;

        IController controller;

        public Joueur(Game jeu, Vector3 positionInitiale, MouseState originalMouseState, float intervalleMAJ, Vector3 dimentionModel)
          : base(jeu, positionInitiale, new Vector3(200, 0, -90), Vector3.Up, originalMouseState, intervalleMAJ)
        {
            IntervalleMAJ = intervalleMAJ;
            DimensionModel = dimentionModel;
        }

        public override void Initialize()
        {
            client = (Game as Game1).client;
            controller = new ControllerClavier(Game, OriginalMouseState);
            (controller as GameComponent).Initialize();

            TempsDepuisDerniereMAJ = 0;
            SetPositionVolume(Position);

            ZoneDeCollisionModel = new BoundingBox(Vector3.Zero, DimensionModel);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 AnciennePosition = Position;
            TempsDepuisDerniereMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Game.Window.Title = this.Position.ToString();

            GérerSouris();

                Vitesse += new Vector3(0, -5f, 0) * (float)TempsDepuisDerniereMAJ;
                if (!EnCollision)
                    SetPosition(Position + Vitesse * (float)TempsDepuisDerniereMAJ);


                //if (EnCollision)
                //   Game.Window.Title = "En Collision";
                //else
                //   Game.Window.Title = "";

                if (AnciennePosition != Position)
                {
                    //envoie la position au serveur
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write((byte)PacketTypes.POSITION);
                    om.Write(Position.X);
                    om.Write(Position.Y);
                    om.Write(Position.Z);
                    client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                }


                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                TempsDepuisDerniereMAJ = 0;
            base.Update(gameTime);
        }

        private void GérerSouris()
        {
            SetDirection(controller.GetDirectionVu());

            Vector3 dir = controller.GetDirection();
            Position += dir.X * new Vector3(Direction.X, 0, Direction.Z) * 0.05f;
            Position += dir.Z * new Vector3(GetLatéral().X, 0, GetLatéral().Z) * 0.05f;
        }

        public bool EstEnCollision(BoundingSphere autreZoneCollison)
        {
            return ZoneDeCollisionModel.Intersects(autreZoneCollison);
        }
        //Modifie la position du volume en modifiant les deux coins le définissant en fonction de la position envoyé
        private void SetPositionVolume(Vector3 position)
        {
            ZoneDeCollisionModel = new BoundingBox(position - DimensionModel / 2, position + DimensionModel / 2);
        }

        //Envoie le volume de collision afin de traite les collision avec l'objet.
        public BoundingBox GetVolume()
        {
            SetPositionVolume(this.Position);
            return ZoneDeCollisionModel;
        }

        //Permet d'indiquer a l'objet si il est actuellement en etat de collision
        public void SetEnCollision(bool enCollision, IPhysique autre)
        {
            EnCollision = enCollision;
            if (EnCollision)
            {
                Position = new Vector3(Position.X, autre.GetVolume().Max.Y + DimensionModel.Y / 2, Position.Z);
                Vitesse = new Vector3(Vitesse.X, 0.5f, Vitesse.Z);
            }
        }
    }
}
