using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishManager : MonoBehaviour
{
    [Header("UI элементы")]
    public GameObject finishPanel;

    [Header("Аудио")]
    [Tooltip("Перетащи сюда AudioSource, который играет фоновую музыку на уровне")]
    public AudioSource gameplayMusic; 
    
    private AudioSource victoryMusic; // Сюда автоматически подтянется победный звук

    private void Start()
    {
        if (finishPanel != null)
            finishPanel.SetActive(false);

        // Автоматически находим AudioSource, который висит на этом же объекте FinishZone
        victoryMusic = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            finishPanel.SetActive(true);

            // РАБОТА СО ЗВУКОМ:
            // 1. Останавливаем фоновую музыку геймплея, если она была привязана
            if (gameplayMusic != null && gameplayMusic.isPlaying)
            {
                gameplayMusic.Stop(); 
            }

            // 2. Включаем победную музыку
            if (victoryMusic != null)
            {
                victoryMusic.Play();
            }

            // ВАЖНО: Ставим паузу только ПОСЛЕ запуска звука.
            // В Unity звуки из AudioSource продолжают играть при Time.timeScale = 0,
            // если у них не включены эффекты, зависящие от физического времени.
            Time.timeScale = 0f; 
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}