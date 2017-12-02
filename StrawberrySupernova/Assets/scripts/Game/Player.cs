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
	    bool isJumping;
	    bool attemptJump;
	    Vector3 desiredRotation;
		GameController controller;
		[HideInInspector]
        public Inventory inventory;
		[HideInInspector]
		public int layer;

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
			FindObjectOfType<App>().events.NewHourEvent.AddListener(NextHour);
	    }

		public void Start()
		{
			controller = FindObjectOfType<GameController>();
		}

		public void NextHour(int hour)
		{
			if(hour == 2)
				StartCoroutine(PassOut());
		}

	    public void FixedUpdate()
	    {

		    // Detect layer
		    layer = (int)Math.Floor(transform.position.y * Consts.TILE_SIZE);

            if(controller.inputManager == null || !controller.inputManager.directInputEnabled)
				return;

	        // Handle walking
	        if(rigidBody.velocity.magnitude < 1.0f)
	            rigidBody.AddForce(
	                Vector3.ClampMagnitude(walkDir * Consts.PLAYER_SPEED, Consts.PLAYER_SPEED) * Time.deltaTime,
	                ForceMode.Impulse
	                );
	        walkDir = new Vector3();

	        // Do rotation towards mouse
	        float step = Consts.PLAYER_ROTATION_SPEED * Time.deltaTime;
			Vector3 newDir = Vector3.RotateTowards(transform.forward, desiredRotation, step, 0.0F);
			SetRotation(newDir);

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
				Debug.Log(Mathf.Abs(found));
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
	    public void WalkInDirection(Direction dir)
	    {
	        switch(dir)
	        {
	            case Direction.Up:
	                walkDir += Vector3.forward;
	                break;
	            case Direction.Down:
	                walkDir += Vector3.back;
	                break;
	            case Direction.Left:
	                walkDir += Vector3.left;
	                break;
	            case Direction.Right:
	                walkDir += Vector3.right;
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

		public IEnumerator PassOut()
		{
			controller.StartCutscene();
			yield return DialoguePopup.ShowDialoguePopup(
				"Oh no!",
				"You pass out from exhaustion..."
			);
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeOut(4f, new Color(1f,1f,1f)));
            controller.worldTimer.gameTime += new GameTime(hours: 4);
			yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeIn(4f, new Color(1f,1f,1f)));
			controller.EndCutscene();
		}

	}

}