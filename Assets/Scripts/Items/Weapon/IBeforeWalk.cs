using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IBeforeWalk
{
    public Task<bool> BeforeWalk(Vector2Int delta, int priority);
}
