using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
	private float timer = 15.0f;
	private Text timerText;
	public Text aiScore;
	public Text playerScore;

	private Player_Base AIPlayer;
	private Player_Base Player;

	public GameObject finish;
	private string victory;

	private float waitTime = 2.0f;

	private int aiCnt;
	private int playerCnt;

	private bool isFinish = false;

	private void Start()
	{
		timerText = GetComponent<Text>();
		AIPlayer = GameObject.FindGameObjectWithTag("AI").GetComponent<Player_Base>();
		Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Base>();
	}

	private void Update()
	{
		if (timer > 0)
			timer -= Time.deltaTime;

		aiCnt = AIPlayer.attackedCnt;
		playerCnt = Player.attackedCnt;

		// Timer
		setTimer();

		timerText.text = timer.ToString("F0");
		aiScore.text = "AI : " + aiCnt.ToString();
		playerScore.text = "ÈÞ¸Õ : " + playerCnt.ToString();
	}

	void pauseGame()
	{
		waitTime -= Time.deltaTime;
		if ((int)waitTime <= 0)
		{
			Time.timeScale = 0;
		}
	}

	void setTimer()
	{
		if ((int)timer <= 0 && !isFinish)
		{
			Debug.Log("AI " + aiCnt);
			Debug.Log("Player " + playerCnt);

			if (aiCnt > Player.attackedCnt)  // »ç¶÷ÀÌ ÀÌ±è
			{
				victory = "ÈÞ¸Õ ½Â!";
				AIPlayer.setFinish();
			}
			else if (aiCnt < playerCnt) // AI ÀÌ±è
			{
				victory = "AI ½Â!";
				Player.setFinish();
			}
			else // ºñ±è
			{
				victory = "ºñ°å´Ù !";
			}

			isFinish = true;
			finish.SetActive(true);
			finish.GetComponent<Text>().text = victory;
		}
		if(isFinish)
			pauseGame();
	}
}
