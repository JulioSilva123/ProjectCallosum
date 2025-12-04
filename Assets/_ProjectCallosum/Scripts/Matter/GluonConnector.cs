using Assets._ProjectCallosum.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Matter
{
    [RequireComponent(typeof(LineRenderer))]
    public class GluonConnector : MonoBehaviour
    {
        private QuantumEntity _me;
        private LineRenderer _line;
        private ElementaryParticle _myParticleInfo;

        [Header("Configuração do Glúon")]
        public float connectionRange = 1.5f;
        public float flowSpeed = 5.0f; // Velocidade da animação visual

        void Start()
        {
            _me = GetComponent<QuantumEntity>();
            _line = GetComponent<LineRenderer>();
            _myParticleInfo = GetComponent<ElementaryParticle>();

            if (_myParticleInfo == null || _myParticleInfo.Family != ParticleFamily.Quark)
            {
                _line.enabled = false;
                this.enabled = false;
            }
        }

        void LateUpdate()
        {
            // Se o LineRenderer foi removido ou desligado, paramos.
            if (_line == null || !_line.enabled) return;

            // 1. Procurar o parceiro mais próximo
            QuantumEntity target = FindStrongestConnection();

            if (target != null)
            {
                // --- ATUALIZAÇÃO VISUAL ---

                // Define os pontos da linha (De mim -> Para o alvo)
                _line.positionCount = 2;
                _line.SetPosition(0, transform.position);
                _line.SetPosition(1, target.Position);

                // Calcula a tensão (quanto mais longe, mais tenso)
                float distance = Vector3.Distance(transform.position, target.Position);
                float stress = Mathf.Clamp01(distance / connectionRange);

                // Engrossa a linha conforme a tensão aumenta (Confinamento de Cor)
                float width = Mathf.Lerp(0.15f, 0.5f, stress);
                _line.startWidth = width;
                _line.endWidth = width;

                // Animação de Textura (Fluxo correndo)
                Material mat = _line.material;
                if (mat != null)
                {
                    // Move a textura no eixo X baseada no tempo
                    float offset = Time.time * flowSpeed;
                    mat.mainTextureOffset = new Vector2(offset, 0);

                    // Muda a cor com a tensão: Amarelo (Relaxado) -> Vermelho (Tenso)
                    Color baseColor = Color.Lerp(Color.yellow, Color.red, stress);
                    baseColor.a = 0.5f; // Mantém transparência em 50%
                    _line.startColor = baseColor;
                    _line.endColor = baseColor;
                }

                // --- ATUALIZAÇÃO FÍSICA (A ORIGEM DA MASSA) ---

                // Calcula a Energia Potencial armazenada no tubo de glúons
                // Fórmula Linear de QCD: Energia = Força * Distância
                float strongForce = UniversePhysics.StrongForceConstant;
                double potentialEnergy = strongForce * distance;

                // Como E = m (em unidades naturais), essa energia vira massa.
                // Dividimos por 2 porque o tubo é compartilhado entre dois quarks.
                double massFromGluon = potentialEnergy * 0.5f;

                // Injeta essa massa na propriedade dinâmica da Entidade
                // Isso fará o quark ficar "pesado" e difícil de mover, estabilizando o núcleo.
                _me.GluonEnergyMass = massFromGluon;

                // --- LINHA DE TESTE (Adicione isso) ---
                // Se o valor for maior que 1, avisa no console
                if (massFromGluon > 1.0)
                {
                    // Comente esta linha depois de ver que funcionou!
                     Debug.Log($"[Gluon] Gerando Massa: {massFromGluon:F1} MeV para {_me.Name}");
                }


            }
            else
            {
                // Se não tem conexão, esconde a linha e zera a massa extra
                _line.positionCount = 0;
                _me.GluonEnergyMass = 0;
            }
        }

        private QuantumEntity FindStrongestConnection()
        {
            QuantumEntity bestTarget = null;
            float closestDist = connectionRange;

            foreach (var other in QuantumEntity.AllEntities)
            {
                if (other == _me) continue;
                var p = other as ElementaryParticle;
                // Só conecta Quark com Quark
                if (p == null || p.Family != ParticleFamily.Quark) continue;

                float dist = Vector3.Distance(transform.position, other.Position);
                if (dist < closestDist && dist > 0.1f)
                {
                    closestDist = dist;
                    bestTarget = other;
                }
            }
            return bestTarget;
        }
    }
}
