﻿using System;
using System.Collections;
using System.Collections.Generic;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class Character : MonoBehaviour
    {
        public struct Appearence
        {
            public string top;
            public string bottom;
            public string shoes;
            public string headAccessory;
            public string backAccessory;

            public string eyes;
            public string eyebrows;
            public string mouth;
            public string nose;
            public string faceDetail1;
            public float faceDetail1Opacity;
            public string faceDetail2;
            public float faceDetail2Opacity;

            public Color eyeColor;
            public Color skinColor;
            public Color hairColor;
        }

        public struct Information
        {
            public string Name;
            public int seasonBirthday;
            public int dayBirthday;
        }

        [Range(.1f, 3f)]
        public float headScale = 1f;

        [Range(1f, 3f)]
        public float lowerSpineScale = 1f;

        public Animator mainAnimator;

        [HideInInspector]
        public int layer;

        protected Rigidbody rigidBody;
        protected GameController controller;
        protected Vector3 walkDirBuffer;
        protected Vector3 lookDirection;
        protected bool isJumping;
        protected bool attemptJump;
        protected Vector3 desiredRotation;
        protected bool lockFacing;

        protected GameObject visualsHolder;

        protected string baseModelName = "basemodel";
        protected Appearence appearence;
        protected Information information;
        protected CharacterFace face;

        protected List<Transform> lowerSpineBones;
        protected List<Transform> headBones;

        public TilePosition CurrentTilePosition
        {
            get
            {
                var wPos = new WorldPosition()
                {
                    x = this.transform.position.x,
                    y = this.transform.position.z
                };
                var tPos = new TilePosition(wPos)
                {
                    layer = layer
                };
                return tPos;
            }
        }

        public virtual void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.freezeRotation = true;
        }

        public virtual void Start()
        {
            controller = FindObjectOfType<GameController>();
            RegenerateVisuals();
            RegenerateFace();
        }

        public virtual void FixedUpdate()
        {
            // Detect layer
            layer = (int) Math.Floor(transform.position.y * Consts.TILE_SIZE);

            if(controller.GameInputManager == null || !controller.GameInputManager.directInputEnabled)
                return;

            // Handle walking
            if(rigidBody.velocity.magnitude < 1.0f)
                rigidBody.AddForce(
                    walkDirBuffer * Consts.CHARACTER_MOVE_SPEED * Time.deltaTime,
                    ForceMode.Impulse
                );
            mainAnimator.SetBool("DoWalk", Mathf.Abs(rigidBody.velocity.magnitude) > 0.1f);

            // Do rotation towards movement direction
            if(Mathf.Abs(walkDirBuffer.sqrMagnitude) > 0f && !lockFacing)
                lookDirection = walkDirBuffer;
            desiredRotation = (transform.position - (transform.position - lookDirection)).normalized;
            float step = Consts.CHARACTER_ROTATION_SPEED * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, desiredRotation, step, 0.0F);
            SetRotation(newDir);

            walkDirBuffer = Vector3.zero;
            lockFacing = false;

            // Deal with jumping
            isJumping = (Mathf.Abs(rigidBody.velocity.y) > 0.01f);
            if(isJumping)
                attemptJump = false;
            else if(attemptJump)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0.0f);
                rigidBody.AddForce(0, Consts.CHARACTER_JUMP_FORCE * Time.deltaTime, 0, ForceMode.Impulse);
                attemptJump = false;
            }

        }

        /*
         * Rotates the character towards a WorldObject and returns when it finishes
         */
        public IEnumerator TurnTowardsWorldObject(WorldObject worldObject)
        {
            var turnToPos = worldObject.gameObject.transform.position;
            var myPos = transform.position;

            while(true)
            {
                Vector3 newDir = Vector3.RotateTowards(
                    transform.forward,
                    turnToPos - myPos,
                    Consts.CHARACTER_ROTATION_SPEED * Time.deltaTime,
                    0.0F
                );
                SetRotation(newDir);

                float found = Vector3.Angle(transform.forward, turnToPos - myPos);
                if(Mathf.Abs(found) < 40f)
                    break;

                yield return new WaitForFixedUpdate();
            }
        }

        /*
          Tells the character to walk in the specified direction.
          It can be called again to add an additional direction to
          allow for diaganol movement.
         */
        public void WalkInDirection(Direction dir, bool _lockFacing)
        {
            lockFacing = _lockFacing;
            switch(dir)
            {
                case Direction.Up:
                    walkDirBuffer += Vector3.forward;
                    break;
                case Direction.Down:
                    walkDirBuffer += Vector3.back;
                    break;
                case Direction.Left:
                    walkDirBuffer += Vector3.left;
                    break;
                case Direction.Right:
                    walkDirBuffer += Vector3.right;
                    break;
            }
        }

        /*
         * Points to a specified direction based on two axis.
         */
        public void LookInDirection(Vector3 direction)
        {
            lookDirection = direction;
        }

        /*
         * Character will turn towards the world position passed
         */
        public void TurnToWorldPoint(Vector3 turnTo)
        {
            if((transform.position - turnTo).magnitude > .2f)
                desiredRotation = turnTo - transform.position;
        }

        /*
         * Gets the tile position directly in front of the character
         */
        public TilePosition GetTileInFrontOf()
        {
            var pos = (transform.position + (lookDirection.normalized * .5f));
            var tilePos = new TilePosition(pos) {layer = layer};
            return tilePos;
        }

        /*
         * Call if the character should try to jump if possible
         */
        public void Jump()
        {
            attemptJump = true;
        }

        /*
         * When passed a tile it will immediately position and
         * turn the character to the passed tile position definition.
         */
        public void SetPositionToTile(TilePosition pos)
        {
            transform.position = new Vector3(
                pos.x * Consts.TILE_SIZE,
                pos.layer * Consts.TILE_SIZE,
                pos.y * Consts.TILE_SIZE
            );
            var baseRotation = DirectionHelper.DirectionToDegrees(pos.dir);
            transform.localRotation = Quaternion.Euler(0, -baseRotation, 0);
        }

        /*
         * Immediately rotate the character to the supplied direction
         */
        public void SetRotation(Vector3 newDir)
        {
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }

        /*
         * Returns the appearence struct
         */
        public Appearence GetAppearence()
        {
            return appearence;
        }

        /*
         * Returns the information struct
         */
        public Information GetInformation()
        {
            return information;
        }

        /*
         * Rename the character
         */
        public void SetName(string newName)
        {
            information.Name = newName.Trim();
        }

        /*
         * Sets birthday
         */
        public void SetBirthday(int day, int season)
        {
            information.dayBirthday = (day > 0 && day <= Consts.NUM_DAYS_IN_SEASON ? day : 1);
            information.seasonBirthday = (season > 0 && season <= Consts.SEASONS.Length ? season : 1);
        }

        /*
         * Sets eyes
         */
        public void SetEyes(string newEyes)
        {
            appearence.eyes = newEyes;
            RegenerateFace();
        }

        /*
         * Sets eyebrows
         */
        public void SetEyebrows(string newEyebrows)
        {
            appearence.eyebrows = newEyebrows;
            RegenerateFace();
        }

        /*
         * Sets nose
         */
        public void SetNose(string newNose)
        {
            appearence.nose = newNose;
            RegenerateFace();
        }

        /*
         * Sets mouth
         */
        public void SetMouth(string newMouth)
        {
            appearence.mouth = newMouth;
            RegenerateFace();
        }

        /*
         * Sets face detail 1
         */
        public void SetFaceDetail1(string newFaceDetail)
        {
            appearence.faceDetail1 = newFaceDetail;
            RegenerateFace();
        }

        /*
         * Sets face detail 2
         */
        public void SetFaceDetail2(string newFaceDetail)
        {
            appearence.faceDetail2 = newFaceDetail;
            RegenerateFace();
        }

        protected void RegenerateVisuals()
        {
            if(visualsHolder == null)
            {
                visualsHolder = new GameObject("visuals");
                visualsHolder.transform.SetParent(transform, false);
            }

            headBones = new List<Transform>();
            lowerSpineBones = new List<Transform>();
            armatures = new List<Transform>();

            visualsHolder.DestroyAllChildren();

            var baseModel = AddModelToVisuals(Consts.CHARACTERS_BASE_VISUAL_PATH + baseModelName);
            var bonesToClone = baseModel.GetComponentInChildren<SkinnedMeshRenderer>();

            var faceModel = AddModelToVisuals(Consts.CHARACTERS_BASE_VISUAL_PATH + baseModelName + "_face", bonesToClone);
            face = faceModel.GetComponentInChildren<CharacterFace>();

            if(appearence.top != "")
                AddModelToVisuals(Consts.CHARACTERS_TOPS_VISUAL_PATH + appearence.top, bonesToClone);

            mainAnimator.Rebind();
        }

        protected void RegenerateFace()
        {
            if(face == null)
            {
                Debug.Log("Face object is not set");
                return;
            }
            face.Recreate(appearence);
        }

        protected List<Transform> armatures;

        protected GameObject AddModelToVisuals(string prefabPath, SkinnedMeshRenderer bonesToClone = null)
        {
            var resource = Resources.Load(prefabPath) as GameObject;
            if(resource == null)
            {
                Debug.Log("Can't find visuals model " + prefabPath);
                return null;
            }

            var newObject = Instantiate(resource);
            newObject.transform.SetParent(visualsHolder.transform, false);
            armatures.Add(newObject.transform.Find("Armature").transform);

            if(bonesToClone != null)
            {
                var boneClone = newObject.AddComponent<BoneClone>();
                boneClone.rendererToClone = bonesToClone;
            }

            var modelmeshrenderer = newObject.GetComponentInChildren<SkinnedMeshRenderer>();
            foreach(var b in modelmeshrenderer.bones)
            {
                if(b.name == "head")
                    headBones.Add(b);
                if(b.name == "spine.lower")
                    lowerSpineBones.Add(b);
            }

            return newObject;
        }

        protected void LateUpdate()
        {
            foreach(var a in armatures)
                a.localScale = Vector3.one;
            if(headBones.Count > 0)
                foreach(var head in headBones)
                    head.localScale = new Vector3(headScale, headScale, headScale);

            if(lowerSpineBones.Count > 0)
                foreach(var spine in lowerSpineBones)
                    spine.localScale = new Vector3(lowerSpineScale, lowerSpineScale, lowerSpineScale);
        }
    }
}