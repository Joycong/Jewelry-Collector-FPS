using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Setup(MemoryPool pool) // 타격 이펙트는 메모리풀로 관리 
    {
        memoryPool = pool;
    }

    private void Update()
    {
        // 파티클이 재생중이 아니면 비활성화
        if (particle.isPlaying == false) 
        {
            memoryPool.DeactivatePoolItem(gameObject); // 파티클 오브젝트 비활성화
        }
    }
}
