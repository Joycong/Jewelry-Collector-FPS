using System.Collections.Generic;
using UnityEngine;

public class MemoryPool // ������Ʈ�� �ı����� �ʰ� ��Ȱ��ȭ�Ͽ� ����(�޸� Ǯ)
{
    // �޸� Ǯ�� �����Ǵ� ������Ʈ ����
    private class PoolItem
    {
        public bool isActive; // "gameObject"�� Ȱ��ȭ/��Ȱ��ȭ ����
        public GameObject gameObject; // ȭ�鿡 ���̴� ���� ���ӿ�����Ʈ
    }

    private int increaseCount = 5; // ������Ʈ�� ������ �� Instantiate()�� �߰� �����Ǵ� ������Ʈ ����
    private int maxCount; // ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ����
    private int activeCount; // ���� ���ӿ� ���ǰ� �ִ�(Ȱ��ȭ) ������Ʈ ����

    private GameObject poolObject; // ������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������
    private List<PoolItem> poolItemList; // �����Ǵ� ��� ������Ʈ�� �����ϴ� ����Ʈ

    public int MaxCount => maxCount; // �ܺο��� ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
    public int ActiveCount => activeCount; // �ܺο��� ���� Ȱ��ȭ �Ǿ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ

    // ������Ʈ�� �ӽ÷� �����Ǵ� ��ġ
    private Vector3 tempPosition = new Vector3(48, 1, 48);

    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>();

        InstantiateObjects(); // ���� 5���� ������ �̸� ����
    }

    /// <summary>
    /// increaseCount ������ ������Ʈ�� ����
    /// </summary>
    public void InstantiateObjects() // ������Ʈ�� increaseCount������ŭ ����
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; ++i)
        { 
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject); // ������Ʈ ����
            poolItem.gameObject.transform.position = tempPosition; // ������ ������Ʈ ��ġ tempPosition���� ���� (�� ���� �������� ����)
            poolItem.gameObject.SetActive(false); // �ٷ� ������� ���� ���� �ֱ� ������ ������ �ʰ� ��

            poolItemList.Add(poolItem);
        }
    }

    /// <summary>
    /// ���� ��������(Ȱ��/��Ȱ��) ��� ������Ʈ�� ����
    /// </summary>
    public void DestroyObjects() // ������Ʈ ����(�����ؾ��ϴ� �ı� �޼ҵ��̹Ƿ� �ִ��� ���� ���) -> ���� �ٲ�ų� ������ ����� �� �ѹ��� �����Ͽ� ��� ���� ������Ʈ�� �ѹ��� ����
    {
        if (poolItemList == null) return;
    
        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    /// <summary>
    /// poolItemList�� ����Ǿ� �ִ� ������Ʈ�� Ȱ��ȭ�ؼ� ���
    /// ���� ��� ������Ʈ�� ������̸� InstantiateObjects()�� �߰� ����
    /// </summary>
    public GameObject ActivatePoolItem() // ���� ��Ȱ��ȭ ������ ������Ʈ �� �ϳ��� Ȱ��ȭ�� ����� ���
    {
        if(poolItemList == null) return null; // ����Ʈ ��������� �������� ������Ʈ�� ���� ���̹Ƿ� null��ȯ

        // ���� �����ؼ� �����ϴ� ��� ������Ʈ ������ ���� Ȱ��ȭ ������ ������Ʈ ���� ��
        // ��� ������Ʈ�� Ȱ��ȭ �����̸� ���ο� ������Ʈ �ʿ�
        if (maxCount == activeCount) // maxCount == activeCount�̸� ��� ������Ʈ�� Ȱ��ȭ �Ǿ� ���ӿ��� ������� ���̹Ƿ�
        {
            InstantiateObjects(); // �߰��� ������Ʈ ����(��Ȱ�� ������Ʈ�� ���� ��)
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i) // poolItemList�� ��ҵ��� Ž���Ͽ� ��Ȱ��ȭ ������ ������Ʈ�� ã�� Ȱ��ȭ ��Ű��
        { 
            PoolItem poolItem = poolItemList[i];

            if (poolItem.isActive == false) 
            {
                activeCount++;
                
                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject; // �ܺο��� ����ϵ��� ������Ʈ ��ȯ
            }
        }

        return null;
    }

    /// <summary>
    /// ���� ����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    /// </summary>
    public void DeactivatePoolItem(GameObject removeObject) // �Ű������� �޾ƿ� removeObject�� ������ ������Ʈ�� ����Ʈ ����߿� ã��, �ش��Ҹ� ��Ȱ��ȭ ��(���� Ȱ��ȭ ������ ������Ʈ removeObject�� ��Ȱ��ȭ ���·� ����� ������ �ʰ� ��)
    { 
        if(poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for(int i=0;i<count;++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.gameObject.transform.position = tempPosition; // ��Ȱ��ȭ�Ǵ� ������Ʈ ��ġ
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    
    }

    /// <summary>
    /// ���ӿ� ������� ��� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    /// </summary>
    public void DeactivateAllPoolItems() // ���� Ȱ��ȭ ������ ��� ���ӿ�����Ʈ�� ������ �ʰ� ��
    {
        // ����Ʈ�� Ž���ϸ� ������Ʈ�� null�� �ƴϰ�, Ȱ��ȭ ������ ����Ʈ ��ҵ��� ��� ��Ȱ��ȭ ���·� ����
        if (poolItemList == null) return; 

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.gameObject.transform.position = tempPosition; // ��� ������Ʈ�� ��ġ�� tempPosition���� ����
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}
