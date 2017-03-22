using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        Keys[] AnciennesTouches { get; set; }
        Keys[] NouvellesTouches { get; set; }
        KeyboardState ÉtatClavier { get; set; }
        MouseState ÉtatSouris { get; set; }
        MouseState AncienÉtatSouris { get; set; }

        public InputManager(Game game)
           : base(game)
        {

        }

        public override void Initialize()
        {
            AnciennesTouches = new Keys[0];
            NouvellesTouches = new Keys[0];
        }

        public override void Update(GameTime gameTime)
        {
            AnciennesTouches = NouvellesTouches;
            ÉtatClavier = Keyboard.GetState();
            NouvellesTouches = ÉtatClavier.GetPressedKeys();
            AncienÉtatSouris = ÉtatSouris;
            ÉtatSouris = Mouse.GetState();
        }

        public bool EstClavierActivé
        {
            get { return NouvellesTouches.Length > 0; }
        }

        public bool EstSourisActive
        {
            get
            {
                return EstAncienClicDroit() || EstAncienClicGauche();
            }
        }

        public bool EstEnfoncée(Keys touche)
        {
            return ÉtatClavier.IsKeyDown(touche);
        }

        public bool EstNouvelleTouche(Keys touche)
        {
            int nbTouches = AnciennesTouches.Length;
            bool estNouvelleTouche = ÉtatClavier.IsKeyDown(touche);
            int i = 0;
            while (i < nbTouches && estNouvelleTouche)
            {
                estNouvelleTouche = AnciennesTouches[i] != touche;
                ++i;
            }
            return estNouvelleTouche;
        }

        public bool EstAncienClicDroit()
        {
            return ÉtatSouris.RightButton == ButtonState.Pressed;
        }

        public bool EstAncienClicGauche()
        {
            return ÉtatSouris.LeftButton == ButtonState.Pressed;
        }

        public bool EstNouveauClicDroit()
        {
            return ÉtatSouris.RightButton == ButtonState.Pressed && AncienÉtatSouris.RightButton == ButtonState.Released;
        }

        public bool EstNouveauClicGauche()
        {
            return ÉtatSouris.LeftButton == ButtonState.Pressed && AncienÉtatSouris.LeftButton == ButtonState.Released;
        }

        public Point GetPositionSouris()
        {
            return new Point(ÉtatSouris.X, ÉtatSouris.Y);
        }
    }
}