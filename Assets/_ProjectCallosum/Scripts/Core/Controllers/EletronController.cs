using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Core.Controllers
{
    public class ElectronController : MonoBehaviour
    {
        [Header("UI Local")]
        // Arraste o 'Panel_Inspector' que está dentro DESTE prefab para aqui
        [SerializeField] private GameObject myInspectorPanel;

        private void Start()
        {
            // Garante que o painel comece desligado ao instanciar
            if (myInspectorPanel != null)
                myInspectorPanel.SetActive(false);
        }

        // Método público para ser chamado pelo SystemManager ou Click
        public void ToggleInspector(bool isOpen)
        {
            if (myInspectorPanel != null)
            {
                myInspectorPanel.SetActive(isOpen);
            }
        }
    }
}
