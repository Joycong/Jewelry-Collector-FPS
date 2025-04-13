using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed; // 걷는 속도
    [SerializeField]
    private float runSpeed; // 뛰는 속도

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100; // 최대 HP
    private int currentHP; // 현재 HP

    public float WalkSpeed => walkSpeed; // 외부에서 값 확인하는 용도
    public float RunSpeed => runSpeed; // 외부에서 값 확인하는 용도

    public int CurrentHP => currentHP; // 외부에서 열람할 수 있도록 프로퍼티
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

    public void IncreaseHP(int hp) // 체력 증가
    {
        int previousHP = currentHP;

        currentHP = currentHP + hp > maxHP ? maxHP : currentHP + hp; // 최대 체력을 넘지는 않도록

        onHPEvent.Invoke(previousHP, currentHP);
    }
}
