using UnityEngine;

public class CasingMemoryPool : MonoBehaviour // 메모리 풀을 이용하여 탄피 생성을 제어
{
    [SerializeField]
    private GameObject casingPrefab; // 탄피 오브젝트
    private MemoryPool memoryPool; // 탄피 메모리풀

    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefab); // 메모리풀 변수에 메모리 할당 (casingPrefab이 현재 메모리풀에서 생성/관리하는 오브젝트)
    }

    public void SpawnCasing(Vector3 position, Vector3 direction) // 위치와 방향을 매개변수로 받아옴
    {// 현재 메모리풀에 저장되어 있는 오브젝트 중 비활성화 상태의 오브젝트를 하나 선택하여 활성화하고 위치와 회전값을 설정한 후 Casing클래스의 Setup메소드 호출
        GameObject item = memoryPool.ActivatePoolItem(); 
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(memoryPool, direction);
    }
}
