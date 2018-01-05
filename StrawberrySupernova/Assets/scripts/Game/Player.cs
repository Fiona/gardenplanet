using System;
using StompyBlondie;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{

    public class Player: MonoBehaviour
    {

        Rigidbody rigidBody;
        Vector3 walkDir;
        Vector3 walkDirBuffer;
        bool isJumping;
        bool attemptJump;
        Vector3 desiredRotation;
        GameController controller;
        bool lockFacing;
        [HideInInspector]
        public Inventory inventory;
        [HideInInspector]
        public int layer;
        public float maxEnergy;
        public float currentEnergy;

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

            inventory = new Inventory(Consts.PLAYER_INVENTORY_MAX_STACKS);
            maxEnergy = Consts.PLAYER_START_ENERGY;
            currentEnergy = maxEnergy;
        }

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            SetPassOutEvent();
        }

        public void PassOutTimeEvent(GameTime gameTime)
        {
            StartCoroutine(PassOut());
        }

        public void FixedUpdate()
        {

            // Detect layer
            layer = (int)Math.Floor(transform.position.y * Consts.TILE_SIZE);

            if(controller.GameInputManager == null || !controller.GameInputManager.directInputEnabled)
                return;

            // Handle walking
            if(rigidBody.velocity.magnitude < 1.0f)
                rigidBody.AddForce(
                    walkDirBuffer * Consts.PLAYER_SPEED * Time.deltaTime,
                    ForceMode.Impulse
                );

            // Do rotation towards movement direction
            if(Mathf.Abs(walkDirBuffer.sqrMagnitude) > 0f && !lockFacing)
                walkDir = walkDirBuffer;
            desiredRotation = (transform.position - (transform.position - walkDir)).normalized;
            float step = Consts.PLAYER_ROTATION_SPEED * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, desiredRotation, step, 0.0F);
            SetRotation(newDir);

            walkDirBuffer = new Vector3();
            lockFacing = false;

            // Deal with jumping
            isJumping = (Mathf.Abs(rigidBody.velocity.y) > 0.01f);
            if(isJumping)
                attemptJump = false;
            else if(attemptJump)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0.0f);
                rigidBody.AddForce(0, Consts.PLAYER_JUMP_FORCE * Time.deltaTime, 0, ForceMode.Impulse);
                attemptJump = false;
            }

        }

        /*
         * Rotates the player towards an object and returns when it finishes
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
                    Consts.PLAYER_ROTATION_SPEED*Time.deltaTime,
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
          Tells the player to walk in the specified direction.
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
         Player will turn towards the world position passed
         */
        public void TurnToWorldPoint(Vector3 turnTo)
        {
            if((transform.position - turnTo).magnitude > .2f)
                desiredRotation = turnTo - transform.position;
        }

        /*
         Gets the tile position directly in front of the player
         */
        public TilePosition GetTileInFrontOf()
        {
            var pos = (transform.position + (walkDir*.5f));
            var tilePos = new TilePosition(pos){layer = layer};
            return tilePos;
        }

        /*
         Call if the player should try to jump if possible
         */
        public void Jump()
        {
            attemptJump = true;
        }

        /*
         * When passed a tile it will immediately position and
         * turn the player to the passed tile position definition.
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

        public void SetRotation(Vector3 newDir)
        {
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }

        public IEnumerator Sleep()
        {
            controller.worldTimer.DontRemindMe(PassOutTimeEvent);
            yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeOut(2f));
            controller.worldTimer.GoToNextDay(Consts.PLAYER_WAKE_HOUR);
            SetPassOutEvent();
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeIn(3f));
            currentEnergy = maxEnergy;
        }

        public IEnumerator PassOut()
        {
            controller.StartCutscene();
            yield return DialoguePopup.ShowDialoguePopup(
                "Oh no!",
                "You pass out from exhaustion..."
            );
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeOut(4f, new Color(1f,1f,1f)));
            controller.worldTimer.DontRemindMe(PassOutTimeEvent);
            controller.worldTimer.gameTime = new GameTime(
                days: controller.worldTimer.gameTime.Days + 1,
                hours: Consts.PLAYER_PASS_OUT_WAKE_HOUR
            );
            SetPassOutEvent();
            controller.worldTimer.DoTimerEvents();
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeIn(4f, new Color(1f,1f,1f)));
            currentEnergy = maxEnergy * .75f;
            controller.EndCutscene();
        }

        /*
         * Attempt to use up some energy, true if successfully reduced.
         */
        public bool ConsumeEnergy(float amount)
        {
            if(Math.Abs(currentEnergy) < 0.01f)
                return false;
            currentEnergy -= amount;
            if(currentEnergy < 0.01f)
            {
                currentEnergy = 0f;
                StartCoroutine(PassOut());
            }
            return true;
        }

        /*
         * Attempt to increase energy by an amount, true if successfully increased.
         */
        public bool IncreaseEnergy(float amount)
        {
            if(currentEnergy >= maxEnergy)
                return false;
            currentEnergy += amount;
            if(currentEnergy > maxEnergy)
                currentEnergy = maxEnergy;
            return true;
        }

        private void SetPassOutEvent()
        {
            var passOutOn = new GameTime(days: controller.worldTimer.gameTime.Days + 1, hours: Consts.PLAYER_PASS_OUT_HOUR);
            controller.worldTimer.RemindMe(passOutOn, PassOutTimeEvent);
        }

    }

}