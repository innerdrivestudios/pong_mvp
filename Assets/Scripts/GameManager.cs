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
	[SerializeField] private Text _gameInfoText;				//Blinking text at center of the screen
	[SerializeField] private Text _player1ScoreText;
	[SerializeField] private Text _player2ScoreText;

	[SerializeField] private Puck _puck;
	[SerializeField] private Paddle _player1;
	[SerializeField] private Paddle _player2;

	//track some internal administration for the game
	private int _whoWon = 0;
	private int _p1Score = 0;
	private int _p2Score = 0;
	private bool _inPlay = false;

	private void Start()
	{
		enterWaitState();
	}

	private void enterWaitState()
	{
		setGameInfoText("Press space to start");
		AudioListener.pause = true;
		setScore(0, 0);
		_inPlay = false;
		
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
		_puck.Reset();
		_inPlay = true;

		//disables Update method to be called
		enabled = false;
	}

	/**
	 * Sets a blinking text through a coroutine (or kills it if you pass null)
	 */
	private void setGameInfoText(string infoText)
	{
		_gameInfoText.text = infoText;
		
		if (string.IsNullOrEmpty(infoText))
		{
			_gameInfoText.enabled = false;
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
			_gameInfoText.enabled = true;
			yield return new WaitForSeconds(1f);
			_gameInfoText.enabled = false;
			yield return new WaitForSeconds(1f);
		}
	}

	/**
	 * Update the scores and check whether the game has ended.
	 * If not, reset the puck and the paddles.
	 */
	private void setScore(int p1NewScore, int p2NewScore)
	{
		_p1Score = p1NewScore;
		_p2Score = p2NewScore;

		_player1ScoreText.text = "" + _p1Score;
		_player2ScoreText.text = "" + _p2Score;

		if (_p1Score > 9 || _p2Score > 9)
		{
			_whoWon = _p1Score > _p2Score ? 1 : 2;
			enterGameOverState();
		}
		else
		{
			_puck.Reset();
			_player1.Reset();
			_player2.Reset();
		}
	}

	private void enterGameOverState()
	{
		setGameInfoText($"      Game over - Player {_whoWon} won\n   Press space   to start over");
		AudioListener.pause = true;
		_inPlay = false;
		enabled = true;
	}

	/** 
	 * Called by one of the goal gameobjects
	 */
	public void OnGoalHit (Obstacle.ObstacleId playerWhoScored)
	{
		if (!_inPlay) return;

		Debug.Log($"{playerWhoScored} scored !");

		if (playerWhoScored == Obstacle.ObstacleId.Player1)
		{
			setScore(_p1Score + 1, _p2Score);
		} 
		else
		{
			setScore(_p1Score, _p2Score + 1);
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
