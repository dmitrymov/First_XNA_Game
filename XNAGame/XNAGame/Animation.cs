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

	//********************************     Animation     *******************************************
	class Animation
	{
		int screenWidth;
		int screenHeight;
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
			Player player = new Player(Content.Load<Texture2D>("1234"), Content.Load<Texture2D>("bullet"), new Vector2(100, screenHeight - 160), 80, 135, screenWidth);
			currentLevel.Player = player;
			currentLevel.EnemyTexture = Content.Load<Texture2D>("enemy");
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
		}

		public void Update(GameTime gameTime)
		{
			backgrounds.Update();
			currentLevel.Update(gameTime);
		}

		public void ChangeToNextLevel()
		{
			Player pl = currentLevel.Player;
			int i = -1;
			if (currentLevel != null)
				i = levelList.IndexOf(currentLevel);
			if (i >= 0 && i < levelList.Count() - 1)
			{
				currentLevel = levelList[i + 1];
				pl.ChangePosition(new Vector2(100, screenHeight - 70));
				currentLevel.Player = pl;
			}
			
		}

		public int GetLevelSizeX()
		{
			return currentLevel.GetSizeX();
		}


	}


	//********************************     Level   *************************************************
	class Level
	{

		List<Texture2D> textureList;
		List<Rectangle> rectangleList;
		Player player;
		Texture2D enemyTexture;
		List<Enemy> enemys;

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
			enemys = new List<Enemy>();
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
			else if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				// no need to move if begin or end of level
				// compare background firstt and last point
				for (int i = 0; i < rectangleList.Count; i++)
				{
					Rectangle temp = rectangleList[i];
					temp.X += 3;
				}
			}
			CheckOnFloor();
			player.Update(gameTime);
			EnemyUpdate(gameTime);
		}

		private void EnemyUpdate(GameTime gameTime)
		{
			CheckSmashWithEnemy();
			if (enemys.Count > 0)
			{
				foreach (var enemy in enemys)
				{
					enemy.Update(gameTime);
				}
				for (int i = 0; i < enemys.Count; i++)
				{
					int distance = Math.Abs((int)player.Position.X - (int)enemys[i].GetXYPosition().X);
					if (distance > 1000)
					{
						enemys[i] = null;
						enemys.RemoveAt(i);
						i--;
					}
				}
			}
			else
				enemys.Add(new Enemy(enemyTexture));
		}


		private void CheckOnFloor()
		{
			Vector2 pos = player.Position;
			Rectangle playerRec = player.Rectangle;
			foreach (var rec in rectangleList)
			{
				if (pos.X >= rec.X && pos.X <= rec.X + rec.Width)
				{
					int distance = (int)pos.Y + (playerRec.Height / 2) - (int)rec.Y;
					if (distance >= 0 && distance <= 2)
					{
						player.OnFloor = true;
						return;
					}
				}
			}
			player.OnFloor = false;
		}

		private void CheckSmashIntoObjects()
		{
			// if any object prevent to move => player.Smash = true;
			player.Smash = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < textureList.Count; i++)
			{
				spriteBatch.Draw(textureList[i], rectangleList[i], Color.White);
			}
			player.Draw(spriteBatch);
			foreach (var enemy in enemys)
			{
				enemy.Draw(spriteBatch);
			}
		}

		public Player Player { get { return player; } set { player = value; } }

		public Texture2D EnemyTexture { get { return enemyTexture; } set { enemyTexture = value; } }

		public void ChangeLevel()
		{
			// ??????
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

		private void CheckSmashWithEnemy()
		{
			for(int i = 0; i < enemys.Count; i++)
			{
				int distance = Math.Abs((int)player.Position.X - (int)enemys[i].GetXYPosition().X);
				if (distance < 40)
				{
					enemys.RemoveAt(i);
					i--;
					// exit game
				}
			}
		}

	}

	//********************************     Player    ***********************************************
	class Player
	{
		const int movingSpeed = 3;
		const float interval = 50;
		bool smash = false;
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
		bool flying;
		int JumpForce;
		List<Bullet> bulletList;
		int stand = 0;
		bool onFloor = false;

		public Player(Texture2D newTexture, Texture2D bulletTex, Vector2 newPosition, int newFrameWidth, int newFrameHeigth, int width)
		{
			bulletTexture = bulletTex;
			texture = newTexture;
			position = newPosition;
			frameHeight = newFrameHeigth;
			frameWidth = newFrameWidth;
			screenWidth = width;
			flying = false;
			JumpForce = 0;
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
				velocity.X = movingSpeed;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				if(!flying)
					AnimateLeft(gameTime);
				velocity.X = -movingSpeed;
			}
			else if (!flying)
			{
				velocity = Vector2.Zero;
				currentFrame = stand;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up) && !flying)
			{
				JumpForce = 50;
				flying = true;
			}
			GravityForce(gameTime);
			if (position.X  - frameWidth/2 + velocity.X > 0 && position.X + frameWidth/2 + velocity.X < screenWidth)
				position += velocity;
			BulletsUpdate(gameTime);
		}

		public void ChangePosition(Vector2 vec)
		{
			position.X = vec.X;
			position.Y = vec.Y;
		}

		public bool OnFloor { get { return onFloor; } set { onFloor = value; } }

		public bool Smash { get { return smash; } set { smash = value; } }

		public Vector2 Position { get { return position; } set { position = value; } }

		public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

		public Rectangle Rectangle { get { return rectangle; } }

		// check if shooted and update bullets
		private void BulletsUpdate(GameTime gameTime)
		{
			bulletTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds/2;
			if (Keyboard.GetState().IsKeyDown(Keys.Space) && bulletTimer > interval)
			{
				int side = 1;
				if(stand != 0)
					side = -1;
				bulletList.Add(new Bullet(bulletTexture, (int)position.X + frameWidth/2, (int)position.Y - 30, velocity, side));
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
			stand = 0;
		}

		private void AnimateLeft(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
			if (timer > interval)
			{
				currentFrame--;
				timer = 0;
				if (currentFrame > 9 || currentFrame < 5)
					currentFrame = 9;
			}
			stand = 9;
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
			if (JumpForce > 0)
			{
				if(JumpForce > 0)
					position.Y -= 2;
				if(JumpForce < 0)
					position.Y += 2;
				JumpForce -= 2;
			}
			else
			{
				flying = false;
				JumpForce = 50;
			}
		}

		private void GravityForce(GameTime gameTime)
		{
			if (JumpForce > 0)
			{
				position.Y -= 2;
				AnimateJump(gameTime);
				JumpForce -= 2;
				return;
			}
			// add here if smashed to obsticle
			else if (!onFloor)
			{
				flying = true;
				position.Y += 2;
				AnimateJump(gameTime);
				return;
			}
			flying = false;
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

		public Bullet(Texture2D tex, int x, int y, Vector2 newVelocity, int side)
		{
			distance = 0;
			texture = tex;
			rectangle = new Rectangle(x, y, texture.Width, texture.Height);
			velocity = newVelocity * 5;
			if (velocity.X == 0)
				velocity.X = 15*side;
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

			if (velocity.X < 0)
			{
				spriteBatch.Draw(texture, rectangle, null, Color.White, 3.14f, new Vector2(-60, 15), SpriteEffects.None, 0);
			}
			else
			{
				spriteBatch.Draw(texture, rectangle, Color.White);
			}
		}

		public int GetDistance()
		{
			return distance;
		}

	}


	class Enemy
	{
		const int movingSpeed = 3;
		const int interval = 50;
		Texture2D texture;
		Rectangle rectangle;
		Vector2 position;
		Vector2 origin;
		int currentFrame;
		int direction;
		int frameWidth = 80;
		int frameHeight = 135;

		float timer = 0;

		public Enemy(Texture2D newTexture)
		{
			texture = newTexture;
			position = ChoosePosition();
			rectangle = new Rectangle(currentFrame*frameWidth, 0, frameWidth, frameHeight);
		}

		public void Update(GameTime gameTime)
		{
			rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
			position.X += movingSpeed * direction;	// move
			origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
			AnimateMove(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0);
		}

		public Vector2 GetXYPosition()
		{
			return position;
		}

		public int GetDirection()
		{
			return direction;
		}


		private void AnimateMove(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
			if (timer > interval)
			{
				timer = 0;
				ChangeFrame();
			}
		}

		private void ChangeFrame()
		{
			currentFrame += direction;
			if (direction > 0 && currentFrame > 4)
				currentFrame = 0;
			if (direction < 0 && currentFrame < 5)
				currentFrame = 9;
		}

		private Vector2 ChoosePosition()
		{
			int y = 375;
			Random ran = new Random();
			int x = ran.Next(0, 2);
			if (x == 0)
			{
				x = -20;
				direction = 1;
				currentFrame = 0;
			}
			else
			{
				x = 820;
				direction = -1;
				currentFrame = 9;
			}
			return new Vector2(x,y);
		}

	}

}
