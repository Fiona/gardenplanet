using System;
using System.Collections;
using System.Collections.Generic;
using StompyBlondie;
using UnityEngine;

namespace GardenPlanet
{

    public enum CharacterAction
    {
        Eat = 1,
        Yawn = 2,
        PassOut = 3
    }

    public class Character : MonoBehaviour
    {
        public struct Appearence
        {
            public string top;
            public string bottom;
            public string shoes;
            public string headAccessory;
            public string backAccessory;
            public string hair;

            public string eyes;
            public string eyebrows;
            public string mouth;
            public string nose;
            public string faceDetail1;
            public float faceDetail1Opacity;
            public bool faceDetail1FlipHorizontal;
            public string faceDetail2;
            public float faceDetail2Opacity;
            public bool faceDetail2FlipHorizontal;

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
        [HideInInspector]
        public Transform holdItemHolder;
        [HideInInspector]
        public bool isPlayer;

        // Locomotion related members
        protected Rigidbody rigidBody;
        protected GameController controller;
        protected Vector3 moveDirBuffer;
        protected Vector3 lookDirection;
        protected bool isJumping;
        protected bool attemptJump;
        protected Vector3 desiredRotation;
        protected bool lockFacing;
        protected bool doWalk;

        // Visuals related members
        protected GameObject visualsHolder;

        protected string baseModelName = "basemodel";
        protected Appearence appearence;
        protected Information information;
        protected GameObject baseModel;
        protected GameObject hairModel;
        protected CharacterFace face;

        protected List<Transform> lowerSpineBones;
        protected List<Transform> headBones;

        // Action and world related members
        protected CharacterAction currentAction = 0;
        protected InWorldItem itemCurrentlyHolding;

        protected bool passedOut;

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
            appearence = Player.defaultAppearence;
            information = Player.defaultInformation;
        }

        public virtual void Start()
        {
            controller = FindObjectOfType<GameController>();
            RegenerateVisuals();
            RegenerateFace();
            isPlayer = this == controller.player;
        }

        public virtual void FixedUpdate()
        {
            // Detect layer
            layer = (int) Math.Floor(transform.position.y * Consts.TILE_SIZE);

            // Deal with actions
            if(currentAction != 0 || passedOut)
            {
                mainAnimator.SetBool("DoWalk", false);
                mainAnimator.SetBool("DoRun", false);
                return;
            }

            // Visibly holding item?
            mainAnimator.SetBool("IsHolding", itemCurrentlyHolding != null);

            // Override stop for actions
            if(controller.GameInputManager == null || !controller.GameInputManager.directInputEnabled)
                return;

            // Handle walking/running
            rigidBody.AddForce(
                moveDirBuffer * Consts.CHARACTER_MOVE_ACCELERATION * Time.deltaTime,
                ForceMode.Impulse
            );
            rigidBody.velocity = Vector3.ClampMagnitude(
                rigidBody.velocity, doWalk ? Consts.CHARACTER_MAX_WALK_SPEED : Consts.CHARACTER_MAX_RUN_SPEED
            );

            if(Mathf.Abs(rigidBody.velocity.magnitude) > .5f)
            {
                mainAnimator.SetBool("DoWalk", doWalk);
                mainAnimator.SetBool("DoRun", !doWalk);
            }
            else
            {
                mainAnimator.SetBool("DoWalk", false);
                mainAnimator.SetBool("DoRun", false);
            }

            // Do rotation towards movement direction
            if(Mathf.Abs(moveDirBuffer.sqrMagnitude) > 0f && !lockFacing)
                lookDirection = moveDirBuffer;
            desiredRotation = (transform.position - (transform.position - lookDirection)).normalized;
            float step = Consts.CHARACTER_ROTATION_SPEED * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, desiredRotation, step, 0.0F);
            SetRotation(newDir);

            moveDirBuffer = Vector3.zero;
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
            moveDirBuffer = Vector3.zero;
            lookDirection = Vector3.zero;

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
                if(Mathf.Abs(found) < 20f)
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
            if(currentAction > 0)
                return;
            lockFacing = _lockFacing;
            switch(dir)
            {
                case Direction.Up:
                    moveDirBuffer += Vector3.forward;
                    break;
                case Direction.Down:
                    moveDirBuffer += Vector3.back;
                    break;
                case Direction.Left:
                    moveDirBuffer += Vector3.left;
                    break;
                case Direction.Right:
                    moveDirBuffer += Vector3.right;
                    break;
            }
        }

        /*
         * Points to a specified direction based on two axis.
         */
        public void LookInDirection(Vector3 direction)
        {
            if(currentAction > 0)
                return;
            lookDirection = direction;
        }

        /*
         * Character will turn towards the world position passed
         */
        public void TurnToWorldPoint(Vector3 turnTo)
        {
            if(currentAction > 0)
                return;
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
            if(currentAction > 0)
                return;
            attemptJump = true;
        }

        /*
         * Call to change the state of walking or running
         */
        public void SetDoWalk(bool _doWalk)
        {
            doWalk = _doWalk;
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
         * A coroutine that tells the character to immediately do the action passed, will immeditaly break if the
         * action cannot be done.
         */
        public IEnumerator DoAction(CharacterAction action)
        {
            // Can't start new action without finishing previous one
            if(currentAction > 0)
                yield break;

            // If player we should disable direct input and restore it after
            var prevInput = controller.GameInputManager.directInputEnabled;
            if(isPlayer)
                controller.GameInputManager.directInputEnabled = false;

            // Start action
            currentAction = action;

            // animation for eating
            if(currentAction == CharacterAction.Eat)
                mainAnimator.SetBool("DoEat", true);
            // animation for yawning
            if(currentAction == CharacterAction.Yawn)
            {
                mainAnimator.SetBool("DoYawn", true);
                if(this == controller.player)
                    controller.PlayerStopHoldingItem();
            }
            // animation for passing out
            if(currentAction == CharacterAction.PassOut)
                mainAnimator.SetBool("DoPassOut", true);

            // Wait for action to finish
            while(currentAction > 0)
                yield return new WaitForFixedUpdate();

            if(isPlayer)
                controller.GameInputManager.directInputEnabled = prevInput;
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
         * Allows setting the appearence struct
         */
        public void SetAppearence(Appearence _appearence)
        {
            appearence = _appearence;
            RegenerateVisuals();
            RegenerateFace();
        }

        /*
         * Allows setting the information struct
         */
        public void SetInformation(Information _information)
        {
            information = _information;
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
         * Sets skin colour
         */
        public void SetSkinColour(Color newColor)
        {
            appearence.skinColor = newColor;
            RegenerateSkin();
        }

        /*
         * Sets eyes
         */
        public void SetEyes(string newEyes)
        {
            appearence.eyes = newEyes;
            RegenerateFace();
        }

        public void SetEyeColour(Color newColour)
        {
            appearence.eyeColor = newColour;
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

        public void SetFaceDetail1Opacity(float newOpacity)
        {
            appearence.faceDetail1Opacity = newOpacity;
            RegenerateFace();
        }

        public void SetFaceDetail1FlipHorizontal(bool flipValue)
        {
            appearence.faceDetail1FlipHorizontal = flipValue;
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

        public void SetFaceDetail2Opacity(float newOpacity)
        {
            appearence.faceDetail2Opacity = newOpacity;
            RegenerateFace();
        }

        public void SetFaceDetail2FlipHorizontal(bool flipValue)
        {
            appearence.faceDetail2FlipHorizontal = flipValue;
            RegenerateFace();
        }

        /*
         * Sets hair
         */
        public void SetHair(string newHair)
        {
            appearence.hair = newHair;
            RegenerateVisuals();
        }

        /*
         * Sets hair colour
         */
        public void SetHairColour(Color newColor)
        {
            appearence.hairColor = newColor;
            RegenerateHairColour();
        }

        /*
         * Sets top
         */
        public void SetTop(string newTop)
        {
            appearence.top = newTop;
            RegenerateVisuals();
        }

        /*
         * Sets bottom
         */
        public void SetBottom(string newBottom)
        {
            appearence.bottom = newBottom;
            RegenerateVisuals();
        }

        /*
         * Sets shoes
         */
        public void SetShoes(string newShoes)
        {
            appearence.shoes = newShoes;
            RegenerateVisuals();
        }

        /*
         * Sets head accessory
         */
        public void SetHeadAccessory(string newHeadAcessory)
        {
            appearence.headAccessory = newHeadAcessory;
            RegenerateVisuals();
        }

        /*
         * Sets back accessory
         */
        public void SetBackAccessory(string newBackAccessory)
        {
            appearence.backAccessory= newBackAccessory;
            RegenerateVisuals();
        }

        /*
         * Call to start holding the item passed, any item currently being held will be put away.
         */
        public bool StartHoldingItem(ItemType itemType, Hashtable attributes)
        {
            StopHoldingItem();
            if(currentAction != 0 || passedOut)
                return false;
            itemCurrentlyHolding = controller.SpawnItem(itemType, attributes);
            if(itemCurrentlyHolding == null)
                return false;
            itemCurrentlyHolding.HoldInCharactersHands(this);
            return true;
        }

        /*
         * Call to stop holding the currently held item
         */
        public void StopHoldingItem()
        {
            if(itemCurrentlyHolding != null)
                itemCurrentlyHolding.PutBackIntoInventory();
            itemCurrentlyHolding = null;
        }

        /*
         * Call to drop the item currently being held
         */
        public bool DropHoldingItem()
        {
            if(itemCurrentlyHolding == null)
                return false;
            itemCurrentlyHolding.DropFromHands(this == controller.player);
            return true;
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

            baseModel = AddModelToVisuals(Consts.CHARACTERS_BASE_VISUAL_PATH + baseModelName);
            holdItemHolder = baseModel.transform.FindRecursive("item");
            if(holdItemHolder == null)
                Debug.LogError("Can't find a child called item in character!");
            var bonesToClone = baseModel.GetComponentInChildren<SkinnedMeshRenderer>();
            RegenerateSkin();

            var faceModel = AddModelToVisuals(Consts.CHARACTERS_BASE_VISUAL_PATH + baseModelName + "_face", bonesToClone);
            face = faceModel.GetComponentInChildren<CharacterFace>();

            if(appearence.top != "")
                AddModelToVisuals(Consts.CHARACTERS_TOPS_VISUAL_PATH + appearence.top, bonesToClone);
            if(appearence.bottom != "")
                AddModelToVisuals(Consts.CHARACTERS_BOTTOMS_VISUAL_PATH+ appearence.bottom, bonesToClone);
            if(appearence.shoes != "")
                AddModelToVisuals(Consts.CHARACTERS_SHOES_VISUAL_PATH + appearence.shoes, bonesToClone);
            if(appearence.backAccessory != "")
                AddModelToVisuals(Consts.CHARACTERS_BACK_ACCESSORIES_VISUAL_PATH + appearence.backAccessory, bonesToClone);
            if(appearence.headAccessory != "")
                AddModelToVisuals(Consts.CHARACTERS_HEAD_ACCESSORIES_VISUAL_PATH + appearence.headAccessory, bonesToClone);

            hairModel = null;
            if(appearence.hair != "")
                hairModel = AddModelToVisuals(Consts.CHARACTERS_HAIR_VISUAL_PATH + appearence.hair, bonesToClone);
            RegenerateHairColour();

            StartCoroutine(RebindAnimation());
        }

        private IEnumerator RebindAnimation()
        {
            yield return new WaitForFixedUpdate();
            mainAnimator.Rebind();
        }

        protected void RegenerateSkin()
        {
            if(baseModel == null)
                return;
            var skin = baseModel.GetComponentInChildren<SkinnedMeshRenderer>();
            foreach(var mat in skin.materials)
                mat.color = appearence.skinColor;
        }

        protected void RegenerateHairColour()
        {
            if(hairModel == null)
                return;
            hairModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color = appearence.hairColor;
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
            newObject.SetLayerRecursively(Consts.COLLISION_LAYER_CHARACTERS);
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

        // Animation event: Nom some
        protected void AnimatorNom()
        {
            if(itemCurrentlyHolding != null)
                itemCurrentlyHolding.transform.localScale -= new Vector3(.35f, .35f, .35f);
        }

        // Animation event: EatItem
        protected void AnimatorEatItemDone()
        {
            mainAnimator.SetBool("DoEat", false);
            currentAction = 0;
            if(itemCurrentlyHolding != null)
                Destroy(itemCurrentlyHolding.gameObject);
            itemCurrentlyHolding = null;
        }

        // Animation event: Close eyes
        protected void AnimatorCloseEyes()
        {
            face.SetFaceState(CharacterFace.FaceState.EYES_CLOSED);
        }

        // Animation event: YawnDone
        protected void AnimatorYawnDone()
        {
            face.SetFaceState(CharacterFace.FaceState.NORMAL);
            mainAnimator.SetBool("DoYawn", false);
            currentAction = 0;
            if(this == controller.player)
                controller.itemHotbar.UpdateItemInHand();
        }

        // Animation event: PassOutMid
        protected void AnimatorPassOutMid()
        {
            if(this == controller.player)
                controller.PlayerDropItemInHand();
            else
                DropHoldingItem();
        }

        // Animation event: PassOutDone
        protected void AnimatorPassOutDone()
        {
            currentAction = 0;
            passedOut = true;
        }

    }
}