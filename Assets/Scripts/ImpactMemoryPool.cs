using UnityEngine;

public enum ImpactType { Normal = 0, Obstacle, Enemy, InteractionObject, } // �� Ÿ�� ó���� ���� �������� ����
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab; // �ǰ� ����Ʈ
    private MemoryPool[] memoryPool; // �ǰ� ����Ʈ �޸�Ǯ

    private void Awake()
    {
        // �ǰ� ����Ʈ�� ���� �����̸� �������� memoryPool ����
        memoryPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
        if (hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("InteractionObject")) // Ÿ���� �±װ� InteractionObject�� ������Ʈ ���� ���� ���� �ٲ��
        {
            Color color = hit.transform.GetComponentInChildren<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
        }
    }

    public void SpawnImpact(Collider other, Transform knifeTransform)
    {
        // �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
        if (other.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
        else if (other.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
        else if (other.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
        else if (other.CompareTag("InteractionObject"))
        {
            Color color = other.transform.GetComponentInChildren<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation), color);
        }
    }

    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation, Color color = new Color()) 
    { 
        GameObject item = memoryPool[(int)type].ActivatePoolItem(); // �ǰ� ����Ʈ ����
        item.transform.position = position; // ��ġ����
        item.transform.rotation = rotation; // ȸ������
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

        if (type == ImpactType.InteractionObject) // �ε��� ������Ʈ�� �Ӽ��� InteractionObject�̸� ��ƼŬ �ý����� ���ο� �ִ� ��ŸƮ �÷��� �Ű������� �޾ƿ� �÷��� ����
        {
            ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
            main.startColor = color;
        }
    }
}
