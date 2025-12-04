using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Data
{
    [CreateAssetMenu(fileName = "NewElement", menuName = "Callosum/Element Data")]
    public class ElementData : ScriptableObject
    {
        public string Symbol;
        public string ElementName;
        public int AtomicNumber;
        public float AtomicMass;
        public int[] ShellConfiguration;
        public Color CPKColor;
    }
}
