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
        GameManager.instance.OpenTaskPanel(true);
    }

    void ExitInteractionRange()
    {
        isPlayerInRange = false;
        GameManager.instance.OpenTaskPanel(false);
    }

    void TriggerInteraction()
    {
        GameManager.instance.AddTask(2);
        GameManager.instance.OpenTaskPanel(false);
    }

    // ���ӻ�������Χ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}