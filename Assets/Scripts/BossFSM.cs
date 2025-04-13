using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8;       // �ν� ���� (�� ���� �ȿ� ������ "Pursuit" ���·� ����)
    [SerializeField]
    private float pursuitLimitRange = 10;           // ���� ���� (�� ���� �ٱ����� ������ "Wander" ���·� ����)

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;                // �߻�ü ������
    [SerializeField]
    private Transform projectileSpawnPoint;         // �߻�ü ���� ��ġ
    [SerializeField]
    private float attackRange = 5;              // ���� ���� (�� ���� �ȿ� ������ "Attack" ���·� ����)
    [SerializeField]
    private float attackRate = 1;                   // ���� �ӵ�

    private EnemyState enemyState = EnemyState.None;    // ���� �� �ൿ
    private float lastAttackTime = 0;               // ���� �ֱ� ���� ����

    private Status status;                          // �̵��ӵ� ���� ����
    private NavMeshAgent navMeshAgent;                  // �̵� ��� ���� NavMeshAgent
    private Transform target;                           // ���� ���� ��� (�÷��̾�)
    private EnemyMemoryPool enemyMemoryPool;                // �� �޸� Ǯ (�� ������Ʈ ��Ȱ��ȭ�� ���)




    public void TakeDamage(int damage) // ���� ���ݹ޾��� �� ȣ���ϴ� �޼ҵ�
    {
        bool isDie = status.DecreaseHP(damage);

        if (isDie == true) // ������
        {
            enemyMemoryPool.DeactivateEnemy(gameObject); // �� ������Ʈ ��Ȱ��ȭ
        }
    }
}

