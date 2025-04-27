using UnityEngine;
using UnityEngine.UI;

public class DeathScreenDisplay : MonoBehaviour
{
    public RawImage backgroundImage;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (EnemyAI.deathScreenBackground != null)
        {
            backgroundImage.texture = EnemyAI.deathScreenBackground;
        }
    }

    void OnDisable() 
    {
        if (EnemyAI.deathScreenBackground != null) {
            Destroy(EnemyAI.deathScreenBackground);
            EnemyAI.deathScreenBackground = null;
        }
    }
}