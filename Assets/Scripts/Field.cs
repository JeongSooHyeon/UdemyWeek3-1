using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 필드 생성 프로그램
public class Field : MonoBehaviour
{
	public GameObject m_blockObject = null; // 블록 조립식                 

	public GameObject m_player1Object = null;   // 플레이어 1의 조립식                
	public GameObject m_player2Object = null;	

	public static readonly int FIELD_GRID_X = 9;	// 필드의 X그리드수
	public static readonly int FIELD_GRID_Y = 9;    // 필드의 Y그리드수

	public static readonly float BLOCK_SCALE = 2.0f;    // 블록 스케일(블록 1개 크기)
	public static readonly Vector3 BLOCK_OFFSET = new Vector3(1, 1, 1); // 블록 배치 오프셋

	// 배치할 물체 종류
	public enum ObjectKind
	{
		Empty       //	0	공백
		, Block     //	1	블록
		, Player1   //	2	플레이어1
		, Player2   //	3	플레이어2
	}

	public static readonly int[] GRID_OBJECT_DATA = new int[] {			// 배치 데이터
	//	0이 공란, 1이 블록
	1,  1,  1,  1,  1,  1,  1,  1,  1,
	1,  2,  0,  0,  0,  0,  0,  0,  1,
	1,  0,  1,  1,  1,  0,  1,  0,  1,
	1,  0,  0,  0,  0,  0,  0,  0,  1,
	1,  0,  1,  0,  1,  1,  1,  0,  1,
	1,  0,  1,  0,  1,  0,  0,  0,  1,
	1,  0,  1,  0,  0,  0,  1,  0,  1,
	1,  0,  0,  0,  1,  0,  0,  3,  1,
	1,  1,  1,  1,  1,  1,  1,  1,  1,

	// 배치할 때 위아래가 뒤집히므로 주의해야 한다.
};

	private GameObject m_blockParent = null;	// 생성한 블럭의 부모용 오브젝트

	private void Awake()
	{
		InitializeField();  //필드초기화
	}

	// 필드의 초기화
	// 배열 변수를 초기화하여 외벽과 기둥을 만들다
	private void InitializeField()
	{
		// 블록 부모 만들기
		m_blockParent = new GameObject();
		m_blockParent.name = "BlockParent";
		m_blockParent.transform.parent = transform;

		// 블록을 만들다
		GameObject originalObject;  // 생성하는 블럭의 원래 객체
		GameObject instanceObject;  // 블록을 일단 넣어두는 변수
		Vector3 position;   //블록 생성위치

		// 바깥틀과 안에 기둥을 세워가는
		int gridX;
		int gridY;
		for (gridX = 0; gridX < FIELD_GRID_X; gridX++)
		{
			for (gridY = 0; gridY < FIELD_GRID_Y; gridY++)
			{
				// 이 위치에는 무엇을 둘 것인가?
				switch ((ObjectKind)GRID_OBJECT_DATA[gridX + (gridY * FIELD_GRID_X)])
				{
					case ObjectKind.Block:
						// 벽
						originalObject = m_blockObject;
						break;
					case ObjectKind.Player1:
						// 플레이어
						originalObject = m_player1Object;
						break;
					case ObjectKind.Player2:
						// 플레이어
						originalObject = m_player2Object;
						break;
					default:
						// 그외는 빈칸
						originalObject = null;
						break;
				}

				// 빈칸이라면 여기까지
				if (null == originalObject)
				{
					continue;
				}

				// 블록 생성위치
				position = new Vector3(gridX * BLOCK_SCALE, 0, gridY * BLOCK_SCALE) + BLOCK_OFFSET; // Unity에서는 XZ 평면이 지평선         

				// 블록생성 복제할 대상 생성 위치 회전
				instanceObject = Instantiate(originalObject, position, originalObject.transform.rotation) as GameObject;
				// 이름 변경
				instanceObject.name = "" + originalObject.name + "(" + gridX + "," + gridY + ")";   // 그리드위치 적어놓기

				// 로컬저울(크기) 변경
				instanceObject.transform.localScale = (Vector3.one * BLOCK_SCALE);

				// 앞에서 언급한 부모 밑에 달다
				instanceObject.transform.parent = m_blockParent.transform;
			}
		}
	}
}
