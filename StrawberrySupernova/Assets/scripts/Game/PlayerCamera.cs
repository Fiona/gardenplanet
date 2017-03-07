using System;
using UnityEngine;

namespace StrawberryNova
{

	public class PlayerCamera: MonoBehaviour
	{
	    private GameObject target = null;
	    private Vector3 offset;
	    private float distance;
	    private float speed;

	    public void Awake()
	    {
	    }
			
	    public void LateUpdate()
	    {
	        if(target == null)
	            return;

	        var rotation = transform.rotation;
	        // Gets a unit vector in the direction of the camera and multiplies
	        // by the distance. This is negated so it pushes away from the origin.
	        // The resulting position would be at the world origin so we need to
	        // adjust by the target world position.
			var desiredPosition = GetTargetPosition(target, distance);
	        // Smooth the camera movement a bit
	        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);
	    }

		public void SetTarget(GameObject target, float distance)
		{
			transform.position = GetTargetPosition(target, distance);
		}

		public void LockTarget(GameObject target, float distance, float speed = 1.0f)
		{
			this.target = target;
			this.distance = distance;
			this.speed = speed;
		}

		Vector3 GetTargetPosition(GameObject _target, float _distance)
		{
			return (-((transform.rotation * Vector3.forward) * _distance)) + _target.transform.position;
		}

	}

}