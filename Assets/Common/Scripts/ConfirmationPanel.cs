using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConfirmationPanel : Singletone<ConfirmationPanel>
{
    [SerializeField] private TextMeshProUGUI text;

    public GameObject panel;
    [SerializeField] private GameObject cancelButton;

    private TaskCompletionSource<bool> currentConfirmation;

    public void Awake() {
        panel.SetActive(false);
    }

    public async Task<bool> AskConfirmation(string message, bool canCancel = true) {
        if (currentConfirmation != null) {
            return false;
        }
        Debug.LogFormat($"Show confirmation panel {message}");
        panel.SetActive(true);
        text.text = message;
        cancelButton.SetActive(canCancel);
        currentConfirmation = new TaskCompletionSource<bool>();
        return await currentConfirmation.Task;
    }

    public void OK() {
        if (currentConfirmation == null) {
            return;
        }
        panel.SetActive(false);

        var lastConfirmation = currentConfirmation;
        currentConfirmation = null;

        lastConfirmation.SetResult(true);
    }

    public void Cancel() {
        if (currentConfirmation == null) {
            return;
        }
        panel.SetActive(false);

        var lastConfirmation = currentConfirmation;
        currentConfirmation = null;

        lastConfirmation.SetResult(false);
    }
}
