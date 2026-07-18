using UnityEngine;
using UnityEngine.SceneManagement; // Этот модуль обязателен для управления сценами

public class MainMenuManager : MonoBehaviour
{
    // Метод для кнопки "Играть"
    public void PlayGame()
    {
        // Загружаем сцену с геймплеем по ее точному имени
        SceneManager.LoadScene("test"); 
    }

    // Метод для кнопки "Выход"
    public void QuitGame()
    {
        Debug.Log("Игрок нажал Выход"); // Это мы увидим в консоли Unity во время теста
        Application.Quit(); // А эта строчка закроет игру в собранном .exe файле
    }
}
