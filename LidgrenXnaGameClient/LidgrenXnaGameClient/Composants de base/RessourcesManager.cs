using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaGameClient
{
   public class RessourcesManager<T>
   {
      Game Jeu { get; set; }
      string RépertoireDesTextures { get; set; }
      List<RessourceDeBase<T>> ListeTextures { get; set; }

      public RessourcesManager(Game jeu, string répertoireDesTextures)
      {
         Jeu = jeu;
         RépertoireDesTextures = répertoireDesTextures;
         ListeTextures = new List<RessourceDeBase<T>>();
      }

      public void Add(string nom, T texture2DÀAjouter)
      {
         RessourceDeBase<T> textureÀAjouter = new RessourceDeBase<T>(nom, texture2DÀAjouter);
         if (!ListeTextures.Contains(textureÀAjouter))
         {
            ListeTextures.Add(textureÀAjouter);
         }
      }

      void Add(RessourceDeBase<T> textureÀAjouter)
      {
         textureÀAjouter.Load();
         ListeTextures.Add(textureÀAjouter);
      }

      public T Find(string nomTexture)
      {
         const int TEXTURE_PAS_TROUVÉE = -1;
         RessourceDeBase<T> textureÀRechercher = new RessourceDeBase<T>(Jeu.Content, RépertoireDesTextures, nomTexture);
         int indexTexture = ListeTextures.IndexOf(textureÀRechercher);
         if (indexTexture == TEXTURE_PAS_TROUVÉE)
         {
            Add(textureÀRechercher);
            indexTexture = ListeTextures.Count - 1;
         }
         return ListeTextures[indexTexture].Ressource ;
      }
   }
}
