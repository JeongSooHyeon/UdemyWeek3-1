using UnityEngine;
using System.Collections;

// 플레이어 클래스(AI가 조작함)
public class Player_AI : Player_Base
{
	// 검사 방향
	private enum CheckDir
	{
		//← ↑ → ↓ 키 순서
		Left        // 왼쪽
		, Up        // 위
		, Right     // 오른쪽
		, Down      // 아래
		, EnumMax   // 키 종류 개수
	}
	// 검사 정보
	private enum CheckData
	{
		X           // x축
		, Y         // Y축
		, EnumMax   // 축 개수
	}

	// 검사 방향
	private static readonly int[][] CHECK_DIR_LIST = new int[(int)CheckDir.EnumMax][] {	
	//										 X		 Y
	 new int[ (int)CheckData.EnumMax] {     -1,      0      }
	,new int[ (int)CheckData.EnumMax] {      0,      1      }
	,new int[ (int)CheckData.EnumMax] {      1,      0      }
	,new int[ (int)CheckData.EnumMax] {      0,     -1      }
	};

	private static readonly int AI_PRIO_MIN = 99;    // AI 우선순위 중 가장 낮은 값

	private static readonly float AI_INTERVAL_MIN = 0.5f;	// AI 사고 간격 가장 짧은 값
	private static readonly float AI_INTERVAL_MAX = 0.8f;	// AI 사고 간격 가장 긴 값

	private static readonly float AI_IGNORE_DISTANCE = 2.0f;    // 이 이상 플레이어에게 다가가지 않는다

	private static readonly float SHOOT_INTERVAL = 1.0f;	// 사격 간격

	private float m_aiInterval = 0f;	// AI 사고를 갱신할 때까지의 시간
	private float m_shootInterval = 0f;	// 사격 간격

	private PlayerInput m_pressInput = PlayerInput.Move_Left;	// AI의 입력 종류

	private int playerIdx = -1;
	private int aiIdx = -1;

	// 입력 처리 검사
	protected override void GetInput()
	{
		// 사용자가 조종하는 플레이어 오브젝트를 얻는다
		GameObject mainObject = Player_Key.m_mainPlayer;
		if (null != mainObject)
		{
			// 플레이어가 죽었으면 가만히 있도록 
			if (mainObject.GetComponent<Player_Key>().isDead)
				return;
		}
		else
			// 플레이어가 없으면 사고를 중단
			return;

		// AI의 사고를 갱신할 때까지의 시간
		m_aiInterval -= Time.deltaTime;

		// 사격하는 사고를 갱신할 때까지의 시간
		m_shootInterval -= Time.deltaTime;

		// 플레이어와 자신의 거리를 계산한다.
		Vector3 aiSubPosition = (transform.position - mainObject.transform.position);
		aiSubPosition.y = 0f;

		// 거리가 생기면 움직인다, 움직임 다시 계산?
		if (aiSubPosition.magnitude > AI_IGNORE_DISTANCE)
		{
			// 일정 시간마다 AI를 갱신한다
			if (m_aiInterval < 0f)
			{
				// 다음 사고까지 기다릴 시간. 무작위로 결정
				m_aiInterval = Random.Range(AI_INTERVAL_MIN, AI_INTERVAL_MAX);	// 랜덤하게 시간결정

				// 현재 AI 위치에서 상하좌우의 우선순위를 얻는다
				int[] prioTable = GetMovePrioTable();

				// 가장 우선순위가 높은 장소의 숫자를 가져온다
				int highest = AI_PRIO_MIN;
				
				for (int i = 0; i < (int)CheckDir.EnumMax; i++)
				{
					// 값이 작을수록 웃너순위가 높다
					if (highest > prioTable[i])
					{
						// 우선순위 갱신
						highest = prioTable[i];
					}
				}

				// 어느 방향의 우선순위가 높은지 경정한다
				PlayerInput pressInput = PlayerInput.Move_Left;
				if (highest == prioTable[(int)CheckDir.Left])
				{
					// 왼쪽으로 이동
					pressInput = PlayerInput.Move_Left;
				}
				else
				if (highest == prioTable[(int)CheckDir.Right])
				{
					// 오른쪽으로 이동
					pressInput = PlayerInput.Move_Right;
				}
				else
				if (highest == prioTable[(int)CheckDir.Up])
				{
					// 위로 이동
					pressInput = PlayerInput.Move_Up;
				}
				else
				if (highest == prioTable[(int)CheckDir.Down])
				{
					// 아래로 이동
					pressInput = PlayerInput.Move_Down;
				}
				m_pressInput = pressInput;
			}

			// 입력
			m_playerInput[(int)m_pressInput] = true;
		}

		// 사격 사고를 실시할지 판단한다
		if (m_shootInterval < 0f)
		{
			// x 혹은 z 방향의 거리가 가까울 경우 직선상에 있다고 판단하면 사격한다
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward * 10.0f, out hit))
			{
				if (hit.transform.gameObject.tag == "Player")   // 앞에 플레이어가 있고
				{
					if ((Mathf.Abs(aiSubPosition.x) < 1f) || (Mathf.Abs(aiSubPosition.z) < 1f))
					{
						// 사격
							m_playerInput[(int)PlayerInput.Shoot] = true;

						// 다음 사격은 이 시간만큼 경과할 때까지 기다린다(연사 억제 기능)
						m_shootInterval = SHOOT_INTERVAL;
					}
				}
			}
		}
	}

/*	private bool canShot()
    {
		if(Mathf.Abs(aiIdx-playerIdx) > Field.FIELD_GRID_X - 3)
        {
			if(aiIdx > 0)
            {
				while (aiIdx > playerIdx) { 
				if(Field.ObjectKind.Block == (Field.ObjectKind)Field.GRID_OBJECT_DATA[aiIdx - Field.FIELD_GRID_X])
                    {
						return false;
                    }
				}
            }
            else
            {
				while (aiIdx < playerIdx)
				{
					if (Field.ObjectKind.Block == (Field.ObjectKind)Field.GRID_OBJECT_DATA[playerIdx - Field.FIELD_GRID_X])
					{
						return false;
					}
				}
			}
        }
        else
        {

        }
		return true;
    }*/
	// 위치에서 그리드로 변환 그리드X
	private int GetGridX(float posX)
	{
		// 그리드 범위 내에 들어가도록 Mathf.Clamp에서 제한을 가하다
		return Mathf.Clamp((int)((posX) / Field.BLOCK_SCALE), 0, (Field.FIELD_GRID_X - 1));
	}
	// 위치에서 그리드로 변환 그리드Y
	private int GetGridY(float posZ)
	{
		// Unity에서는 XZ 평면이 지평선
		return Mathf.Clamp((int)((posZ) / Field.BLOCK_SCALE), 0, (Field.FIELD_GRID_Y - 1));
	}

	// AI가 이동할 때 우선순위 산출
	private int[] GetMovePrioTable()
	{
		int i, j;

		// 자기자신(AI)의 위치
		Vector3 aiPosition = transform.position;
		// 그리드로 변환
		int aiX = GetGridX(aiPosition.x);
		int aiY = GetGridY(aiPosition.z);
		aiIdx = aiX + (aiY * Field.FIELD_GRID_X);

		// 사용자가 움직이고 있는 플레이어의 객체를 가져옵니다.
		GameObject mainObject = Player_Key.m_mainPlayer;
		// 공격 목표 위치 가져오기
		Vector3 playerPosition = mainObject.transform.position;
		// 그리드로 변환
		int playerX = GetGridX(playerPosition.x);
		int playerY = GetGridY(playerPosition.z);
		int playerGrid = playerX + (playerY * Field.FIELD_GRID_X);
		playerIdx = playerGrid;

		// 그리드의 각 위치의 우선 순위를 저장하는 배열
		int[] calcGrid = new int[(Field.FIELD_GRID_X * Field.FIELD_GRID_Y)];
		// 초기화
		for (i = 0; i < (Field.FIELD_GRID_X * Field.FIELD_GRID_Y); i++)
		{
			// 우선도를 최저로 하다
			calcGrid[i] = AI_PRIO_MIN;
		}

		// 플레이어가 현재 있는 장소에 우선 1을 넣다
		calcGrid[playerGrid] = 1;

		// 체크하는 우선순위는 우선 1부터
		int checkPrio = 1;
		// 체크용 변수
		int checkX;
		int checkY;
		int tempX;
		int tempY;
		int tempGrid;

		// 뭔가 체크하면 true
		bool update;
		do
		{
			// 초기화
			update = false;

			// 체크 개시
			for (i = 0; i < (Field.FIELD_GRID_X * Field.FIELD_GRID_Y); i++)
			{
				// 체크하는 우선순위가 아니라면 무시
				if (checkPrio != calcGrid[i])
				{
					continue;
				}

				// 이 그리드가 체크하는 우선순위 위치
				checkX = (i % Field.FIELD_GRID_X);
				checkY = (i / Field.FIELD_GRID_X);

				// 거기서부터 상하좌우의 장소를 체크
				for (j = 0; j < (int)CheckDir.EnumMax; j++)
				{
					// 조사 장소 옆
					tempX = (checkX + CHECK_DIR_LIST[j][(int)CheckData.X]);
					tempY = (checkY + CHECK_DIR_LIST[j][(int)CheckData.Y]);
					// 그리드 밖?
					if ((tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y))
					{
						// 장외라서 무시
						continue;
					}
					// 이곳을 조사하다
					tempGrid = (tempX + (tempY * Field.FIELD_GRID_X));

					// 옆이 벽인지 체크
					if (Field.ObjectKind.Block == (Field.ObjectKind)Field.GRID_OBJECT_DATA[tempGrid])
					{
						// 벽이면 무시
						continue;
					}

					// 이 장소의 우선순위 숫자가 현재 체크하고 있는 우선순위보다 크면 갱신
					if (calcGrid[tempGrid] > (checkPrio + 1))
					{
						// 값 갱신
						calcGrid[tempGrid] = (checkPrio + 1);   // 이 숫자가 다음에 체크할 때의 우선 순위
						update = true;                          // 플래그를 세우다
					}
				}
			}

			// 체크하는 우선도를 +1하다
			checkPrio++;

			//뭔가 갱신이 있으면 다시 돌린다
		} while (update);

		// AI 주변 우선 순위표
		int[] prioTable = new int[(int)CheckDir.EnumMax];

		// 우선도 테이블이 생성되면 AI 주변 우선도 취득
		for (i = 0; i < (int)CheckDir.EnumMax; i++)
		{
			// 조사 장소 옆
			tempX = (aiX + CHECK_DIR_LIST[i][(int)CheckData.X]);
			tempY = (aiY + CHECK_DIR_LIST[i][(int)CheckData.Y]);
			// 그리드 밖?
			if ((tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y))
			{
				// 장외이므로 우선도를 최저로 하다
				prioTable[i] = AI_PRIO_MIN;
				continue;
			}

			// 이 장소의 우선도를 대입
			tempGrid = (tempX + (tempY * Field.FIELD_GRID_X));
			prioTable[i] = calcGrid[tempGrid];
		}


		// 우선순위 테이블을 디버깅 출력
		{
			// 디버깅용 문자열
			string temp = "";

			// 우선도 테이블이 생성되면 AI 주변 우선도 취득
			temp += "PRIO TABLE\n";
			for (tempY = 0; tempY < Field.FIELD_GRID_Y; tempY++)
			{
				for (tempX = 0; tempX < Field.FIELD_GRID_X; tempX++)
				{
					// Y축은 상하 반대로 출력되어 버리기 때문에 거꾸로 한다
					temp += "\t\t" + calcGrid[tempX + ((Field.FIELD_GRID_Y - 1 - tempY) * Field.FIELD_GRID_X)] + "";

					// 자기 위치
					if ((aiX == tempX) && (aiY == (Field.FIELD_GRID_Y - 1 - tempY)))
					{
						temp += "*";
					}
				}
				temp += "\n";
			}
			temp += "\n";

			// 이동 방향별 우선순위 정보
			temp += "RESULT\n";
			for (i = 0; i < (int)CheckDir.EnumMax; i++)
			{
				// 이 장소의 우선도를 대입
				temp += "\t" + prioTable[i] + "\t" + (CheckDir)i + "\n";
			}

			// 출력
			//Debug.Log("" + temp);
		}


		// 4방향 우선순위 정보를 반환하다
		return prioTable;
	}
}