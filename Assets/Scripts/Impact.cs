using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Setup(MemoryPool pool) // Ÿ�� ����Ʈ�� �޸�Ǯ�� ���� 
    {
        memoryPool = pool;
    }

    private void Update()
    {
        // ��ƼŬ�� ������� �ƴϸ� ��Ȱ��ȭ
        if (particle.isPlaying == false) 
        {
            memoryPool.DeactivatePoolItem(gameObject); // ��ƼŬ ������Ʈ ��Ȱ��ȭ
        }
    }
}
