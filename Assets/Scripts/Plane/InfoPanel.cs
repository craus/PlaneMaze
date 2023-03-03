using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : Singletone<InfoPanel>
{
    public GameObject panel;

    public TextMeshProUGUI text;
    public Image icon;

    public TextMeshProUGUI title;

    public HashSet<IExplainable> viewedInfo = new HashSet<IExplainable>();

    public void Show(IExplainable explainable) {
        if (viewedInfo.Contains(explainable.Sample)) {
            return;
        }
        viewedInfo.Add(explainable.Sample);
        panel.SetActive(true);
        text.text = explainable.Text;
        icon.sprite = explainable.Icon;
        icon.color = explainable.IconColor;
        icon.material = explainable.IconMaterial;
    }

    public void Awake() {
        panel.SetActive(false);
    }
}
