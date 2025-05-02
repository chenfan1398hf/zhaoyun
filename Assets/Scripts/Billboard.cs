using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 获取主摄像机（如果场景中只有一个摄像机）
        mainCamera = Camera.main;

        // 如果没有找到主摄像机，尝试手动查找
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 让血条始终面向摄像机
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                            mainCamera.transform.rotation * Vector3.up);
        }
    }
}