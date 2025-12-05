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
        [Header("Dados Base do Elétron")]
        public string camada = "K";
        // A massa agora vem do UniversePhysics, não é mais uma variável solta aqui

        public float energiaBase = -13.6f;

        // Agora a velocidadeBase serve apenas como referência inicial para o cálculo orbital
        public float velocidadeInicialReferencia = 2200f;
        public float potencialEletricoBase = 14.5f;

        [Header("Configurações Visuais")]
        public Color corDoEletron = Color.cyan;

        // ESTADO FÍSICO REAL (Dinâmico)
        private Vector3 velocidadeVetorial; // O vetor velocidade real (Direção e Magnitude)
        private float raioInicial;

        // Componentes
        private SpriteRenderer spriteRenderer;
        private TrailRenderer trailRenderer;
        private ElectronInfoPanel uiManager;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            trailRenderer = GetComponent<TrailRenderer>();
        }

        void Start()
        {
            // 1. Configuração Visual
            if (spriteRenderer) spriteRenderer.color = corDoEletron;
            if (trailRenderer)
            {
                trailRenderer.startColor = corDoEletron;
                trailRenderer.endColor = new Color(corDoEletron.r, corDoEletron.g, corDoEletron.b, 0);
            }

            // 2. Busca da UI
            uiManager = FindFirstObjectByType<ElectronInfoPanel>(FindObjectsInactive.Include);

            // 3. CONFIGURAÇÃO FÍSICA INICIAL (REAL)
            // Define posição inicial baseada na camada
            ConfigurarPosicaoInicial();

            // CALCULAR VELOCIDADE ORBITAL ESTÁVEL
            // Para não cair no núcleo, v = sqrt(Força * raio / massa)
            // F_eletrica = F_centripeta
            // (k * Q * q) / r^2 = (m * v^2) / r
            // v^2 = (k * Q * q) / (m * r)

            float k = UniversePhysics.CoulombConstant;
            float r = transform.localPosition.magnitude;
            float m = UniversePhysics.ElectronMass; // Usando constante centralizada

            // Usamos a constante global de escala do núcleo para manter a consistência
            float cargaProduto = UniversePhysics.VirtualNucleusChargeScale;

            // Velocidade necessária para manter a órbita circular
            float velocidadeNecessaria = Mathf.Sqrt((k * cargaProduto) / (m * r));

            // Define o vetor velocidade inicial TANGENTE à posição (Perpendicular ao raio)
            // Se posição é (x, y), tangente é (-y, x)
            Vector3 direcaoTangente = new Vector3(-transform.localPosition.y, transform.localPosition.x, 0).normalized;

            velocidadeVetorial = direcaoTangente * velocidadeNecessaria;
        }

        void Update()
        {
            // === SIMULAÇÃO FÍSICA REAL (NADA DE TRILHOS) ===

            float dt = Time.deltaTime;

            // 1. Identificar onde é o centro (Núcleo)
            // Se estiver "filho" do átomo, o centro é (0,0,0) local. Se estiver no mundo, é o parent.position.
            Vector3 centro = transform.parent != null ? Vector3.zero : Vector3.zero;
            Vector3 posicaoAtual = transform.localPosition; // Usando local para cálculos relativos ao átomo

            Vector3 direcaoNucleo = (centro - posicaoAtual).normalized;
            float distancia = posicaoAtual.magnitude;

            // Softening para evitar singularidade (divisão por zero se bater no núcleo)
            if (distancia < 0.5f) distancia = 0.5f;

            // 2. Calcular Força Elétrica (Coulomb)
            // F = k * Q1 * Q2 / r^2
            float k = UniversePhysics.CoulombConstant;
            float cargaProduto = UniversePhysics.VirtualNucleusChargeScale; // Usando constante centralizada
            float forcaMagnitude = (k * cargaProduto) / (distancia * distancia);

            Vector3 forcaEletrica = direcaoNucleo * forcaMagnitude;

            // 3. Obter Massa Relativística (Einstein)
            // Quanto mais rápido, mais pesado = mais difícil de curvar (Inércia)
            float massaAtual = GetMassaRelativistica();

            // 4. Segunda Lei de Newton (F = ma -> a = F/m)
            Vector3 aceleracao = forcaEletrica / massaAtual;

            // 5. Integração de Movimento (Euler)
            velocidadeVetorial += aceleracao * dt;

            // Aplicar movimento
            transform.localPosition += velocidadeVetorial * dt;

            // Opcional: Re-alinhar o elétron para olhar para onde vai (visual)
            // if (velocidadeVetorial != Vector3.zero) transform.right = velocidadeVetorial;
        }

        void ConfigurarPosicaoInicial()
        {
            int n = 1;
            switch (camada.ToUpper())
            {
                case "K": n = 1; break;
                case "L": n = 2; break;
                case "M": n = 3; break;
                default: n = 4; break;
            }

            // Define apenas a Posição Inicial. O resto é física.
            float raioBase = UniversePhysics.ElectronLayerStep; // Usando constante centralizada
            raioInicial = n * raioBase;

            // Começa em um ângulo aleatório
            float angulo = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angulo) * raioInicial;
            float y = Mathf.Sin(angulo) * raioInicial;

            transform.localPosition = new Vector3(x, y, 0);

            // Ajusta energia base para display
            if (energiaBase == -13.6f) energiaBase = -13.6f / (n * n);
        }

        void OnMouseDown()
        {
            if (uiManager != null)
            {
                uiManager.SendMessage("SelectElectron", this, SendMessageOptions.DontRequireReceiver);
            }
        }

        // =================================================================
        // GETTERS PARA INSPECTOR (DADOS REAIS DA SIMULAÇÃO)
        // =================================================================

        public float GetVelocidadeAtual()
        {
            // Retorna a magnitude real do vetor velocidade
            return velocidadeVetorial.magnitude;
        }

        public float GetEnergiaAtual()
        {
            // Energia Total = Cinética + Potencial
            // K = 0.5 * m * v^2
            // U = -k * q1 * q2 / r

            float v = GetVelocidadeAtual();
            float r = transform.localPosition.magnitude;
            float m = UniversePhysics.ElectronMass; // Usando constante centralizada
            float k = UniversePhysics.CoulombConstant;
            float cargaProduto = UniversePhysics.VirtualNucleusChargeScale;

            float cinetica = 0.5f * m * v * v;
            // Potencial é negativo (ligado)
            float potencial = -(k * cargaProduto) / r;

            // Retornamos um valor escalado para parecer eV (Elétron-volt) bonito na UI
            return (cinetica + potencial) * 0.01f;
        }

        public float GetPotencialAtual()
        {
            return potencialEletricoBase;
        }

        public float GetMassaRelativistica()
        {
            float v = velocidadeVetorial.magnitude;

            // Usando a velocidade da luz definida no slider global
            // Multiplicador 100.0f ajusta a escala da Unity para a escala "LightSpeed" do slider
            float c = UniversePhysics.LightSpeed;

            // Evita erros matemáticos se v > c (o que não deve acontecer na física real, mas na simulação sim)
            if (c <= 0) c = 1;

            float ratio = Mathf.Clamp(v / c, 0f, 0.99f);
            float gamma = 1.0f / Mathf.Sqrt(1.0f - (ratio * ratio));

            return UniversePhysics.ElectronMass * gamma; // Usando constante centralizada
        }

        public Vector2 GetPosition() => transform.position;
    }
}
