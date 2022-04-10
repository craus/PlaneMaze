using TMPro;
using UnityEngine;

public sealed class DebugPanel : Singletone<DebugPanel>
{
    [SerializeField]
    private TextMeshPro text;

    [Multiline]
    [SerializeField] [ReadOnly] string fullLog = "";

    [SerializeField]
    private TextMeshProUGUI textUI;

    [SerializeField]
    private int maxLength = 100;

    private void Start()
    {
        if (!Extensions.InUnityEditor())
        {
            if (text)
            {
                text.enabled = false;
            }
            if (textUI)
            {
                textUI.enabled = false;
            }
        }
    }

    public void Message(string text)
    {
        if (this.text != null)
        {
            this.text.text += text + "\n";
            this.text.text = this.text.text.Suffix(maxLength);
        }
        if (textUI != null)
        {
            textUI.text += text + "\n";
            textUI.text = textUI.text.Suffix(maxLength);
        }
        fullLog += text + "\n";
    }
}
