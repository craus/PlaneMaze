using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TextSetter : MonoBehaviour
{
    public StringValueProvider textProvider;
    public TMPro.TextMeshProUGUI text;

    public void Update() {
        text.text = textProvider.Value;
    }
}
