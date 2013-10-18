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

namespace XNAGame
{
	class Flinstone
	{
		Texture2D texture;
		Rectangle rectangle;

		public Flinstone(Texture2D newTexture, Rectangle newRec)
		{
			texture = newTexture;
			rectangle = newRec;
		}

		public void Update()
		{
			if(Keyboard.GetState().IsKeyDown(Keys.Left)){
				rectangle.X -= 3;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				rectangle.X += 3;
			}
		}


		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, rectangle, Color.White);
		}


	}
}
