using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, } // 현재 적의 행동을 나타내는 열거형 정의

public class EnemyFSM : MonoBehaviour
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

    private Animator animator;

    public GameObject winTextObject;


    public void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool) // 타겟 정보는 적을 생성하는 EnemyMemoryPool에서 매개변수로 넘겨받도록, Setup()로 변경
    {
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        // NavMeshAgent 컴포넌트에서 회전을 업데이트하지 않도록 설정
        navMeshAgent.updateRotation = false; // 오브젝트 회전 제어하지 못하도록
    }

    private void OnEnable()
    {
        // 적이 활성화될 때 적의 상태를 "대기"로 설정 (대기상태로 시작하도록)
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        // 적이 비활성화될 때 현재 재생중인 상태를 종료하고, 상태를 "None"으로 설정
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // 현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 없기 때문에 return
        if (enemyState == newState) return;

        // 이전에 재생중이던 상태 종료
        StopCoroutine(enemyState.ToString());
        // 현재 적의 상태를 newState로 설정
        enemyState = newState;
        // 새로운 상태 재생
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle() // ChangeState()에서 EnemyState열거형에 정의되어있는 상수.ToString()으로 코루틴 메소드를 재생하므로 열거형에 정의되어있는 상수와 동일한 이름의 코루틴 메소드를 정의해야함
    {
        // n초 후에 "배회" 상태로 변경하는 코루틴 실행
        StartCoroutine("AutoChangeFromIdleToWander");

        while (true)
        {
            // "대기" 상태일 때 하는 행동
            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // 상태를 "배회"로 변경
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander() // 배회 행동
    {
        float currentTime = 0;
        float maxTime = 10;

        // 이동 속도 설정
        navMeshAgent.speed = status.WalkSpeed;

        // 목표 위치 설정
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // 목표 위치로 회전
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            currentTime += Time.deltaTime;

            // 목표위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있으면
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                // 상태를 "대기"로 변경
                ChangeState(EnemyState.Idle);
            }

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;        // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0;       // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;        // 최소 각도
        int wanderJitterMax = 360;      // 최대 각도


        // 현재 씬의 인덱스를 가져오기
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기 (구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = new Vector3(67f, 0f, 32f);

        // 현재 씬에 따라 다르게 동작.
        if (currentSceneIndex == 1)
        { 
            rangeScale = new Vector3(67f, 0f, 32f);
        }
        else if (currentSceneIndex == 2)
        {
            rangeScale = new Vector3(129f, 0f, 119f);
        }
        else if (currentSceneIndex == 3)
        {
            rangeScale = new Vector3(102f, 0f, 84f);
        }

        // 자신의 위치를 중심으로 반지름(wanderRadius) 거리, 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Pursuit() // 타겟 추적
    {
        while (true)
        {
            // 이동 속도 설정 (배회할 때는 걷는 속도로 이동. 추적할 때는 뛰는 속도로 이동)
            navMeshAgent.speed = status.RunSpeed;

            // 목표위치를 현재 플레이어의 위치로 설정
            navMeshAgent.SetDestination(target.position);

            // 타겟 방향을 계속 주시하도록 함
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        // 공격할 때는 이동을 멈추도록 설정
        navMeshAgent.ResetPath();

        while (true)
        {
            // 타겟 방향 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > attackRate) // 공격 주기보다 Time.time - lastAttackTime가 클때마다 실행
            {
                // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
                lastAttackTime = Time.time;

                // 발사체 생성
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);

                animator.SetTrigger("fire");
            }

            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // 내 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // 바로 돌기
        transform.rotation = Quaternion.LookRotation(to - from);
        // 서서히 돌기
        //Quaternion rotation = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        // 플레이어(Target)와 적의 거리 계산 후 거리에 따라 행동 선택
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            animator.SetBool("Movement", false);
        }
        else if (distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit); // 인식 범위 (이 범위 안에 들어오면 "Pursuit" 상태로 변경)
            animator.SetBool("Movement", true);
        }
        else if (distance >= pursuitLimitRange)
        {
            ChangeState(EnemyState.Wander); // 추적 범위 (이 범위 바깥으로 나가면 "Wander" 상태로 변경)
            animator.SetBool("Movement", true);
        }
    }

    private void OnDrawGizmos()
    {
        // "배회" 상태일 때 이동할 경로 표시
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        // 목표 인식 범위를 씬에 시각적으로 표현
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 추적 범위를 씬에 시각적으로 표현
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        // 공격 범위를 씬에 시각적으로 표현
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(int damage) // 적이 공격받았을 때 호출하는 메소드
    {
        bool isDie = status.DecreaseHP(damage);

        if (isDie == true) // 죽으면
        {
            enemyMemoryPool.DeactivateEnemy(gameObject); // 적 오브젝트 비활성화

            if (gameObject.name == "Boss(Clone)")
            {
               SceneManager.LoadScene("MYFPSGAME sin#4");
            }
        }
    }
}

