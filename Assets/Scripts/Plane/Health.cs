using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Health : MonoBehaviour
{
    [SerializeField] private bool showHeartsOnFullHealth = false;

    public List<Transform> hearts;

    public int current = 3;
    public int max = 3;

    public int Current => current;

    public async Task Hit(int amount) {
        current = Mathf.Clamp(current - amount, 0, max); 
        if (current <= 0) {
            await GetComponent<Unit>().Die();
        } else {
            UpdateHearts();
        }
    }

    public async Task Heal(int amount) {
        current = Mathf.Clamp(current + amount, 0, max);
        UpdateHearts();
    }

    public void Awake() {
        UpdateHearts();
    }

    private void UpdateHearts() {
        if (current < max || showHeartsOnFullHealth) {
            for (int i = 0; i < hearts.Count; i++) {
                hearts[i].gameObject.SetActive(current >= i + 1);
                hearts[i].localPosition = new Vector3(i - current * 0.5f + 0.5f, 0, 0);
            }
        } else {
            hearts.ForEach(h => h.gameObject.SetActive(false));
        }
    }
}
