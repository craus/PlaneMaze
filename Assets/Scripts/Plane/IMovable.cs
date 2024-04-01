using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IMovable
{
    public async Task BeforeMove() {
    }

    public async Task Move() {
    }

    public void OnGameStart() {
    }
}
