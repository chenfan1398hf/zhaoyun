using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // ��ȡ������������������ֻ��һ���������
        mainCamera = Camera.main;

        // ���û���ҵ���������������ֶ�����
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // ��Ѫ��ʼ�����������
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                            mainCamera.transform.rotation * Vector3.up);
        }
    }
}