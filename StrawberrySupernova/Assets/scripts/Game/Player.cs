using System;
using UnityEngine;

public class Player: MonoBehaviour
{

    private Rigidbody rigidBody;
    private Vector3 walkDir = new Vector3();
    private bool isJumping = false;
    private bool attemptJump = false;
    private Vector3 desiredRotation;

    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    public void Update()
    {

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
        transform.rotation = Quaternion.LookRotation(newDir);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

        // Deal with jumping
        isJumping = (Mathf.Abs(rigidBody.velocity.y) > 0.01f);
        if(isJumping)
            attemptJump = false;
        else if(attemptJump)
        {
            rigidBody.AddForce(0, Consts.PLAYER_JUMP_FORCE * Time.deltaTime, 0, ForceMode.Impulse);
            attemptJump = false;
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
}
