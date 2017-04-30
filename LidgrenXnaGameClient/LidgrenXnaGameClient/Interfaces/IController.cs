using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace XnaGameClient
{
   interface IController
   {
      Vector3 GetPosition();
      Vector3 GetDirection();
      Vector3 GetDirectionVu();
   }
}
