using System;

namespace StrawberryNova
{
	public class DirectionHelper
	{
		/*
		 * Pushes a Direction enum in either clockwise or
		 * anti-clockwise.
		 */
		public static Direction RotateDirection(Direction dir, RotationalDirection rot)
		{
			var newDirection = Direction.Down;
			if(rot == RotationalDirection.AntiClockwise)
			{
				switch(dir)
				{
					case Direction.Down:
						newDirection = Direction.Left;
						break;
					case Direction.Left:
						newDirection = Direction.Up;
						break;
					case Direction.Up:
						newDirection = Direction.Right;
						break;
					case Direction.Right:
						newDirection = Direction.Down;
						break;
				}
			}
			else if(rot == RotationalDirection.Clockwise)
			{
				switch(dir)
				{
					case Direction.Down:
						newDirection = Direction.Right;
						break;
					case Direction.Left:
						newDirection = Direction.Down;
						break;
					case Direction.Up:
						newDirection = Direction.Left;
						break;
					case Direction.Right:
						newDirection = Direction.Up;
						break;
				}
			}
			return newDirection;
		}

		/*
		 * Returns the direction passed as degrees, for use in
		 * setting Euler rotations of objects.
		 */
		public static float DirectionToDegrees(Direction dir)
		{
			var degrees = 180f;
			if(dir == Direction.Right)
				degrees -= 90f;
			if(dir == Direction.Up)
				degrees -= 180f;
			if(dir == Direction.Left)
				degrees += 90f;			
			return degrees;
		}

	}
}

