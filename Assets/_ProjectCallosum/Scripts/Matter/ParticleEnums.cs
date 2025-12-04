using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._ProjectCallosum.Scripts.Matter
{
    // A "Família" define as regras gerais (Quem sente a força forte? Quem tem cor?)
    public enum ParticleFamily
    {
        Quark,      // Constituintes do núcleo (Sentem Força Forte)
        Lepton,     // Elétrons e Neutrinos (Não sentem Força Forte)
        Boson       // Mensageiros de Força (Fótons, Glúons)
    }

    // O "Sabor" (Flavor) define a identidade exata (Massa e Carga)
    public enum ParticleFlavor
    {
        // --- QUARKS (Matéria Hadrônica) ---
        Up,         // O mais leve (+2/3)
        Down,       // O parceiro do próton (-1/3)
        Charm,      // Exótico pesado (+2/3)
        Strange,    // Exótico estranho (-1/3)
        Top,        // O mais pesado de todos (+2/3)
        Bottom,     // (-1/3)

        // --- LEPTONS (Matéria Leve) ---
        Electron,   // O que orbita (-1)
        Muon,       // Primo gordo do elétron (-1)
        Tau,        // Primo gigante do elétron (-1)

        // Neutrinos (Fantasmas sem carga)
        ElectronNeutrino,
        MuonNeutrino,
        TauNeutrino,

        // --- BOSONS (As Forças) ---
        Photon,     // Luz / Eletromagnetismo
        Gluon,      // Cola Nuclear (Força Forte)
        ZBoson,     // Força Fraca (Neutro)
        WBoson,     // Força Fraca (Carregado)
        Higgs,      // Confere massa

        // --- TEÓRICO (Para sua pesquisa de unificação) ---
        Graviton    // A gravidade quântica (Spin 2)
    }
}
