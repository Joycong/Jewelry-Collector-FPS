using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        // "Player" ������Ʈ �������� �ڽ� ������Ʈ��
        // "arms_assault_rifle_01" ������Ʈ�� Animator ������Ʈ�� �ִ�
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        // animator.SetFloat("ParamName", value); : Animator View�� �ִ� float Ÿ�� ���� "ParamName"�� ���� value�� ����
        set => animator.SetFloat("movementSpeed", value);
        // float f = animator.GetFloat("ParamName"); : Animator View�� �ִ� float Ÿ�� ���� "ParamName"�� ���� ��ȯ
        get => animator.GetFloat("movementSpeed");
    }

    public void OnReload()
    {
        animator.SetTrigger("onReload"); // onReload�� on����, ������ �ִϸ��̼� ���
    }

    // Assault Rifle ���콺 ������ Ŭ�� �׼� (default/aim mode)
    public bool AimModeIs
    {
        set => animator.SetBool("isAimMode", value);
        get => animator.GetBool("isAimMode");
    }

    public void Play(string stateName, int layer, float normalizedTime)
    { 
        animator.Play(stateName, layer, normalizedTime);
    }

    public bool CurrentAnimationIs(string name) // �Ű������� �޾ƿ� name�ִϸ��̼��� ���� ��������� Ȯ���ϰ�, �� ����� ��ȯ
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public void SetFloat(string paramName, float value)
    {
        animator.SetFloat(paramName, value);
    }
}
