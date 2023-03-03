using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class DangerSprite : MonoBehaviour
{
    public SpriteRenderer sprite;

    public void Awake() {
        sprite.enabled = false;
    }
}
