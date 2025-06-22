// Assets/Scripts/RoomCameraController.cs
using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Transform targetRoomTransform;
    private Vector3 targetPosition;

    void Update()
    {
        if (targetRoomTransform != null)
        {
            // 카메라 위치를 목표 방의 위치로 부드럽게 이동
            targetPosition = new Vector3(targetRoomTransform.position.x, targetRoomTransform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    // DungeonManager가 호출하여 카메라의 목표를 새로운 방으로 설정
    public void MoveToNewRoom(Transform newRoomTransform)
    {
        targetRoomTransform = newRoomTransform;
    }
}