using System.Collections;
using UnityEngine;
using System;

public class PlayerScaleController : MonoBehaviour
{
    [Header("Движение")]
    public float forwardSpeed = 10f;
    public float lateralSpeed = 7f; // Скорость смещения по бокам

    [Header("Настройки анимации")]
    public float scaleDuration = 0.15f;

    [Header("Параметры анизотропного масштаба")]
    public Vector3 baseScale = Vector3.one;
    public Vector3 squeezeWidthScale = new Vector3(0.3f, 1.5f, 1f);
    public Vector3 squeezeHeightScale = new Vector3(1.3f, 0.3f, 1f);
    public Vector3 breakWallsScale = new Vector3(1.4f, 1.4f, 1.4f);

    [Header("Архитектурные теги")]
    public string destructibleWallTag = "BreakableWall";

    public event Action<Vector3> OnScaleChanged;

    private bool canBreakWalls = false;
    private Coroutine scaleCoroutine;

    // Кэш текущего целевого состояния, чтобы не спамить корутины каждый кадр
    private Vector3 currentTargetScale;

    private void Start()
    {
        currentTargetScale = baseScale;
        transform.localScale = baseScale;
    }

    private void Update()
    {
        Move();
        HandleScaleInput();
    }

    private void Move()
    {
        // Вектор движения вперед
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.deltaTime;

        // Вектор движения в стороны (опрашивает A/D или стрелки влево/вправо)
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 lateralMove = Vector3.right * horizontalInput * lateralSpeed * Time.deltaTime;

        // Складываем векторы и применяем
        transform.Translate(forwardMove + lateralMove);
    }

    private void HandleScaleInput()
    {
        // По умолчанию предполагаем, что кнопки отпущены (базовый масштаб)
        Vector3 newTargetScale = baseScale;
        bool newCanBreakWalls = false;

        // Приоритет кнопок через else if. Используем GetKey (удержание), а не GetKeyDown
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

        // Запускаем перестроение формы ТОЛЬКО если целевой масштаб изменился
        if (newTargetScale != currentTargetScale)
        {
            currentTargetScale = newTargetScale;
            canBreakWalls = newCanBreakWalls;
            ChangeScaleSmooth(currentTargetScale);
        }
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
        if (canBreakWalls && collision.gameObject.CompareTag(destructibleWallTag))
        {
            Destroy(collision.gameObject);
        }
    }
}