using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movement;
    private float projectileDistance = 30;  // 발사체 최대 발사거리
    private int damage = 5;                 // 발사체 공격력

    public void Setup(Vector3 position) // 매개변수로 받아온 position 위치를 지나가는 방향으로 이동
    {
        movement = GetComponent<MovementTransform>();

        StartCoroutine("OnMove", position);
    }

    private IEnumerator OnMove(Vector3 targetPosition) // 이동방향 설정 및 이동 범위 초과 여부
    {
        Vector3 start = transform.position;

        movement.MoveTo((targetPosition - transform.position).normalized); // 이동방향 설정

        while (true)
        {
            if (Vector3.Distance(transform.position, start) >= projectileDistance) // 발사체가 생성된 start위치와 현재 위치의 거리가 projectileDistance보다 크거나 같으면,,
            {
                Destroy(gameObject); // 발사체 삭제

                yield break; // 코루틴 종료
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 발사체와 부딪힌 오브젝트의 태그가 "Player"이면
        {
            // Debug.Log("Player Hit"); // 로그 출력
            other.GetComponent<PlayerController>().TakeDamage(damage); // 데미지만큼 체력 감소

            Destroy(gameObject); // 발사체 삭제
        }
    }
}

