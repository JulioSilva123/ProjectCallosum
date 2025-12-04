using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public class DynamicAtomCamera : MonoBehaviour
    {
        [Header("Configurações de Zoom")]
        public float minDistance = 2.0f;  // Zoom máximo (perto)
        public float maxDistance = 100.0f; // Zoom mínimo (longe)
        public float scrollSensitivity = 5.0f; // Velocidade do zoom
        public float smoothSpeed = 5.0f;    // Suavidade do movimento

        [Header("Alvo")]
        public Vector3 offset = new Vector3(0, 0, -10); // Posição padrão da câmera

        private float _currentZoomLevel = 10.0f; // Distância atual

        void Start()
        {
            // Inicia com a distância que estiver na cena
            _currentZoomLevel = Mathf.Abs(transform.position.z);
        }

        void Update()
        {
            float scrollInput = 0.0f;

            // 1. Tenta ler a Rodinha do Mouse
            scrollInput = Input.mouseScrollDelta.y;

            // 2. Tenta ler o Teclado (Se o mouse não mexeu)
            // Tecla 'E' ou '+' para Aproximar
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.KeypadPlus))
            {
                scrollInput = 1.0f * Time.deltaTime * 5.0f; // Multiplicador para ficar suave
            }
            // Tecla 'Q' ou '-' para Afastar
            else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.KeypadMinus))
            {
                scrollInput = -1.0f * Time.deltaTime * 5.0f;
            }

            // 3. Aplica o Zoom
            if (scrollInput != 0.0f)
            {
                // Negativo aproxima, Positivo afasta (ou vice-versa dependendo da lógica)
                _currentZoomLevel -= scrollInput * scrollSensitivity;

                // Trava nos limites
                _currentZoomLevel = Mathf.Clamp(_currentZoomLevel, minDistance, maxDistance);
            }
        }

        void LateUpdate()
        {
            // Se não tem nada na tela, não faz nada
            if (QuantumEntity.AllEntities.Count == 0) return;

            // 2. ENCONTRAR O CENTRO DO ÁTOMO
            // Calcula a média das posições de todas as partículas vivas
            // Isso garante que a câmera foque no grupo, não só no próton
            var bounds = new Bounds(QuantumEntity.AllEntities[0].Position, Vector3.zero);
            foreach (var p in QuantumEntity.AllEntities)
            {
                bounds.Encapsulate(p.Position);
            }

            Vector3 centerPoint = bounds.center;

            // 3. CALCULAR POSIÇÃO FINAL
            // O alvo é o centro do átomo, afastado para trás (eixo Z) pelo valor do Zoom
            Vector3 targetPosition = centerPoint - new Vector3(0, 0, _currentZoomLevel);

            // 4. MOVER SUAVEMENTE (LERP)
            // A câmera desliza até a nova posição em vez de teleportar
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
    }
}
