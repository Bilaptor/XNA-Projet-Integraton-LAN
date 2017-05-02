using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaGameClient
{
    public interface IPhysique
    {
        BoundingBox GetVolume();

        void SetEnCollision(bool enCollision, IPhysique autre);
    }
}
