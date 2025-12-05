using Assets._ProjectCallosum.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Matter
{
    public class Electron : MonoBehaviour
    {
        [Header("Dados do Elétron")]
        public string camada = "K";
        public float energia = -13.6f;
        public float velocidade = 2200f;

        // Referência ao script da UI
        private ElectronInfoPanel uiManager;

        //void Start()
        //{
        //    // Encontra o painel na cena automaticamente (lento, mas seguro para setups simples)
        //    uiManager = FindAnyObjectByType<ElectronInfoPanel>();
        //}

        void Start()
        {
            // O 'true' dentro dos parênteses diz: "Procure inclusive objetos DESATIVADOS"
            uiManager = FindFirstObjectByType<ElectronInfoPanel>(FindObjectsInactive.Include);

            if (uiManager == null)
            {
                Debug.LogError("CUIDADO: Não achei o ElectronInfoPanel nem procurando nos desativados!");
            }
        }


        // Evento nativo da Unity que detecta clique no objeto (precisa de um Collider)
        void OnMouseDown()
        {
            Debug.Log("CLIQUE DETECTADO NO OBJETO: " + gameObject.name); // <--- Adicionei isso

            if (uiManager != null)
            {
                uiManager.DisplayInfo(camada, energia, velocidade);
            }
            else
            {
                Debug.LogError("ERRO: A variável uiManager está VAZIA (null)!"); // <--- E isso
            }
        }
    }
}
