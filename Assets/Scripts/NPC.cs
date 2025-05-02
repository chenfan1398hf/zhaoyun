using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactionKey = KeyCode.F;
    public float interactionRange = 3f;

    [Header("UI References")]
    public GameObject interactionPrompt; // ��ʾ��F��UIԪ��

    [Header("Events")]
    public UnityEvent onInteracted; // ����ʱ�������¼�

    private bool isPlayerInRange = false;

    private void Update()
    {
        // ��ⰴ������
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            TriggerInteraction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnterInteractionRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitInteractionRange();
        }
    }

    void EnterInteractionRange()
    {
        isPlayerInRange = true;
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

    void ExitInteractionRange()
    {
        isPlayerInRange = false;
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void TriggerInteraction()
    {
        // ���������¼�
        onInteracted.Invoke();

        // �ر���ʾ����ѡ��
        ExitInteractionRange();

        Debug.Log("NPC��֪����ҵĽ�������");
    }

    // ���ӻ�������Χ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}