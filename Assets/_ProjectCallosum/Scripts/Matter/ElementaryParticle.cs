using Assets._ProjectCallosum.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Matter
{
   

    public class ElementaryParticle : QuantumEntity
    {
        [Header("Identity")]
        public ParticleFamily Family;
        public ParticleFlavor Flavor;


        // --- NOVO: SPIN QUÂNTICO ---
        // Valor +1 (Up) ou -1 (Down).
        // Na física real é +1/2 e -1/2, mas usaremos inteiros para facilitar a lógica.
        // --- MUDANÇA AQUI: SPIN COM GATILHO VISUAL ---
        [SerializeField] private int _quantumSpin;

        public int QuantumSpin
        {
            get { return _quantumSpin; }
            set
            {
                _quantumSpin = value;
                UpdateVisuals(); // Mudou o spin? Muda a cor na hora!
            }
        }


        [Header("String Theory")]
        public double VibrationFrequency; // Define a identidade real da partícula

        [field: SerializeField]
        public string ColorCharge { get; set; } // Para Quarks

        public void Initialize(ParticleFamily family, ParticleFlavor flavor, double mass, double charge)
        {
            this.Family = family;
            this.Flavor = flavor;
            this.Mass = mass;
            this.ElectricCharge = charge;
            this.Name = flavor.ToString();

            // Garante que a cor atualize ao nascer
            UpdateVisuals();

        }

        private void UpdateVisuals()
        {


            //Debug.Log($"Flavor do Elétron: {Flavor}");


            //if (Flavor != ParticleFlavor.Electron) return;
            if (Family != ParticleFamily.Lepton) return;

            Color targetColor = Color.white;


            //Debug.Log($"quantumSpin do Elétron: {_quantumSpin}");


            // SUAS CORES
            if (_quantumSpin > 0) targetColor = new Color32(191, 177, 3, 255); // Dourado (+1)
            else if (_quantumSpin < 0) targetColor = Color.cyan; // Ciano (-1)
            else return; // Se for 0, não faz nada



            // 1. PINTAR O CORPO (MESH RENDERER)
            var rend = GetComponent<Renderer>();
            if (rend != null)
            {
                // Tenta todas as propriedades possíveis de shader para garantir que funcione
                // Material.color altera o "_Color" (padrão)
                rend.material.color = targetColor;

                if (rend.material.HasProperty("_TintColor"))
                    rend.material.SetColor("_TintColor", targetColor);

                if (rend.material.HasProperty("_BaseColor"))
                    rend.material.SetColor("_BaseColor", targetColor);

                // Se for material Emissivo (Standard), atualiza o brilho também
                if (rend.material.HasProperty("_EmissionColor"))
                    rend.material.SetColor("_EmissionColor", targetColor * 3.0f); // *3 para brilhar muito
            }

            // 2. PINTAR O RASTRO (TRAIL RENDERER)
            var trail = GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.startColor = targetColor;
                trail.endColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
            }
        }


        //public override void UpdateSimulation(float deltaTime)
        //{
        //    base.UpdateSimulation(deltaTime);

        //    // Aqui adicionaremos comportamentos visuais específicos depois
        //    // Ex: Se for Quark, vibrar visualmente
        //}

        public override void UpdateSimulation(float deltaTime)
        {
            base.UpdateSimulation(deltaTime); // Roda a física de atração/repulsão

            // --- NOVO: TREMOR QUÂNTICO (Jitter) ---
            // Adiciona um ruído aleatório na posição para simular energia térmica/quântica
            float jitterAmount = 0.02f;

            // Quarks tremem mais que elétrons (opcional)
            if (Family == ParticleFamily.Quark) jitterAmount = 0.05f;

            transform.position += UnityEngine.Random.insideUnitSphere * jitterAmount;

            UpdateVisuals();

        }


    }
}
