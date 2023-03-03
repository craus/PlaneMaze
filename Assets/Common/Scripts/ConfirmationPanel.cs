using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConfirmationPanel : Singletone<ConfirmationPanel>
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private GameObject panel;

    private Action action;

    public void Awake() {
        panel.SetActive(false);
        action = null;
    }

    public void DoWithConfirmation(Action action, string message) {
        if (this.action != null) {
            return;
        }
        this.action = action;
        panel.SetActive(true);
        text.text = message;
    }

    public void OK() {
        panel.SetActive(false);
        action();
        action = null;
    }

    public void Cancel() {
        panel.SetActive(false);
        action = null;
    }
}
