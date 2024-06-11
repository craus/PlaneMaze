using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Invisibility : MonoBehaviour
{
    [SerializeField] private GameObject model;

    private Invulnerability invulnerability;

    public bool Visible => model.activeSelf;

    private bool insideFog = false;
    public bool InsideFog {
        set {
            if (insideFog != value) {
                insideFog = value;
                Check();
            }
        }
    }

    public void Switch(bool on) {
        if (model.activeSelf == on) {
            model.SetActive(!on); // CHECK: slow?
        }
        UpdateInvulnerabilityIcons();
    }

    private void UpdateInvulnerabilityIcons() {
        if (model.activeSelf && invulnerability != null) {
            invulnerability.UpdateIcons();
        }
    }

    public void Check() { // CHECK: slow?
        Switch(CalculateInvisibility());
    }

    public void Awake() {
        GetComponent<Figure>().afterMove.Add(async (from, to) => Check());
        GetComponents<IInvisibilitySource>().ForEach(iis => iis.OnChange += Check);

        new ValueTracker<bool>(() => model.activeSelf, v => { model.SetActive(v); UpdateInvulnerabilityIcons(); });
        new ValueTracker<bool>(() => insideFog, v => insideFog = v);

        invulnerability = GetComponent<Invulnerability>();
    }

    public void Start() {
        Check();
    }

    private bool HiddenInsideFog() { // CHECK: slow?
        return 
            insideFog &&
            (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).SumDelta() > 1;
    }

    private bool FarFromPlayer() => 
        (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).MaxDelta() >= 2;

    private bool HiddenOutsideFog() => PlayerInsideFog() && FarFromPlayer();

    private bool PlayerInsideFog() => Player.insideFog;

    private bool CalculateInvisibility() {
        if (Player.trueSight) return false;

        if (HiddenInsideFog()) return true;
        if (HiddenOutsideFog()) return true;
        if (GetComponents<IInvisibilitySource>().Any(iis => iis.Invisible)) return true;
        return false;
    }
}
