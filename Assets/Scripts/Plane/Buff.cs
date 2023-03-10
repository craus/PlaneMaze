using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Buff : MonoBehaviour
{
    public GameObject icon;

    [SerializeField] private int current = 0;

    public int Current => current;

    public async Task Gain(int amount) {
        current = Mathf.Clamp(current + amount, 0, 999);
        UpdateIcons();
    }

    public async Task Spend(int amount) {
        current = Mathf.Clamp(current - amount, 0, 999);
        UpdateIcons();
    }

    public void Awake() {
        UpdateIcons();
    }

    public virtual bool Active => current > 0;

    public virtual void UpdateIcons() {
        icon.SetActive(Active);
    }
}
