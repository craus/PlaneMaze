using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Invisibility : MonoBehaviour
{
    [SerializeField] private GameObject model;

    public void Switch(bool on) {
        model.SetActive(!on);
    }

    public void Check() {
        Switch(CalculateInvisibility());
    }

    private bool CalculateInvisibility() {
        var fog = GetComponent<Figure>().Location.GetFigure<Fog>();
        return
            fog != null &&
            fog.On &&
            !Player.instance.figure.Location.Neighbours().Contains(GetComponent<Figure>().Location);
    }
}
