using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IAfterFailedWalk
{
    public Task<bool> AfterFailedWalk(Vector2Int delta, int priority);
}
