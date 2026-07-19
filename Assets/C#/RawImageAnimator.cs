using UnityEngine;
using UnityEngine.UI;

public class RawImageAnimator : MonoBehaviour
{
    public RawImage rawImage;
    public float fps = 30f;

    private Sprite[] frames;
    private int currentFrame;
    private float timer;

    void Start()
    {
        if (rawImage == null) rawImage = GetComponent<RawImage>();

        // Код сам заходит в Resources/render и загружает все картинки по порядку
        frames = Resources.LoadAll<Sprite>("render");

        if (frames == null || frames.Length == 0)
        {
            Debug.LogError("Ошибка: Не удалось найти кадры в папке Assets/Resources/render !");
        }
    }

    void Update()
    {
        if (frames == null || frames.Length == 0 || rawImage == null) return;

        timer += Time.deltaTime;
        if (timer >= 1f / fps)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            rawImage.texture = frames[currentFrame].texture;
        }
    }
}