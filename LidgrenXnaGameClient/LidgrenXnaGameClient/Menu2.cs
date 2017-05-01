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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager GestionInput { get; set; }
        Game Jeu { get; set; }
        public bool Pause { get; private set; }


        bool accepte = true;

        enum GameStat
        {
            Host,
            Join,
            //Options,
            Mainmenu,
            Commandes,
        }
        GameStat CurrentGameStat = GameStat.Mainmenu;

        // Screen size ajustement
        int screenWidth = 800;
        int screenHeight = 480;
        //int screenWidth = 1600;
        //int screenHeight = 900;


        cButton btnHost;
        cButton btnJoin;
        //cButton btnOptions;
        cButton btnExit;
        cButton btnAnnuler;
        cButton btnCommandes;


        public Menu2(Game game, GraphicsDeviceManager pere, bool pause)
            : base(game)
        {
            Jeu = game;
            graphics = pere;
            graphics.IsFullScreen = false;
            Jeu.Content.RootDirectory = "Content";
            Pause = pause;

            SetPause(pause);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization logic here
            //Services.AddService(typeof(InputManager), GestionInput);
            Pause = true;
            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //SetPause(true);
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Screen ajustment
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            Jeu.IsMouseVisible = true;


            btnHost = new cButton(Jeu.Content.Load<Texture2D>("button model host"), graphics.GraphicsDevice);
            btnHost.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 100));

            //btnOptions = new cButton(Content.Load<Texture2D>("button model OPTIONS"), graphics.GraphicsDevice);
            //btnOptions.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 300));

            btnExit = new cButton(Jeu.Content.Load<Texture2D>("button model EXIT"), graphics.GraphicsDevice);
            btnExit.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 400));

            btnJoin = new cButton(Jeu.Content.Load<Texture2D>("button model JOIN"), graphics.GraphicsDevice);
            btnJoin.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 200));

            btnAnnuler = new cButton(Jeu.Content.Load<Texture2D>("button model ANNULER"), graphics.GraphicsDevice);
            btnAnnuler.setPosition(new Vector2(10, 400));

            btnCommandes = new cButton(Jeu.Content.Load<Texture2D>("button model COMMANDES"), graphics.GraphicsDevice);
            btnCommandes.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 300));
            ///350,300 center
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
                    btnHost.Update(mouse);
                    if (btnHost.isClicked)
                    {
                        accepte = false;
                        Pause = false;
                        SetPause(false);
                    }

                    //btnOptions.Update(mouse);
                    //if (btnOptions.isClicked == true)
                    //{
                    //    CurrentGameStat = GameStat.Options;
                    //}

                    btnExit.Update(mouse);
                    if (btnExit.isClicked == true)
                    {
                        Jeu.Exit();
                    }

                    btnJoin.Update(mouse);
                    if (btnJoin.isClicked == true)
                    {
                        accepte = false;
                        Pause = false;
                        SetPause(false);
                    }

                    btnCommandes.Update(mouse);
                    if (btnCommandes.isClicked == true)
                    {
                        CurrentGameStat = GameStat.Commandes;
                    }

                    break;

                //case GameStat.Options:


                //    btnAnnuler.Update(mouse);
                //    if (btnAnnuler.isClicked == true)
                //    {
                //        CurrentGameStat = GameStat.Mainmenu;
                //    }

                //    break;

                case GameStat.Host:


                    break;

                case GameStat.Join:



                    break;

                case GameStat.Commandes:


                    btnAnnuler.Update(mouse);
                    if (btnAnnuler.isClicked == true)
                    {
                        CurrentGameStat = GameStat.Mainmenu;
                    }

                    break;
            }

            //if (GestionInput.EstNouveauClicGauche())
            //{
            //    CurrentGameStat = GameStat.Mainmenu;
            //}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            if (accepte == true)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);


                spriteBatch.Begin();

                switch (CurrentGameStat)
                {
                    case GameStat.Mainmenu:
                        spriteBatch.Draw(Jeu.Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                        btnHost.Draw(spriteBatch);
                        //btnOptions.Draw(spriteBatch);
                        btnExit.Draw(spriteBatch);
                        btnJoin.Draw(spriteBatch);
                        btnCommandes.Draw(spriteBatch);

                        break;

                    //case GameStat.Options:
                    //    spriteBatch.Draw(Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                    //    btnAnnuler.Draw(spriteBatch);

                    //    break;

                    case GameStat.Host:
                        spriteBatch.Draw(Jeu.Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                        btnAnnuler.Draw(spriteBatch);

                        break;

                    case GameStat.Join:
                        spriteBatch.Draw(Jeu.Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                        btnAnnuler.Draw(spriteBatch);

                        break;

                    case GameStat.Commandes:
                        spriteBatch.Draw(Jeu.Content.Load<Texture2D>("back ground menu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                        btnAnnuler.Draw(spriteBatch);

                        break;
                }
                spriteBatch.End();
                base.Draw(gameTime);
            }
        }



    }
}
