using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class MovesReserve : MonoBehaviour
{
    public List<Transform> freezes;
    public List<Transform> bonuses;

    [SerializeField] private int current = 0;

    public int Current => current;

    public async Task Freeze(int amount) {
        current = Mathf.Clamp(current - amount, -999, 999);
        UpdateIcons();
    }

    public async Task Haste(int amount) {
        current = Mathf.Clamp(current + amount, -999, 999);
        UpdateIcons();
    }

    public void Awake() {
        UpdateIcons();
    }

    private void ShowIcons(List<Transform> icons, int amount, bool show) {
        if (show) {
            for (int i = 0; i < icons.Count; i++) {
                icons[i].gameObject.SetActive(amount >= i + 1);
                icons[i].localPosition = new Vector3(i - amount * 0.5f + 0.5f, 0, 0);
            }
        } else {
            icons.ForEach(h => h.gameObject.SetActive(false));
        }
    }

    private void UpdateIcons() {
        ShowIcons(freezes, -current, current < 0);
        ShowIcons(bonuses, current, current > 0);
    }
}
