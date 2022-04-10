using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConstantStringProvider : StringValueProvider
{
    [SerializeField] [TextArea] private string m_value;
    public override string Value => m_value;
}
