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
        const float VITESSE_DÉPLACEMENT = 50f;
        const float VITESSE_CHUTE_MAXIMALE = -87;
        const float VITESSE_CHUTE = -65;
        const float VITESSE_DE_SAUT = 32;
        const int LIMITE_X = 250;
        const int LIMITE_Z = -250;
        const int HAUTEUR_LAVE = 10;
        float TempsDepuisDerniereMAJ;

        bool EnCollision { get; set; }
        BasicEffect EffetDeBase { get; set; }
        public BoundingBox ZoneDeCollisionModel { get; set; }
        Vector3 DimensionModel { get; set; }
        protected float IntervalleMAJ { get; set; }
        NetClient client;
        Vector3 PositionInitial { get; set; }

        Vector3 Vitesse;

        IController controller;

        public Joueur(Game jeu, Vector3 positionInitiale, MouseState originalMouseState, float intervalleMAJ, Vector3 dimentionModel)
          : base(jeu, positionInitiale, new Vector3(200, 0, -90), Vector3.Up, originalMouseState, intervalleMAJ)
        {
            PositionInitial = positionInitiale;
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

            GérerSouris((float)TempsDepuisDerniereMAJ);
            if (TempsDepuisDerniereMAJ >= IntervalleMAJ)
            {
                Vitesse += new Vector3(0, VITESSE_CHUTE, 0) * (float)TempsDepuisDerniereMAJ;
                if (Vitesse.Y < VITESSE_CHUTE_MAXIMALE)
                {
                    Vitesse = new Vector3(Vitesse.X, VITESSE_CHUTE_MAXIMALE, Vitesse.Z);
                }
                if (!EnCollision)
                    SetPosition(Position + Vitesse * (float)TempsDepuisDerniereMAJ);

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

                TempsDepuisDerniereMAJ = 0;
                if (Position.Y <= HAUTEUR_LAVE)
                {
                    Position = PositionInitial;
                }
            }
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            base.Update(gameTime);
        }

        private void GérerSouris(float deltaT)
        {
            Vector2 d = new Vector2(Direction.X, Direction.Z);
            d.Normalize();
            Vector2 l = new Vector2(GetLatéral().X, GetLatéral().Z);
            l.Normalize();
            SetDirection(controller.GetDirectionVu());

            Vector3 dir = controller.GetDirection();

            if (TempsDepuisDerniereMAJ >= IntervalleMAJ)
            {
                Position += dir.X * new Vector3(d.X, 0, d.Y) * VITESSE_DÉPLACEMENT * deltaT;
                Position += dir.Z * new Vector3(l.X, 0, l.Y) * VITESSE_DÉPLACEMENT * deltaT;
            }
            if (Position.X < 0)
                Position = new Vector3(0, Position.Y, Position.Z);
            if (Position.X > LIMITE_X)
                Position = new Vector3(LIMITE_X, Position.Y, Position.Z);
            if (Position.Z > 0)
                Position = new Vector3(Position.X, Position.Y, 0);
            if (Position.Z < LIMITE_Z)
                Position = new Vector3(Position.X, Position.Y, LIMITE_Z);

            //fait en sorte que le joueur ne puisse pa sauter plein de fois dans les airs
            if (Vitesse.Y > -4 && Vitesse.Y < 2)
                Vitesse += new Vector3(0, VITESSE_DE_SAUT * dir.Y, 0);
        }

        public bool EstEnCollision(BoundingSphere autreZoneCollison)
        {
            return ZoneDeCollisionModel.Intersects(autreZoneCollison);
        }
        //Modifie la position du volume en modifiant les deux coins le définissant en fonction de la position envoyé
        private void SetPositionVolume(Vector3 position)
        {                                                                                                                                                 //  la plateforme quand la plateforme est à la hauteur de sa tete
            ZoneDeCollisionModel = new BoundingBox(new Vector3(position.X, position.Y, position.Z) - DimensionModel / 2, new Vector3(position.X, position.Y - 2.5f, position.Z) + DimensionModel / 2);
        }

        //Envoie le volume de collision afin de traiter les collision avec l'objet.
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
                if(Vitesse.Y < 0) { Vitesse = new Vector3(Vitesse.X, 0.5f, Vitesse.Z); }
            }
        }
    }
}
