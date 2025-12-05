using Assets._ProjectCallosum.Scripts.Matter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Core
{

    // --- CONTRATO DO CÉREBRO ---
    public interface IQuantumOracle
    {
        QuantumDecision DecideNextState(QuantumEntity entity);
    }

    public struct QuantumDecision
    {
        public Vector3 ProposedMovement;
        public Quaternion ProposedRotation;
        public double EnergyShift;
    }

    // --- A ENTIDADE QUÂNTICA (O Corpo) ---
    public abstract class QuantumEntity : MonoBehaviour
    {
        // 1. LISTA MESTRA (Registro Global)
        public static List<QuantumEntity> AllEntities = new List<QuantumEntity>();

        protected virtual void OnEnable() => AllEntities.Add(this);
        protected virtual void OnDisable() => AllEntities.Remove(this);

        // 2. IDENTIDADE
        public string Id { get; private set; }
        public string Name { get; protected set; }

        // Atalho para posição do Unity
        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Vector3 Velocity { get; set; }
        public double ElectricCharge { get; protected set; }

        // 3. MASSA DINÂMICA (E = mc^2)
        [Header("Configuração de Massa")]
        [Header("Física de Partículas")]
        [SerializeField] protected double _baseMass; // Massa de repouso do arquivo

        // Massa gerada pela tensão dos glúons (calculada pelo GluonConnector)
        public double GluonEnergyMass;

        // Visualização no Inspector (Para Debug)
        [Header("--- MONITOR DE FÍSICA ---")]
        [Tooltip("Massa Total calculada em tempo real")]
        public float View_TotalMass;


        // A Massa que a física usa de verdade
        public double Mass
        {
            get { return _baseMass + GluonEnergyMass; }
            set { _baseMass = value; }
        }

        // 4. CÉREBRO
        protected IQuantumOracle _brain;

        protected virtual void Awake()
        {
            Id = Guid.NewGuid().ToString();
            _brain = new StandardPhysicsOracle();
        }

        // Loop de Física Fixo (50fps) para estabilidade orbital
        protected virtual void FixedUpdate()
        {
            UpdateSimulation(Time.fixedDeltaTime);
        }

        public virtual void UpdateSimulation(float deltaTime)
        {
            // O Cérebro decide a velocidade baseado nas forças
            var decision = _brain.DecideNextState(this);

            this.Velocity = decision.ProposedMovement;
            this.Position += this.Velocity * deltaTime;

            // Atualiza o display de massa no Inspector
            View_TotalMass = (float)Mass;
        }
    }

    // --- O CÉREBRO FÍSICO (A Lógica do Universo) ---
    public class StandardPhysicsOracle : IQuantumOracle
    {
        public QuantumDecision DecideNextState(QuantumEntity me)
        {
            Vector3 totalForce = Vector3.zero;

            // Identificação: Quem sou eu?
            var mePart = me as ElementaryParticle;
            bool amIQuark = (mePart != null && mePart.Family == ParticleFamily.Quark);
            bool amIElectron = (mePart != null && mePart.Flavor == ParticleFlavor.Electron);

            // LOOP DE INTERAÇÃO (N-Corpos)
            foreach (var other in QuantumEntity.AllEntities)
            {
                if (other == me) continue;

                Vector3 direction = me.Position - other.Position;
                float distance = direction.magnitude;

                // Identificação do Vizinho
                var otherPart = other as ElementaryParticle;
                bool isOtherElectron = (otherPart != null && otherPart.Flavor == ParticleFlavor.Electron);
                bool isOtherQuark = (otherPart != null && otherPart.Family == ParticleFamily.Quark);

                // --- 1. AJUSTE DE COLISÃO (SOFTENING) ---
                // Se for Elétron x Elétron, a colisão é "Dura" (0.1) para forçar camadas.
                // Se for Elétron x Núcleo, a colisão é "Suave" (1.0) para orbitar sem cair.
                float softening = (amIElectron && isOtherElectron) ? 0.1f : 1.0f;
                float distanceSquared = (distance * distance) + (softening * softening);

                // --- 2. FORÇA ELÉTRICA (COULOMB) ---
                float electricForce = UniversePhysics.CoulombConstant * (float)(me.ElectricCharge * other.ElectricCharge) / distanceSquared;

                // --- 3. PRINCÍPIO DE EXCLUSÃO DE PAULI ---
                // Se somos dois elétrons com o MESMO SPIN tentando ocupar o mesmo lugar...
                if (amIElectron && isOtherElectron && (mePart.QuantumSpin == otherPart.QuantumSpin))
                {
                    // Raio de Exclusão da Camada (3.0 metros)
                    float exclusionRadius = 3.0f;

                    if (distance < exclusionRadius)
                    {
                        // Repulsão brutal (500x a força elétrica)
                        float overlap = 1.0f - (distance / exclusionRadius);
                        float pauliForce = UniversePhysics.CoulombConstant * 500.0f * overlap * overlap;

                        // Soma na força elétrica (sempre repulsiva)
                        electricForce += pauliForce;
                    }
                }


                //DEBUG VISUAL: Desenha uma linha amarela mostrando o empurrão
                //Se você não ver linhas amarelas na Scene, a força é zero.
               // Debug.DrawRay(me.Position, direction.normalized * electricForce * 0.1f, Color.yellow);



                // Aplica a força resultante na direção calculada
                totalForce += direction.normalized * electricForce;

                // --- 4. FORÇA FORTE (GLÚONS) ---
                // Só entre Quarks. Funciona como um elástico curto.
                if (amIQuark && isOtherQuark)
                {
                    // Alcance: Entre 0.5 (não esmagar) e 4.0 (não puxar vizinhos longe)
                    if (distance > 0.5f && distance < 4.0f)
                    {
                        // Vetor invertido (Atração)
                        Vector3 glueForce = -direction.normalized * UniversePhysics.StrongForceConstant;
                        totalForce += glueForce;
                    }
                }
            }

            // =========================================================
            // FÍSICA RELATIVÍSTICA (EINSTEIN)
            // =========================================================

            // 1. Massa Total (Repouso + Energia de Glúons)
            double restingMass = me.Mass > 0 ? me.Mass : 0.001;
            double relativisticMass = restingMass;

            // 2. Fator de Lorentz (Gamma)
            float currentSpeed = me.Velocity.magnitude;
            float c = UniversePhysics.LightSpeed;

            if (c > 0)
            {
                // Limita a 99% de C para o universo não quebrar
                float ratio = Mathf.Clamp(currentSpeed / c, 0.0f, 0.99f);
                float gamma = 1.0f / Mathf.Sqrt(1.0f - (ratio * ratio));

                // A massa aumenta com a velocidade
                relativisticMass = restingMass * gamma;
            }

            // =========================================================
            // DINÂMICA (NEWTON)
            // =========================================================

            // F = m.a  ->  a = F / m_relativistica
            // Quanto mais rápido, mais pesado, mais difícil mudar a direção.
            Vector3 acceleration = totalForce / (float)relativisticMass;

            // DAMPING (Atrito do Vácuo)
            float damping;
            if (amIQuark)
                damping = 0.95f;   // Quarks: Freio para núcleo sólido
            else
                damping = 0.9999f; // Elétrons: Quase perpétuo para orbitar

            // Integração de Euler
            Vector3 newVelocity = (me.Velocity + acceleration * Time.fixedDeltaTime) * damping;

            // Segurança final de velocidade
            newVelocity = Vector3.ClampMagnitude(newVelocity, c);

            return new QuantumDecision { ProposedMovement = newVelocity };
        }
    }


    //// --- CONTRATO DO CÉREBRO ---
    //public interface IQuantumOracle
    //{
    //    QuantumDecision DecideNextState(QuantumEntity entity);
    //}

    //public struct QuantumDecision
    //{
    //    public Vector3 ProposedMovement;
    //    public Quaternion ProposedRotation;
    //    public double EnergyShift;
    //}

    //// --- CLASSE BASE DA ENTIDADE ---
    //public abstract class QuantumEntity : MonoBehaviour
    //{
    //    // Lista global de partículas
    //    public static List<QuantumEntity> AllEntities = new List<QuantumEntity>();

    //    protected virtual void OnEnable() => AllEntities.Add(this);
    //    protected virtual void OnDisable() => AllEntities.Remove(this);

    //    // Identidade
    //    public string Id { get; private set; }
    //    public string Name { get; protected set; }

    //    // Atalho para posição do Unity
    //    public Vector3 Position
    //    {
    //        get { return transform.position; }
    //        set { transform.position = value; }
    //    }

    //    public Vector3 Velocity { get; set; }
    //    public double Mass { get; protected set; }
    //    public double ElectricCharge { get; protected set; }

    //    // Cérebro
    //    protected IQuantumOracle _brain;

    //    protected virtual void Awake()
    //    {
    //        Id = Guid.NewGuid().ToString();
    //        _brain = new StandardPhysicsOracle();
    //    }

    //    public void ConnectOracle(IQuantumOracle newBrain)
    //    {
    //        _brain = newBrain;
    //    }

    //    // Simulação Física Temporal (Fixa)
    //    protected virtual void FixedUpdate()
    //    {
    //        UpdateSimulation(Time.fixedDeltaTime);
    //    }

    //    public virtual void UpdateSimulation(float deltaTime)
    //    {
    //        var decision = _brain.DecideNextState(this);

    //        this.Velocity = decision.ProposedMovement;
    //        this.Position += this.Velocity * deltaTime;
    //    }
    //}

    //// --- CÉREBRO FÍSICO (CORRIGIDO V3.0) ---
    //public class StandardPhysicsOracle : IQuantumOracle
    //{
    //    public QuantumDecision DecideNextState(QuantumEntity me)
    //    {
    //        Vector3 totalForce = Vector3.zero;

    //        // Verifica identidade para saber se sente Força Forte
    //        var meParticle = me as Matter.ElementaryParticle;
    //        bool amIQuark = (meParticle != null && meParticle.Family == Matter.ParticleFamily.Quark);

    //        foreach (var other in QuantumEntity.AllEntities)
    //        {
    //            if (other == me) continue;

    //            Vector3 direction = me.Position - other.Position;
    //            float distance = direction.magnitude;

    //            // --- CORREÇÃO 1: SOFTENING (Suavização de Coulomb) ---
    //            // Evita divisão por zero e efeito estilingue
    //            //float softening = 1.0f;
    //            float softening = 0.4f;
    //            float distanceSquared = (distance * distance) + (softening * softening);

    //            // 1. LEI DE COULOMB (Eletricidade)
    //            float electricForce = UniversePhysics.CoulombConstant * (float)(me.ElectricCharge * other.ElectricCharge) / distanceSquared;

    //            // DEBUG VISUAL: Desenha uma linha amarela mostrando o empurrão
    //            // Se você não ver linhas amarelas na Scene, a força é zero.
    //            //Debug.DrawRay(me.Position, direction.normalized * electricForce * 0.1f, Color.yellow);

    //            totalForce += direction.normalized * electricForce;

    //            // 2. FORÇA FORTE (Glúons - Só Quarks)
    //            if (amIQuark)
    //            {
    //                var otherParticle = other as Matter.ElementaryParticle;
    //                bool isOtherQuark = (otherParticle != null && otherParticle.Family == Matter.ParticleFamily.Quark);

    //                // ALTERAÇÃO AQUI:
    //                // Antes: distance < 4.0f (Alcance muito longo, puxa vizinhos longe)
    //                // Agora: distance < 1.5f (Alcance curto, só puxa quem está "dentro" do mesmo próton ou colado)

    //                if (isOtherQuark && distance > 0.3f && distance < 1.5f)
    //                {
    //                    Vector3 glueForce = -direction.normalized * UniversePhysics.StrongForceConstant;
    //                    totalForce += glueForce;
    //                }
    //            }
    //        }

    //        // Newton (F = m.a)
    //        double mass = me.Mass > 0 ? me.Mass : 0.001;
    //        Vector3 acceleration = totalForce / (float)mass;

    //        // --- CORREÇÃO 2: DAMPING INTELIGENTE ---
    //        float damping;
    //        if (amIQuark)
    //            damping = 0.95f;   // Quarks: Freio forte para núcleo sólido
    //        else
    //            damping = 0.9999f; // Elétrons: Quase sem atrito para órbita eterna

    //        Vector3 newVelocity = (me.Velocity + acceleration * Time.fixedDeltaTime) * damping;

    //        // Limite de Velocidade Universal
    //        newVelocity = Vector3.ClampMagnitude(newVelocity, UniversePhysics.LightSpeed);

    //        return new QuantumDecision { ProposedMovement = newVelocity };
    //    }
    //}

}