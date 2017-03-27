using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace XnaGameClient
{
   public class ArrièrePlan : Microsoft.Xna.Framework.DrawableGameComponent
   {
      protected SpriteBatch GestionSprites { get; private set; }
      RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
      string NomImage { get; set; }
      Rectangle ZoneAffichage { get; set; }
      protected Texture2D ImageDeFond { get; private set; }

      public ArrièrePlan(Game jeu, string nomImage)
         : base(jeu)
      {
         NomImage = nomImage;
      }

      public override void Initialize()
      {
         ZoneAffichage = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
         base.Initialize();
      }

      protected override void LoadContent()
      {
         GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
         GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
         ImageDeFond = GestionnaireDeTextures.Find(NomImage);
      }

      public override void Draw(GameTime gameTime)
      {
         GestionSprites.Draw(ImageDeFond, ZoneAffichage, Color.White);
      }
   }
}