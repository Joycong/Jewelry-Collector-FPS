using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform target; // ���� ��ǥ (�÷��̾�)
    [SerializeField]
    private GameObject enemySpawnPointPrefab; // ���� �����ϱ� �� ���� ���� ��ġ�� �˷��ִ� ������
    [SerializeField]
    private GameObject enemyPrefab; // �����Ǵ� �� ������
    [SerializeField]
    private float enemySpawnTime = 1; // �� ���� �ֱ�
    [SerializeField]
    private float enemySpawnLatency = 1; // Ÿ�� ���� �� ���� �����ϱ���� ��� �ð�

    private MemoryPool spawnPointMemoryPool; // �� ���� ��ġ�� �˷��ִ� ������Ʈ ����, Ȱ��/��Ȱ�� ����
    private MemoryPool enemyMemoryPool; // �� ����, Ȱ��/��Ȱ�� ����

    private int numberOfEnemiesSpawnedAtOnce = 1; // ���ÿ� �����Ǵ� ���� ����
    private Vector2Int mapSize = new Vector2Int(0, 0); // �� ũ��

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }

   

    private IEnumerator SpawnTile() // �� ���� ������ ��ġ�� �� ������ �˸��� ���� ��� ����, ���ʿ��� 1�� ���� �ð��� �帧�� ���� ���ÿ� �����Ǵ� �� ����
    {
        int currentNumber = 0;
        int maximumNumber = 50;
        // ���� ���� �ε����� ��������
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // ���� ���� ���� �ٸ��� ����.
        if (currentSceneIndex == 1)
        {
            mapSize = new Vector2Int(66, 30); // �� ũ��
        }
        else if (currentSceneIndex == 2)
        {
            mapSize = new Vector2Int(129, 119); // �� ũ��
        }
        else if (currentSceneIndex == 3)
        {
            mapSize = new Vector2Int(102, 84); // �� ũ��
        }
        
        while (true)
        {
            // ���ÿ� numberOfEnemiesSpawnedAtOnce ���ڸ�ŭ ���� �����ǵ��� �ݺ��� ���
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem(); // ��� ������Ʈ ����

                item.transform.position = new Vector3(Random.Range(-mapSize.x*0.49f, mapSize.x*0.49f), 1,
                                                      Random.Range(-mapSize.y*0.49f, mapSize.y*0.49f)); // �� ���� ������ ��ġ�� ����
                StartCoroutine("SpawnEnemy", item); // ���� �ð� �� ��տ��� �� ����
            }

            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++; // ���ÿ� �����Ǵ� �� ���� ����
            }

            yield return new WaitForSeconds(enemySpawnTime); // �� ������ enemySpawnTime���� �ݺ��Ͽ� ����
        }
    
    }

    private IEnumerator SpawnEnemy(GameObject point)
    { 
        yield return new WaitForSeconds(enemySpawnLatency); // enemySpawnLatency�ð����� ���

        // �� ������Ʈ�� �����ϰ�, ���� ��ġ�� point�� ��ġ�� ����
        GameObject item = enemyMemoryPool.ActivatePoolItem(); // �� ����
        item.transform.position = point.transform.position; // ��ġ�� ����� ��ġ�� �����ϰ� ����

        item.GetComponent<EnemyFSM>().Setup(target, this);

        // Ÿ�� ������Ʈ�� ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point); // ��� ������Ʈ ��Ȱ��ȭ
    }
    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }
}
