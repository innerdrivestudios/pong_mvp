using UnityEngine;

/**
 * Implements the Player paddle.
 * 
 * Basically there are 3 ways to implement movement:
 * - through the transform
 * - through the physics engine (using a rigidbody)
 * - through animation
 * 
 * Animation is not interactive so the choice is between using a transform and a rigidbody.
 * Earlier version of this class used the transform and to be honest it worked fine, but it 
 * is not totally correct: anytime you are moving colliders and expect correct physical
 * interaction between objects you should be using rigidbodies (either dynamic or kinematic).
 * 
 * If you don't (i.e. simply use a transform) then objects might pass through each other etc.
 * The choice between a dynamic or kinematic Rigidbody depends on whether the body only influences
 * physics or is influenced by physics.
 * 
 * In this case we want full control of moving the paddles, which should influence objects they hit,
 * so we'll be using a kinematic rigidbody.
 */
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Paddle : MonoBehaviour
{
    [SerializeField] private string axisName;
    [SerializeField] private float speed;
    [SerializeField] private float range;

    private Rigidbody rb;
    private float input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Update()
    {
        //Using the input manager is easiest since it provides a lot of input setting out of the box,
        //such as frame independent easing, snapping etc. As to the division of work, stick to these rules:
        //- modify rigidbodies in FixedUpdate
        //- do your other stuff in Update (e.g. read input)
        //And yes that can get very clumsy/unwieldy, and no there is no way around that, and yes you are going to google for that anyway
        input = Input.GetAxis(axisName);
    }

	private void FixedUpdate()
	{
        //In this method we move the paddle through kinematic rigidbody methods.

        //Note that since the FixedUpdate rate is... well... fixed we normally don't multiply
        //with a deltaTime, but it is actually better to still do so, since it will require less
        //updates and tweaking in case we ever do want to change the fixed time step in the future.

        //In addition since we are in FixedUpdate we can use either Time.deltaTime or Time.fixedDeltaTime
        //for that, since their values are equal here.

        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Clamp(newPosition.y + speed * input * Time.deltaTime, -range, range);
        rb.MovePosition (newPosition);
    }

	private void OnCollisionEnter(Collision collision)
	{
        //Update the y velocity based of the impacting based on our own impact position
        //We only need to set the direction, the Puck will adjust and normalize the speed based on its setting

        //Get sign of velocity after colliding (-1 or 1)
        float newXSpeed = Mathf.Sign(collision.rigidbody.velocity.x); 
        //unit cube is size 1, this means the y impact point is -0.5 to 0.5, times two = -1 to 1
        float newYSpeed = transform.InverseTransformPoint(collision.GetContact(0).point).y * 2;

        //Now combine these to get a direction from -45 to 45 degrees from the paddle
        collision.rigidbody.velocity = new Vector3(newXSpeed, newYSpeed, 0);
	}

	public void Reset()
	{
        //Reset the y position back to zero.
        //Note that we are not expecting any sort of correct physical behaviour here
        //so we'll just use translate
        transform.Translate(Vector3.up * -transform.position.y);

	}
}
