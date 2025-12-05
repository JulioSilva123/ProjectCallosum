using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._ProjectCallosum
{
    // O "_" no nome da classe ajuda a identificar que é um Manager global
    public class _SystemManager : MonoBehaviour
    {
        public static _SystemManager Instance { get; private set; }

        [Header("Configurações")]
        public GameObject visualElectronPrefab; // Arraste o PREFAB do visual_electron aqui

        // Deixe isso privado, não precisamos arrastar mais
        private GameObject currentPanelInspector;

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); }
        }

        private void Start()
        {
            InitializeCallosum();
        }

        private void InitializeCallosum()
        {
            // 1. Criamos o visual_electron na cena
            if (visualElectronPrefab != null)
            {
                GameObject electron = Instantiate(visualElectronPrefab);

                // 2. AGORA procuramos o painel DENTRO do elétron que acabou de nascer
                // O true no final serve para encontrar mesmo se estiver desativado/hidden
                Transform panelTrans = electron.transform.Find("Canvas/Panel_Inspector");

                if (panelTrans != null)
                {
                    currentPanelInspector = panelTrans.gameObject;
                    currentPanelInspector.SetActive(false); // Garante que comece fechado
                    Debug.Log("Sucesso: Panel_Inspector encontrado e conectado!");
                }
                else
                {
                    // Tenta procurar de forma mais profunda (recursive) caso a estrutura tenha mudado
                    currentPanelInspector = electron.GetComponentInChildren<Canvas>(true).transform.Find("Panel_Inspector")?.gameObject;
                    if (currentPanelInspector == null) Debug.LogError("ERRO: Não achei o Panel_Inspector dentro do visual_electron!");
                }
            }
        }

        // Método para abrir o painel (pode ser chamado por botões)
        public void OpenInspector()
        {
            if (currentPanelInspector != null)
                currentPanelInspector.SetActive(true);
        }
    }
}
