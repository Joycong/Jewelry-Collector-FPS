using UnityEngine;

public class CasingMemoryPool : MonoBehaviour // �޸� Ǯ�� �̿��Ͽ� ź�� ������ ����
{
    [SerializeField]
    private GameObject casingPrefab; // ź�� ������Ʈ
    private MemoryPool memoryPool; // ź�� �޸�Ǯ

    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefab); // �޸�Ǯ ������ �޸� �Ҵ� (casingPrefab�� ���� �޸�Ǯ���� ����/�����ϴ� ������Ʈ)
    }

    public void SpawnCasing(Vector3 position, Vector3 direction) // ��ġ�� ������ �Ű������� �޾ƿ�
    {// ���� �޸�Ǯ�� ����Ǿ� �ִ� ������Ʈ �� ��Ȱ��ȭ ������ ������Ʈ�� �ϳ� �����Ͽ� Ȱ��ȭ�ϰ� ��ġ�� ȸ������ ������ �� CasingŬ������ Setup�޼ҵ� ȣ��
        GameObject item = memoryPool.ActivatePoolItem(); 
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(memoryPool, direction);
    }
}
