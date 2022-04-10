using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class Marker : MonoBehaviour, IMarkable
    {
        public Mark mark;

        public bool Mentiones(Mark mark)
        {
            return this.mark == mark;
        }

        public void Awake() {
            mark.target = this;
        }
    }

    public static class MarkerExtensions
    {
        public static bool Marked(this Component component, Mark mark)
        {
            return Marked(component, mark.Single());
        }

        public static bool Marked(this Component component, IEnumerable<Mark> marks) {
            return component.GetComponents<Marker>().Any(marker => marks.Contains(marker.mark));
        }

        public static bool Mentioned(this Component component, Mark mark)
        {
            return Mentioned(component, mark.Single());
        }

        public static bool Mentioned(this Component component, IEnumerable<Mark> marks)
        {
            return component.GetComponents<IMarkable>().Any(markable => marks.Any(mark => markable.Mentiones(mark)));
        }

        public static Marker FindMark(this Component component, Mark mark)
        {
            return component.GetComponentsInChildren<Marker>().FirstOrDefault(marker => marker.mark == mark);
        }
    }
}