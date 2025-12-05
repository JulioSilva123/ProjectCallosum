using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ProjectCallosum.Scripts.Utils
{
    public static class CallosumUtils
    {
        // Define um prefixo padrão para filtrar no Console do Unity depois
        private const string PREFIX = "<b>[Callosum]</b> ";

        // --- 1. LOG SIMPLES ---
        // Em vez de Debug.Log("oi"), você usa CallosumUtils.Log("oi");
        public static void Log(object message)
        {
            Debug.Log($"{PREFIX}{message}");
        }

        // --- 2. LOG COM COR (Para destacar eventos importantes) ---
        // Ex: CallosumUtils.LogColor("Átomo criado!", "green");
        public static void LogColor(object message, string colorName)
        {
            Debug.Log($"{PREFIX}<color={colorName}>{message}</color>");
        }

        // --- 3. LOG DE AVISO (Amarelo no Unity) ---
        public static void LogWarning(object message)
        {
            Debug.LogWarning($"{PREFIX}<color=yellow>ATENÇÃO:</color> {message}");
        }

        // --- 4. LOG DE ERRO (Vermelho no Unity) ---
        public static void LogError(object message)
        {
            Debug.LogError($"{PREFIX}<color=red>ERRO CRÍTICO:</color> {message}");
        }
    }
}
