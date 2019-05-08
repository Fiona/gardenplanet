using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using StompyBlondie;
using StompyBlondie.Common.Types;
using StompyBlondie.Utils;
using StompyBlondie.Systems;
using StompyBlondie.Extensions;

namespace GardenPlanet
{

    public enum CharacterAction
    {
        Eat = 1,
        Yawn = 2,
        PassOut = 3,
        BedStart = 4,
        BedEnd = 5
    }

    public class Character : MonoBehaviour
    {
        public struct CharacterData
        {
            public Appearence appearence;
            public Information information;
        }

        public struct Appearence
        {
            public string top;
            public string bottom;
            public string shoes;
            public string headAccessory;
            public string backAccessory;
            public string hair;
            public bool hideHair;

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
            public string id;
            public string Name;
            public int seasonBirthday;
            public int dayBirthday;

            public class Bed
            {
                public string type;
                public MapWorldPosition location;
            };
            public Bed bed;
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
        public Map currentMap;
        public bool isPlayer => id == Consts.CHAR_ID_PLAYER;

        public string id
        {
            get { return information.id; }
            set { information.id = value; }
        }

        protected GameController controller;
        protected Controller baseController;

        // Locomotion related members
        protected Rigidbody rigidBody;
        protected Collider collider;
        protected Vector3 moveDirBuffer;
        protected Vector3 lookDirection;
        protected bool isJumping;
        protected bool attemptJump;
        protected Vector3 desiredRotation;
        protected bool lockFacing;
        protected bool doWalk;

        // Visuals related members
        protected GameObject visualsHolder;

        protected string baseModelName = "baseModel";
        protected Appearence appearence;
        protected Information information;
        protected GameObject baseModel;
        protected GameObject hairModel;
        protected CharacterFace face;
        protected Dictionary<string, Transform> boneTransforms;

        protected List<Transform> lowerSpineBones;
        protected List<Transform> headBones;

        // Action and world related members
        protected CharacterAction currentAction = 0;
        protected InWorldItem itemCurrentlyHolding;
        protected Effect sleepingEffect;

        protected bool passedOut;

        public MapTilePosition CurrentTilePosition
        {
            get
            {
                var wPos = new WorldPosition
                {
                    x = this.transform.position.x,
                    y = this.transform.position.z
                };
                var tPos = new MapTilePosition(currentMap, wPos)
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
            collider = GetComponent<Collider>();
            appearence = Player.defaultAppearence;
            information = Player.defaultInformation;
            controller = FindObjectOfType<GameController>();
            baseController = FindObjectOfType<Controller>();
            boneTransforms = new Dictionary<string, Transform>();
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
                moveDirBuffer * Consts.CHARACTER_MOVE_ACCELERATION * Time.fixedDeltaTime,
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
            float step = Consts.CHARACTER_ROTATION_SPEED * Time.fixedDeltaTime;
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
                rigidBody.AddForce(0, Consts.CHARACTER_JUMP_FORCE * Time.fixedDeltaTime, 0, ForceMode.Impulse);
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
         * Points to a specified eight direction
         */
        public void LookInDirection(EightDirection direction)
        {
            if(currentAction > 0)
                return;
            var baseRotation = DirectionHelper.DirectionToDegrees(direction);
            lookDirection = new Vector3(0, baseRotation, 0);
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
        public void SetPositionToTile(TilePosition pos, Map map = null)
        {
            transform.position = new Vector3(
                pos.x * Consts.TILE_SIZE,
                pos.layer * Consts.TILE_SIZE,
                pos.y * Consts.TILE_SIZE
            );
            var baseRotation = DirectionHelper.DirectionToDegrees(pos.dir);
            transform.localRotation = Quaternion.Euler(0, -baseRotation, 0);
            if(map != null)
                currentMap = map;
        }

        /*
         * When passed a TileMarker it will immediately position and
         * turn the character to the passed Tile Marker position.
         */
        public void SetPositionToTile(TileMarker tileMarker, Map map = null)
        {
            transform.position = new Vector3(
                tileMarker.x * Consts.TILE_SIZE,
                tileMarker.layer * Consts.TILE_SIZE,
                tileMarker.y * Consts.TILE_SIZE
            );
            var baseRotation = DirectionHelper.DirectionToDegrees(tileMarker.direction);
            transform.localRotation = Quaternion.Euler(0, -baseRotation, 0);
            if(map != null)
                currentMap = map;
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
         * Immediately rotate the character to the supplied rotation in degrees
         */
        public void SetRotation(float degrees)
        {
            transform.localRotation = Quaternion.Euler(0, degrees, 0);
        }

        /*
         * A coroutine that tells the character to immediately do the action passed, will immeditaly break if the
         * action cannot be done.
         */
        public IEnumerator DoAction(CharacterAction action, GameObject actionObject = null)
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
                if(this == controller.world.player)
                    controller.PlayerStopHoldingItem();
            }
            // animation for passing out
            if(currentAction == CharacterAction.PassOut)
                mainAnimator.SetBool("DoPassOut", true);
            // animation for getting into bed
            if(currentAction == CharacterAction.BedStart)
                yield return StartCoroutine(JumpIntoBed(actionObject));
            if(currentAction == CharacterAction.BedEnd)
                yield return StartCoroutine(JumpOutOfBed(actionObject));

            // Wait for action to finish
            while(currentAction > 0)
                yield return new WaitForFixedUpdate();

            if(isPlayer)
                controller.GameInputManager.directInputEnabled = prevInput;
        }

        /*
         * Gets bed information for this character or null if they don't have bed info set
         */
        public Information.Bed GetBedInformation()
        {
            return information.bed;
        }

        /*
         * Sets the bed information for this character
         */
        public void SetBed(Information.Bed bedInformation)
        {
            information.bed = bedInformation;
            controller.world.SetBed(id, information.bed);
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
         * Up-to-date character data struct
         */
        public CharacterData GetCharacterData => new CharacterData
        {
            appearence = appearence,
            information = information
        };

        /*
         * Updates data in one struct
         */
        public void SetCharacterData(CharacterData newData)
        {
            SetInformation(newData.information);
            SetAppearence(newData.appearence);
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
        public bool StartHoldingItem(ItemType itemType, Attributes attributes)
        {
            StopHoldingItem();
            if(currentAction != 0 || passedOut)
                return false;
            itemCurrentlyHolding = controller.world.SpawnItem(itemType, attributes);
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
            itemCurrentlyHolding.DropFromHands(this == controller.world.player);
            return true;
        }

        /*
         * Stops the character colliding with anything
         */
        public void DisableCollision()
        {
            collider.enabled = false;
            rigidBody.detectCollisions = false;
        }

        /*
         * Starts the character colliding with other stuff
         */
        public void EnableCollision()
        {
            collider.enabled = true;
            rigidBody.detectCollisions = true;
        }

        /*
         * Turns off all physics on this character
         */
        public void DisableRigidbody()
        {
            rigidBody.isKinematic = true;
        }

        /*
         * Turns physics back on for this character
         */
        public void EnableRigidbody()
        {
            rigidBody.isKinematic = false;
        }

        /*
         * Returns the direction the character wants to face
         */
        public Vector3 GetFacing()
        {
            return desiredRotation;
        }

        /*
         * Returns how fast the character is moving
         */
        public float GetSpeed()
        {
            return rigidBody.velocity.magnitude;
        }

        /*
         * Saves out the information on this Character to a character data json file
         */
        public void SaveToFile()
        {
            var filepath = GetCharacterDataFilePath();

            // Check directories exist
            if(!Directory.Exists(Consts.DATA_DIR))
                Directory.CreateDirectory(Consts.DATA_DIR);
            if(!Directory.Exists(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_CHARACTERS_DATA)))
                Directory.CreateDirectory(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_CHARACTERS_DATA));

            // Collate data and save out
            JsonHandler.SerializeToFile(GetCharacterData, filepath);
        }

        /*
         * The path to save character data files out to
         */
        public string GetCharacterDataFilePath()
        {
            return Path.Combine(
                Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_CHARACTERS_DATA),
                $"{id}.{Consts.FILE_EXTENSION_CHARACTER_DATA}"
            );
        }

        protected void RegenerateVisuals()
        {
            if(visualsHolder != null)
                Destroy(visualsHolder);

            visualsHolder = new GameObject("visuals");
            visualsHolder.transform.SetParent(transform, false);

            // Copy animator into the visuals holder, it looks after the animations
            var newAnimator = visualsHolder.AddComponent<Animator>();
            newAnimator.runtimeAnimatorController = mainAnimator.runtimeAnimatorController;
            newAnimator.avatar = mainAnimator.avatar;
            newAnimator.applyRootMotion = mainAnimator.applyRootMotion;
            newAnimator.updateMode = mainAnimator.updateMode;
            newAnimator.cullingMode = mainAnimator.cullingMode;

            DestroyImmediate(mainAnimator);
            mainAnimator = newAnimator;

            // Add animation event pass-through component
            var animationEventListener = visualsHolder.AddComponent<CharacterAnimatorEventListener>();
            animationEventListener.character = this;

            headBones = new List<Transform>();
            lowerSpineBones = new List<Transform>();
            armatures = new List<Transform>();

            baseModel = AddModelToVisuals(Consts.CHARACTERS_BASE_MODEL_VISUAL_PATH + baseModelName);
            var bonesToClone = baseModel.GetComponentInChildren<SkinnedMeshRenderer>();
            RegenerateSkin();

            var faceModel = AddModelToVisuals(Consts.CHARACTERS_BASE_MODEL_VISUAL_PATH + baseModelName + "Face", bonesToClone);
            face = faceModel.GetComponentInChildren<CharacterFace>();

            if(appearence.top != "")
                AddModelToVisuals(Consts.CHARACTERS_TOPS_VISUAL_PATH + appearence.top, bonesToClone);
            if(appearence.bottom != "")
                AddModelToVisuals(Consts.CHARACTERS_BOTTOMS_VISUAL_PATH+ appearence.bottom, bonesToClone);
            if(appearence.shoes != "")
                AddModelToVisuals(Consts.CHARACTERS_SHOES_VISUAL_PATH + appearence.shoes, bonesToClone);
            if(appearence.backAccessory != "")
                AddModelToVisuals(Consts.CHARACTERS_BACK_ACCESSORIES_VISUAL_PATH + appearence.backAccessory, bonesToClone);
            var hideHair = false;
            if(appearence.headAccessory != "")
            {
                AddModelToVisuals(Consts.CHARACTERS_HEAD_ACCESSORIES_VISUAL_PATH + appearence.headAccessory,
                    bonesToClone);
                hideHair = baseController.globalConfig.appearence.headAccessories[appearence.headAccessory].hideHair;
            }

            hairModel = null;
            if(appearence.hair != "" && !hideHair)
                hairModel = AddModelToVisuals(Consts.CHARACTERS_HAIR_VISUAL_PATH + appearence.hair, bonesToClone);

            RegenerateHairColour();
            StartCoroutine(RebindAnimation());
        }

        private IEnumerator RebindAnimation()
        {
            var itemTransforms = new[] {"item", "leg_foot_L", "leg_foot_R", "head"};
            AnimatorUtility.OptimizeTransformHierarchy(visualsHolder, itemTransforms);

            yield return new WaitForFixedUpdate();

            holdItemHolder = transform.FindRecursive("item");
            if(holdItemHolder == null)
                Debug.LogError("Can't find a child called item in character!");
            boneTransforms = new Dictionary<string, Transform>();
            foreach(var trans in itemTransforms)
                boneTransforms[trans] = transform.FindRecursive(trans);
            baseModel = transform.FindRecursive("basemodel").gameObject;
            var findHair = transform.FindRecursive("hair");
            hairModel = findHair ? findHair.gameObject : null;

            mainAnimator.Rebind();
        }

        protected void RegenerateSkin()
        {
            if(baseModel == null)
                return;
            var skin = baseModel.GetComponentInChildren<SkinnedMeshRenderer>();
            foreach(var mat in skin.materials)
                mat.SetColor("_BaseColor", appearence.skinColor);
        }

        protected void RegenerateHairColour()
        {
            if(hairModel == null)
                return;
            hairModel.GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BaseColor", appearence.hairColor);
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
            var armature = newObject.transform.Find("Armature");
            if(armature)
                armatures.Add(armature.transform);

            var modelmeshrenderer = newObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if(!modelmeshrenderer)
                return newObject;
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
            /*
            foreach(var a in armatures)
                a.localScale = Vector3.one;
            if(headBones.Count > 0)
                foreach(var head in headBones)
                    head.localScale = new Vector3(headScale, headScale, headScale);

            if(lowerSpineBones.Count > 0)
                foreach(var spine in lowerSpineBones)
                    spine.localScale = new Vector3(lowerSpineScale, lowerSpineScale, lowerSpineScale);
            */
        }

        private IEnumerator JumpIntoBed(GameObject bedObject)
        {
            // Find bed anim markers
            var bedAnimMarker = bedObject.transform.FindRecursive("BedAnimStart").transform;

            // TODO: Pathfind and walk to the position
            transform.position = new Vector3(bedAnimMarker.position.x, transform.position.y, bedAnimMarker.position.z);
            transform.rotation = bedAnimMarker.rotation;

            // Disable collisions and physics so we can safely intersect with the bed
            DisableCollision();
            DisableRigidbody();

            yield return new WaitForSeconds(.5f);

            // Start bed anim and wait for it to finish
            mainAnimator.SetBool("DoBed", true);
            while(mainAnimator.GetBool("DoBed"))
                yield return new WaitForFixedUpdate();

            // Do a snore
            sleepingEffect = controller.effectsManager.CreateEffect(EffectsType.LOOP_SLEEPING, boneTransforms["head"].position);
        }

        private IEnumerator JumpOutOfBed(GameObject bedObject)
        {
            // Place player in bed
            var bedAnimMarker = bedObject.transform.FindRecursive("BedAnimStart").transform;
            transform.position = new Vector3(bedAnimMarker.position.x, transform.position.y, bedAnimMarker.position.z);
            transform.rotation = bedAnimMarker.rotation;

            // Disable collisions and physics so we can safely intersect with the bed
            DisableCollision();
            DisableRigidbody();

            // Kill sleeping efffect
            if(sleepingEffect != null)
            {
                sleepingEffect.MultiplySpeed(3f);
                controller.effectsManager.RemoveEffect(sleepingEffect);
            }

            // Wait a bit
            yield return new WaitForSeconds(2f);

            // Start exit anim and wait for it to finish
            mainAnimator.SetBool("Sleeping", false);
            mainAnimator.SetBool("DoBed", true);
            while(mainAnimator.GetBool("DoBed"))
                yield return new WaitForFixedUpdate();

            // Put collisions and physics back on
            transform.forward = -transform.forward;

            EnableCollision();
            EnableRigidbody();
        }

        // Animation event: Nom some
        public void AnimatorNom()
        {
            if(itemCurrentlyHolding != null)
                itemCurrentlyHolding.transform.localScale -= new Vector3(.35f, .35f, .35f);
        }

        // Animation event: EatItem
        public void AnimatorEatItemDone()
        {
            mainAnimator.SetBool("DoEat", false);
            currentAction = 0;
            if(itemCurrentlyHolding != null)
                Destroy(itemCurrentlyHolding.gameObject);
            itemCurrentlyHolding = null;
        }

        // Animation event: Close eyes
        public void AnimatorCloseEyes()
        {
            face.SetFaceState(CharacterFace.FaceState.EYES_CLOSED);
        }

        // Animation event: Open eyes
        public void AnimatorOpenEyes()
        {
            face.SetFaceState(CharacterFace.FaceState.NORMAL);
        }

        // Animation event: YawnDone
        public void AnimatorYawnDone()
        {
            face.SetFaceState(CharacterFace.FaceState.NORMAL);
            mainAnimator.SetBool("DoYawn", false);
            currentAction = 0;
            if(this == controller.world.player)
                controller.itemHotbar.UpdateItemInHand();
        }

        // Animation event: PassOutMid
        public void AnimatorPassOutMid()
        {
            if(this == controller.world.player)
                controller.PlayerDropItemInHand();
            else
                DropHoldingItem();
        }

        // Animation event: PassOutDone
        public void AnimatorPassOutDone()
        {
            currentAction = 0;
            passedOut = true;
        }

        // Animation event: LeftFootStep
        public void AnimatorLeftFootStep()
        {
            controller.effectsManager.CreateEffect(EffectsType.ONESHOT_STEPDUST, boneTransforms["leg_foot_L"].position);
        }

        // Animation event: RightFootStep
        public void AnimatorRightFootStep()
        {
            controller.effectsManager.CreateEffect(EffectsType.ONESHOT_STEPDUST, boneTransforms["leg_foot_R"].position);
        }

        // Animation event: BedStartDone
        public void AnimatorBedStartDone()
        {
            mainAnimator.SetBool("DoBed", false);
            mainAnimator.SetBool("Sleeping", true);
            currentAction = 0;
        }

        // Animation event: BedEndDone
        public void AnimatorBedEndDone()
        {
            mainAnimator.SetBool("DoBed", false);
            mainAnimator.SetBool("Sleeping", false);
            currentAction = 0;
        }
    }
}