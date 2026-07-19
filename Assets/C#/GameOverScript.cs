using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class GameOverScript : MonoBehaviour
{
    public Text timeText;
    public AudioSource backgroundMusic;
    public AudioClip clickSoundClip;

    public void Setup(float timeSurvived)
    {
        gameObject.SetActive(true);
        timeText.text = "Run Time: " + timeSurvived.ToString("F1") + " sec";

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }
    }

   
    public void RestartGame()
    {
       
        StartCoroutine(LoadSceneWithDelay(SceneManager.GetActiveScene().buildIndex));
    }

  
    public void ExitGame()
    {
       
        StartCoroutine(LoadSceneWithDelay("MainMenu"));
    }

  
    private IEnumerator LoadSceneWithDelay(int sceneIndex)
    {
        PlayClickSound();
       
        yield return new WaitForSecondsRealtime(0.15f);
        SceneManager.LoadScene(sceneIndex);
    }

   
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        PlayClickSound();
        
        yield return new WaitForSecondsRealtime(0.15f);
        SceneManager.LoadScene(sceneName);
    }

    private void PlayClickSound()
    {
        if (clickSoundClip != null)
        {
            
            AudioSource.PlayClipAtPoint(clickSoundClip, Camera.main.transform.position);
        }
    }
}