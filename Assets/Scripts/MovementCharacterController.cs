using UnityEngine;

[RequireComponent(typeof(CharacterController))] // �� ��ũ��Ʈ�� ���� ������Ʈ�� ������Ʈ�� �����ϸ� �ش� ������Ʈ�� ���� �߰���(CharacterController)
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed; // �̵��ӵ�
    private Vector3 moveForce; // �̵� �� (x, z�� y���� ������ ����� ���� �̵��� ����)

    [SerializeField]
    private float jumpForce; // ���� ��
    [SerializeField]
    private float gravity; // �߷� ���

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // �̵��ӵ��� ������ ������� �ʵ��� ��
        get => moveSpeed;
    }

    private CharacterController characterController; // �÷��̾� �̵� ��� ���� ������Ʈ

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // ����� �������� �߷¸�ŭ y�� �̵��ӵ� ����
        if (!characterController.isGrounded) // characterController.isGrounded ������Ƽ : ���ӿ�����Ʈ�� ���� �ٴڰ� �浹�ϴ��� üũ(�浹�� �� true ��ȯ)
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        // 1�ʴ� moveForce �ӷ����� �̵�
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction) // ���� �Ʒ��� �ٶ󺸰� �̵��� ��� ĳ���Ͱ� �������� �߰ų� �Ʒ��� ��������� �ϱ� ������ direction�� �״�� ������� �ʰ�, moveFoce������ x, z���� �־ ���
    // ī�޶� ȸ������ ���� ������ ���ϱ� ������ ȸ�� ���� ���ؼ� �����ؾ� ��
    {
        // �̵� ���� = ĳ������ ȸ�� �� * ���� ��
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);
    
        // �̵� �� = �̵����� * �ӵ�
        moveForce=new Vector3 (direction.x*moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        // �÷��̾ �ٴڿ� ���� ���� ���� ����
        if (characterController.isGrounded) // characterController.isGrounded ������Ƽ : ���ӿ�����Ʈ�� ���� �ٴڰ� �浹�ϴ��� üũ(�浹�� �� true ��ȯ)
        {
            moveForce.y = jumpForce;
        }
    }
}
