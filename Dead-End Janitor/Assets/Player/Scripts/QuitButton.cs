using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    // This method is called when the Quit button is clicked
    private void Start() {
        Button quit_b = transform.GetComponent<Button>();
        quit_b.onClick.AddListener(QuitGame);
    }
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();

        // Note: This won't work in the Unity Editor. To test, use Debug.Log.
#if UNITY_EDITOR
        Debug.Log("Game Quit in Editor");
#endif
    }
}
