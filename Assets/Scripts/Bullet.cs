using UnityEngine;
using System.Collections;


// 총알 클래스

public class Bullet : HitObject
{
	private static readonly float bulletMoveSpeed = 10.0f;  // 1초간 총알이 진행되는 거리  
	public GameObject hitEffectPrefab = null;   // 히트이펙트의 조립식                     

	private void Update()
	{
		// 이동
		{
			// 1초간 이동량
			Vector3 vecAddPos = (Vector3.forward * bulletMoveSpeed);
			/*
					Vector3. forward는 new Vector3 (0f, 0f, 1f) 와 같습니다

					그 밖에도 여러가지가 있으니 ↓페이지를 참조해보세요
					http://docs.unity3d.com/ScriptReference/Vector3.html

					그리고 Vector3에 transform.rotation을 곱하면 그 방향으로 구부려줍니다.
					이때 Vector3는 Z+ 방향을 정면으로 생각합니다.
			 */

			// 이동량,회전량에는 Time.deltaTime을 곱하여 실행환경(프레임수차이)에 따른 차이가 나지 않도록 합니다.
			transform.position += ((transform.rotation * vecAddPos) * Time.deltaTime);
		}
	}

	// 자신의 GameObject에 Collider
	private void OnTriggerEnter(Collider hitCollider)
	{

		// 히트(닿았을 때) 검사
		if (false == IsHitOK(hitCollider.gameObject))
		{
			// 히트가 없으면 그냥 종료
			return;
		}

		// 히트 효과 프리팹이 있으면
		if (null != hitEffectPrefab)
		{
			// 현재 위치에 히트 효과 생성
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
		}

		// 해당 게임 오브젝트를 Hierarchy에서 제거
		Destroy(gameObject);
	}
}