using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._ProjectCallosum.Scripts.Core
{
    public static class UniversePhysics
    {
        //// Constantes (Hardcoded por enquanto, variáveis no futuro)
        //public static float GravitationalConstant = 1.0f;
        //public static float CoulombConstant = 10.0f;
        //public static float StrongForceConstant = 10.0f; //100.0f;
        //public static float LightSpeed = 50.0f;


        //// Gravidade desligada na escala atômica
        //public static float GravitationalConstant = 0.0f;

        //// Força Elétrica ajustada para a escala de MeV
        //public static float CoulombConstant = 50.0f;

        //// Força Forte poderosa para segurar o núcleo
        //public static float StrongForceConstant = 500.0f;

        //// Velocidade da luz alta para permitir órbitas rápidas
        //public static float LightSpeed = 100.0f;



        // --- VALORES PADRÃO (CONSTANTES IMUTÁVEIS) ---
        // Guardamos aqui os valores que sabemos que funcionam (o "Backup")
        public const float DEFAULT_GRAVITY = 0.0f;
        public const float DEFAULT_COULOMB = 50.0f; //1000.0f;
        public const float DEFAULT_STRONG = 500.0f; //10000.0f;
        public const float DEFAULT_LIGHTSPEED = 100.0f;

        // --- VALORES DINÂMICOS (VARIÁVEIS) ---
        // Estes são os que vamos alterar com os Sliders
        public static float GravitationalConstant = DEFAULT_GRAVITY;
        public static float CoulombConstant = DEFAULT_COULOMB;
        public static float StrongForceConstant = DEFAULT_STRONG;
        public static float LightSpeed = DEFAULT_LIGHTSPEED;
        // --- NOVO: Fator de ódio entre elétrons ---
        // Aumente isso para expulsar elétrons extras das camadas
        public static float ElectronRepulsionFactor = 10.0f;


        // --- CONSTANTES DE PARTÍCULAS E SIMULAÇÃO ---
        public const float ElectronMass = 9.109e-31f;      // Massa real do elétron
        public const float ElectronLayerStep = 2.0f;       // Distância visual entre camadas na Unity
        public const float VirtualNucleusChargeScale = 1000.0f; // Multiplicador para estabilidade orbital



        // Método para resetar tudo de uma vez
        public static void ResetToDefaults()
        {
            GravitationalConstant = DEFAULT_GRAVITY;
            CoulombConstant = DEFAULT_COULOMB;
            StrongForceConstant = DEFAULT_STRONG;
            LightSpeed = DEFAULT_LIGHTSPEED;

            ElectronRepulsionFactor = 10.0f;

        }




    }
}
