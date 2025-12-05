using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public class ElectronBehavior : MonoBehaviour
    {
        [Header("Dados do Elétron")]
        public string eletronNome = "Elétron Camada K";
        public string eletronDetalhes = "Carga: -1 | Massa: Desprezível";

        // Essa função é chamada automaticamente pela Unity quando clicamos no Collider
        void OnMouseDown()
        {
            // Verifica se não clicou na UI por cima (opcional, mas recomendado)
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            Debug.Log("Cliquei no elétron: " + eletronNome);

            // Chama o Inspector para aparecer com NOSSOS dados
            if (InspectorManager.Instance != null)
            {
                InspectorManager.Instance.ShowElectronInfo(eletronNome, eletronDetalhes);
            }
        }
    }
}
