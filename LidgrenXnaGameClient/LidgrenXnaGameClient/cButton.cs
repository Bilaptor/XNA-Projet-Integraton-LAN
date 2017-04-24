using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaGameClient
{
    class cButton
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        int compt = 0;

        Color colour = new Color(255, 255, 255, 255);

        public Vector2 size;

        public cButton(Texture2D newTexture, GraphicsDevice graphics)
        {
            texture = newTexture;
            // ScreenW = 800, ScreeH = 600
            // ImgW = 100, ImgH = 20
            // 400     140
            //8  30
            size = new Vector2(graphics.Viewport.Width / 5, graphics.Viewport.Height / 8);
            isClicked = false;

        }

        

        public bool isClicked;

        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(rectangle))
            {
                if(mouse.LeftButton == ButtonState.Pressed)
                {
                    isClicked = true;
                    compt += 1;

                }
            }
            else
            {
                isClicked = false;
            }
        }

        public void setPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, colour);
        }
    }
}
