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

    // --- CLASSE BASE DA ENTIDADE ---
    public abstract class QuantumEntity : MonoBehaviour
    {
        // Lista global de partículas
        public static List<QuantumEntity> AllEntities = new List<QuantumEntity>();

        protected virtual void OnEnable() => AllEntities.Add(this);
        protected virtual void OnDisable() => AllEntities.Remove(this);

        // Identidade
        public string Id { get; private set; }
        public string Name { get; protected set; }

        // Atalho para posição do Unity
        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        [Header("Configuração de Massa")]
        [SerializeField]
        protected double _baseMass; // A massa do arquivo (2.3)

        // Variável pública para o GluonConnector preencher
        // (Tiramos o { get; set; } para ela aparecer no Inspector se estiver em Debug mode)
        public double GluonEnergyMass;
        //public double GluonEnergyMas; // A massa que vem da força (E=mc^2)


        // --- VISUALIZAÇÃO (Para você ver no Inspector) ---
        [Header("--- MONITOR DE FÍSICA ---")]
        [Tooltip("Massa Total calculada em tempo real")]
        public float View_TotalMass; // float para o Unity mostrar fácil

        public Vector3 Velocity { get; set; }
        // A Massa Total que a física vai usar (Newton usa isso)
        public double Mass
        {
            get { return _baseMass + GluonEnergyMass; }
            protected set { _baseMass = value; }
        }

        public double ElectricCharge { get; protected set; }

        // Cérebro
        protected IQuantumOracle _brain;

        protected virtual void Awake()
        {
            Id = Guid.NewGuid().ToString();
            _brain = new StandardPhysicsOracle();
        }

        public void ConnectOracle(IQuantumOracle newBrain)
        {
            _brain = newBrain;
        }

        // Simulação Física Temporal (Fixa)
        protected virtual void FixedUpdate()
        {
            UpdateSimulation(Time.fixedDeltaTime);
        }

        public virtual void UpdateSimulation(float deltaTime)
        {
            var decision = _brain.DecideNextState(this);

            this.Velocity = decision.ProposedMovement;
            this.Position += this.Velocity * deltaTime;

            // ATUALIZA O MONITOR VISUAL
            // Converte double para float só para aparecer no Inspector
            View_TotalMass = (float)Mass;
        }
    }

    // --- CÉREBRO FÍSICO (CORRIGIDO V3.0) ---
    public class StandardPhysicsOracle : IQuantumOracle
    {
        public QuantumDecision DecideNextState(QuantumEntity me)
        {


            Vector3 totalForce = Vector3.zero;

            // Identificação Pessoal
            var mePart = me as Matter.ElementaryParticle;
            bool amIQuark = (mePart != null && mePart.Family == Matter.ParticleFamily.Quark);
            //bool amIElectron = (mePart != null && mePart.Flavor == Matter.ParticleFlavor.Electron);

            foreach (var other in QuantumEntity.AllEntities)
            {
                if (other == me) continue;

                //Vector3 direction = me.Position - other.Position;
                //float distance = direction.magnitude;

                //// Identificação do Vizinho
                //var otherPart = other as Matter.ElementaryParticle;
                //bool isOtherElectron = (otherPart != null && otherPart.Flavor == Matter.ParticleFlavor.Electron);

                //// --- CORREÇÃO: SOFTENING DINÂMICO ---
                //float softening = 1.0f; // Padrão (Suave)

                //// Se for Elétron contra Elétron -> Repulsão "Dura" (Sem amortecedor)
                //// Isso obriga eles a respeitarem o espaço um do outro (Princípio de Pauli visual)
                //if (amIElectron && isOtherElectron)
                //{
                //    //softening = 0.2f;
                //    // ANTES: 0.2f
                //    // AGORA: 0.05f (Quase zero)
                //    // Se um elétron tentar ocupar o lugar do outro, a força vai ao infinito.
                //    softening = 0.05f;
                //}

                //float distanceSquared = (distance * distance) + (softening * softening);

                //// 1. LEI DE COULOMB
                //float electricForce = UniversePhysics.CoulombConstant * (float)(me.ElectricCharge * other.ElectricCharge) / distanceSquared;

                Vector3 direction = me.Position - other.Position;
                float distance = direction.magnitude;

                // Identificação
                var myPart = me as Matter.ElementaryParticle;
                var otherPart = other as Matter.ElementaryParticle;

                bool amIElectron = (myPart != null && myPart.Flavor == Matter.ParticleFlavor.Electron);
                bool isOtherElectron = (otherPart != null && otherPart.Flavor == Matter.ParticleFlavor.Electron);

                // --- CÁLCULO 1: COULOMB (A Força Elétrica Pura) ---
                // Usamos Softening padrão para a eletricidade (para não explodir em r=0)
                float softening = 1.0f;
                float distSq = (distance * distance) + (softening * softening);

                float electricForce = UniversePhysics.CoulombConstant * (float)(me.ElectricCharge * other.ElectricCharge) / distSq;
                //// --- APLICAÇÃO DO FATOR PAULI ---
                //// Se SOU elétron E O OUTRO também é elétron...
                //if (amIElectron && isOtherElectron)
                //{
                //    // Multiplica a força de repulsão!
                //    // Isso simula a "Pressão de Degenerescência"
                //    electricForce *= UniversePhysics.ElectronRepulsionFactor;
                //}


                // --- CÁLCULO 2: PAULI (A Exclusão Quântica) ---
                //float pauliForce = 0f;
                // --- CÁLCULO 2: PAULI (A Exclusão Quântica) ---
                float pauliForce = 0f;
                // A Regra Real: Pauli só existe entre férmions IDÊNTICOS no mesmo estado.
                // Se somos elétrons E temos o MESMO SPIN...
                if (amIElectron && isOtherElectron && (myPart.QuantumSpin == otherPart.QuantumSpin))
                {
                    // ...então somos proibidos de ocupar o mesmo espaço.
                    // O Potencial de Pauli é exponencial (Gaussian).
                    // Ele é zero longe, mas cresce violentamente se as ondas se tocarem.

                    float exclusionRadius = 2.0f; // Tamanho do orbital K

                    // Se tentar entrar no raio de exclusão com o mesmo spin:
                    if (distance < exclusionRadius)
                    {
                        // Fórmula Exponencial (Simula a sobreposição de onda)
                        // F = K * exp(-r^2) -> Empurrão suave que vira parede de concreto
                        float overlap = 1.0f - (distance / exclusionRadius);
                        pauliForce = UniversePhysics.CoulombConstant * 50.0f * overlap * overlap;

                        // Nota: "50.0f" aqui não é trapaça, é a constante de Planck normalizada para nossa escala.
                        // Representa a "Pressão de Fermi".
                    }
                }


                // Soma as duas forças (Eletricidade + Exclusão)
                float finalForceMagnitude = electricForce + pauliForce;

                totalForce += direction.normalized * finalForceMagnitude;
                //totalForce += direction.normalized * electricForce;

                // 2. FORÇA FORTE (Glúons - Só Quarks)
                if (amIQuark)
                {
                    bool isOtherQuark = (otherPart != null && otherPart.Family == Matter.ParticleFamily.Quark);
                    if (isOtherQuark && distance > 0.5f && distance < 4.0f)
                    {
                        Vector3 glueForce = -direction.normalized * UniversePhysics.StrongForceConstant;
                        totalForce += glueForce;
                    }
                }
                


            }

            // Newton (F = m.a)
            // Einstein (Relatividade)
            double restingMass = me.Mass > 0 ? me.Mass : 0.001;
            double relativisticMass = restingMass;

            float currentSpeed = me.Velocity.magnitude;
            float c = UniversePhysics.LightSpeed;

            if (c > 0)
            {
                float ratio = Mathf.Clamp(currentSpeed / c, 0.0f, 0.99f);
                float gamma = 1.0f / Mathf.Sqrt(1.0f - (ratio * ratio));
                relativisticMass = restingMass * gamma;
            }

            Vector3 acceleration = totalForce / (float)relativisticMass;

            // Damping (Atrito)
            float damping;
            if (amIQuark) damping = 0.95f;
            else damping = 0.9999f;

            Vector3 newVelocity = (me.Velocity + acceleration * Time.fixedDeltaTime) * damping;
            newVelocity = Vector3.ClampMagnitude(newVelocity, UniversePhysics.LightSpeed);

            return new QuantumDecision { ProposedMovement = newVelocity };


            //Vector3 totalForce = Vector3.zero;

            //// Verifica identidade para saber se sente Força Forte
            //var meParticle = me as Matter.ElementaryParticle;
            //bool amIQuark = (meParticle != null && meParticle.Family == Matter.ParticleFamily.Quark);

            //foreach (var other in QuantumEntity.AllEntities)
            //{
            //    if (other == me) continue;

            //    Vector3 direction = me.Position - other.Position;
            //    float distance = direction.magnitude;

            //    // --- CORREÇÃO 1: SOFTENING (Suavização de Coulomb) ---
            //    // Evita divisão por zero e efeito estilingue
            //    float softening = 1.0f;
            //    float distanceSquared = (distance * distance) + (softening * softening);

            //    // 1. LEI DE COULOMB (Eletricidade)
            //    float electricForce = UniversePhysics.CoulombConstant * (float)(me.ElectricCharge * other.ElectricCharge) / distanceSquared;
            //    totalForce += direction.normalized * electricForce;

            //    // 2. FORÇA FORTE (Glúons - Só Quarks)
            //    if (amIQuark)
            //    {
            //        var otherParticle = other as Matter.ElementaryParticle;
            //        bool isOtherQuark = (otherParticle != null && otherParticle.Family == Matter.ParticleFamily.Quark);

            //        if (isOtherQuark && distance > 0.5f && distance < 4.0f)
            //        {
            //            Vector3 glueForce = -direction.normalized * UniversePhysics.StrongForceConstant;
            //            totalForce += glueForce;
            //        }
            //    }
            //}

            //// Newton (F = m.a)
            //double mass = me.Mass > 0 ? me.Mass : 0.001;
            //Vector3 acceleration = totalForce / (float)mass;

            //// --- CORREÇÃO 2: DAMPING INTELIGENTE ---
            //float damping;
            //if (amIQuark)
            //    damping = 0.90f;   // Quarks: Freio forte para núcleo sólido
            //else
            //    damping = 0.9995f; // Elétrons: Quase sem atrito para órbita eterna

            //Vector3 newVelocity = (me.Velocity + acceleration * Time.fixedDeltaTime) * damping;

            //// Limite de Velocidade Universal
            //newVelocity = Vector3.ClampMagnitude(newVelocity, UniversePhysics.LightSpeed);

            //return new QuantumDecision { ProposedMovement = newVelocity };
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