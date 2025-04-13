using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        // "Player" 오브젝트 기준으로 자식 오브젝트인
        // "arms_assault_rifle_01" 오브젝트에 Animator 컴포넌트가 있다
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        // animator.SetFloat("ParamName", value); : Animator View에 있는 float 타입 변수 "ParamName"의 값을 value로 설정
        set => animator.SetFloat("movementSpeed", value);
        // float f = animator.GetFloat("ParamName"); : Animator View에 있는 float 타입 변수 "ParamName"의 값을 반환
        get => animator.GetFloat("movementSpeed");
    }

    public void OnReload()
    {
        animator.SetTrigger("onReload"); // onReload를 on시켜, 재장전 애니메이션 재생
    }

    // Assault Rifle 마우스 오른쪽 클릭 액션 (default/aim mode)
    public bool AimModeIs
    {
        set => animator.SetBool("isAimMode", value);
        get => animator.GetBool("isAimMode");
    }

    public void Play(string stateName, int layer, float normalizedTime)
    { 
        animator.Play(stateName, layer, normalizedTime);
    }

    public bool CurrentAnimationIs(string name) // 매개변수로 받아온 name애니메이션이 현재 재생중인지 확인하고, 그 결과를 반환
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public void SetFloat(string paramName, float value)
    {
        animator.SetFloat(paramName, value);
    }
}
