using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform target; // 적의 목표 (플레이어)
    [SerializeField]
    private GameObject enemySpawnPointPrefab; // 적이 등장하기 전 적의 등장 위치를 알려주는 프리팹
    [SerializeField]
    private GameObject enemyPrefab; // 생성되는 적 프리팹
    [SerializeField]
    private float enemySpawnTime = 1; // 적 생성 주기
    [SerializeField]
    private float enemySpawnLatency = 1; // 타일 생성 후 적이 등장하기까지 대기 시간

    private MemoryPool spawnPointMemoryPool; // 적 등장 위치를 알려주는 오브젝트 생성, 활성/비활성 관리
    private MemoryPool enemyMemoryPool; // 적 생성, 활성/비활성 관리

    private int numberOfEnemiesSpawnedAtOnce = 1; // 동시에 생성되는 적의 숫자
    private Vector2Int mapSize = new Vector2Int(0, 0); // 맵 크기

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }

   

    private IEnumerator SpawnTile() // 맵 내부 임의의 위치에 적 등장을 알리는 빨간 기둥 생성, 최초에는 1명 생성 시간이 흐름에 따라 동시에 생성되는 수 증가
    {
        int currentNumber = 0;
        int maximumNumber = 50;
        // 현재 씬의 인덱스를 가져오기
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // 현재 씬에 따라 다르게 동작.
        if (currentSceneIndex == 1)
        {
            mapSize = new Vector2Int(66, 30); // 맵 크기
        }
        else if (currentSceneIndex == 2)
        {
            mapSize = new Vector2Int(129, 119); // 맵 크기
        }
        else if (currentSceneIndex == 3)
        {
            mapSize = new Vector2Int(102, 84); // 맵 크기
        }
        
        while (true)
        {
            // 동시에 numberOfEnemiesSpawnedAtOnce 숫자만큼 적이 생성되도록 반복문 사용
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem(); // 기둥 오브젝트 생성

                item.transform.position = new Vector3(Random.Range(-mapSize.x*0.49f, mapSize.x*0.49f), 1,
                                                      Random.Range(-mapSize.y*0.49f, mapSize.y*0.49f)); // 맵 내부 임의의 위치에 생성
                StartCoroutine("SpawnEnemy", item); // 일정 시간 후 기둥에서 적 생성
            }

            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++; // 동시에 스폰되는 적 숫자 증가
            }

            yield return new WaitForSeconds(enemySpawnTime); // 이 과정을 enemySpawnTime마다 반복하여 수행
        }
    
    }

    private IEnumerator SpawnEnemy(GameObject point)
    { 
        yield return new WaitForSeconds(enemySpawnLatency); // enemySpawnLatency시간동안 대기

        // 적 오브젝트를 생성하고, 적의 위치를 point의 위치로 설정
        GameObject item = enemyMemoryPool.ActivatePoolItem(); // 적 생성
        item.transform.position = point.transform.position; // 위치는 기둥의 위치와 동일하게 설정

        item.GetComponent<EnemyFSM>().Setup(target, this);

        // 타일 오브젝트를 비활성화
        spawnPointMemoryPool.DeactivatePoolItem(point); // 기둥 오브젝트 비활성화
    }
    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }
}
