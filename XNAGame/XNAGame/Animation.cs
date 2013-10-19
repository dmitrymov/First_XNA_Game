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
			Human player = new Human(Content.Load<Texture2D>("1234"), Content.Load<Texture2D>("bullet"), new Vector2(100, screenHeight - 160), 80, 135, screenWidth);
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
					int distance = Math.Abs((int)player.Position.X - (int)enemys[i].Position.X);
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
			player.OnFloor = false;
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
						break;
					}
				}
			}
			foreach (var enemy in enemys)
			{
				enemy.OnFloor = false;
				foreach (var rec in rectangleList)
				{
					if (pos.X >= rec.X && pos.X <= rec.X + rec.Width)
					{
						int distance = (int)pos.Y + (playerRec.Height / 2) - (int)rec.Y;
						if (distance >= 0 && distance <= 2)
						{
							enemy.OnFloor = true;
							break;
						}
					}
				}
			}
		}

		// this function gets Human and Enemy as Player
		// check if some object in game blocks the movement
		private void CheckBlockedWithObject(Player p)
		{
			foreach (var rec in rectangleList)
			{
				
				// if any object prevent to move => player.Smash = true;
			}
			p.Blocked = false;
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
				int distance = Math.Abs((int)player.Position.X - (int)enemys[i].Position.X);
				if (distance < 40)
				{
					enemys.RemoveAt(i);
					i--;
					// exit game
				}
			}
		}

	}

	

}
