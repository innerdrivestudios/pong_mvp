using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/**
 *  Simple GameManager to tie all elements in our Pong game together.
 */
[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
	//references to all the required game objects
	[SerializeField] private Text gameInfoText;				//Blinking text at center of the screen
	[SerializeField] private Text player1ScoreText;
	[SerializeField] private Text player2ScoreText;

	[SerializeField] private Puck puck;
	[SerializeField] private Paddle player1;
	[SerializeField] private Paddle player2;

	//track some internal administration for the game
	private int whoWon = 0;
	private int p1Score = 0;
	private int p2Score = 0;
	private bool inPlay = false;

	private void Start()
	{
		enterWaitState();
	}

	private void enterWaitState()
	{
		setGameInfoText("Press space to start");
		AudioListener.pause = true;
		setScore(0, 0);
		inPlay = false;
		
		//enables Update method to be called 
		enabled = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) enterPlayState();
	}

	private void enterPlayState()
	{
		setGameInfoText(null);
		AudioListener.pause = false;
		setScore(0, 0);
		puck.Reset();
		inPlay = true;

		//disables Update method to be called
		enabled = false;
	}

	/**
	 * Sets a blinking text through a coroutine (or kills it if you pass null)
	 */
	private void setGameInfoText(string infoText)
	{
		gameInfoText.text = infoText;
		
		if (string.IsNullOrEmpty(infoText))
		{
			gameInfoText.enabled = false;
			StopAllCoroutines();
		}
		else
		{
			StartCoroutine(doGameInfoTextBlinking());
		}
	}

	/**
	 * Blinking text coroutine.
	 */
	private IEnumerator doGameInfoTextBlinking()
	{
		while (true)
		{
			gameInfoText.enabled = true;
			yield return new WaitForSeconds(1f);
			gameInfoText.enabled = false;
			yield return new WaitForSeconds(1f);
		}
	}

	/**
	 * Update the scores and check whether the game has ended.
	 * If not, reset the puck and the paddles.
	 */
	private void setScore(int p1NewScore, int p2NewScore)
	{
		p1Score = p1NewScore;
		p2Score = p2NewScore;

		player1ScoreText.text = "" + p1Score;
		player2ScoreText.text = "" + p2Score;

		if (p1Score > 9 || p2Score > 9)
		{
			whoWon = p1Score > p2Score ? 1 : 2;
			enterGameOverState();
		}
		else
		{
			puck.Reset();
			player1.Reset();
			player2.Reset();
		}
	}

	private void enterGameOverState()
	{
		setGameInfoText($"      Game over - Player {whoWon} won\n   Press space   to start over");
		AudioListener.pause = true;
		inPlay = false;
		enabled = true;
	}

	/** 
	 * Called by one of the goal gameobjects
	 */
	public void OnGoalHit (Obstacle.ObstacleId playerWhoScored)
	{
		if (!inPlay) return;

		Debug.Log($"{playerWhoScored} scored !");

		if (playerWhoScored == Obstacle.ObstacleId.Player1)
		{
			setScore(p1Score + 1, p2Score);
		} 
		else
		{
			setScore(p1Score, p2Score + 1);
		}
	}

	/** 
	 * Called by one of the boundary gameobjects, just here for debugging.
	 */
	public void OnBoundaryHit()
	{
		Debug.Log($"Boundary hit!");
	}

	/** 
	 * Called by one of the player gameobjects, just here for debugging.
	 */
	public void OnPlayerHit(Obstacle.ObstacleId playerThatHit)
	{
		Debug.Log($"{playerThatHit} hit the puck!");
	}
}
