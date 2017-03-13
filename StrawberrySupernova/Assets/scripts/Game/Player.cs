using System;
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
		bool inputEnabled;

	    public void Awake()
	    {
	        rigidBody = GetComponent<Rigidbody>();
	        rigidBody.freezeRotation = true;
			inputEnabled = true;
	    }

	    public void Update()
	    {

			if(!inputEnabled)
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

			// Interact with objects
			foreach(var obj in FindObjectsOfType<WorldObjectInteractable>())
			{
				if(Vector3.Distance(transform.position, obj.transform.position) < Consts.PLAYER_INTERACT_DISTANCE)
				{
					obj.Focus();
					if(Input.GetMouseButtonDown(0))
						obj.InteractWith();
				}
			}
			/*
			RaycastHit interactHit;
			var ray = new Ray(transform.position, transform.forward);
			if(Physics.Raycast(ray, out interactHit, Consts.PLAYER_INTERACT_DISTANCE, 1 << Consts.COLLISION_LAYER_WORLD_OBJECTS))
			{						
				var worldObj = controller.worldObjectManager.GetWorldObjectByGameObject(interactHit.transform.gameObject);
				controller.PlayerLookingAtWorldObject(worldObj);
			}
			else
				controller.PlayerNotLookingAtWorldObject();
*/
	    }

		/*
		 * Rotates the player towards an object and returns when it finishes
		 */
		public IEnumerator TurnTowardsWorldObject(ObjectWorldPosition worldObject)
		{
			inputEnabled = false;

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

				//var angle = Mathf.Atan2(turnToPos.y-myPos.y, turnToPos.x-myPos.x) * Mathf.Rad2Deg;

				float dot = Vector3.Dot((turnToPos-myPos).normalized, transform.forward);
				float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;  


				Debug.Log(angle);
				if(Mathf.Abs(angle) < 5f)
					break;
				yield return new WaitForFixedUpdate();
			}
			Debug.Log("done");
			inputEnabled = true;
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
				pos.x,
				(pos.layer * Consts.TILE_HEIGHT),
				pos.y			
			);
			var baseRotation = DirectionHelper.DirectionToDegrees(pos.dir);
			transform.localRotation = Quaternion.Euler(0, -baseRotation, 0);
		}

		public void SetRotation(Vector3 newDir)
		{
			transform.rotation = Quaternion.LookRotation(newDir);
			transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
		}

	}

}