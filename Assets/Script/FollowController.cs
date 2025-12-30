using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowController : MonoBehaviour
{
    public void StartGame()
    {
        if (SceneTransition.I != null) SceneTransition.I.LoadScene(1);
        else SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        var playerPersist = Object.FindFirstObjectByType<PlayerPersist>();
        if (playerPersist) Destroy(playerPersist.gameObject);

        var uiPersist = Object.FindFirstObjectByType<UIPersist>();
        if (uiPersist) Destroy(uiPersist.gameObject);

        if (SceneTransition.I != null) SceneTransition.I.LoadScene("SelectCharacter");
        else SceneManager.LoadScene("SelectCharacter");
        #else
    Application.Quit();
        #endif
    }

}