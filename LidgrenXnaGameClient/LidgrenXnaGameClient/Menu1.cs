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
    /// This is the main type for your game
    /// </summary>
    public class Menu1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager GestionInput { get; set; }

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
        int screenHeight = 600;

        cButton btnHost;
        cButton btnJoin;
        //cButton btnOptions;
        cButton btnExit;
        cButton btnAnnuler;
        cButton btnCommandes;


        public Menu1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //Services.AddService(typeof(InputManager), GestionInput);
            
            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Screen ajustment
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            IsMouseVisible = true;

            btnHost = new cButton(Content.Load<Texture2D>("button model host"), graphics.GraphicsDevice);
            btnHost.setPosition(new Vector2((screenWidth/2)-(screenWidth/ 10), 100));

            //btnOptions = new cButton(Content.Load<Texture2D>("button model OPTIONS"), graphics.GraphicsDevice);
            //btnOptions.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 300));

            btnExit = new cButton(Content.Load<Texture2D>("button model EXIT"), graphics.GraphicsDevice);
            btnExit.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 400));

            btnJoin = new cButton(Content.Load<Texture2D>("button model JOIN"), graphics.GraphicsDevice);
            btnJoin.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 200));

            btnAnnuler = new cButton(Content.Load<Texture2D>("button model ANNULER"), graphics.GraphicsDevice);
            btnAnnuler.setPosition(new Vector2((screenWidth / 2) - (screenWidth / 10), 500));

            btnCommandes = new cButton(Content.Load<Texture2D>("button model COMMANDES"), graphics.GraphicsDevice);
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            MouseState mouse = Mouse.GetState();

            switch (CurrentGameStat)
            {
                case GameStat.Mainmenu:
                    btnHost.Update(mouse);
                    if (btnHost.isClicked)
                    {
                        
                       
                    }

                    //btnOptions.Update(mouse);
                    //if (btnOptions.isClicked == true)
                    //{
                    //    CurrentGameStat = GameStat.Options;
                    //}

                    btnExit.Update(mouse);
                    if (btnExit.isClicked == true)
                    {
                        Exit();
                    }

                    btnJoin.Update(mouse);
                    if (btnJoin.isClicked == true)
                    {
                        CurrentGameStat = GameStat.Join;
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
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();
            
            switch (CurrentGameStat)
            {
                case GameStat.Mainmenu:
                    spriteBatch.Draw(Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
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
                    spriteBatch.Draw(Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                    btnAnnuler.Draw(spriteBatch);

                    break;

                case GameStat.Join:
                    spriteBatch.Draw(Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                    btnAnnuler.Draw(spriteBatch);

                    break;

                case GameStat.Commandes:
                    spriteBatch.Draw(Content.Load<Texture2D>("background 1"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Red);
                    btnAnnuler.Draw(spriteBatch);

                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
