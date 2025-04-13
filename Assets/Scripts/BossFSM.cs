using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8;       // 인식 범위 (이 범위 안에 들어오면 "Pursuit" 상태로 변경)
    [SerializeField]
    private float pursuitLimitRange = 10;           // 추적 범위 (이 범위 바깥으로 나가면 "Wander" 상태로 변경)

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;                // 발사체 프리팹
    [SerializeField]
    private Transform projectileSpawnPoint;         // 발사체 생성 위치
    [SerializeField]
    private float attackRange = 5;              // 공격 범위 (이 범위 안에 들어오면 "Attack" 상태로 변경)
    [SerializeField]
    private float attackRate = 1;                   // 공격 속도

    private EnemyState enemyState = EnemyState.None;    // 현재 적 행동
    private float lastAttackTime = 0;               // 공격 주기 계산용 변수

    private Status status;                          // 이동속도 등의 정보
    private NavMeshAgent navMeshAgent;                  // 이동 제어를 위한 NavMeshAgent
    private Transform target;                           // 적의 공격 대상 (플레이어)
    private EnemyMemoryPool enemyMemoryPool;                // 적 메모리 풀 (적 오브젝트 비활성화에 사용)




    public void TakeDamage(int damage) // 적이 공격받았을 때 호출하는 메소드
    {
        bool isDie = status.DecreaseHP(damage);

        if (isDie == true) // 죽으면
        {
            enemyMemoryPool.DeactivateEnemy(gameObject); // 적 오브젝트 비활성화
        }
    }
}

