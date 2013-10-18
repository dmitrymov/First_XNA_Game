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
	// need to addbackground to animation

	class Animation
	{
		int screenWidth;
		int screenHeight;
		Player player;
		List<Level> levelList;
		Level currentLevel;
		AllBackgrounds backgrounds;

		public Animation(ContentManager Content, int width, int height, List<Level> newLevelList)
		{
			if (!(newLevelList == null))
			{
				levelList = newLevelList;
				currentLevel = levelList[0];
			}
			else
			{
				levelList = new List<Level>();
				currentLevel = new Level(null, null);
			}
			screenWidth = width;
			screenHeight = height;
			player = new Player(Content.Load<Texture2D>("1234"), Content.Load<Texture2D>("bullet"), new Vector2(100, screenHeight - 70), 80, 135, screenWidth);
			List<Texture2D> l = new List<Texture2D>();
			l.Add(Content.Load<Texture2D>("background1"));
			l.Add(Content.Load<Texture2D>("tank_background"));
			l.Add(Content.Load<Texture2D>("tank_background2"));
			backgrounds = new AllBackgrounds(l, screenWidth, screenHeight);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			backgrounds.Draw(spriteBatch);
			currentLevel.Draw(spriteBatch);
			player.Draw(spriteBatch);
		}

		public void Update(GameTime gameTime)
		{
			backgrounds.Update();
			currentLevel.Update(gameTime);
			player.Update(gameTime);
		}

		public void ChangeToNextLevel()
		{
			int i = -1;
			if (currentLevel != null)
				i = levelList.IndexOf(currentLevel);
			if (i >= 0 && i < levelList.Count() - 1)
			{
				currentLevel = levelList[i + 1];
				// change player location
			}
		}

		public int GetLevelSizeX()
		{
			return currentLevel.GetSizeX();
		}

	}


	class Level
	{

		List<Texture2D> textureList;
		List<Rectangle> rectangleList;

		public Level(List<Texture2D> newTextureList, List<Rectangle> newRectangleList)
		{
			if (newTextureList == null || newRectangleList == null)
			{
				newTextureList = new List<Texture2D>();
				newRectangleList = new List<Rectangle>();
			}
			if (newTextureList.Count != newRectangleList.Count)
			{
				// hz 4e delat
			}
			textureList = newTextureList;
			rectangleList = newRectangleList;
		}

		public void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				// no need to move if begin or end of level
				// compare background firstt and last point
				for (int i = 0; i < rectangleList.Count; i++)
				{
					Rectangle temp = rectangleList[i];
					temp.X -= 3;
				}
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				// no need to move if begin or end of level
				// compare background firstt and last point
				for (int i = 0; i < rectangleList.Count; i++)
				{
					Rectangle temp = rectangleList[i];
					temp.X += 3;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < textureList.Count; i++)
			{
				spriteBatch.Draw(textureList[i], rectangleList[i], Color.White);
			}
		}

		public void ChangeLevel()
		{
			
		}


		public int GetSizeX()
		{
			int ans = 0;
			foreach (var rec in rectangleList)
			{
				ans += rec.Width;
			}
			return ans;
		}
	}

	//********************************     Player    ***********************************************
	class Player
	{
		float bulletTimer;
		Texture2D bulletTexture;
		int screenWidth;
		Texture2D texture;
		Rectangle rectangle;
		Vector2 position;
		Vector2 origin;
		Vector2 velocity;
		int currentFrame;
		int frameHeight;
		int frameWidth;
		float timer;
		float interval = 50;
		bool flying;
		int flyIndicate;
		List<Bullet> bulletList;

		public Player(Texture2D newTexture, Texture2D bulletTex, Vector2 newPosition, int newFrameWidth, int newFrameHeigth, int width)
		{
			bulletTexture = bulletTex;
			texture = newTexture;
			position = newPosition;
			frameHeight = newFrameHeigth;
			frameWidth = newFrameWidth;
			screenWidth = width;
			flying = false;
			flyIndicate = 50;
			bulletList = new List<Bullet>();
			bulletTimer = 0;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0);
			foreach (var bullet in bulletList)
			{
				bullet.Draw(spriteBatch);
			}
		}

		public void Update(GameTime gameTime)
		{
			rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
			origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				if (!flying)
					AnimateRight(gameTime);
				velocity.X = 3;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				if(!flying)
					AnimateLeft(gameTime);
				velocity.X = -3;
			}
			else if (!flying)
			{
				velocity = Vector2.Zero;
				currentFrame = 0;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up))
			{
				flying = true;
			}
			if (flying)
			{
				if(AnimateJump(gameTime)){
					MoveUp();
				}
			}
			if (position.X  - frameWidth/2 + velocity.X > 0 && position.X + frameWidth/2 + velocity.X < screenWidth)
				position += velocity;
			BulletsUpdate(gameTime);
		}

		// check if shooted and update bullets
		private void BulletsUpdate(GameTime gameTime)
		{
			bulletTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds/2;
			if (Keyboard.GetState().IsKeyDown(Keys.Space) && bulletTimer > interval)
			{
				bulletList.Add(new Bullet(bulletTexture, (int)position.X + frameWidth/2, (int)position.Y - 30, velocity));
				bulletTimer = 0;
			}
			foreach (var bullet in bulletList)
			{
				bullet.Update(gameTime);
			}
			for (int i = 0; i < bulletList.Count; i++)
			{
				if (bulletList[i].GetDistance() > screenWidth * 2)
				{
					bulletList.RemoveAt(i);
					i--;
				}
			}
		}

		private void AnimateRight(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
			if (timer > interval)
			{
				currentFrame++;
				timer = 0;
				if (currentFrame > 4)
					currentFrame = 0;
			}
		}

		private void AnimateLeft(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
			if (timer > interval)
			{
				currentFrame++;
				timer = 0;
				if (currentFrame > 9 || currentFrame < 5)
					currentFrame = 5;
			}
		}

		private bool AnimateJump(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (timer > interval)
			{
				if (currentFrame >= 12)
					return true;
				currentFrame++;
				timer = 0;
				if (currentFrame < 10)
					currentFrame = 10;
			}
			return false;
		}

		private void MoveUp()
		{
			if (flyIndicate >= -50)
			{
				if(flyIndicate > 0)
					position.Y -= 2;
				if(flyIndicate < 0)
					position.Y += 2;
				flyIndicate -= 2;
			}
			else
			{
				flying = false;
				flyIndicate = 50;
			}
		}


	}



	class Bullet
	{
		const int interval = 30;
		float timer;
		int distance;
		Texture2D texture;
		Rectangle rectangle;
		Vector2 velocity;

		public Bullet(Texture2D tex, int x, int y, Vector2 newVelocity)
		{
			distance = 0;
			texture = tex;
			rectangle = new Rectangle(x, y, texture.Width, texture.Height);
			velocity = newVelocity * 5;
			if (velocity.X == 0)
				velocity.X = 15;
			timer = 0;
		}

		public void Update(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (timer > interval)
			{
				distance += (int)Math.Abs(velocity.X);
				rectangle.X += (int)velocity.X;
				timer = 0;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, rectangle, Color.White);
			
		}

		public int GetDistance()
		{
			return distance;
		}

	}
}
