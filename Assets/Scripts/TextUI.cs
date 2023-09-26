using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
	private float timer = 15.0f;	// 게임 플레이 시간
	private Text timerText;	// 시간 표시할 Text UI
	public Text aiScore;	// ai의 점수 Text
	public Text playerScore;	// 휴먼의 점수 Text

	private Player_Base AIPlayer;	
	private Player_Base Player;

	public GameObject finish;	// finish UI
	private string victory;	// 승리한 사람의 텍스트

	private float waitTime = 2.0f;	// 게임이 끝나고 애니메이션이 끝까지 실행될 때까지 기다리는 시간

	private int aiCnt;	// 맞은 count
	private int playerCnt;	// 맞은 count

	private bool isFinish = false;	// 게임 끝남

	private void Start()
	{
		timerText = GetComponent<Text>();
		AIPlayer = GameObject.FindGameObjectWithTag("AI").GetComponent<Player_Base>();
		Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Base>();
	}

	private void Update()
	{
		// 게임 실행 시간 세기
		if (timer > 0)
			timer -= Time.deltaTime;


		// Timer
		setTimer();

		// UI 표시
		timerText.text = timer.ToString("F0");
		aiScore.text = "AI : " + aiCnt.ToString();
		playerScore.text = "휴먼 : " + playerCnt.ToString();
	}

	// 게임 멈추기
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
		aiCnt = AIPlayer.attackedCnt;
		playerCnt = Player.attackedCnt;

		if ((int)timer <= 0 && !isFinish)
		{
			if (aiCnt > playerCnt)  // 사람이 이김
			{
				victory = "휴먼 승!";
				AIPlayer.setFinish();	// AI의 사망 애니메이션 실행
			}
			else if (aiCnt < playerCnt) // AI 이김
			{
				victory = "AI 승!";
				Player.setFinish();	// 사람의 사망 애니메이션 실행
			}
			else // 비김
			{
				victory = "비겼다 !";
			}

			isFinish = true;
			finish.SetActive(true);
			finish.GetComponent<Text>().text = victory;
		}
		if(isFinish)
			pauseGame();
	}
}