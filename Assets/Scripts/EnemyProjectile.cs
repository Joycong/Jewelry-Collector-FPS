using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movement;
    private float projectileDistance = 30;  // �߻�ü �ִ� �߻�Ÿ�
    private int damage = 5;                 // �߻�ü ���ݷ�

    public void Setup(Vector3 position) // �Ű������� �޾ƿ� position ��ġ�� �������� �������� �̵�
    {
        movement = GetComponent<MovementTransform>();

        StartCoroutine("OnMove", position);
    }

    private IEnumerator OnMove(Vector3 targetPosition) // �̵����� ���� �� �̵� ���� �ʰ� ����
    {
        Vector3 start = transform.position;

        movement.MoveTo((targetPosition - transform.position).normalized); // �̵����� ����

        while (true)
        {
            if (Vector3.Distance(transform.position, start) >= projectileDistance) // �߻�ü�� ������ start��ġ�� ���� ��ġ�� �Ÿ��� projectileDistance���� ũ�ų� ������,,
            {
                Destroy(gameObject); // �߻�ü ����

                yield break; // �ڷ�ƾ ����
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �߻�ü�� �ε��� ������Ʈ�� �±װ� "Player"�̸�
        {
            // Debug.Log("Player Hit"); // �α� ���
            other.GetComponent<PlayerController>().TakeDamage(damage); // ��������ŭ ü�� ����

            Destroy(gameObject); // �߻�ü ����
        }
    }
}

