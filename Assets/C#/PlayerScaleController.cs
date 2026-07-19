using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerScaleController : MonoBehaviour
{
    [Header("Настройки банана")]
    public string bananaTag = "Banana";
    public Renderer modelRenderer;

    private bool isSlowed = false;
    private float defaultSpeed;

    [Header("Настройки смерти")]
    public float knockbackForce = 15f;
    public float restartDelay = 1.5f;
    public GameOverScript GameOverScreen;

    private bool isDead = false;
    private Rigidbody rb;
    private float survivalTimer = 0f;

    [Header("Движение")]
    public float forwardSpeed = 10f;
    public float lateralSpeed = 7f;
    public string obstacleTag = "Wall";

    [Header("Процедурная анимация (Бег)")]
    public Transform visualModel;
    public float bobFrequency = 10f;
    public float bobAmplitude = 0.1f;

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
    private float defaultVisualY;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultSpeed = forwardSpeed; // Сохраняем скорость для сброса банана

        

        currentTargetScale = baseScale;
        transform.localScale = baseScale;

        if (visualModel != null)
        {
            defaultVisualY = visualModel.localPosition.y;
        }
    }

    private void Update()
    {
        if (isDead) return;

        // Считаем время жизни, пока игрок жив
        survivalTimer += Time.deltaTime;

        Move();
        HandleScaleInput();
        AnimateRun();

        survivalTimer += Time.deltaTime;
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

    private void AnimateRun()
    {
        if (visualModel == null) return;

        float currentY = defaultVisualY + Mathf.Abs(Mathf.Sin(Time.time * bobFrequency)) * bobAmplitude;
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
        if (canBreakWalls && collision.gameObject.CompareTag(destructibleWallTag))
        {
            Destroy(collision.gameObject);
            return;
        }

        if (collision.gameObject.CompareTag(obstacleTag))
        {
            GameOver();
        }
    }

    // --- ЛОГИКА БАНАНА ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bananaTag))
        {
            Destroy(other.gameObject);

            if (!isSlowed)
            {
                StartCoroutine(BananaDebuffRoutine());
            }
        }
    }

<<<<<<< HEAD
    private IEnumerator BananaDebuffRoutine()
    {
        isSlowed = true;
        forwardSpeed = defaultSpeed * 0.55f;
=======
    public AudioClip deathSoundClip;

    private AudioSource audioSource;
    
>>>>>>> 2efe0792e5755363b47d9804c704d2c359fcd42a

        float duration = 2f;
        float blinkInterval = 0.15f;
        float timePassed = 0f;

        while (timePassed < duration)
        {
            if (modelRenderer != null)
            {
                modelRenderer.enabled = !modelRenderer.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);
            timePassed += blinkInterval;
        }

        if (modelRenderer != null)
        {
            modelRenderer.enabled = true;
        }

        forwardSpeed = defaultSpeed;
        isSlowed = false;
    }

    // --- ЛОГИКА СМЕРТИ ---
    public void GameOver()
    {
<<<<<<< HEAD
        if (isDead) return;
        isDead = true;
=======
        
        if ( deathSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(deathSoundClip, transform.position);
        }
>>>>>>> 2efe0792e5755363b47d9804c704d2c359fcd42a

        // Физическое отталкивание
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            Vector3 knockbackDirection = new Vector3(0f, 1f, -1.5f).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }

        // Запускаем корутину ожидания экрана
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Ждем пока свинка красиво отлетит, прежде чем показывать UI и удалять объект
        yield return new WaitForSeconds(restartDelay);

        if (GameOverScreen != null)
        {
            GameOverScreen.Setup(survivalTimer);
        }

       

        
        Destroy(gameObject);
    }
<<<<<<< HEAD
}
=======
}
    /*  private void Die()
      {
          Debug.Log("Смерть! Перезагрузка сцены...");
          // Перезагружаем текущую сцену
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }*/

>>>>>>> 2efe0792e5755363b47d9804c704d2c359fcd42a
