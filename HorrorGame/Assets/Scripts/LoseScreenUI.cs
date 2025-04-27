using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenUI : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
