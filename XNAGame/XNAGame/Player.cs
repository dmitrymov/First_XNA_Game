using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGame
{

	//********************************     Player    ***********************************************
	class Player
	{
		protected const int movingSpeed = 3;
		protected const float interval = 50;
		protected int direction;
		//int stand = 0;
		protected bool blocked = false;
		protected Vector2 blockedVecor;
		//protected int screenWidth;
		protected Texture2D texture;
		protected Rectangle rectangle;
		protected Vector2 position;
		protected Vector2 origin;
		//Vector2 velocity;
		protected int currentFrame;
		protected const int frameHeight = 135;
		protected const int frameWidth = 80;
		protected float timer;
		protected bool flying;
		protected bool onFloor = false;

		public Player(Texture2D newTexture)
		{
			texture = newTexture;
			rectangle = new Rectangle(0, 0, frameWidth, frameHeight);
			blockedVecor = new Vector2(0, 0);
			flying = false;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0);
		}

		public virtual void Update(GameTime gameTime)
		{
			rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
			origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
			GravityForce(gameTime);
		}

		public void ChangePosition(Vector2 vec)
		{
			position.X = vec.X;
			position.Y = vec.Y;
		}

		public int Direction { get { return direction; } }

		public bool OnFloor { get { return onFloor; } set { onFloor = value; } }

		public bool Blocked { get { return blocked; } set { blocked = value; } }

		public Vector2 BlockedVector { get { return blockedVecor; } set { blockedVecor = value; } }

		public Vector2 Position { get { return position; } set { position = value; } }

		public Rectangle Rectangle { get { return rectangle; } }

		/*
		protected void AnimateRight(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
			if (timer > interval)
			{
				currentFrame++;
				timer = 0;
				if (currentFrame > 4)
					currentFrame = 0;
			}
			//stand = 0;
		}

		protected void AnimateLeft(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
			if (timer > interval)
			{
				currentFrame--;
				timer = 0;
				if (currentFrame > 9 || currentFrame < 5)
					currentFrame = 9;
			}
			//stand = 9;
		}

		protected bool AnimateJump(GameTime gameTime)	//falling
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
		}*/

		protected virtual void GravityForce(GameTime gameTime)
		{
			if (!onFloor)
			{
				flying = true;
				position.Y += 2;
				//AnimateJump(gameTime);
				return;
			}
			flying = false;
		}


	}


	//**************************************************   Human     ****************************************
	class Human : Player
	{
		//const int movingSpeed = 3;
		//const float interval = 50;
		//int stand = 0;
		//bool smash = false;
		float bulletTimer;
		Texture2D bulletTexture;
		int screenWidth;
		//Texture2D texture;
		//Rectangle rectangle;
		//Vector2 position;
		//Vector2 origin;
					//Vector2 velocity;
		//int currentFrame;
		//int frameHeight;
		//int frameWidth;
		//float timer;
		//bool flying;
		int JumpForce;
		List<Bullet> bulletList;
		//bool onFloor = false;

		public Human(Texture2D newTexture, Texture2D bulletTex, Vector2 newPosition, int newFrameWidth, int newFrameHeigth, int width)
			: base(newTexture)
		{
			bulletTexture = bulletTex;
			//texture = newTexture;
			position = newPosition;
			//frameHeight = newFrameHeigth;
			//frameWidth = newFrameWidth;
			screenWidth = width;
			//flying = false;
			JumpForce = 0;
			bulletList = new List<Bullet>();
			bulletTimer = 0;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0);
			base.Draw(spriteBatch);
			foreach (var bullet in bulletList)
			{
				bullet.Draw(spriteBatch);
			}
		}

		public override void Update(GameTime gameTime)
		{
			rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
			origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				if (!flying && (!blocked || blockedVecor.X <= 0))
					AnimateRight(gameTime);

				//velocity.X = movingSpeed;
				direction = 1;
				position.X += movingSpeed * direction;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				if (!flying && (!blocked || blockedVecor.X >= 0))
					AnimateLeft(gameTime);
				//velocity.X = -movingSpeed;
				direction = -1;
				position.X += movingSpeed * direction;
			}
			else if (!flying)
			{
				//velocity = Vector2.Zero;
				if (direction > 0)
					currentFrame = 0;
				else
					currentFrame = 9;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up) && !flying)
			{
				JumpForce = 50;
				flying = true;
			}
			GravityForce(gameTime);
			//if (position.X - frameWidth / 2 + movingSpeed > 0 && position.X + frameWidth / 2 + movingSpeed < screenWidth)
				//position.X += movingSpeed * direction;
			BulletsUpdate(gameTime);
		}

		// check if shooted and update bullets
		private void BulletsUpdate(GameTime gameTime)
		{
			bulletTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
			if (Keyboard.GetState().IsKeyDown(Keys.Space) && bulletTimer > interval)
			{
				bulletList.Add(new Bullet(bulletTexture, (int)position.X + frameWidth / 2, (int)position.Y - 30, movingSpeed, direction));
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
				currentFrame--;
				timer = 0;
				if (currentFrame > 9 || currentFrame < 5)
					currentFrame = 9;
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

		protected override void GravityForce(GameTime gameTime)
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

	//*****************************   Enemy   *****************************************************
	class Enemy : Player
	{

		public Enemy(Texture2D newTexture) : base(newTexture)
		{
			//texture = newTexture;
			position = ChoosePosition();
			rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
		}

		public override void Update(GameTime gameTime)
		{
			//rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
			//origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
			base.Update(gameTime);
			position.X += movingSpeed * direction;	// move
			AnimateMove(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
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
			return new Vector2(x, y);
		}

	}


	//*********************************   Bullet    ******************************************
	class Bullet
	{
		const int interval = 30;
		float timer;
		int distance;
		Texture2D texture;
		Rectangle rectangle;
		int movingSpeed;

		public Bullet(Texture2D tex, int x, int y, int speed, int side)
		{
			distance = 0;
			texture = tex;
			rectangle = new Rectangle(x, y, texture.Width, texture.Height);
			movingSpeed = speed * 5 * side;
			if (movingSpeed == 0)
				movingSpeed = 15 *side;
			timer = 0;
		}

		public void Update(GameTime gameTime)
		{
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (timer > interval)
			{
				distance += Math.Abs(movingSpeed);
				rectangle.X += movingSpeed;
				timer = 0;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{

			if (movingSpeed < 0)
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


	

}
