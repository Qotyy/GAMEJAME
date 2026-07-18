using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerScaleController : MonoBehaviour
{

    [Header("Настройки смерти")]
    public float knockbackForce = 15f; // Сила отлета
    public float restartDelay = 1.5f;  // Время до перезагрузки сцены

    private bool isDead = false;
    private Rigidbody rb;

    [Header("Движение")]
    public float forwardSpeed = 10f;
    public float lateralSpeed = 7f;
    public string obstacleTag = "Wall";

    [Header("Процедурная анимация (Бег)")]
    public Transform visualModel; // Перетяни сюда дочерний объект с 3D-моделью
    public float bobFrequency = 10f; // Частота шагов (скорость подпрыгиваний)
    public float bobAmplitude = 0.1f; // Высота подпрыгивания при беге

    [Header("Настройки анимации масштаба")]
    public float scaleDuration = 0.15f;

    [Header("Параметры анизотропного масштаба")]
    public Vector3 baseScale = Vector3.one;
    public Vector3 squeezeWidthScale = new Vector3(0.3f, 1.3f, 1f);
    public Vector3 squeezeHeightScale = new Vector3(1.3f, 0.3f, 1f);
    public Vector3 breakWallsScale = new Vector3(1.4f, 1.4f, 1.4f);

    [Header("Архитектурные теги")]
    public string destructibleWallTag = "BreakableWall";

    public event Action<Vector3> OnScaleChanged;

    private bool canBreakWalls = false;
    private Coroutine scaleCoroutine;
    private Vector3 currentTargetScale;
    private float defaultVisualY; // Исходная высота модели

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Получаем компонент

        currentTargetScale = baseScale;
        transform.localScale = baseScale;

        if (visualModel != null)
        {
            defaultVisualY = visualModel.localPosition.y;
        }
    }

    private void Update()
    {
        if (isDead) return; // Блокируем всё, если персонаж умер

        Move();
        HandleScaleInput();
        AnimateRun();
    }

    private void Move()
    {
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.deltaTime;
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 lateralMove = Vector3.right * horizontalInput * lateralSpeed * Time.deltaTime;

        transform.Translate(forwardMove + lateralMove);
    }

    private void HandleScaleInput()
    {
        Vector3 newTargetScale = baseScale;
        bool newCanBreakWalls = false;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            newTargetScale = squeezeWidthScale;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            newTargetScale = squeezeHeightScale;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            newTargetScale = breakWallsScale;
            newCanBreakWalls = true;
        }

        if (newTargetScale != currentTargetScale)
        {
            currentTargetScale = newTargetScale;
            canBreakWalls = newCanBreakWalls;
            ChangeScaleSmooth(currentTargetScale);
        }
    }

    // Алгоритм математической анимации бега
    private void AnimateRun()
    {
        if (visualModel == null) return;

        // Mathf.Abs(Sin) создает эффект дуги. Объект "отскакивает" только вверх от базовой точки, имитируя шаги.
        float currentY = defaultVisualY + Mathf.Abs(Mathf.Sin(Time.time * bobFrequency)) * bobAmplitude;

        // Применяем локальную позицию только к визуальной модели
        visualModel.localPosition = new Vector3(visualModel.localPosition.x, currentY, visualModel.localPosition.z);
    }

    private void ChangeScaleSmooth(Vector3 targetScale)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale));
        OnScaleChanged?.Invoke(targetScale);
    }

    private IEnumerator ScaleRoutine(Vector3 targetScale)
    {
        Vector3 initialScale = transform.localScale;
        float timePassed = 0f;

        while (timePassed < scaleDuration)
        {
            timePassed += Time.deltaTime;
            float percent = timePassed / scaleDuration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, percent);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Проверка на пробитие деревянной стены
        if (canBreakWalls && collision.gameObject.CompareTag(destructibleWallTag))
        {
            Destroy(collision.gameObject);
            return; // Выходим, чтобы не проверять другие столкновения
        }

        // 2. Проверка на столкновение с препятствием (Смерть)
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            GameOver();
        }
    }

    // Архитектурно выносим смерть в отдельный метод. 
    // Позже сюда можно будет добавить партиклы взрыва или вызов GameManager.
    private float survivalTimer = 0f;

    public GameOverScript GameOverScreen;



    public void GameOver()
    {

        GameOverScreen.Setup(survivalTimer);

        Destroy(gameObject);
    }
    /*  private void Die()
      {
          Debug.Log("Смерть! Перезагрузка сцены...");
          // Перезагружаем текущую сцену
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }*/
}
