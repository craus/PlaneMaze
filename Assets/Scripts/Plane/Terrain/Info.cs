using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Info : Terrain
{
    public IExplainable explainable;

    public void Awake() {

        //Player.instance.figure.afterMove.Add(AfterPlayerMove);

        GetComponent<Figure>().collide = async (from, figure) => {
            if (figure == null) {
                return;
            }
            var victim = figure.GetComponent<Player>();
            if (victim != null) {
                InfoPanel.instance.Show(explainable, repeatable: true);
            }
        };
    }
}
