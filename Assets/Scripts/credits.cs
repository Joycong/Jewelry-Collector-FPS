using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class credits : MonoBehaviour
{
    void Update()
    {
        // 1�ʿ� 100�� y������ �̵�
        transform.Translate(Vector3.up * 100f * Time.deltaTime);

        // y ��ġ�� 4500 �̻��� �� �� ��ȯ
        if (transform.position.y >= 4500f)
        {
            SceneManager.LoadScene("MYFPSGAME sin main");
        }
    }
}
