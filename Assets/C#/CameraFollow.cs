using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Перетяни сюда своего игрока в инспекторе
    public Vector3 offset = new Vector3(0, 3, -7); // Настрой смещение камеры
    public float smoothSpeed = 10f; // Скорость доводки камеры

    private void LateUpdate()
    {
        if (player == null) return;

        // Целевая позиция камеры
        Vector3 targetPosition = player.position + offset;

        // Плавное следование за позицией (не зависит от масштаба игрока!)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}