using System.Collections.Generic;
using UnityEngine;

public class MemoryPool // 오브젝트를 파괴하지 않고 비활성화하여 관리(메모리 풀)
{
    // 메모리 풀로 관리되는 오브젝트 정보
    private class PoolItem
    {
        public bool isActive; // "gameObject"의 활성화/비활성화 정보
        public GameObject gameObject; // 화면에 보이는 실제 게임오브젝트
    }

    private int increaseCount = 5; // 오브젝트가 부족할 때 Instantiate()로 추가 생성되는 오브젝트 개수
    private int maxCount; // 현재 리스트에 등록되어 있는 오브젝트 개수
    private int activeCount; // 현재 게임에 사용되고 있는(활성화) 오브젝트 개수

    private GameObject poolObject; // 오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
    private List<PoolItem> poolItemList; // 관리되는 모든 오브젝트를 저장하는 리스트

    public int MaxCount => maxCount; // 외부에서 현재 리스트에 등록되어 있는 오브젝트 개수 확인을 위한 프로퍼티
    public int ActiveCount => activeCount; // 외부에서 현재 활성화 되어 있는 오브젝트 개수 확인을 위한 프로퍼티

    // 오브젝트가 임시로 보관되는 위치
    private Vector3 tempPosition = new Vector3(48, 1, 48);

    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>();

        InstantiateObjects(); // 최초 5개의 아이템 미리 생성
    }

    /// <summary>
    /// increaseCount 단위로 오브젝트를 생성
    /// </summary>
    public void InstantiateObjects() // 오브젝트를 increaseCount개수만큼 생성
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; ++i)
        { 
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject); // 오브젝트 생성
            poolItem.gameObject.transform.position = tempPosition; // 생성된 오브젝트 위치 tempPosition으로 설정 (안 쓰는 공간에서 관리)
            poolItem.gameObject.SetActive(false); // 바로 사용하지 않을 수도 있기 때문에 보이지 않게 함

            poolItemList.Add(poolItem);
        }
    }

    /// <summary>
    /// 현재 관리중인(활성/비활성) 모든 오브젝트를 삭제
    /// </summary>
    public void DestroyObjects() // 오브젝트 삭제(지양해야하는 파괴 메소드이므로 최대한 적게 사용) -> 씬이 바뀌거나 게임이 종료될 때 한번만 수행하여 모든 게임 오브젝트를 한번에 삭제
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
    /// poolItemList에 저장되어 있는 오브젝트를 활성화해서 사용
    /// 현재 모든 오브젝트가 사용중이면 InstantiateObjects()로 추가 생성
    /// </summary>
    public GameObject ActivatePoolItem() // 현재 비활성화 상태의 오브젝트 중 하나를 활성화로 만들어 사용
    {
        if(poolItemList == null) return null; // 리스트 비어있으면 관리중인 오브젝트가 없는 것이므로 null반환

        // 현재 생성해서 관리하는 모든 오브젝트 개수와 현재 활성화 상태인 오브젝트 개수 비교
        // 모든 오브젝트가 활성화 상태이면 새로운 오브젝트 필요
        if (maxCount == activeCount) // maxCount == activeCount이면 모든 오브젝트가 활성화 되어 게임에서 사용중인 것이므로
        {
            InstantiateObjects(); // 추가로 오브젝트 생성(비활성 오브젝트가 없을 때)
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i) // poolItemList의 요소들을 탐색하여 비활성화 상태의 오브젝트를 찾아 활성화 시키고
        { 
            PoolItem poolItem = poolItemList[i];

            if (poolItem.isActive == false) 
            {
                activeCount++;
                
                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject; // 외부에서 사용하도록 오브젝트 반환
            }
        }

        return null;
    }

    /// <summary>
    /// 현재 사용이 완료된 오브젝트를 비활성화 상태로 설정
    /// </summary>
    public void DeactivatePoolItem(GameObject removeObject) // 매개변수로 받아온 removeObject와 동일한 오브젝트를 리스트 요소중에 찾아, 해당요소를 비활성화 함(현재 활성화 상태인 오브젝트 removeObject를 비활성화 상태로 만들어 보이지 않게 함)
    { 
        if(poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for(int i=0;i<count;++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.gameObject.transform.position = tempPosition; // 비활성화되는 오브젝트 위치
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    
    }

    /// <summary>
    /// 게임에 사용중인 모든 오브젝트를 비활성화 상태로 설정
    /// </summary>
    public void DeactivateAllPoolItems() // 현재 활성화 상태인 모든 게임오브젝트를 보이지 않게 함
    {
        // 리스트를 탐색하며 오브젝트가 null이 아니고, 활성화 상태인 리스트 요소들을 모두 비활성화 상태로 설정
        if (poolItemList == null) return; 

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.gameObject.transform.position = tempPosition; // 모든 오브젝트의 위치를 tempPosition으로 설정
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}
