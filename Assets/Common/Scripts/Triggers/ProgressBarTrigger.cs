using System.Collections;
using System.Collections.Generic;
using Endo;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Endo
{
    public class ProgressBarTrigger : MonoBehaviour
    {
        [SerializeField] private TextMeshPro m_ProgressbarText;
        [SerializeField] private GameObject m_Battons;
        [SerializeField] private bool once;

        public UnityEvent activate;

        [ReadOnly] [SerializeField] private bool triggered;

        public void Run()
        {
                if (!once || !triggered)
                {
                    triggered = true;
                    activate.Invoke();
                }
        }
            

        public void SetTextValue()
        {
            m_ProgressbarText.SetText(m_Battons.GetComponent<TextMeshPro>().text);
        }
    }
}