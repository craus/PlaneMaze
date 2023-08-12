using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class SpriteDirection : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> leftSprites;
    [SerializeField] private List<SpriteRenderer> rightSprites;

    public void SetDirection(Vector2Int direction) {
        leftSprites.ForEach(sr => sr.flipX = direction.x > 0);
        rightSprites.ForEach(sr => sr.flipX = direction.x < 0);
    }
}
