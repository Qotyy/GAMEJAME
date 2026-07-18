using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public Text timeText;
    public void Setup(float timeSurvived)
    {
        gameObject.SetActive(true);

        timeText.text = "Run Time: " + timeSurvived.ToString("F1") + " sec";
    } 

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitGame()
    {
        Debug.Log("Игра закрылась!");
        Application.Quit();
    }
}
