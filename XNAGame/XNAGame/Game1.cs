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

	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		int screenWidth;
		int screenHeight;
		Animation animation;
		enum GameState { MainMenu, LevelSelect, Level1, Level2 }
		//GameState currentGameState = GameState.MainMenu;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			//graphics.ToggleFullScreen();
			screenWidth = GraphicsDevice.Viewport.Width;
			screenHeight = GraphicsDevice.Viewport.Height;
			List<Level> levelList = LevelsCreate();
			animation = new Animation(Content, screenWidth, screenHeight, levelList);
			
			base.Initialize();
		}

		private List<Level> LevelsCreate()
		{
			List<Level> ans = new List<Level>();
			ans.Add(CreateDefaultLevel());
			return ans;
		}

		private Level CreateDefaultLevel()
		{
			List<Texture2D> newTxt = new List<Texture2D>();
			List<Rectangle> newRec = new List<Rectangle>();
			newTxt.Add(Content.Load<Texture2D>("ground"));
			newTxt.Add(Content.Load<Texture2D>("ground"));
			newRec.Add(new Rectangle(0, GraphicsDevice.Viewport.Height - 40, GraphicsDevice.Viewport.Width, newTxt[0].Height));
			newRec.Add(new Rectangle(200, GraphicsDevice.Viewport.Height - 60, 40, 40));
			Level defLevel = new Level(newTxt, newRec);
			return defLevel;
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}


		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			animation.Update(gameTime);
			base.Update(gameTime);
		}


		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			animation.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
