using UnityEngine;
using UnityEngine.Events;

/**
 * Simple Obstacle which can trigger an UnityEvent on any object of your choice passing in the ID we've set.
 */
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class Obstacle : MonoBehaviour
{
	public enum ObstacleId { None = 0, Player1 = 1, Player2 = 2 };
	
	[Tooltip("Obstacles in this game can below to None, Player1 or Player 2")] 
	[SerializeField] private ObstacleId id;
	//what event shall we trigger when we get hit?
	[SerializeField] private UnityEvent<ObstacleId> OnObstacleHit;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		//Note the ? operator which says: only call the method after it, if the object before is not null.
		OnObstacleHit?.Invoke(id);
		audioSource?.Play();
	}
}
