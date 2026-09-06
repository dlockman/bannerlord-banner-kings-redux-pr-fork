using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static BannerKings.Managers.PopulationManager;

namespace BannerKings.Utils
{
    /// <summary>
    /// Soft-integration bridge to Fourberie.
    ///
    /// Mechanism: BK pulls _stringClanDico from Fourberie to react to or modify grudges.
    ///
    /// BK incorporates grudges into deciding who their rival is in Vassal or Ruler Politics Behavior
    public static class FourberieBridge
    {
        private static bool _resolved;
        private static Dictionary<string, int> _grudgeDict;

        /// <summary>True if Fourberie is loaded and it can access needed functionality.</summary>
        public static bool Available
        {
            get
            {
                if (!ModCompat.Fourberie) return false;
                Resolve();
                return _grudgeDict != null;
            }
        }
        /// <summary>Returns grudge value against the player for the given clan.</summary>
        public static int GetExistingGrudge(Clan clan)
        {
            _grudgeDict.TryGetValue(clan.StringId, out int existingGrudge);
            return existingGrudge;
        }

        /// <summary>Sets grudge value against the player for the given clan.</summary>
        public static void SetGrudgeValue(Clan clan, int amount)
        {
            _grudgeDict[clan.StringId] = amount;
        }

        private static void Resolve()
        {
            if (_resolved) return;
            _resolved = true;
            try
            {
                Type fourberieBehaviorType = AccessTools.TypeByName("FourberieBehavior");
                _grudgeDict = Traverse.Create(fourberieBehaviorType).Field("_stringClanDico").GetValue<Dictionary<string, int>>();
            }
            catch
            {
                // Fourberie internal layout changed — leave the methods null so callers
                // degrade to independent BK behaviour.
            }
        }
    }
}
