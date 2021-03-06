using System;
using System.IO;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace GardenPlanet
{

    public class InGameCamera: MonoBehaviour
    {
        public float distance;
        public float speed;
        public float verticalTargetAdjustment = .4f;
        [Range(0f, 1f)]
        public float lookAheadStrength = .4f;

        private GameObject target = null;
        private Vector3 offset;
        private Vector3 lookAheadAdjust;
        private Camera camera;

        public void Awake()
        {
            camera = GetComponent<Camera>();
            lookAheadAdjust = Vector3.zero;
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

        public void InstantSetTarget(GameObject target, float distance)
        {
            transform.position = GetTargetPosition(target, distance);
        }

        public void LockTarget(GameObject target, float distance, float speed = 1.0f)
        {
            this.target = target;
            this.distance = distance;
            this.speed = speed;
        }

        public void SetLookAheadCharacter(Character character)
        {
            lookAheadAdjust = character.GetFacing() * character.GetSpeed();
        }

        Vector3 GetTargetPosition(GameObject _target, float _distance)
        {
            var pos = _target.transform.position +
                      (lookAheadAdjust * lookAheadStrength) +
                      (Vector3.up * verticalTargetAdjustment);
            return (-((transform.rotation * Vector3.forward) * _distance)) + pos;
        }

        /*
         * Uses the Unity WorldToScreenPoint method to convert a Vector3 position in world space to screen space.
         */
        public Vector3 WorldToScreenPoint(Vector3 worldPoint)
        {
            return camera.WorldToScreenPoint(worldPoint);
        }
    }

}