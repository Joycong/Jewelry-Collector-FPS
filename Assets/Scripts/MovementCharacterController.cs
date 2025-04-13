using UnityEngine;

[RequireComponent(typeof(CharacterController))] // 이 스크립트를 게임 오브젝트에 컴포넌트로 적용하면 해당 컴포넌트도 같이 추가됨(CharacterController)
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed; // 이동속도
    private Vector3 moveForce; // 이동 힘 (x, z와 y축을 별도로 계산해 실제 이동에 적용)

    [SerializeField]
    private float jumpForce; // 점프 힘
    [SerializeField]
    private float gravity; // 중력 계수

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // 이동속도는 음수가 적용되지 않도록 함
        get => moveSpeed;
    }

    private CharacterController characterController; // 플레이어 이동 제어를 위한 컴포넌트

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 허공에 떠있으면 중력만큼 y축 이동속도 감소
        if (!characterController.isGrounded) // characterController.isGrounded 프로퍼티 : 게임오브젝트의 발이 바닥과 충돌하는지 체크(충돌할 때 true 반환)
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        // 1초당 moveForce 속력으로 이동
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction) // 위나 아래를 바라보고 이동할 경우 캐릭터가 공중으로 뜨거나 아래로 가라앉으려 하기 때문에 direction을 그대로 사용하지 않고, moveFoce변수에 x, z값만 넣어서 사용
    // 카메라 회전으로 전방 방향이 변하기 때문에 회전 값을 곱해서 연산해야 함
    {
        // 이동 방향 = 캐릭터의 회전 값 * 방향 값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);
    
        // 이동 힘 = 이동방향 * 속도
        moveForce=new Vector3 (direction.x*moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        // 플레이어가 바닥에 있을 때만 점프 가능
        if (characterController.isGrounded) // characterController.isGrounded 프로퍼티 : 게임오브젝트의 발이 바닥과 충돌하는지 체크(충돌할 때 true 반환)
        {
            moveForce.y = jumpForce;
        }
    }
}
