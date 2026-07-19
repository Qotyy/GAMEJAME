using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Настройки UI")]
    public float loadDelay = 0.5f; // Время задержки перед загрузкой

    // Метод для кнопки "Играть"
    public void PlayGame()
    {
        // Запускаем асинхронную корутину для ожидания
        StartCoroutine(PlayGameRoutine());
    }

    private IEnumerator PlayGameRoutine()
    {
        // Принцип ожидания: приостанавливаем выполнение на 0.5 секунд
        yield return new WaitForSeconds(loadDelay);

        // Загружаем сцену после паузы
        SceneManager.LoadScene("test");
    }

    // Метод для кнопки "Выход"
    public void QuitGame()
    {
        Debug.Log("Игрок нажал Выход");
        Application.Quit();
    }
}