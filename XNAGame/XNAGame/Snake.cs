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
	class Snake
	{

		List<SnakeNode> snake;
		Texture2D head;
		Texture2D regular;
		Texture2D tail;

		public Snake(Texture2D headTexture, Texture2D tailTexture, Texture2D regularTexture, Rectangle newRectangle)
		{
			snake = new List<SnakeNode>();
			head = headTexture;
			regular = regularTexture;
			tail = tailTexture;
			snake.Add(new SnakeNode(head, newRectangle));
			addNode();
			addNode();
		}

		public void Update()
		{

		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var node in snake)
			{
				node.Draw(spriteBatch);
			}
		}

		public void Move()
		{
			int num = snake.Count;
			while (num > 0)
			{
				snake[num] = new SnakeNode(snake[num - 1]);
			}
			snake.Last().ChangeTexture(tail);
			snake.First().ChangeTexture(head);
			snake[0].Move();
		}

		public void addNode()
		{
			SnakeNode temp = snake.Last();
			temp.ChangeTexture(regular);
			snake.Add(new SnakeNode(tail, new Rectangle(temp.GetX()-temp.GetWidth(), temp.GetY(),
						temp.GetWidth(), temp.GetHeight())));

		}

		public void ChangeDirection(Vector2 direction)
		{
			
		}

	}

	class SnakeNode
	{
		Texture2D texture;
		Rectangle rectangle;
		Vector2 velocity;

		public SnakeNode(Texture2D newTexture, Rectangle newRec)
		{
			texture = newTexture;
			rectangle = newRec;
			velocity.X = 3;
			velocity.Y = 0;
		}

		public SnakeNode(SnakeNode copy)
		{
			this.texture = copy.texture;
			this.rectangle = copy.rectangle;
			this.velocity = copy.velocity;
		}

		public SnakeNode(Texture2D newTexture, Rectangle newRec, Vector2 vel)
		{
			texture = newTexture;
			rectangle = newRec;
			velocity = vel;
		}

		public void Update() { }

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, rectangle, Color.White);
		}

		public void Move(SnakeNode prevNode)
		{
			return;
		}

		
		public void Move()
		{
			rectangle.X += (int)velocity.X;
			rectangle.Y += (int)velocity.Y;
		}

		public void ChangeTexture(Texture2D newTexture)
		{
			texture = newTexture;
		}

		public void ChangeVelocity(Vector2 newVel)
		{
			velocity = newVel;
		}

		public int GetX() { return rectangle.X; }

		public int GetY() { return rectangle.Y; }

		public int GetWidth() { return rectangle.Width; }

		public int GetHeight() { return rectangle.Height; }

		public Vector2 GetVelocity()
		{
			return velocity;
		}

	}


}
