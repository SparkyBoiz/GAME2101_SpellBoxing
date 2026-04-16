using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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
        StartCoroutine(ExitWithDelay());
    }

    private IEnumerator ExitWithDelay()
    {
        yield return new WaitForSeconds(1);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
}
