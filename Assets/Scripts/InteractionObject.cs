using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    [Header("Interaction Object")]
    [SerializeField]
    protected int maxHP = 100; // ������Ʈ �ִ� ü��
    protected int currentHP; // ������Ʈ ���� ü��

    private void Awake()
    {
        currentHP = maxHP;
    }

    public abstract void TakeDamage(int damage); // �߻� �޼ҵ�
}

