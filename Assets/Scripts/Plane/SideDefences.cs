using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class SideDefences : MonoBehaviour
{
    public List<SideDefence> sideDefences;

    public void Awake() {
        UpdateIcons();
    }

    public void Update() {
        if (GetComponent<Player>() != null) {
            UpdateIcons();
        }
    }

    private void UpdateIcons() {

        if (GetComponent<Player>() != null) {
            foreach (var d in sideDefences) {
                d.icon.SetActive(
                    Inventory.instance.items
                        .SelectAll(item => item.GetComponent<ISideDefence>())
                        .Any(x => x.GivesDefenceFrom(d.direction))
                );
            }
        } else {
            foreach (var d in sideDefences) {
                d.icon.SetActive(false);
            }
        }
    }
}
