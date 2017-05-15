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
    public class Menu2 : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //instanciation des constantes
        public bool Pause { get; private set; }
        bool accepte = true;
        int HAUTEURPLAYBUTTON = 125;
        int HAUTEURCOMMANDESBUTTON = 225;
        int HAUTEUREXITBUTTON = 325;
        int HAUTEURANNULERBUTTON = 400;
        int CENTRERBUTTON1 = 2;
        int CENTRERBUTTON2 = 10;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager GestionInput { get; set; }
        Game Jeu { get; set; }

        //Instanciation des boutons
        cButton btnExit;
        cButton btnAnnuler;
        cButton btnCommandes;
        cButton btnPlay;

        //Instanciation des Gamestats
        enum GameStat
        {
            Mainmenu,
            Commandes,
        }
        GameStat CurrentGameStat = GameStat.Mainmenu;

        //Ajustement de l'écran
        int screenWidth = 800;
        int screenHeight = 480;
        

        /// <summary>
        /// Le constructeur de la classe
        /// </summary>
        /// <param name="game"></param>
        /// <param name="pere"></param>
        /// <param name="pause"></param>
        public Menu2(Game game, GraphicsDeviceManager graphicd, bool pause)
            : base(game)
        {
            Jeu = game;
            graphics = graphicd;
            graphics.IsFullScreen = false;
            Jeu.Content.RootDirectory = "Content";
            Pause = pause;
            SetPause(pause);
        }


        public override void Initialize()
        {
            Pause = true;
            SetPause(true);
            base.Initialize();
        }


       
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Ajustement de l'écran
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            Jeu.IsMouseVisible = true;

            
            //Création du bouton Play
            btnPlay = new cButton(Jeu.Content.Load<Texture2D>("button model Play"), graphics.GraphicsDevice);
            btnPlay.setPosition(new Vector2((screenWidth / CENTRERBUTTON1) - (screenWidth / CENTRERBUTTON2), HAUTEURPLAYBUTTON));


            //Création du bouton Exit
            btnExit = new cButton(Jeu.Content.Load<Texture2D>("button model EXIT"), graphics.GraphicsDevice);
            btnExit.setPosition(new Vector2((screenWidth / CENTRERBUTTON1) - (screenWidth / CENTRERBUTTON2), HAUTEUREXITBUTTON));


            //Création du bouton Annuler
            btnAnnuler = new cButton(Jeu.Content.Load<Texture2D>("button model ANNULER"), graphics.GraphicsDevice);
            btnAnnuler.setPosition(new Vector2(CENTRERBUTTON2, HAUTEURANNULERBUTTON));


            //Création du bouton Commandes
            btnCommandes = new cButton(Jeu.Content.Load<Texture2D>("button model COMMANDES"), graphics.GraphicsDevice);
            btnCommandes.setPosition(new Vector2((screenWidth / CENTRERBUTTON1) - (screenWidth / CENTRERBUTTON2), HAUTEURCOMMANDESBUTTON));
        }
       

        private void SetPause(bool enPause)
        {
            foreach (GameComponent g in Game.Components)
                if (g is IPausable)
                    (g as IPausable).GérerPause(enPause);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            switch (CurrentGameStat)
            {
                case GameStat.Mainmenu:
                    btnPlay.Update(mouse);
                    if (btnPlay.isClicked)
                    {
                        Jeu.IsMouseVisible = false;
                        accepte = false;
                        Pause = false;
                        SetPause(false);
                    }


                    btnExit.Update(mouse);
                    if (btnExit.isClicked == true)
                    {
                        Jeu.Exit();
                    }


                    btnCommandes.Update(mouse);
                    if (btnCommandes.isClicked == true)
                    {
                        CurrentGameStat = GameStat.Commandes;
                    }

                    break;

       
                case GameStat.Commandes:


                    btnAnnuler.Update(mouse);
                    if (btnAnnuler.isClicked == true)
                    {
                        CurrentGameStat = GameStat.Mainmenu;
                    }

                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            //le if décide se qui s'affiche ou non
            if (accepte == true)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                switch (CurrentGameStat)
                {
                    case GameStat.Mainmenu:
                        spriteBatch.Draw(Jeu.Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                        btnPlay.Draw(spriteBatch);
                        btnExit.Draw(spriteBatch);
                        btnCommandes.Draw(spriteBatch);

                        break;


                    case GameStat.Commandes:
                        spriteBatch.Draw(Jeu.Content.Load<Texture2D>("back ground menu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                        btnAnnuler.Draw(spriteBatch);

                        break;
                }
                spriteBatch.End();
                base.Draw(gameTime);
            }
        }



    }
}
