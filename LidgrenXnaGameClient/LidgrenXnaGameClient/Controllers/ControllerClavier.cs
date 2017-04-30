using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace XnaGameClient
{
    class ControllerClavier : GameComponent, IController
    {
        InputManager GestionInput;
        MouseState OriginalMouseState { get; set; }
        float angleHorizontal = 0;
        float angleVertical = 0;
        const float VITESSE_ROTATION_SOURIS = 0.0025f;


        public ControllerClavier(Game game, MouseState originalMouseState) : base(game)
        {
            OriginalMouseState = originalMouseState;
        }

        public override void Initialize()
        {
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;

            base.Initialize();
        }

        public Vector3 GetPosition()
        {
            return Vector3.Zero;
        }

        public Vector3 GetDirectionVu()
        {
            //Game.Window.Title = DirectionVu.ToString();

            MouseState currentMouseState = Mouse.GetState();
            if (OriginalMouseState != currentMouseState)
            {
                float déplacementX = OriginalMouseState.X - currentMouseState.X;
                float déplacementY = OriginalMouseState.Y - currentMouseState.Y;

                if (déplacementX == 0)
                {
                    déplacementX = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
                }

                if (déplacementY == 0)
                {
                    déplacementY = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
                }

                angleHorizontal -= déplacementX * VITESSE_ROTATION_SOURIS;
                angleHorizontal %= (float)(Math.PI * 2);

                angleVertical += déplacementY * VITESSE_ROTATION_SOURIS;
                if (angleVertical > (Math.PI / 2))
                    angleVertical = (float)(Math.PI / 2 - 0.01);
                if (angleVertical < -(Math.PI / 2))
                    angleVertical = (float)(-Math.PI / 2 + 0.01);
            }

            return Vector3.Normalize(new Vector3((float)Math.Cos(angleHorizontal), (float)Math.Tan(angleVertical), (float)Math.Sin(angleHorizontal)));
        }

        public Vector3 GetDirection()
        {
            float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S));
            float déplacementLatéral = (GérerTouche(Keys.A) - GérerTouche(Keys.D));

            return new Vector3(déplacementDirection, 0, déplacementLatéral);
        }

        private int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }
    }
}
