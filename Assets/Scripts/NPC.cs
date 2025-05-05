using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactionKey = KeyCode.F;
    public float interactionRange = 3f;

    [Header("UI References")]
    public GameObject interactionPrompt; // 提示按F的UI元素

    [Header("Events")]
    public UnityEvent onInteracted; // 交互时触发的事件

    private bool isPlayerInRange = false;

    private void Update()
    {
        // 检测按键输入
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

    // 可视化交互范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}