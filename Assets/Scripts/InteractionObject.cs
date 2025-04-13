using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    [Header("Interaction Object")]
    [SerializeField]
    protected int maxHP = 100; // 오브젝트 최대 체력
    protected int currentHP; // 오브젝트 현재 체력

    private void Awake()
    {
        currentHP = maxHP;
    }

    public abstract void TakeDamage(int damage); // 추상 메소드
}

