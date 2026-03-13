using UnityEngine;
using UnityEngine.SceneManagement;
public class M_MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void OpenInstrucitons()
    {
        SceneManager.LoadScene(1);
    }


    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
