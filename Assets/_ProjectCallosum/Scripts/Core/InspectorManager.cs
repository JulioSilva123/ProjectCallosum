using Assets._ProjectCallosum.Scripts.Matter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public class InspectorManager : MonoBehaviour
    {
        public static InspectorManager Instance; // Singleton para acesso fácil

        [Header("UI Components")]
        public GameObject panelObj; // O próprio objeto Panel_Inspector
        public TextMeshProUGUI nameText; // Referência ao texto do nome
        public TextMeshProUGUI infoText; // Referência ao texto de detalhes (massa, carga, etc)

        void Awake()
        {
            // Configuração do Singleton
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Começa com o painel escondido
            HideInspector();
        }

        // Função chamada pelo Elétron
        public void ShowElectronInfo(string nome, string detalhes)
        {
            panelObj.SetActive(true); // Mostra o painel
            nameText.text = nome;
            infoText.text = detalhes;
        }

        public void HideInspector()
        {
            panelObj.SetActive(false);
        }
    }
}
