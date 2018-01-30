using System;
using UnityEngine;

namespace StompyBlondie
{
	public class Bobber: MonoBehaviour
	{
		public enum Dimension{X, Y, Z};

		public float bobDistance;
		public Dimension bobDirection;
		public float bobSpeed = 1f;

		Vector3 initialPosition;
		Vector3 targetPosition;
		float movementAmount;
		bool movementDir;

		public void Start()
		{
			initialPosition = transform.localPosition;
			if(bobDirection == Dimension.X)
				targetPosition = initialPosition + new Vector3(bobDistance, 0f, 0f);
			if(bobDirection == Dimension.Y)
				targetPosition = initialPosition + new Vector3(0f, bobDistance, 0f);
			if(bobDirection == Dimension.Z)
				targetPosition = initialPosition + new Vector3(0f, 0f, bobDistance);
		}

		public void Update()
		{
			if(movementAmount >= 1f)
			{
				movementAmount = 0f;
				movementDir = !movementDir;
			}
			else
				movementAmount += bobSpeed * Time.deltaTime;
			if(movementDir)
				transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, movementAmount);
			else
				transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, movementAmount);
		}
	}
}

