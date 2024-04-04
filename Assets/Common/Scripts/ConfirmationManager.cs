using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConfirmationManager : Singletone<ConfirmationManager>
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI text2;

    public GameObject confirmationPanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject startPanel;
    public GameObject infoPanel;
    [SerializeField] private GameObject cancelButton;

    private TaskCompletionSource<bool> currentConfirmation;
    public GameObject currentPanel;
    public bool canCancel;
    public bool canConfirmByAnyButton;

    public bool AwaitingConfirmation => currentConfirmation != null;

    public void Awake() {
        confirmationPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        startPanel.SetActive(false);
    }

    public async Task<bool> AskConfirmation(
        string message = null, 
        bool canCancel = true, 
        bool canConfirmByAnyButton = false, 
        GameObject panel = null,
        Action customShow = null
    ) {
        if (currentConfirmation != null) {
            return false;
        }
        panel ??= confirmationPanel;
        Debug.LogFormat($"Show confirmation panel {message}");
        if (customShow == null) {
            panel.SetActive(true);
        } else {
            customShow();
        }
        if (message != null) {
            text.text = message;
            text2.text = message;
        }
        currentPanel = panel;
        this.canCancel = canCancel;
        this.canConfirmByAnyButton = canConfirmByAnyButton;
        cancelButton.SetActive(canCancel);
        currentConfirmation = new TaskCompletionSource<bool>();
        return await currentConfirmation.Task;
    }

    public void OK() {
        if (currentConfirmation == null) {
            return;
        }
        currentPanel.SetActive(false);

        var lastConfirmation = currentConfirmation;
        currentConfirmation = null;

        lastConfirmation.SetResult(true);
    }

    public void Cancel() {
        if (currentConfirmation == null || !canCancel) {
            return;
        }
        currentPanel.SetActive(false);

        var lastConfirmation = currentConfirmation;
        currentConfirmation = null;

        lastConfirmation.SetResult(false);
    }

    public void AnyButton() {
        if (canConfirmByAnyButton) OK();
    }

    public void DropAllConfirmations() {
        if (currentPanel != null) {
            currentPanel.SetActive(false);
        }
        currentConfirmation = null;
    }
}
