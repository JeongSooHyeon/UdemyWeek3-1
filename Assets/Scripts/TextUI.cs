using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
	private float timer = 15.0f;	// ���� �÷��� �ð�
	private Text timerText;	// �ð� ǥ���� Text UI
	public Text aiScore;	// ai�� ���� Text
	public Text playerScore;	// �޸��� ���� Text

	private Player_Base AIPlayer;	
	private Player_Base Player;

	public GameObject finish;	// finish UI
	private string victory;	// �¸��� ����� �ؽ�Ʈ

	private float waitTime = 2.0f;	// ������ ������ �ִϸ��̼��� ������ ����� ������ ��ٸ��� �ð�

	private int aiCnt;	// ���� count
	private int playerCnt;	// ���� count

	private bool isFinish = false;	// ���� ����

	private void Start()
	{
		timerText = GetComponent<Text>();
		AIPlayer = GameObject.FindGameObjectWithTag("AI").GetComponent<Player_Base>();
		Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Base>();
	}

	private void Update()
	{
		// ���� ���� �ð� ����
		if (timer > 0)
			timer -= Time.deltaTime;


		// Timer
		setTimer();

		// UI ǥ��
		timerText.text = timer.ToString("F0");
		aiScore.text = "AI : " + aiCnt.ToString();
		playerScore.text = "�޸� : " + playerCnt.ToString();
	}

	// ���� ���߱�
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
			if (aiCnt > playerCnt)  // ����� �̱�
			{
				victory = "�޸� ��!";
				AIPlayer.setFinish();	// AI�� ��� �ִϸ��̼� ����
			}
			else if (aiCnt < playerCnt) // AI �̱�
			{
				victory = "AI ��!";
				Player.setFinish();	// ����� ��� �ִϸ��̼� ����
			}
			else // ���
			{
				victory = "���� !";
			}

			isFinish = true;
			finish.SetActive(true);
			finish.GetComponent<Text>().text = victory;
		}
		if(isFinish)
			pauseGame();
	}
}