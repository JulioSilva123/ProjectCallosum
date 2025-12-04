using Assets._ProjectCallosum.Scripts.Matter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Data
{
    [CreateAssetMenu(fileName = "NewParticle", menuName = "Callosum/Particle Definition")]
    public class ParticleDefinition : ScriptableObject
    {
        public string ParticleName;
        public ParticleFamily Family;
        public ParticleFlavor Flavor;
        public float MassMeV;
        public float Charge;
        public GameObject VisualPrefab;
    }
}
