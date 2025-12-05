using Assets._ProjectCallosum.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public class GodPanel : MonoBehaviour
    {
        //[Header("Componentes da UI")]
        //public Slider sliderCoulomb;
        //public Text textCoulomb;

        //public Slider sliderStrong;
        //public Text textStrong;

        //public Slider sliderTime; // Controla a velocidade da luz (Câmera Lenta)
        //public Text textTime;

        //public Button btnReset;
        //public Button btnClear; // --- NOVO: Botão de Limpar
        [Header("Visual (HDR)")] // --- NOVO
        public Slider sliderGlow;
        public Text textGlow;
        public Material[] neonMaterials; // Lista de materiais para controlar

        // Memória para guardar as cores originais (para não perder a referência)
        private Dictionary<Material, Color> _originalColors = new Dictionary<Material, Color>();



        [Header("Controle Mestre")] // --- NOVO
        public Slider sliderMaster;
        public Text textMaster;

        [Header("Física (Base)")]
        public Slider sliderCoulomb;
        public Text textCoulomb;
        public Slider sliderStrong;
        public Text textStrong;

        [Header("Tempo")]
        public Slider sliderTime;
        public Text textTime;

        [Header("Controles")]
        public Button btnReset;
        public Button btnClear;
        public Toggle toggleFullscreen;


        //public Toggle toggleFullscreen;




        // Inicialização
        void Start()
        {



            // --- NOVO: Configurar Slider de Brilho ---
            sliderGlow.minValue = 1.0f; // Brilho Normal
            sliderGlow.maxValue = 5.0f; // Super Neon (5x)
            sliderGlow.value = 1.0f;

            // Salva as cores originais de cada material na memória
            foreach (var mat in neonMaterials)
            {
                if (mat != null && !_originalColors.ContainsKey(mat))
                {
                    // Se o shader tiver a propriedade "_Color" ou "_TintColor"
                    if (mat.HasProperty("_Color"))
                        _originalColors.Add(mat, mat.color);
                    else if (mat.HasProperty("_TintColor")) // Para alguns shaders de partícula antigos
                        _originalColors.Add(mat, mat.GetColor("_TintColor"));
                }
            }





            // --- CONFIGURAÇÃO DO MASTER ---
            sliderMaster.minValue = 0.1f; // 10% da força
            sliderMaster.maxValue = 3.0f; // 300% da força (Turbo)
            sliderMaster.value = 1.0f;    // Começa normal (100%)


            // 1. Configurar os valores iniciais dos sliders para bater com a física atual
            sliderCoulomb.value = UniversePhysics.CoulombConstant;
            sliderStrong.value = UniversePhysics.StrongForceConstant;
            sliderTime.value = UniversePhysics.LightSpeed;

            // 2. Configurar os limites (Min e Max) dos Sliders
            sliderCoulomb.minValue = 0;
            sliderCoulomb.maxValue = 5000; // Permite força 5x maior
            sliderCoulomb.value = UniversePhysics.DEFAULT_COULOMB;

            sliderStrong.minValue = 0;
            sliderStrong.maxValue = 137000;
            sliderStrong.value = UniversePhysics.DEFAULT_STRONG;

            sliderTime.minValue = 10;
            sliderTime.maxValue = 500;
            sliderTime.value = UniversePhysics.DEFAULT_LIGHTSPEED;

            // 3. Conectar o Botão de Reset
            btnReset.onClick.AddListener(ResetPhysics);


            // --- NOVO: Conecta o clique do botão à função de limpar
            if (btnClear != null) btnClear.onClick.AddListener(ClearAllParticles);


            //toggleFullscreen.isOn = Screen.fullScreen;
            //toggleFullscreen.onValueChanged.AddListener(SetFullscreen);


            UpdateLabels();
        }

        // Loop principal: Verifica se o usuário mexeu nos sliders
        void Update()
        {
            //// Aplica o valor do Slider na Física
            //UniversePhysics.CoulombConstant = sliderCoulomb.value;
            //UniversePhysics.StrongForceConstant = sliderStrong.value;
            //UniversePhysics.LightSpeed = sliderTime.value;




            // --- A MÁGICA DO EQUILÍBRIO ---
            // O valor que vai para a física é: (O que está no slider) X (O Multiplicador Master)
            float masterMult = sliderMaster.value;

            UniversePhysics.CoulombConstant = sliderCoulomb.value * masterMult;
            UniversePhysics.StrongForceConstant = sliderStrong.value * masterMult;

            // A velocidade da luz não deve ser multiplicada pela força, 
            // ela é uma constante de tempo independente.
            UniversePhysics.LightSpeed = sliderTime.value;

            // --- NOVO: Atualiza o Brilho em Tempo Real ---
            UpdateGlow(sliderGlow.value);

            UpdateLabels();
        }

        void UpdateGlow(float intensity)
        {
            foreach (var mat in neonMaterials)
            {
                if (mat != null && _originalColors.ContainsKey(mat))
                {
                    Color baseColor = _originalColors[mat];

                    // A Mágica do HDR: Multiplicamos a cor pelo número
                    // (1.0, 0, 0) * 3 = (3.0, 0, 0) -> Vermelho Super Brilhante
                    Color hdrColor = baseColor * intensity;

                    // Mantemos o Alpha original (transparência) para não estragar o glúon
                    hdrColor.a = baseColor.a;

                    if (mat.HasProperty("_Color")) mat.color = hdrColor;
                    else if (mat.HasProperty("_TintColor")) mat.SetColor("_TintColor", hdrColor);
                }
            }
        }



        // --- NOVO: O Método Destruidor ---
        void ClearAllParticles()
        {
            // Precisamos criar uma CÓPIA da lista para percorrer.
            // Se usarmos a lista original, ela muda de tamanho enquanto deletamos
            // e gera um erro "Collection was modified".
            var allParticles = QuantumEntity.AllEntities.ToArray();

            foreach (var particle in allParticles)
            {
                if (particle != null)
                {
                    Destroy(particle.gameObject);
                }
            }


            // AGORA (Mais limpo e colorido):
            CallosumUtils.Log($"Universo limpo com sucesso!");
        }


        void UpdateLabels()
        {

            // --- NOVO: Texto do Brilho ---
            textGlow.text = $"Intensidade Visual: <color=#00FFFF>{sliderGlow.value:F1}x</color>";      



            float masterMult = sliderMaster.value;


            // --- CÁLCULO DA PROPORÇÃO REAL (A VERDADE CIENTÍFICA) ---

            float currentStrong = UniversePhysics.StrongForceConstant; // Ex: 10.000
            float currentCoulomb = UniversePhysics.CoulombConstant;    // Ex: 1.000

            // Na natureza, a razão Strong/Coulomb é aprox 137
            float naturalRatio = 137.0f;

            // Se fossemos realistas, quanto deveria ser o Coulomb?
            float realisticCoulomb = currentStrong / naturalRatio; // 10000 / 137 = 72.9

            // Quantas vezes estamos "exagerando" a força elétrica?
            // Se realistic é 73 e usamos 1000 -> 1000 / 73 = 13.7x
            float amplification = 0;
            if (realisticCoulomb > 0) amplification = currentCoulomb / realisticCoulomb;

            // --- ATUALIZAÇÃO DOS TEXTOS ---

            //// 1. Eletricidade (Mostra o exagero)
            //textCoulomb.text = $"Eletricidade (k): {currentCoulomb:F0} <size=20>N·m²/C²</size>\n" +
            //                   $"<size=30><color=#FF5555>(Amplificada {amplification:F1}x)</color></size>";

            // --- ELETRICIDADE ---
            // Mostramos: Valor Base (Slider) -> Valor Final (Aplicado)
            float finalCoulomb = UniversePhysics.CoulombConstant;
            textCoulomb.text = $"Eletricidade Base: {sliderCoulomb.value:F0}\n" +
                               $"<size=30><color=#AAAAAA>Aplicado: {amplification:F1} N·m²/C²</color></size>";




            //// Atualiza o texto na tela para mostrar o número exato
            //textCoulomb.text = $"Eletricidade: {UniversePhysics.CoulombConstant:F0}";
            //textStrong.text = $"Força Forte: {UniversePhysics.StrongForceConstant:F0}";
            //textTime.text = $"Vel. Luz: {UniversePhysics.LightSpeed:F0}";

            // --- ELETROMAGNETISMO (Constante de Coulomb - k) ---
            // N·m²/C² é a unidade real, mas fica muito grande na tela. 
            // Vamos usar uma notação científica simplificada.
            // textCoulomb.text = $"Eletricidade (k): {UniversePhysics.CoulombConstant:F0} <size=20>N·m²/C²</size>";

            // --- FORÇA FORTE (Energia Nuclear) ---
            // Em física de partículas, força é Energia / Distância (MeV/fm).
            //textStrong.text = $"Força Forte: {UniversePhysics.StrongForceConstant:F0} <size=20>MeV/fm</size>";





            // 2. FORÇA FORTE (Com comparativo Real)
            //float currentStrong = UniversePhysics.StrongForceConstant;

            // A Tensão da Corda real é aprox 1 GeV/fm = 1000 MeV/fm
            float realReference = 1000.0f;
            float multiplier = currentStrong / realReference;

            // Exibição: Mostra o valor do jogo e, embaixo, a proporção
            // Usamos cor amarela (#FFD700) para destacar a nota científica
            textStrong.text = $"Força Forte: {currentStrong:F0} <size=20>MeV/fm</size>\n" +
                              $"<size=30><color=#FFD700>(~{multiplier:F1}x Força Real)</color></size>";











            // --- VELOCIDADE DA LUZ (c) ---
            // Mostramos em m/s simulados.
            //textTime.text = $"Vel. Luz (c): {UniversePhysics.LightSpeed:F0} <size=20>m/s</size>";

            // --- VELOCIDADE DA LUZ (Com conversão para Km/h) ---
            float c = UniversePhysics.LightSpeed;

            // Conversão: Se 1 unidade = 1 metro, então c = m/s.
            // Para km/h, multiplicamos por 3.6.
            float kmh = c * 3.6f;

            textTime.text = $"Vel. Luz (c): {c:F0} <size=20>m/s</size>\n<size=30><color=#AAAAAA>(~{kmh:F0} km/h)</color></size>";






        }

        void ResetPhysics()
        {
            // Chama o reset da classe estática
            UniversePhysics.ResetToDefaults();

            sliderMaster.value = 1.0f; // Resetar o Master também
            // Atualiza os sliders visuais para voltarem para o meio
            sliderCoulomb.value = UniversePhysics.DEFAULT_COULOMB;
            sliderStrong.value = UniversePhysics.DEFAULT_STRONG;
            sliderTime.value = UniversePhysics.DEFAULT_LIGHTSPEED;
        }
    }
}
