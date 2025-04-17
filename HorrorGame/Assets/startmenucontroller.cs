using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class startmenucontroller : MonoBehaviour
{
    public void OnStartClick() {
        SceneManager.LoadScene("Test_Scene");
    }

    public void OnExitClick() {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
