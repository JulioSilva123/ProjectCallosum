using Assets._ProjectCallosum.Data;
using Assets._ProjectCallosum.Scripts.Matter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Core
{

    public class QuantumSpawner : MonoBehaviour
    {
        [Header("Templates")]
        public ParticleDefinition upQuark;
        public ParticleDefinition downQuark;
        public ParticleDefinition electron;

        [Header("Configuração")]
        public float protonRadius = 0.5f;



        //// Contador global para saber qual o próximo spin a ser dado
        //private int _nextElectronSpin = 1;
        // CONTROLE DE SPIN (Alterna entre 1 e -1)
        private int _nextSpin = 1;

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.P)) SpawnProton();
            if (Input.GetKeyDown(KeyCode.N)) SpawnNeutron();
            // ... atalhos existentes ...
            if (Input.GetKeyDown(KeyCode.E)) SpawnElectron(); // <--- NOVO ATALHO 'E'


            if (Input.GetKeyDown(KeyCode.H)) SpawnHydrogen();            
            if (Input.GetKeyDown(KeyCode.M)) SpawnHydrogenMolecule();


            // Atalhos de Teclado (Para teste rápido sem UI)            
            
            if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnHeliumAtom();
            if (Input.GetKeyDown(KeyCode.Alpha3)) SpawnLithiumAtom();
            if (Input.GetKeyDown(KeyCode.Alpha4)) SpawnBerylliumAtom();
        }



        // --- MÉTODOS DE CRIAÇÃO ---

        public void SpawnHydrogen()
        {
            SpawnProton();
            // Reseta o spin para começar bonito
            _nextSpin = 1;
            CreateElectronOrbital(Vector3.right * 4.0f, Vector3.up, 1.0f); // Carga núcleo +1
        }

        public void SpawnHeliumAtom()
        {
            // Núcleo Alfa
            SpawnProtonCluster(transform.position + new Vector3(0.5f, 0.5f, 0));
            SpawnProtonCluster(transform.position + new Vector3(-0.5f, -0.5f, 0));
            SpawnNeutronCluster(transform.position + new Vector3(-0.5f, 0.5f, 0));
            SpawnNeutronCluster(transform.position + new Vector3(0.5f, -0.5f, 0));

            _nextSpin = 1; // Reinicia ciclo de spin

            // Elétron 1 (Spin +1)
            CreateElectronOrbital(Vector3.right * 4.0f, Vector3.up, 2.0f);
            // Elétron 2 (Spin -1)
            CreateElectronOrbital(Vector3.left * 4.0f, Vector3.down, 2.0f);
        }

        public void SpawnLithiumAtom()
        {
            // Núcleo grande (simplificado)
            for (int i = 0; i < 3; i++) SpawnProtonCluster(transform.position + UnityEngine.Random.insideUnitSphere);
            for (int i = 0; i < 4; i++) SpawnNeutronCluster(transform.position + UnityEngine.Random.insideUnitSphere);

            _nextSpin = 1;

            // 3 Elétrons
            CreateElectronOrbital(Vector3.right * 3.0f, Vector3.forward, 3.0f);
            CreateElectronOrbital(Vector3.left * 3.0f, Vector3.back, 3.0f);
            CreateElectronOrbital(Vector3.up * 3.0f, Vector3.right, 3.0f);
        }

        public void SpawnBerylliumAtom()
        {
            for (int i = 0; i < 4; i++) SpawnProtonCluster(transform.position + UnityEngine.Random.insideUnitSphere);
            for (int i = 0; i < 5; i++) SpawnNeutronCluster(transform.position + UnityEngine.Random.insideUnitSphere);

            _nextSpin = 1;

            // 4 Elétrons (Vão brigar pelas camadas)
            CreateElectronOrbital(Vector3.right * 1.5f, Vector3.up, 4.0f);
            CreateElectronOrbital(Vector3.left * 1.5f, Vector3.down, 4.0f);
            CreateElectronOrbital(Vector3.up * 5.0f, Vector3.right, 2.0f); // Já lança longe pra ajudar
            CreateElectronOrbital(Vector3.down * 5.0f, Vector3.left, 2.0f);
        }

        // --- MÉTODOS AUXILIARES ---

        // Método inteligente que cria, calcula velocidade e ATRIBUI SPIN
        private void CreateElectronOrbital(Vector3 offset, Vector3 direction, float qNucleus)
        {
            if (electron == null) return;

            Vector3 pos = transform.position + offset;
            GameObject obj = Instantiate(electron.VisualPrefab, pos, Quaternion.identity);
            var script = obj.AddComponent<ElementaryParticle>();
            script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

            // --- AQUI ESTÁ A CORREÇÃO: ENVIAR O SPIN ---
            script.QuantumSpin = _nextSpin;
            _nextSpin *= -1; // Inverte para o próximo (1 vira -1, -1 vira 1)
            // -------------------------------------------

            // Física Orbital
            float k = UniversePhysics.CoulombConstant;
            float speed = Mathf.Sqrt((k * qNucleus) / (electron.MassMeV * offset.magnitude));

            script.Velocity = direction.normalized * speed;
        }





        // =========================================================
        // NÍVEL 1: NÚCLEONS (Os blocos básicos)
        // =========================================================
        public void SpawnProton() { if (upQuark != null) SpawnProtonCluster(transform.position); }
        public void SpawnNeutron() { if (upQuark != null) SpawnNeutronCluster(transform.position); }


        // =========================================================
        // NÍVEL 0: PARTÍCULAS ISOLADAS
        // =========================================================

        public void SpawnElectron()
        {
            if (electron == null) return;

            // Cria um elétron livre em posição aleatória perto do centro
            Vector3 pos = transform.position + UnityEngine.Random.insideUnitSphere * 2.0f;

            // Usamos o método auxiliar que já existe no seu código
            // Damos uma velocidade inicial pequena (Drift) para ele não ficar estático
            SpawnElectronVector(pos - transform.position, UnityEngine.Random.insideUnitSphere * 2.0f);

            Utils.CallosumUtils.LogColor("Elétron Livre Criado!", "yellow");
        }






        private void SpawnProtonCluster(Vector3 c)
        {
            CreateParticle(upQuark, c); CreateParticle(upQuark, c); CreateParticle(downQuark, c);
        }
        private void SpawnNeutronCluster(Vector3 c)
        {
            CreateParticle(upQuark, c); CreateParticle(downQuark, c); CreateParticle(downQuark, c);
        }
        //private void CreateParticle(ParticleDefinition def, Vector3 pos)
        //{
        //    GameObject obj = Instantiate(def.VisualPrefab, pos + UnityEngine.Random.insideUnitSphere * 0.2f, Quaternion.identity);
        //    var s = obj.AddComponent<ElementaryParticle>();
        //    s.Initialize(def.Family, def.Flavor, def.MassMeV, def.Charge);
        //}
        //public void SpawnProton()
        //{
        //    if (upQuark == null || downQuark == null) return;
        //    SpawnProtonCluster(transform.position);
        //}
        ////public void SpawnProton()
        ////{
        ////    if (upQuark == null || downQuark == null) return;

        ////    // Cria triângulo compacto
        ////    CreateParticle(upQuark, transform.position + new Vector3(0, protonRadius, 0));
        ////    CreateParticle(upQuark, transform.position + new Vector3(protonRadius, -protonRadius, 0));
        ////    CreateParticle(downQuark, transform.position + new Vector3(-protonRadius, -protonRadius, 0));
        ////}
        //public void SpawnNeutron()
        //{
        //    if (upQuark == null || downQuark == null) return;
        //    SpawnNeutronCluster(transform.position);
        //}


        // =========================================================
        // NÍVEL 2: ÁTOMOS SIMPLES
        // =========================================================

        //// --- HIDROGÊNIO (1p + 1e) ---
        //public void SpawnHydrogen()
        //{
        //    SpawnProton(); // Cria o núcleo

        //    // Reseta o spin para começar bonito
        //    _nextSpin = 1;

        //    if (electron == null) return;




        //    // Cria elétron longe
        //    float r = 4.0f;
        //    Vector3 startPos = transform.position + new Vector3(r, 0, 0);

        //    // Calcula velocidade orbital estável
        //    float k = UniversePhysics.CoulombConstant;
        //    float qProd = 1.0f; // Carga 1x1
        //    float m = electron.MassMeV;
        //    float speed = Mathf.Sqrt((k * qProd) / (m * r));

        //    // Lança com 95% da velocidade para fechar a elipse
        //    SpawnElectronVector(Vector3.right * r, Vector3.up * (speed * 0.95f));
        //}




        //public void SpawnHydrogen()
        //{
        //    // 1. Cria Núcleo
        //    SpawnProton();
        //    if (electron == null) return;

        //    // 2. Cria Elétron distante
        //    float r = 4.0f;
        //    Vector3 startPos = transform.position + new Vector3(r, 0, 0);

        //    GameObject eleObj = Instantiate(electron.VisualPrefab, startPos, Quaternion.identity);
        //    eleObj.name = "Orbital Electron";

        //    var script = eleObj.AddComponent<ElementaryParticle>();
        //    script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

        //    // 3. CÁLCULO ORBITAL (Mecânica Celeste)
        //    // V = Raiz( (K * Q1 * Q2) / (M * R) )
        //    float k = UniversePhysics.CoulombConstant; // 50
        //    float qProd = 1.0f; // Carga 1 vs 1
        //    float m = electron.MassMeV; // 0.511

        //    float perfectSpeed = Mathf.Sqrt((k * qProd) / (m * r));

        //    Debug.Log($"[Callosum] Velocidade Orbital Calculada: {perfectSpeed}");

        //    // Aplica velocidade tangente (Para cima)
        //    script.Velocity = Vector3.up * perfectSpeed;
        //}











        // --- HIDROGÊNIO MOLECULAR (H2) ---
        public void SpawnHydrogenMolecule()
        {
            Vector3 center = transform.position;
            float atomDistance = 2.0f; // Núcleos próximos

            SpawnProtonCluster(center + Vector3.left * (atomDistance / 2));
            SpawnProtonCluster(center + Vector3.right * (atomDistance / 2));

            if (electron == null) return;

            // Elétrons orbitando o par
            float r = 4.0f;
            float k = UniversePhysics.CoulombConstant;
            float qNucleus = 2.0f; // Carga efetiva +2
            float m = electron.MassMeV;
            float speed = Mathf.Sqrt((k * qNucleus) / (m * r));

            SpawnElectronVector(Vector3.up * r, Vector3.right * speed);
            SpawnElectronVector(Vector3.down * r, Vector3.left * speed);
        }


        //// --- RECEITA DO H2 (Hidrogênio Molecular) ---
        //public void SpawnHydrogenMolecule()
        //{
        //    // Distância entre os dois núcleos (protons)
        //    float atomDistance = 4.0f;
        //    Vector3 center = transform.position;

        //    // Posições dos Núcleos
        //    Vector3 nuc1Pos = center + Vector3.left * (atomDistance / 2);
        //    Vector3 nuc2Pos = center + Vector3.right * (atomDistance / 2);

        //    // 1. Cria os Núcleos (Usando a receita do Próton)
        //    CreateProtonAt(nuc1Pos);
        //    CreateProtonAt(nuc2Pos);

        //    if (electron == null) return;

        //    // 2. O Segredo da Ligação: Onde colocar os elétrons?
        //    // Vamos colocá-los no "plano de corte" entre os átomos.
        //    // Um em cima, um embaixo. Eles vão girar como um anel no meio.

        //    float electronDist = 2.5f; // Distância do centro da molécula

        //    // Elétron 1 (Cima)
        //    SpawnBondingElectron(center + Vector3.up * electronDist, Vector3.right);

        //    // Elétron 2 (Baixo)
        //    SpawnBondingElectron(center + Vector3.down * electronDist, Vector3.left);
        //}



        // --- HÉLIO (2p + 2n + 2e) ---
        //public void SpawnHeliumAtom()
        //{
        //    // Núcleo Alfa (Compacto)
        //    float d = 0.6f;
        //    SpawnProtonCluster(transform.position + new Vector3(d, d, 0));
        //    SpawnProtonCluster(transform.position + new Vector3(-d, -d, 0));
        //    SpawnNeutronCluster(transform.position + new Vector3(-d, d, 0));
        //    SpawnNeutronCluster(transform.position + new Vector3(d, -d, 0));


        //    // Reseta o spin para começar bonito
        //    _nextSpin = 1;

        //    if (electron == null) return;

        //    // Elétrons
        //    float r = 4.0f;
        //    float k = UniversePhysics.CoulombConstant;
        //    float speed = Mathf.Sqrt((k * 2.0f) / (electron.MassMeV * r));

        //    SpawnElectronVector(Vector3.right * r, Vector3.up * speed);
        //    SpawnElectronVector(Vector3.left * r, Vector3.down * speed);
        //}
        // --- RECEITA DO HÉLIO (2p, 2n, 2e) ---
        //public void SpawnHeliumAtom()
        //{
        //    // 1. Cria o Núcleo Pesado (Partícula Alfa)
        //    // Vamos usar posições manuais para criar um "tetraedro" compacto
        //    float d = protonRadius * 0.8f;

        //    // Prótons e Nêutrons misturados
        //    CreateParticle(upQuark, transform.position + new Vector3(d, d, 0));
        //    CreateParticle(upQuark, transform.position + new Vector3(-d, -d, 0)); // Prótons opostos

        //    CreateParticle(downQuark, transform.position + new Vector3(-d, d, 0));
        //    CreateParticle(downQuark, transform.position + new Vector3(d, -d, 0)); // Nêutrons opostos (simplificado)

        //    // Adiciona mais alguns quarks para dar massa/cola (Total 12 quarks)
        //    for (int i = 0; i < 4; i++)
        //        CreateParticle(upQuark, transform.position + UnityEngine.Random.insideUnitSphere * 0.5f);
        //    for (int i = 0; i < 4; i++)
        //        CreateParticle(downQuark, transform.position + UnityEngine.Random.insideUnitSphere * 0.5f);

        //    if (electron == null) return;

        //    // 2. Os Dois Elétrons
        //    // Vamos lançar eles de lados opostos para ver se eles acham o equilíbrio.
        //    float r = 4.0f;

        //    // Carga do núcleo é +2. Calculamos a velocidade para isso.
        //    float k = UniversePhysics.CoulombConstant;
        //    float qProd = 2.0f * 1.0f; // Núcleo (+2) * Elétron (-1)
        //    float m = electron.MassMeV;
        //    float speed = Mathf.Sqrt((k * qProd) / (m * r));

        //    // Elétron 1 (Direita, indo pra cima)
        //    SpawnElectronVector(Vector3.right * r, Vector3.up * speed);

        //    // Elétron 2 (Esquerda, indo pra baixo)
        //    SpawnElectronVector(Vector3.left * r, Vector3.down * speed);
        //}

        // =========================================================
        // NÍVEL 3: ÁTOMOS COMPLEXOS (Camadas)
        // =========================================================

        // --- LÍTIO (3p + 4n + 3e) ---
        //public void SpawnLithiumAtom()
        //{
        //    // Núcleo Caótico
        //    for (int i = 0; i < 3; i++) SpawnProtonCluster(transform.position + UnityEngine.Random.insideUnitSphere * 0.8f);
        //    for (int i = 0; i < 4; i++) SpawnNeutronCluster(transform.position + UnityEngine.Random.insideUnitSphere * 0.8f);
        //    // Reseta o spin para começar bonito
        //    _nextSpin = 1;
        //    if (electron == null) return;

        //    float r = 3.5f;
        //    float k = UniversePhysics.CoulombConstant;
        //    // Carga nuclear +3
        //    float speed = Mathf.Sqrt((k * 3.0f) / (electron.MassMeV * r));

        //    // Tentamos colocar 3 na mesma órbita e ver a física expulsar um
        //    SpawnElectronVector(Vector3.right * r, Vector3.forward * speed);
        //    SpawnElectronVector(Vector3.left * r, Vector3.back * speed);
        //    SpawnElectronVector(Vector3.up * r, Vector3.right * speed);
        //}

        // --- BERÍLIO (4p + 5n + 4e) ---
        //public void SpawnBerylliumAtom()
        //{
        //    // 1. O NÚCLEO (Compacto)
        //    for (int i = 0; i < 4; i++) SpawnProtonCluster(transform.position + UnityEngine.Random.insideUnitSphere * 0.5f);
        //    for (int i = 0; i < 5; i++) SpawnNeutronCluster(transform.position + UnityEngine.Random.insideUnitSphere * 0.5f);
        //    // Reseta o spin para começar bonito
        //    _nextSpin = 1;
        //    if (electron == null) return;

        //    // Constantes
        //    float k = UniversePhysics.CoulombConstant;
        //    float m = electron.MassMeV;

        //    // --- CAMADA K (Interna) ---
        //    // Raio MINÚSCULO (1.0). Eles precisam girar freneticamente para não cair no núcleo de carga +4.
        //    float rK = 1.0f;
        //    float qNucleusK = 4.0f; // Sente a carga total
        //    float speedK = Mathf.Sqrt((k * qNucleusK) / (m * rK));

        //    // Lançamos em eixo X
        //    SpawnElectronVector(Vector3.right * rK, Vector3.up * speedK);
        //    SpawnElectronVector(Vector3.left * rK, Vector3.down * speedK);

        //    // --- CAMADA L (Externa) ---
        //    // Raio GRANDE (6.0). 
        //    // Truque da Química: Eles "sentem" que o núcleo tem carga +2, 
        //    // porque os 2 elétrons de dentro cancelam metade da carga.
        //    float rL = 6.0f;
        //    float qNucleusL = 2.0f; // Carga Efetiva (Z_eff) = 4 - 2 = 2
        //    float speedL = Mathf.Sqrt((k * qNucleusL) / (m * rL));

        //    // Lançamos em eixo Y (Perpendicular para não bater nos de dentro)
        //    SpawnElectronVector(Vector3.up * rL, Vector3.right * speedL);
        //    SpawnElectronVector(Vector3.down * rL, Vector3.left * speedL);




        //    //// Núcleo Pesado
        //    //for (int i = 0; i < 4; i++) SpawnProtonCluster(transform.position + UnityEngine.Random.insideUnitSphere * 1.0f);
        //    //for (int i = 0; i < 5; i++) SpawnNeutronCluster(transform.position + UnityEngine.Random.insideUnitSphere * 1.0f);

        //    //if (electron == null) return;

        //    //// Camada K (Rápida e Perto)
        //    //float rK = 1.5f;
        //    //float speedK = Mathf.Sqrt((UniversePhysics.CoulombConstant * 4.0f) / (electron.MassMeV * rK));
        //    //SpawnElectronVector(Vector3.right * rK, Vector3.up * speedK);
        //    //SpawnElectronVector(Vector3.left * rK, Vector3.down * speedK);

        //    //// Camada L (Lenta e Longe)
        //    //float rL = 5.0f;
        //    //// Carga efetiva menor devido à blindagem dos elétrons internos
        //    //float speedL = Mathf.Sqrt((UniversePhysics.CoulombConstant * 2.0f) / (electron.MassMeV * rL));
        //    //SpawnElectronVector(Vector3.up * rL, Vector3.right * speedL);
        //    //SpawnElectronVector(Vector3.down * rL, Vector3.left * speedL);
        //}

        // =========================================================
        // MÉTODOS AUXILIARES (HELPER FUNCTIONS)
        // =========================================================

        //private void SpawnProtonCluster(Vector3 center)
        //{
        //    // Próton = 2 Up + 1 Down
        //    CreateParticle(upQuark, center + new Vector3(0, protonRadius, 0));
        //    CreateParticle(upQuark, center + new Vector3(protonRadius, -protonRadius, 0));
        //    CreateParticle(downQuark, center + new Vector3(-protonRadius, -protonRadius, 0));
        //}

        //private void SpawnNeutronCluster(Vector3 center)
        //{
        //    // Nêutron = 1 Up + 2 Down
        //    CreateParticle(upQuark, center + new Vector3(0, protonRadius, 0));
        //    CreateParticle(downQuark, center + new Vector3(protonRadius, -protonRadius, 0));
        //    CreateParticle(downQuark, center + new Vector3(-protonRadius, -protonRadius, 0));
        //}



        //private void SpawnElectronVector(Vector3 posOffset, Vector3 velocity)
        //{
        //    Vector3 finalPos = transform.position + posOffset;
        //    GameObject eleObj = Instantiate(electron.VisualPrefab, finalPos, Quaternion.identity);
        //    eleObj.name = "Electron";

        //    var script = eleObj.AddComponent<ElementaryParticle>();
        //    script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);
        //    script.Velocity = velocity;


        //    // --- NOVO: ATRIBUIÇÃO DE SPIN REAL ---
        //    script.QuantumSpin = _nextSpin;

        //    // Inverte para o próximo (Se era 1, vira -1. Se era -1, vira 1)
        //    _nextSpin *= -1;

        //    Debug.Log($"Elétron criado com Spin: {script.QuantumSpin}");

        //}



        // --- VOLTANDO PARA A VERSÃO SEGURA ---

        private void SpawnElectronVector(Vector3 posOffset, Vector3 velocity)
        {
            Vector3 finalPos = transform.position + posOffset;
            GameObject eleObj = Instantiate(electron.VisualPrefab, finalPos, Quaternion.identity);
            eleObj.name = "Electron";

            // SEMPRE ADICIONA NOVO (Garante que não pega lixo zerado)
            var script = eleObj.AddComponent<ElementaryParticle>();

            script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);
            script.Velocity = velocity;

            script.QuantumSpin = _nextSpin;
            _nextSpin *= -1;
        }

        private void CreateParticle(ParticleDefinition template, Vector3 pos)
        {
            // Posição com leve aleatoriedade para não encavalar
            GameObject newObj = Instantiate(template.VisualPrefab, pos + UnityEngine.Random.insideUnitSphere * 0.2f, Quaternion.identity);
            newObj.name = template.ParticleName;

            // SEMPRE ADICIONA NOVO
            var script = newObj.AddComponent<ElementaryParticle>();

            script.Initialize(template.Family, template.Flavor, template.MassMeV, template.Charge);
        }




        //// Método auxiliar simples
        //void SpawnElectronVector(Vector3 posOffset, Vector3 velocity)
        //{
        //    Vector3 finalPos = transform.position + posOffset;
        //    GameObject eleObj = Instantiate(electron.VisualPrefab, finalPos, Quaternion.identity);
        //    var script = eleObj.AddComponent<ElementaryParticle>();
        //    script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);
        //    script.Velocity = velocity;
        //}





        // Método auxiliar para criar Próton em local específico
        void CreateProtonAt(Vector3 pos)
        {
            CreateParticle(upQuark, pos + new Vector3(0, protonRadius, 0));
            CreateParticle(upQuark, pos + new Vector3(protonRadius, -protonRadius, 0));
            CreateParticle(downQuark, pos + new Vector3(-protonRadius, -protonRadius, 0));
        }

        // Método auxiliar para elétron de ligação (COVALENTE)
        void SpawnBondingElectron(Vector3 pos, Vector3 dir)
        {
            GameObject eleObj = Instantiate(electron.VisualPrefab, pos, Quaternion.identity);
            eleObj.name = "Bonding Electron";
            var script = eleObj.AddComponent<ElementaryParticle>();
            script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

            // CÁLCULO DE VELOCIDADE PARA MOLÉCULA
            // Na molécula, o elétron sente atração de DOIS núcleos, mas repulsão do outro elétron.
            // A matemática exata é complexa, então usamos a regra empírica:
            // "Velocidade menor que a atômica para permitir a captura mútua".

            float k = UniversePhysics.CoulombConstant;
            float qProd = 1.0f;
            float m = electron.MassMeV;
            float r = 4.0f; // Raio aproximado

            float atomicSpeed = Mathf.Sqrt((k * qProd) / (m * r));

            // FATOR DE LIGAÇÃO: 0.6f (60% da velocidade)
            // Se for rápido (1.0), ele foge. Se for lento (0.6), ele fica preso no meio.
            script.Velocity = dir.normalized * (atomicSpeed * 0.6f);
        }


        

        //private void CreateParticle(ParticleDefinition template, Vector3 pos)
        //{
        //    GameObject newObj = Instantiate(template.VisualPrefab, pos, Quaternion.identity);
        //    newObj.name = template.ParticleName;
        //    var script = newObj.AddComponent<ElementaryParticle>();
        //    script.Initialize(template.Family, template.Flavor, template.MassMeV, template.Charge);
        //}


         


    }


    //public class QuantumSpawner : MonoBehaviour
    //{
    //    [Header("Templates de Partículas")]
    //    public ParticleDefinition upQuark;
    //    public ParticleDefinition downQuark;
    //    public ParticleDefinition electron; // <--- NOVO: Campo para arrastar o Elétron

    //    [Header("Configurações")]
    //    public float protonRadius = 0.5f;

    //    private void Update()
    //    {
    //        // Atalho P: Cria apenas o núcleo (Próton)
    //        if (Input.GetKeyDown(KeyCode.P))
    //        {
    //            SpawnProton();
    //        }

    //        // Atalho H: Cria o átomo completo (Hidrogênio)
    //        if (Input.GetKeyDown(KeyCode.H))
    //        {
    //            SpawnHydrogen();
    //        }


    //        // --- NOVOS ATALHOS ---
    //        if (Input.GetKeyDown(KeyCode.N)) SpawnNeutron(); // Cria Nêutron isolado
    //        if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnHeliumNucleus(); // Cria Núcleo de Hélio (Partícula Alfa)

    //        // Atalho M: Molécula de H2
    //        if (Input.GetKeyDown(KeyCode.M)) SpawnHydrogenMolecule();
    //    }


    //    public void SpawnHydrogenMolecule()
    //    {
    //        float atomDistance = 5.0f; // Distância entre núcleos
    //        Vector3 center = transform.position;

    //        // Posições dos Núcleos (Esquerda e Direita)
    //        Vector3 nucleusPos1 = center + Vector3.left * (atomDistance / 2);
    //        Vector3 nucleusPos2 = center + Vector3.right * (atomDistance / 2);

    //        // 1. Cria os Núcleos (Prótons Pesados)
    //        CreateHydrogenNucleus(nucleusPos1);
    //        CreateHydrogenNucleus(nucleusPos2);

    //        // 2. Cria os Elétrons no "Ponto de Ligação" (No meio, mas girando)
    //        // Em vez de colocar longe, colocamos eles quase no meio do caminho.
    //        // Isso força eles a orbitarem AMBOS os núcleos (trajetória em forma de 8).

    //        float electronOffset = 1.5f; // Afastamento do eixo central

    //        // Elétron 1 (Cima)
    //        SpawnBondingElectron(center + new Vector3(0, electronOffset, 0), Vector3.right * 15.0f);

    //        // Elétron 2 (Baixo) - Spin oposto visual
    //        SpawnBondingElectron(center + new Vector3(0, -electronOffset, 0), Vector3.left * 15.0f);
    //    }

    //    // Método auxiliar para criar núcleo limpo
    //    private void CreateHydrogenNucleus(Vector3 pos)
    //    {
    //        CreateParticle(upQuark, pos + new Vector3(0, protonRadius, 0));
    //        CreateParticle(upQuark, pos + new Vector3(protonRadius, -protonRadius, 0));
    //        CreateParticle(downQuark, pos + new Vector3(-protonRadius, -protonRadius, 0));
    //    }

    //    // Método auxiliar para o elétron de ligação
    //    private void SpawnBondingElectron(Vector3 pos, Vector3 initialVelocity)
    //    {
    //        GameObject eleObj = Instantiate(electron.VisualPrefab, pos, Quaternion.identity);
    //        var script = eleObj.AddComponent<ElementaryParticle>();
    //        script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);
    //        script.Velocity = initialVelocity;
    //    }


    //    // --- RECEITA DO NÊUTRON (1 Up + 2 Down) ---
    //    public void SpawnNeutron()
    //    {
    //        if (upQuark == null || downQuark == null) return;

    //        // Cria um triângulo invertido em relação ao próton (só estético)
    //        CreateParticle(upQuark, transform.position + new Vector3(0, protonRadius, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(protonRadius, -protonRadius, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(-protonRadius, -protonRadius, 0));
    //    }

    //    // --- O DESAFIO DO HÉLIO (2 Prótons + 2 Nêutrons) ---
    //    public void SpawnHeliumNucleus()
    //    {
    //        // Posições manuais para formar um tetraedro compacto (Bola)
    //        float d = protonRadius * 1.5f;

    //        // Próton 1 (Superior Esquerda)
    //        CreateParticle(upQuark, transform.position + new Vector3(-d, d, 0));
    //        CreateParticle(upQuark, transform.position + new Vector3(-d + 0.3f, d, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(-d, d - 0.3f, 0));

    //        // Próton 2 (Inferior Direita)
    //        CreateParticle(upQuark, transform.position + new Vector3(d, -d, 0));
    //        CreateParticle(upQuark, transform.position + new Vector3(d + 0.3f, -d, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(d, -d - 0.3f, 0));

    //        // Nêutron 1 (Superior Direita - A Cola)
    //        CreateParticle(upQuark, transform.position + new Vector3(d, d, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(d + 0.3f, d, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(d, d - 0.3f, 0));

    //        // Nêutron 2 (Inferior Esquerda - A Cola)
    //        CreateParticle(upQuark, transform.position + new Vector3(-d, -d, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(-d + 0.3f, -d, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(-d, -d - 0.3f, 0));
    //    }


    //    // --- RECEITA DO PRÓTON (Núcleo) ---
    //    public void SpawnProton()
    //    {
    //        if (upQuark == null || downQuark == null)
    //        {
    //            Debug.LogError("[Callosum] Faltam templates dos Quarks no Spawner!");
    //            return;
    //        }

    //        // Cria o trio (2 Up + 1 Down) bem pertinho
    //        CreateParticle(upQuark, transform.position + new Vector3(0, protonRadius, 0));
    //        CreateParticle(upQuark, transform.position + new Vector3(protonRadius, -protonRadius, 0));
    //        CreateParticle(downQuark, transform.position + new Vector3(-protonRadius, -protonRadius, 0));
    //    }

    //    // --- RECEITA DO HIDROGÊNIO (Próton + Elétron Orbitando) ---
    //    public void SpawnHydrogen()
    //    {
    //        //SpawnProton();

    //        //if (electron == null) return;

    //        //// Distância inicial
    //        //float distance = 3.0f;
    //        //Vector3 startPos = transform.position + new Vector3(distance, 0, 0);

    //        //GameObject eleObj = Instantiate(electron.VisualPrefab, startPos, Quaternion.identity);
    //        //eleObj.name = "Orbital Electron";

    //        //var script = eleObj.AddComponent<ElementaryParticle>();
    //        //script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

    //        //// --- CÁLCULO AUTOMÁTICO DE ÓRBITA ---
    //        //// Fórmula: V = Raiz( (K * Q1 * Q2) / (Massa * Raio) )

    //        //float k = UniversePhysics.CoulombConstant; // 10
    //        //float qProd = 1.0f * 1.0f; // Carga do núcleo (1) x Carga elétron (1)
    //        //float m = electron.MassMeV; // 0.5
    //        //float r = distance; // 3.0

    //        //// Conta: (10 * 1) / (0.5 * 3.0) = 10 / 1.5 = 6.66
    //        //// Raiz de 6.66 = ~2.58
    //        //float perfectSpeed = Mathf.Sqrt((k * qProd) / (m * r));

    //        //Debug.Log($"[Callosum] Velocidade Calculada para Órbita: {perfectSpeed}");

    //        //// Aplica a velocidade perfeita
    //        ////script.Velocity = Vector3.up * perfectSpeed;
    //        //script.Velocity = Vector3.up * (perfectSpeed * 0.95f);







    //        //// 1. Cria Núcleo
    //        //SpawnProton();
    //        //if (electron == null) return;

    //        //// 2. Cria Elétron distante
    //        //float r = 4.0f;
    //        //Vector3 startPos = transform.position + new Vector3(r, 0, 0);

    //        //GameObject eleObj = Instantiate(electron.VisualPrefab, startPos, Quaternion.identity);
    //        //eleObj.name = "Orbital Electron";

    //        //var script = eleObj.AddComponent<ElementaryParticle>();
    //        //script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

    //        //// 3. CÁLCULO ORBITAL (Mecânica Celeste)
    //        //// V = Raiz( (K * Q1 * Q2) / (M * R) )
    //        //float k = UniversePhysics.CoulombConstant; // 50
    //        //float qProd = 1.0f; // Carga 1 vs 1
    //        //float m = electron.MassMeV; // 0.511

    //        //float perfectSpeed = Mathf.Sqrt((k * qProd) / (m * r));

    //        //Debug.Log($"[Callosum] Velocidade Orbital Calculada: {perfectSpeed}");

    //        //// Aplica velocidade tangente (Para cima)
    //        //script.Velocity = Vector3.up * perfectSpeed;



    //        //// 1. Cria o Núcleo (Próton) no centro do Spawner
    //        //// Isso chama o método que cria os 3 Quarks vibrando
    //        //SpawnProton();

    //        //// Validação de segurança para não travar a Unity
    //        //if (electron == null)
    //        //{
    //        //    Debug.LogError("[Callosum] ERRO: Template do Elétron não foi arrastado para o Spawner!");
    //        //    return;
    //        //}

    //        //// 2. Define a posição inicial do Elétron
    //        //// r = 4.0f é uma boa distância visual para ver a órbita na tela
    //        //float r = 2.0f;
    //        //Vector3 startPos = transform.position + new Vector3(r, 0, 0); // Nasce à direita

    //        //// 3. Instancia o objeto visual (A esfera amarela)
    //        //GameObject eleObj = Instantiate(electron.VisualPrefab, startPos, Quaternion.identity);
    //        //eleObj.name = "Orbital Electron";

    //        //// 4. Injeta a Alma (O Script de Física)
    //        //var script = eleObj.AddComponent<ElementaryParticle>();
    //        //script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

    //        //// 5. CÁLCULO CIENTÍFICO DA VELOCIDADE
    //        //// Para não cair nem fugir, a força centrífuga deve igualar a atração elétrica.
    //        //// Fórmula: V = Raiz( (K * Carga1 * Carga2) / (Massa * Raio) )

    //        //float k = UniversePhysics.CoulombConstant; // Lê do seu arquivo UniversePhysics (ex: 1000)
    //        //float qProd = 1.0f;         // Carga do Próton (1) x Carga do Elétron (1)
    //        //float m = electron.MassMeV; // Massa do Elétron (ex: 0.511)

    //        //float perfectSpeed = Mathf.Sqrt((k * qProd) / (m * r));

    //        //Debug.Log($"[Callosum] Hidrogênio Criado | Raio: {r} | Vel: {perfectSpeed}");

    //        //// 6. APLICAÇÃO DO IMPULSO
    //        //// Se o elétron nasce na DIREITA (Eixo X), empurramos para CIMA (Eixo Y).
    //        //// Isso cria o vetor tangente necessário para girar.

    //        //// Dica: Usamos o valor puro. Se ele fugir por pouco, multiplique por 0.95f
    //        //script.Velocity = Vector3.up * (perfectSpeed * 0.7f); 

    //        //SpawnProton();
    //        //if (electron == null) return;

    //        //// --- MUDANÇA 1: DISTÂNCIA MÍNIMA ---
    //        //// Antes: 4.0f ou 2.0f.
    //        //// Agora: 0.8f (Muito perto! Quase tocando no núcleo)
    //        //float r = 0.8f;

    //        //Vector3 startPos = transform.position + new Vector3(r, 0, 0);

    //        //GameObject eleObj = Instantiate(electron.VisualPrefab, startPos, Quaternion.identity);
    //        //var script = eleObj.AddComponent<ElementaryParticle>();
    //        //script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

    //        //// Fórmula da Velocidade
    //        //float k = UniversePhysics.CoulombConstant;
    //        //float qProd = 1.0f;
    //        //float m = electron.MassMeV;

    //        //float perfectSpeed = Mathf.Sqrt((k * qProd) / (m * r));

    //        //// --- MUDANÇA 2: CORTE DE VELOCIDADE ---
    //        //// Multiplicamos por 0.8f. 
    //        //// Como ele já nasce perto, se dermos 100% da velocidade, ele pode querer abrir a órbita.
    //        //// Com 80%, ele fica preso nessa distância curta.
    //        //script.Velocity = Vector3.up * (perfectSpeed * 0.8f);



    //        SpawnProton();
    //        if (electron == null) return;

    //        // --- DISTÂNCIA MÉDIA ---
    //        // 0.8 foi perto demais. 4.0 foi longe demais.
    //        // Vamos usar 1.5f. É compacta e segura.
    //        float r = 1.5f;

    //        Vector3 startPos = transform.position + new Vector3(r, 0, 0);

    //        GameObject eleObj = Instantiate(electron.VisualPrefab, startPos, Quaternion.identity);
    //        var script = eleObj.AddComponent<ElementaryParticle>();
    //        script.Initialize(electron.Family, electron.Flavor, electron.MassMeV, electron.Charge);

    //        // Fórmula padrão
    //        float k = UniversePhysics.CoulombConstant;
    //        float qProd = 1.0f;
    //        float m = electron.MassMeV;

    //        float perfectSpeed = Mathf.Sqrt((k * qProd) / (m * r));

    //        // USE 100% DA VELOCIDADE
    //        // Como diminuímos o Softening, a física agora é mais "honesta".
    //        // Não precisamos de multiplicadores (ou use 0.95f no máximo).
    //        script.Velocity = Vector3.up * perfectSpeed;


    //    }

    //    // Método auxiliar para criar uma partícula simples parada
    //    private void CreateParticle(ParticleDefinition template, Vector3 pos)
    //    {
    //        GameObject newObj = Instantiate(template.VisualPrefab, pos, Quaternion.identity);
    //        newObj.name = template.ParticleName;

    //        var script = newObj.AddComponent<ElementaryParticle>();
    //        script.Initialize(template.Family, template.Flavor, template.MassMeV, template.Charge);
    //    }
    //}
}
