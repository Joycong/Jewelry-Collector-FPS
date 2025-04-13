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
        // count를 0으로 설정합니다. 
        count = 0;

        SetCountText();

        // Win Text UI의 텍스트 프로퍼티를 빈 문자열로 설정하여 'You Win'(게임 오버 메시지)을 공백으로 만듭니다.
        winTextObject.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        // 교차하는 게임 오브젝트에 'Pick Up' 태그가 할당되어 있는 경우
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);

            // 점수 변수 'count'에 1을 추가합니다.
            count = count + 1;

            // 'SetCountText()' 함수를 실행합니다(아래 참조).
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "수집한 보석 : " + count.ToString() + " / 5";

        if (count >= 5)
        {
            // 'winText'의 텍스트 값을 설정합니다.
            winTextObject.SetActive(true);

            StartCoroutine(LoadNextSceneAfterDelay(3f));
        }
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 다음 씬의 인덱스를 계산합니다.
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // 다음 씬으로 이동합니다.
        SceneManager.LoadScene(nextSceneIndex);
    }
}
