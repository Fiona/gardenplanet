using System;
using UnityEngine;

public class Player: MonoBehaviour
{

    private Rigidbody rigidBody;
    private Vector3 walkDir = new Vector3();

    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    public void Update()
    {
        //Vector3 dir = pushDir + (Vector3.down * 2.0f);
        //Vector3 newPos = transform.position + dir * Time.deltaTime;
        //float deltaDistance = Vector3.Distance(transform.position, newPos);

        //RaycastHit hit;
        //var ray = new Ray(transform.position, dir);
        //if(Physics.Raycast(ray, out hit, deltaDistance, 1 << Consts.COLLISION_LAYER_TILES) == false)
        //rigidBody.MovePosition(newPos);
        //rigidBody.velocity += velocityDir;
        //rigidBody.velocity *= .9f;


        if(rigidBody.velocity.magnitude < 1.0f)
            rigidBody.AddForce(
                Vector3.ClampMagnitude(walkDir * Consts.PLAYER_SPEED, Consts.PLAYER_SPEED) * Time.deltaTime,
                ForceMode.Impulse
                );
        walkDir = new Vector3();
        //rigidBody.AddForce(Physics.gravity);

        //rigidBody.velocity = Vector3.ClampMagnitude(walkDir * Consts.PLAYER_SPEED, Consts.PLAYER_SPEED);


        //velocityDir = Vector3.ClampMagnitude(velocityDir, amount);
    }

    public void LateUpdate()
    {
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

        //pushDir = (vecDir * amount);
        //Vector3 pos = (vecDir * amount) + (Vector3.down * 2.0f);
        //rigidBody.MovePosition(transform.position + pos * Time.deltaTime);
        //rigidBody.AddForce(vecDir * amount);
    }

}
