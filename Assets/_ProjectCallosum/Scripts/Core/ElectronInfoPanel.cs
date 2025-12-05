using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public class ElectronInfoPanel : MonoBehaviour
    {
        [Header("Componentes de Texto da UI")]
        public TextMeshProUGUI camadaText;   // Ex: "Camada: K"
        public TextMeshProUGUI energiaText;  // Ex: "Energia: -13.6 eV"
        public TextMeshProUGUI velocidadeText;

        [Header("Configuração")]
        public GameObject visualPanel; // O GameObject do painel para ativar/desativar

        void Start()
        {
            // Começa com o painel escondido
            ClosePanel();
        }

        // Função pública chamada pelo Elétron
        public void DisplayInfo(string camada, float energia, float velocidade)
        {
            visualPanel.SetActive(true); // Mostra o painel

            // Atualiza os textos
            camadaText.text = "Camada: " + camada;
            energiaText.text = $"Energia: {energia} eV";
            velocidadeText.text = $"Velocidade: {velocidade} km/s";
        }

        public void ClosePanel()
        {
            visualPanel.SetActive(false);
        }
    }
}
