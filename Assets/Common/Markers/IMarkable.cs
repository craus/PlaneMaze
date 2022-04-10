using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public interface IMarkable
    {
        bool Mentiones(Mark mark);
    }
}