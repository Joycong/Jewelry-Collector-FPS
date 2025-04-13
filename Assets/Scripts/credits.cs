using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class credits : MonoBehaviour
{
    void Update()
    {
        // 1초에 100씩 y축으로 이동
        transform.Translate(Vector3.up * 100f * Time.deltaTime);

        // y 위치가 4500 이상일 때 씬 전환
        if (transform.position.y >= 4500f)
        {
            SceneManager.LoadScene("MYFPSGAME sin main");
        }
    }
}
