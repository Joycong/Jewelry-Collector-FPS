using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed; // �ȴ� �ӵ�
    [SerializeField]
    private float runSpeed; // �ٴ� �ӵ�

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100; // �ִ� HP
    private int currentHP; // ���� HP

    public float WalkSpeed => walkSpeed; // �ܺο��� �� Ȯ���ϴ� �뵵
    public float RunSpeed => runSpeed; // �ܺο��� �� Ȯ���ϴ� �뵵

    public int CurrentHP => currentHP; // �ܺο��� ������ �� �ֵ��� ������Ƽ
    public int MaxHP => maxHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = currentHP;

        currentHP = (currentHP - damage > 0) ? currentHP - damage : 0;

        onHPEvent.Invoke(previousHP, currentHP);

        if (currentHP == 0)
        {
            return true;
        }

        return false;
    }

    public void IncreaseHP(int hp) // ü�� ����
    {
        int previousHP = currentHP;

        currentHP = currentHP + hp > maxHP ? maxHP : currentHP + hp; // �ִ� ü���� ������ �ʵ���

        onHPEvent.Invoke(previousHP, currentHP);
    }
}
