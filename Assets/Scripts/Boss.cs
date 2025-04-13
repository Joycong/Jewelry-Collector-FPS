using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    private bool canCount = false;

    void Start()
    {
        Invoke("EnableCounting", 4f);
    }

    void EnableCounting()
    {
        canCount = true;
    }

    private void Update()
    {
        if (canCount == true && gameObject.activeSelf == false)
        {
            StartCoroutine(LoadNextSceneAfterDelay(1f));

        }
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���� ���� �ε����� ����մϴ�.
        //int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // ���� ������ �̵��մϴ�.
        SceneManager.LoadScene("MYFPSGAME sin#1");
    }
}



