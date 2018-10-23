using System;
using System.IO;
using UnityEngine;

namespace GardenPlanet
{

    public class PlayerCamera: MonoBehaviour
    {
        public float distance;
        public float speed;
        public float verticalTargetAdjustment = .4f;
        [Range(0f, 1f)]
        public float lookAheadStrength = .4f;

        private GameObject target = null;
        private Vector3 offset;
        private Vector3 lookAheadAdjust;

        public void Awake()
        {
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

        public void SetLookAheadPlayer(Player player)
        {
            lookAheadAdjust = player.GetFacing() * player.GetSpeed();
        }

        Vector3 GetTargetPosition(GameObject _target, float _distance)
        {
            var pos = _target.transform.position +
                      (lookAheadAdjust * lookAheadStrength) +
                      (Vector3.up * verticalTargetAdjustment);
            return (-((transform.rotation * Vector3.forward) * _distance)) + pos;
        }
    }

}