using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public class CameraFollow : MonoBehaviour
    {
        public float smoothSpeed = 0.125f;
        public Vector3 offset = new Vector3(0, 0, -5); // Distância da câmera

        void LateUpdate()
        {
            // Se não tem partículas, não faz nada
            if (QuantumEntity.AllEntities.Count == 0) return;

            // Calcula o "Centro de Massa" de todas as partículas
            Vector3 centerPoint = Vector3.zero;
            foreach (var p in QuantumEntity.AllEntities)
            {
                centerPoint += p.Position;
            }
            centerPoint /= QuantumEntity.AllEntities.Count;

            // Move a câmera suavemente para esse centro
            Vector3 desiredPosition = centerPoint + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }
}
