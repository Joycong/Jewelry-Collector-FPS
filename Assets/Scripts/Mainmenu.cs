using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    private void Awake()
    {
        // ���콺 ī���� ���̰� ����, ��ġ ���� ����
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

        public void OnClickNewGame()
    {
        SceneManager.LoadScene("MYFPSGAME sin#1");
    }

    public void OnClickLoad()
    { 
    
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
