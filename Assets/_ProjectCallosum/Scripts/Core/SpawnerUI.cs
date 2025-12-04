using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public class SpawnerUI : MonoBehaviour
    {
        [Header("Conexão com o Spawner")]
        public QuantumSpawner spawnerScript; // Arraste o GodHand_Spawner aqui


        [Header("Controle da Janela")] // --- NOVO
        public GameObject panelVisuals; // O objeto "Panel_Creator" inteiro
        public Button btnToggleMenu;    // O botão pequeno que abre/fecha
        private bool _isPanelOpen = true;


        [Header("Botões da Interface")]
        public Button btnProton;
        public Button btnNeutron;
        public Button btnHydrogen;
        public Button btnHydrogenMol; // H2
        public Button btnHelium;
        public Button btnLithium;
        public Button btnBeryllium;

        void Start()
        {
            // Verifica se o spawner está conectado
            if (spawnerScript == null)
            {
                Debug.LogError("SpawnerUI: Falta conectar o QuantumSpawner!");
                return;
            }


            if (spawnerScript == null) return;

            // --- Lógica do Menu Retrátil ---
            if (btnToggleMenu != null)
            {
                btnToggleMenu.onClick.AddListener(ToggleMenu);
            }

            // Conecta cada botão à sua função no Spawner
            // Usamos expressões lambda () => para chamar os métodos

            if (btnProton) btnProton.onClick.AddListener(() => spawnerScript.SpawnProton());
            if (btnNeutron) btnNeutron.onClick.AddListener(() => spawnerScript.SpawnNeutron());

            if (btnHydrogen) btnHydrogen.onClick.AddListener(() => spawnerScript.SpawnHydrogen());
            if (btnHydrogenMol) btnHydrogenMol.onClick.AddListener(() => spawnerScript.SpawnHydrogenMolecule());

            if (btnHelium) btnHelium.onClick.AddListener(() => spawnerScript.SpawnHeliumAtom());
            if (btnLithium) btnLithium.onClick.AddListener(() => spawnerScript.SpawnLithiumAtom());

            // Se você criou o Berílio no spawner, descomente abaixo:
            if (btnBeryllium) btnBeryllium.onClick.AddListener(() => spawnerScript.SpawnBerylliumAtom());
        }


        // Método que liga/desliga o painel
        void ToggleMenu()
        {
            _isPanelOpen = !_isPanelOpen;

            if (panelVisuals != null)
            {
                panelVisuals.SetActive(_isPanelOpen);
            }

            // Opcional: Mudar a cor ou texto do botão para indicar estado
            Text btnText = btnToggleMenu.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = _isPanelOpen ? "FECHAR MENU" : "CRIAR ELEMENTOS";
            }
        }

    }
}
