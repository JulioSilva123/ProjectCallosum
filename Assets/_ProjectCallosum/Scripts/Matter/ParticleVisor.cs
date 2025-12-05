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
        [Header("Componentes Visuais (Auto-Detectados)")]
        public TrailRenderer myTrail;
        public Renderer myRenderer;

        [Header("UI (Auto-Detectada)")]
        public GameObject myLabelCanvas;
        public TextMeshProUGUI myLabelText;

        private QuantumEntity _physics;
        private ElementaryParticle _identity;

        void Start()
        {
            // Auto-detecção de componentes visuais
            if (myTrail == null) myTrail = GetComponent<TrailRenderer>();
            if (myRenderer == null) myRenderer = GetComponent<Renderer>();

            // UI
            if (myLabelCanvas == null) myLabelCanvas = GetComponentInChildren<Canvas>(true)?.gameObject;
            if (myLabelText == null) myLabelText = GetComponentInChildren<TextMeshProUGUI>(true);

            ShowLabel(false);
        }



        void LateUpdate()
        {
            // --- CORREÇÃO: BUSCA TARDIA ---
            // Se ainda não achamos a física (porque o Spawner demorou), tenta achar agora.
            if (_physics == null)
            {
                _physics = GetComponent<QuantumEntity>();
                _identity = GetComponent<ElementaryParticle>();
                return; // Não faz nada neste frame, espera o próximo
            }

            // Só atualiza texto se estiver visível
            if (myLabelCanvas != null && myLabelCanvas.activeSelf)
            {
                myLabelCanvas.transform.rotation = Camera.main.transform.rotation;

                if (_physics != null && myLabelText != null)
                {
                    string flavorName = _identity != null ? _identity.Flavor.ToString() : "Particle";

                    // Mostra Massa, Velocidade e Spin
                    string info = $"<color=yellow>{flavorName}</color>\n" +
                                  $"M: {_physics.Mass:F1} | Q: {_physics.ElectricCharge:F2}\n" +
                                  $"Vel: {_physics.Velocity.magnitude:F1}";

                    myLabelText.text = info;
                }
            }
        }
        //void LateUpdate()
        //{
        //    // Só roda a lógica se o texto estiver visível (Otimização)
        //    if (myLabelCanvas != null && myLabelCanvas.activeSelf)
        //    {
        //        // Billboard: Faz o texto girar para olhar sempre para a câmera
        //        myLabelCanvas.transform.rotation = Camera.main.transform.rotation;

        //        // Atualiza os números
        //        if (_physics != null && myLabelText != null)
        //        {
        //            string flavorName = _identity != null ? _identity.Flavor.ToString() : "Particle";

        //            // Formatação bonita: Nome em Amarelo, Valores em Branco
        //            string info = $"<color=yellow>{flavorName}</color>\n" +
        //                          $"Massa: {_physics.Mass:F1}\n" +
        //                          $"Vel: {_physics.Velocity.magnitude:F1}";

        //            myLabelText.text = info;
        //        }
        //    }
        //}



        // --- MÉTODOS DE CONTROLE ---

        public void SetNeon(bool state)
        {
            if (myRenderer != null) myRenderer.enabled = state;
        }

        public void SetTrail(bool state)
        {
            if (myTrail != null)
            {
                myTrail.enabled = state;
                if (!state) myTrail.Clear();
            }
        }

        public void ShowLabel(bool state)
        {
            if (myLabelCanvas != null) myLabelCanvas.SetActive(state);
        }

        
    }
}
