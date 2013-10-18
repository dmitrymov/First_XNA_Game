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
	abstract class Background
	{
		public Texture2D texture;
		public Rectangle rectangle;

		public void Draw(SpriteBatch spriteBatch){
			spriteBatch.Draw(texture, rectangle, Color.White);
		}

	}

	class ScrollingBackground : Background
	{

		public ScrollingBackground(Texture2D newTexture, Rectangle newRectangle)
		{
			texture = newTexture;
			rectangle = newRectangle;
		}

		public void Update()
		{
			if(Keyboard.GetState().IsKeyDown(Keys.Right))
				rectangle.X -= 3;
			if(Keyboard.GetState().IsKeyDown(Keys.Left))
				rectangle.X += 3;
		}

	}

	class AllBackgrounds
	{
		List<ScrollingBackground> backgroundList;
		//List<Texture2D> textureList;
		//List<Rectangle> rectanleList;

		public AllBackgrounds(List<Texture2D> textureList, int width, int height)
		{
			backgroundList = new List<ScrollingBackground>();
			if (textureList != null)
			{
				for (int i = 0; i < textureList.Count; i++)
				{
					backgroundList.Add(new ScrollingBackground(textureList[i], new Rectangle(i * width, 0, width, height)));
				}
			}
		}

		public void Update()
		{
			if (backgroundList.Count <= 0)
				return;
			for (int i = 0; i < backgroundList.Count; i++)
			{
				if (!(backgroundList[0].rectangle.X == 0 && Keyboard.GetState().IsKeyDown(Keys.Left) ||
						backgroundList[backgroundList.Count - 1].rectangle.X <= 0 && Keyboard.GetState().IsKeyDown(Keys.Right)))
					backgroundList[i].Update();
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var back in backgroundList)
			{
				back.Draw(spriteBatch);
			}
		}

	}

}
