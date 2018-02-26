using System;
using System.Collections;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class Character: MonoBehaviour
    {


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
        protected string topModelName = "testshirt";

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

        public void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.freezeRotation = true;
        }

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            RegenerateVisuals();
        }

        public void FixedUpdate()
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
                    turnToPos-myPos,
                    Consts.CHARACTER_ROTATION_SPEED*Time.deltaTime,
                    0.0F
                );
                SetRotation(newDir);

                float found = Vector3.Angle(transform.forward, turnToPos-myPos);
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
            var pos = (transform.position + (lookDirection.normalized*.5f));
            var tilePos = new TilePosition(pos){layer = layer};
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

        private void RegenerateVisuals()
        {
            if(visualsHolder == null)
            {
                visualsHolder = new GameObject("visuals");
                visualsHolder.transform.SetParent(transform, false);
                visualsHolder.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            visualsHolder.DestroyAllChildren();

            AddModelToVisuals(Consts.CHARACTERS_BASE_VISUAL_PATH + baseModelName);
            AddModelToVisuals(Consts.CHARACTERS_BASE_VISUAL_PATH + baseModelName + "_face");
            AddModelToVisuals(Consts.CHARACTERS_TOPS_VISUAL_PATH + topModelName);
        }

        private void AddModelToVisuals(string prefabPath)
        {
            var resource = Resources.Load(prefabPath) as GameObject;
            if(resource == null)
            {
                Debug.Log("Can't find visuals model " + prefabPath);
                return;
            }

            var newObject = Instantiate(resource);
            newObject.transform.SetParent(visualsHolder.transform, false);
        }

    }
}