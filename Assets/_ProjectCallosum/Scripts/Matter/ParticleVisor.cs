using Assets._ProjectCallosum.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Matter
{
    public class ParticleVisor : MonoBehaviour
    {
        [Header("Componentes Visuais")]
        public TrailRenderer myTrail;
        public GameObject myLabelCanvas;
        public TextMeshProUGUI myLabelText;
        public Renderer myRenderer; // Para o Neon/Brilho

        private QuantumEntity _physics;
        private ElementaryParticle _identity;

        void Start()
        {
            _physics = GetComponent<QuantumEntity>();
            _identity = GetComponent<ElementaryParticle>();

            // Auto-detecta se não tiver sido arrastado
            if (myTrail == null) myTrail = GetComponent<TrailRenderer>();
            if (myRenderer == null) myRenderer = GetComponent<Renderer>();

            // Procura o Canvas e o Texto que criamos no Passo 1
            if (myLabelCanvas == null) myLabelCanvas = GetComponentInChildren<Canvas>(true)?.gameObject;
            if (myLabelText == null) myLabelText = GetComponentInChildren<TextMeshProUGUI>(true);

            // Garante estado inicial
            ShowLabel(false);
        }

        // --- MÉTODOS DE CONTROLE (Chamados pela UI) ---

        public void SetNeon(bool state)
        {
            // Liga/Desliga o renderizador (se quiser sumir com ela)
            // OU troca o material para um sem brilho. 
            // Por simplicidade, vamos desligar o componente visual inteiro se false
            // Se quiser apenas tirar o brilho, teríamos que trocar o material.
            if (myRenderer != null) myRenderer.enabled = state;
        }

        public void SetTrail(bool state)
        {
            if (myTrail != null)
            {
                myTrail.enabled = state;
                if (!state) myTrail.Clear(); // Limpa o rastro antigo
            }
        }

        public void ShowLabel(bool state)
        {
            if (myLabelCanvas != null) myLabelCanvas.SetActive(state);
        }

        // Atualiza os dados do texto em tempo real (se estiver visível)
        void Update()
        {
            if (myLabelCanvas != null && myLabelCanvas.activeSelf)
            {
                // Faz o texto olhar para a câmera (Billboard) para ler fácil
                myLabelCanvas.transform.rotation = Camera.main.transform.rotation;

                // Atualiza os números
                if (_physics != null)
                {
                    string info = $"<color=yellow>{_identity.Flavor}</color>\n" +
                                  $"Massa: {_physics.Mass:F1}\n" +
                                  $"Vel: {_physics.Velocity.magnitude:F1}";

                    myLabelText.text = info;
                }
            }
        }
    }
}
