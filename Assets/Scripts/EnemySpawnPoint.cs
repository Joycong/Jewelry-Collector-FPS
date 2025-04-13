using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed = 4;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect"); // 활성화 될 때 코루틴 시작
    }

    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect"); // 활성화 될 때 코루틴 중지
    }

    private IEnumerator OnFadeEffect()
    { 
        while (true) 
        { 
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
