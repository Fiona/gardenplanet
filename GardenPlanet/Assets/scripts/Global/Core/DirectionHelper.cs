using System;

namespace GardenPlanet
{
	public class DirectionHelper
	{
		/*
		 * Pushes a Direction enum in either a clockwise or
		 * anti-clockwise rotation.
		 */
		public static Direction RotateDirection(Direction dir, RotationalDirection rot)
		{
			var numDirs = Enum.GetNames(typeof(Direction)).Length;
			if(dir == (Direction)0 && rot == RotationalDirection.AntiClockwise)
				return (Direction)(numDirs - 1);
			int rotDir = (rot == RotationalDirection.Clockwise ? 1 : -1);
			return (Direction)(((int)dir + rotDir) % numDirs);
		}

		/*
		 * Pushes an EightDirection enum in either a clockwise or
		 * anti-clockwise rotation.
		 */
		public static EightDirection RotateDirection(EightDirection dir, RotationalDirection rot)
		{
			var numDirs = Enum.GetNames(typeof(EightDirection)).Length;
			if(dir == (EightDirection)0 && rot == RotationalDirection.AntiClockwise)
				return (EightDirection)(numDirs - 1);
			int rotDir = (rot == RotationalDirection.Clockwise ? 1 : -1);
			return (EightDirection)(((int)dir + rotDir) % numDirs);
		}

		/*
		 * Returns the direction passed as degrees, for use in
		 * setting Euler rotations of objects.
		 */
		public static float DirectionToDegrees(Direction dir)
		{
			return 180f + (90f * (int)dir);
		}

		/*
		 * Returns the direction passed as degrees, for use in
		 * setting Euler rotations of objects.
		 */
		public static float DirectionToDegrees(EightDirection dir)
		{
			return 180f + (45f * (int)dir);
		}

	}
}

