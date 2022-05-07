using UnityEngine;

/**
 * This class implements a simple Puck by setting up a Rigidbody with a random
 * velocity which is reset every frame (to avoid slowing down or speeding up the puck during play).
 */
[DisallowMultipleComponent]
//Good practice to declare which components your component expects
[RequireComponent(typeof(Rigidbody))]
public class Puck : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 10;
    [SerializeField] private float _spawnRange = 14;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        randomizeVelocity();        
    }

	private void randomizeVelocity()
	{
        Vector3 velocity =
            new Vector3(
                Random.value > 0.5f ? -1 : 1,   //Either            -1 OR 1
                Random.Range(-1f, 1f),          //Somewhere between -1 .. 1
                0
            );

        velocity.Normalize();

        _rb.velocity = velocity * _maxSpeed;
    }

    private void FixedUpdate()
	{
        //make sure our puck speed never changes
        _rb.velocity = _rb.velocity.normalized * _maxSpeed;
	}

	public void Reset()
	{
        _rb.position = new Vector3(0, 0, 0) + Vector3.up * Random.Range(-_spawnRange, _spawnRange);
        randomizeVelocity();
	}

}
