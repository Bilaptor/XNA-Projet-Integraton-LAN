﻿using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaGameClient
{
    class RessourceDeBase<T> : IEquatable<RessourceDeBase<T>>
    {
        public string Nom { get; private set; }
        public T Ressource { get; private set; }
        ContentManager Content { get; set; }
        string Répertoire { get; set; }

        // Ce constructeur est appelé lorsque l'on construit un objet TextureDeBase
        // à partir d'une image déjà présente en mémoire.
        public RessourceDeBase(string nom, T image)
        {
            Nom = nom;
            Content = null;
            Répertoire = "";
            Ressource = image;
        }

        // Ce constructeur est appelé lorsque l'on construit un objet TextureDeBase
        // à partir du nom d'une image qui sera éventuellement chargée en mémoire.
        public RessourceDeBase(ContentManager content, string répertoire, string nom)
        {
            Nom = nom;
            Content = content;
            Répertoire = répertoire;
            Ressource = default(T);
        }

        public void Load()
        {
            if (Ressource == null)
            {
                string NomComplet = Répertoire + "/" + Nom;
                Ressource = Content.Load<T>(NomComplet);
            }
        }

        #region IEquatable<T> Membres

        public bool Equals(RessourceDeBase<T> other)
        {
            return Nom == other.Nom ;
        }

        #endregion
    }
}
