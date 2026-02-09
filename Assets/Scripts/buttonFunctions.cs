using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        Gamemanager.instance.StateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Gamemanager.instance.StateUnpause();
        if(playerController.instance.playerInfo.Speed > playerController.instance.playerInfo.origSpeed)
        {
            playerController.instance.playerInfo.Speed = playerController.instance.playerInfo.origSpeed;
        }
    }

    public void settings()
    {
        Gamemanager.instance.Settings();
    }

    public void back()
    {
        Gamemanager.instance.Back();
    }

    public void TimerCheck()
    {
        Gamemanager.instance.TimerCheck();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();
#endif
    }
}
