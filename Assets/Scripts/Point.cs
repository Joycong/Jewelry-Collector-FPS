using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Point : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private int count;

    void Start()
    {
        // count�� 0���� �����մϴ�. 
        count = 0;

        SetCountText();

        // Win Text UI�� �ؽ�Ʈ ������Ƽ�� �� ���ڿ��� �����Ͽ� 'You Win'(���� ���� �޽���)�� �������� ����ϴ�.
        winTextObject.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        // �����ϴ� ���� ������Ʈ�� 'Pick Up' �±װ� �Ҵ�Ǿ� �ִ� ���
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);

            // ���� ���� 'count'�� 1�� �߰��մϴ�.
            count = count + 1;

            // 'SetCountText()' �Լ��� �����մϴ�(�Ʒ� ����).
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "������ ���� : " + count.ToString() + " / 5";

        if (count >= 5)
        {
            // 'winText'�� �ؽ�Ʈ ���� �����մϴ�.
            winTextObject.SetActive(true);

            StartCoroutine(LoadNextSceneAfterDelay(3f));
        }
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���� ���� �ε����� ����մϴ�.
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // ���� ������ �̵��մϴ�.
        SceneManager.LoadScene(nextSceneIndex);
    }
}
