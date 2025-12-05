using Assets._ProjectCallosum.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Matter
{
    public class ElectricSparker : MonoBehaviour
    {
        private QuantumEntity _me;
        private ParticleSystem _myCannon;
        private ElementaryParticle _myIdentity; // --- NOVO: Para saber quem eu sou

        [Header("Configuração do Flash")]
        public GameObject photonFlashPrefab;
        public float sparkThreshold = 300.0f;
        public float rechargeTime = 0.2f;

        private float _lastFireTime;

        void Start()
        {
            _me = GetComponent<QuantumEntity>();
            _myIdentity = GetComponent<ElementaryParticle>(); // --- PEGA A IDENTIDADE

            if (photonFlashPrefab != null)
            {
                GameObject cannonObj = Instantiate(photonFlashPrefab, transform.position, Quaternion.identity, transform);
                _myCannon = cannonObj.GetComponent<ParticleSystem>();
            }
        }

        void LateUpdate()
        {

           // Debug.Log($"LateUpdate Força no Elétron");

            if (_myCannon == null || Time.time < _lastFireTime + rechargeTime) return;

            float maxForce = 0f;
            Vector3 bestTargetPos = Vector3.zero;

            foreach (var other in QuantumEntity.AllEntities)
            {
                if (other == _me) continue;

                // --- FILTRO DE LIMPEZA VISUAL ---
                // Se EU sou Quark E o vizinho é Quark -> Não solta faísca elétrica.
                // Deixe que o GluonConnector desenhe a linha.
                if (_myIdentity != null && _myIdentity.Family == ParticleFamily.Quark)
                {
                    var otherIdentity = other as ElementaryParticle;
                    if (otherIdentity != null && otherIdentity.Family == ParticleFamily.Quark)
                    {
                        continue; // Pula este vizinho
                    }
                }
                // ---------------------------------

                float distSq = (other.Position - _me.Position).sqrMagnitude;
                if (distSq < 0.01f) distSq = 0.01f;

                float force = Mathf.Abs(UniversePhysics.CoulombConstant * (float)(_me.ElectricCharge * other.ElectricCharge) / distSq);


                // --- LINHA DE ESPIÃO ---
                // Se a força for maior que 1 (pra não spamar zeros), mostre no console.
                if (force > 1.0f && _me.Name.Contains("Electron"))
                {
                    //Debug.Log($"Força no Elétron: {force:F2}");
                }
                //Debug.Log($"Força no Elétron: {force:F2}");

                if (force > maxForce)
                {
                    maxForce = force;
                    bestTargetPos = other.Position;
                }
            }

            // DEBUG TEMPORÁRIO (Para calibrar)
            // Se a força for maior que 1, mostre no console para sabermos o valor
            if (maxForce > 1.0f)
            {
                //Debug.Log($"Força Elétrica Atual: {maxForce}");
            }

            if (maxForce > sparkThreshold)
            {
                FirePhoton(bestTargetPos);
                _lastFireTime = Time.time;
            }
        }

        void FirePhoton(Vector3 targetPos)
        {
            _myCannon.transform.LookAt(targetPos);
            _myCannon.Play();
        }
    }
}
