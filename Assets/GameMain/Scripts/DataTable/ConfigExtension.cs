//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2026-09-09 15:07:27.864
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BurnicleFramework.Runtime;
using UnityEngine;

namespace BurnicleFramework.Generated
{
    public static class ConfigExtension
    {
        #region 公共解析方法

        private static T[] ParseArray<T>(string rawValue, Func<string, T> parser)
        {
            if (string.IsNullOrEmpty(rawValue))
                return new T[0];

            string[] items = rawValue.Split(',');
            T[] result = new T[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                result[i] = parser(items[i].Trim());
            }

            return result;
        }
        
        private static Vector2 ParseVector2(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return Vector2.zero;

            string[] items = rawValue.Split('-');
            var item1 = float.Parse(items[0]);
            if (items.Length >= 2)
            {
                return new Vector2(item1, float.Parse(items[1]));
            }
            return new Vector2(item1, item1);
        }
        
        private static Vector2Int ParseVector2Int(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return Vector2Int.zero;

            string[] items = rawValue.Split('-');
            var item1 = int.Parse(items[0]);
            if (items.Length >= 2)
            {
                return new Vector2Int(item1, int.Parse(items[1]));
            }
            return new Vector2Int(item1, item1);
        }
        
        private static Vector3 ParseVector3(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return Vector3.zero;

            string[] items = rawValue.Split('-');
            var item1 = float.Parse(items[0]);
            if (items.Length >= 3)
            {
                return new Vector3(item1, float.Parse(items[1]), float.Parse(items[2]));
            }
            return new Vector3(item1, item1, item1);
        }
        
        private static Vector3Int ParseVector3Int(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return Vector3Int.zero;

            string[] items = rawValue.Split('-');
            var item1 = int.Parse(items[0]);
            if (items.Length >= 3)
            {
                return new Vector3Int(item1, int.Parse(items[1]), int.Parse(items[2]));
            }
            return new Vector3Int(item1, item1, item1);
        }
        
        private static Vector4 ParseVector4(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return Vector4.zero;

            string[] items = rawValue.Split('-');
            var x = int.Parse(items[0]);
            var y = int.Parse(items[1]);
            var z = int.Parse(items[2]);
            var w = int.Parse(items[3]);
            return new Vector4(x, y, z, w);
        }
        
        private static Vector4Int ParseVector4Int(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return new Vector4Int(0, 0, 0, 0);

            string[] items = rawValue.Split('-');
            return new Vector4Int(
                int.Parse(items[0]),
                int.Parse(items[1]),
                int.Parse(items[2]),
                int.Parse(items[3]));
        }

        private static Dictionary<TKey, TValue> ParseDictionary<TKey, TValue>(string rawValue, Func<string, TKey> keyParser, Func<string, TValue> valueParser)
        {
            if (string.IsNullOrEmpty(rawValue))
                return new Dictionary<TKey, TValue>();

            string[] pairs = rawValue.Split(',');
            var dict = new Dictionary<TKey, TValue>(pairs.Length);

            foreach (string pair in pairs)
            {
                string trimmedPair = pair.Trim();
                if (string.IsNullOrEmpty(trimmedPair))
                    continue;

                string[] keyValue = trimmedPair.Split('|');
                if (keyValue.Length != 2)
                    continue;

                TKey key = keyParser(keyValue[0].Trim());
                TValue value = valueParser(keyValue[1].Trim());
                dict[key] = value;
            }

            return dict;
        }

        #endregion

        #region 公式解析辅助方法

        /// <summary>
        /// 提取公式中的参数
        /// </summary>
        private static List<string> ExtractFormulaParameters(string formula)
        {
            var parameters = new List<string>();
            if (string.IsNullOrEmpty(formula))
                return parameters;

            // 匹配公式中的参数（单个小写字母）
            var matches = Regex.Matches(formula, @"\b[a-z]\b");
            foreach (Match match in matches)
            {
                string param = match.Value;
                if (!parameters.Contains(param))
                    parameters.Add(param);
            }

            return parameters;
        }

        /// <summary>
        /// 缓存已解析的表达式，提高性能
        /// </summary>
        private static readonly Dictionary<string, MathExpression> _expressionCache = new Dictionary<string, MathExpression>();

        /// <summary>
        /// 执行公式计算（使用参数赋值方式）
        /// </summary>
        private static float EvaluateFormula(string formula, Dictionary<string, float> parameters)
        {
            if (string.IsNullOrEmpty(formula))
                return 0f;

            // 尝试从缓存获取
            if (!_expressionCache.TryGetValue(formula, out var expression))
            {
                expression = MathExpressionHelper.TryEvaluate(formula);
                if (expression != null)
                {
                    _expressionCache[formula] = expression;
                }
                else
                {
                    return 0f;
                }
            }

            // 为表达式参数赋值
            foreach (var param in parameters)
            {
                if (expression.Parameters.ContainsKey(param.Key))
                {
                    expression.Parameters[param.Key].Value = param.Value;
                }
                else
                {
                    // 如果公式中不包含该参数，可能是动态生成的表达式
                    // 重新解析并添加参数
                    _expressionCache.Remove(formula);
                    return EvaluateFormula(formula, parameters);
                }
            }

            return (float)expression.Value;
        }
        
        #endregion

        #region 配置常量定义
        private const string ReinforceLevelMaxKey = "ReinforceLevelMax";
        private const string DefaultEnergyMaxKey = "DefaultEnergyMax";
        private const string RecoverTimePerEnergyKey = "RecoverTimePerEnergy";
        private const string DefaultManureMaxKey = "DefaultManureMax";
        private const string RecoverTimePerManureKey = "RecoverTimePerManure";
        private const string DefaultMineTicketMaxKey = "DefaultMineTicketMax";
        private const string RecoverTimePerMineTicketKey = "RecoverTimePerMineTicket";
        private const string StartGameEnergyCostKey = "StartGameEnergyCost";
        private const string EnergyPackRecoverTimeKey = "EnergyPackRecoverTime";
        private const string EnergyPackRecoverValueKey = "EnergyPackRecoverValue";
        private const string DefaultFieldCountKey = "DefaultFieldCount";
        private const string VideoAdCooldownKey = "VideoAdCooldown";
        private const string FirstRoundInfoTemplateKey = "FirstRoundInfoTemplate";
        private const string RoundInfoTemplateKey = "RoundInfoTemplate";
        private const string CoinModeTemplateKey = "CoinModeTemplate";
        private const string GearUpgradeCoinKey = "GearUpgradeCoin";
        private const string GearUpgradeBlueprintKey = "GearUpgradeBlueprint";
        private const string LevelUpExpKey = "LevelUpExp";
        private const string RewardTotalValueKey = "RewardTotalValue";
        private const string REPlantLuckyLootKey = "REPlantLuckyLoot";
        private const string REMineLuckyLootKey = "REMineLuckyLoot";
        private const string REBlacksmithLuckyLootKey = "REBlacksmithLuckyLoot";
        private const string CommonChestLootListKey = "CommonChestLootList";
        private const string RareChestExtraLootListKey = "RareChestExtraLootList";
        private const string SeedGrowTimeKey = "SeedGrowTime";
        private const string SeedOutput1Key = "SeedOutput1";
        private const string SeedOutput2Key = "SeedOutput2";
        private const string SeedOutput3Key = "SeedOutput3";
        private const string SeedOutput4Key = "SeedOutput4";
        private const string LevelStrength_HpKey = "LevelStrength_Hp";
        private const string LevelStrength_AtkKey = "LevelStrength_Atk";
        private const string RoomStrengthKey = "RoomStrength";
        private const string EnemyEvasionFormulaKey = "EnemyEvasionFormula";
        private const string EliteStrengthKey = "EliteStrength";
        private const string EliteAspectNumberKey = "EliteAspectNumber";
        private const string BossAttackStrengthKey = "BossAttackStrength";
        private const string GoblinCrystalLootKey = "GoblinCrystalLoot";
        private const string PassiveTierWeightsKey = "PassiveTierWeights";
        private const string DemonCostRangeKey = "DemonCostRange";
        private const string CommonPassiveRangeKey = "CommonPassiveRange";
        private const string TrainPassiveRangeKey = "TrainPassiveRange";
        private const string MushroomPassiveRangeKey = "MushroomPassiveRange";
        private const string OmenStatuRangeKey = "OmenStatuRange";
        private const string RestEventHealPercentKey = "RestEventHealPercent";
        private const string DailyAdProductViewsMaxKey = "DailyAdProductViewsMax";
        private const string SliverChestFreeIntervalKey = "SliverChestFreeInterval";
        private const string ObsidianChestFreeIntervalKey = "ObsidianChestFreeInterval";
        private const string SliverChestLootListKey = "SliverChestLootList";
        private const string ObsidianChestLootListKey = "ObsidianChestLootList";
        private const string RainbowChestLootListKey = "RainbowChestLootList";
        private const string ShopChestEpicPityCountKey = "ShopChestEpicPityCount";
        private const string ShopChestSEpicPityCountKey = "ShopChestSEpicPityCount";
        private const string RelicBoxLootListKey = "RelicBoxLootList";
        private const string RelicBoxMythicLootListKey = "RelicBoxMythicLootList";
        private const string RelicPieceConversionKey = "RelicPieceConversion";
        private const string ShopRelicEpicPityCountKey = "ShopRelicEpicPityCount";
        private const string ShopRelicMythicPityCountKey = "ShopRelicMythicPityCount";
        private const string LevelRoomCountKey = "LevelRoomCount";
        private const string UnlockGemHoleRequiredNumberKey = "UnlockGemHoleRequiredNumber";
        private const string LevelNamesKey = "LevelNames";
        private const string LockedRoomFixedEventsKey = "LockedRoomFixedEvents";
        private const string RuneRoomFixedEventsKey = "RuneRoomFixedEvents";
        private const string MinionDropPotionProbabilityKey = "MinionDropPotionProbability";
        private const string EliteDropKeyProbabilityKey = "EliteDropKeyProbability";
        private const string BaseDefenseEfficiencyFactorKey = "BaseDefenseEfficiencyFactor";
        private const string DefenseEfficiencyFactorFormulaKey = "DefenseEfficiencyFactorFormula";
        private const string BaseEvasionEfficiencyFactorKey = "BaseEvasionEfficiencyFactor";
        private const string EvasionEfficiencyFactorFormulaKey = "EvasionEfficiencyFactorFormula";
        private const string GrowUpAttrOptionsKey = "GrowUpAttrOptions";
        private const string BreakAttrOptionsKey = "BreakAttrOptions";
        private const string GrowUpCostFormulaKey = "GrowUpCostFormula";
        private const string MysteryBoxCostKey = "MysteryBoxCost";
        private const string DailyRewardListKey = "DailyRewardList";
        private const string BlessingAdViewsKey = "BlessingAdViews";
        private const string SweepValueRateKey = "SweepValueRate";
        private const string SweepBaseTimesKey = "SweepBaseTimes";
        private const string ShopCoinIncKey = "ShopCoinInc";
        private const string GoldenTicketRewardListKey = "GoldenTicketRewardList";
        private const string ShopWishPityCountKey = "ShopWishPityCount";
        private const string MiningTimeKey = "MiningTime";
        private const string MineTypeWeightsKey = "MineTypeWeights";
        private const string MineLevelWeightsKey = "MineLevelWeights";
        private const string CrystalMineSpawnKey = "CrystalMineSpawn";
        private const string CoinMineValueSpawnKey = "CoinMineValueSpawn";
        private const string DeepMiningMaxProgressKey = "DeepMiningMaxProgress";
        private const string TraderGiftListKey = "TraderGiftList";
        private const string RelicUpgradePieceNumKey = "RelicUpgradePieceNum";
        private const string DragonTreasureRewardValueKey = "DragonTreasureRewardValue";
        private const string DragonTreasureLevelMapKey = "DragonTreasureLevelMap";
        private const string GemEchoMaxLevelKey = "GemEchoMaxLevel";
        private const string GemEchoAttrBonusPerLevelKey = "GemEchoAttrBonusPerLevel";
        private const string GemEchoBlessingMaxKey = "GemEchoBlessingMax";
        private const string GemEchoBlessingBonusRateKey = "GemEchoBlessingBonusRate";
        private const string GemEchoBaseEchoStoneCostKey = "GemEchoBaseEchoStoneCost";
        private const string GemEchoBaseCoinCostKey = "GemEchoBaseCoinCost";
        private const string GemEchoBaseRateFormulaKey = "GemEchoBaseRateFormula";
        private const string GemEchoBlessingGainFormulaKey = "GemEchoBlessingGainFormula";
        private const string GemEchoMinBlessingFormulaKey = "GemEchoMinBlessingFormula";
        private const string SeedManorCoinKey = "SeedManorCoin";
        private const string MineManorCoinKey = "MineManorCoin";
        private const string AdventureTicketMaxKey = "AdventureTicketMax";
        private const string ShortcutGiftRewardListKey = "ShortcutGiftRewardList";
        #endregion

        #region 配置获取方法
        /// <summary>
        /// 获取配置：AdventureTicketMax
        /// 原始值：3
        /// </summary>
        public static int GetAdventureTicketMax(this ConfigComponent component)
        {
            return component.GetInt(AdventureTicketMaxKey);
        }

        /// <summary>
        /// 获取配置：BaseDefenseEfficiencyFactor
        /// 原始值：350
        /// </summary>
        public static int GetBaseDefenseEfficiencyFactor(this ConfigComponent component)
        {
            return component.GetInt(BaseDefenseEfficiencyFactorKey);
        }

        /// <summary>
        /// 获取配置：BaseEvasionEfficiencyFactor
        /// 原始值：700
        /// </summary>
        public static int GetBaseEvasionEfficiencyFactor(this ConfigComponent component)
        {
            return component.GetInt(BaseEvasionEfficiencyFactorKey);
        }

        /// <summary>
        /// 获取配置：BlessingAdViews
        /// 原始值：10,30,80,150,300,500,800,1200,2000,5000
        /// </summary>
        public static int[] GetBlessingAdViews(this ConfigComponent component)
        {
            string rawValue = component.GetString(BlessingAdViewsKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：BreakAttrOptions
        /// 原始值：1001-10,1005-5,1002-150,1001-10,1004-5,1002-150,1005-5,1001-10,1002-150,1004-5,1001-10,1002-150,1002-150,1005-5,1001-10,1002-150,1004-5,1001-10
        /// </summary>
        public static Vector2Int[] GetBreakAttrOptions(this ConfigComponent component)
        {
            string rawValue = component.GetString(BreakAttrOptionsKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：CoinMineValueSpawn
        /// 原始值：1.5-1.8,2-2.5,3-3.5,4.5-5,7.5-8
        /// </summary>
        public static Vector2[] GetCoinMineValueSpawn(this ConfigComponent component)
        {
            string rawValue = component.GetString(CoinMineValueSpawnKey);
            return ParseArray(rawValue, ParseVector2);
        }

        /// <summary>
        /// 获取配置：CoinModeTemplate
        /// 原始值：1,2001,1,2004,2,1002
        /// </summary>
        public static int[] GetCoinModeTemplate(this ConfigComponent component)
        {
            string rawValue = component.GetString(CoinModeTemplateKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：CommonChestLootList
        /// 原始值：1005-100,4001-25,4002-25,5001-100,5002-25
        /// </summary>
        public static Vector2Int[] GetCommonChestLootList(this ConfigComponent component)
        {
            string rawValue = component.GetString(CommonChestLootListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：CommonPassiveRange
        /// 原始值：1001-9999
        /// </summary>
        public static Vector2Int GetCommonPassiveRange(this ConfigComponent component)
        {
            return ParseVector2Int(component.GetString(CommonPassiveRangeKey));
        }

        /// <summary>
        /// 获取配置：CrystalMineSpawn
        /// 原始值：6-8,12-15,22-30,65-70,150-160
        /// </summary>
        public static Vector2Int[] GetCrystalMineSpawn(this ConfigComponent component)
        {
            string rawValue = component.GetString(CrystalMineSpawnKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：DailyAdProductViewsMax
        /// 原始值：1001|10000-3,1002|150-3,1003|30-3,1004|50-3,1006|20-10
        /// </summary>
        public static Dictionary<int, Vector2Int> GetDailyAdProductViewsMax(this ConfigComponent component)
        {
            string rawValue = component.GetString(DailyAdProductViewsMaxKey);
            return ParseDictionary(rawValue, int.Parse, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：DailyRewardList
        /// 原始值：1001-2000-0,1002-200-1,4003-2-1,5001-20-0,2002-1-1,1002-200-1,1005-30-0,1001-16888-1,1002-240-1,4002-4-0,1007-1-1,4004-1-1,1001-16888-1,1002-300-1,2003-1-1,5001-30-1
        /// </summary>
        public static Vector3Int[] GetDailyRewardList(this ConfigComponent component)
        {
            string rawValue = component.GetString(DailyRewardListKey);
            return ParseArray(rawValue, ParseVector3Int);
        }

        /// <summary>
        /// 获取配置：DeepMiningMaxProgress
        /// 原始值：80
        /// </summary>
        public static int GetDeepMiningMaxProgress(this ConfigComponent component)
        {
            return component.GetInt(DeepMiningMaxProgressKey);
        }

        /// <summary>
        /// 获取配置：DefaultEnergyMax
        /// 原始值：40
        /// </summary>
        public static int GetDefaultEnergyMax(this ConfigComponent component)
        {
            return component.GetInt(DefaultEnergyMaxKey);
        }

        /// <summary>
        /// 获取配置：DefaultFieldCount
        /// 原始值：2
        /// </summary>
        public static int GetDefaultFieldCount(this ConfigComponent component)
        {
            return component.GetInt(DefaultFieldCountKey);
        }

        /// <summary>
        /// 获取配置：DefaultManureMax
        /// 原始值：100
        /// </summary>
        public static int GetDefaultManureMax(this ConfigComponent component)
        {
            return component.GetInt(DefaultManureMaxKey);
        }

        /// <summary>
        /// 获取配置：DefaultMineTicketMax
        /// 原始值：30
        /// </summary>
        public static int GetDefaultMineTicketMax(this ConfigComponent component)
        {
            return component.GetInt(DefaultMineTicketMaxKey);
        }

        /// <summary>
        /// 获取配置：DemonCostRange
        /// 原始值：0.1-0.15,0.15 -0.2,0.2 -0.25,0.25-0.3
        /// </summary>
        public static Vector2[] GetDemonCostRange(this ConfigComponent component)
        {
            string rawValue = component.GetString(DemonCostRangeKey);
            return ParseArray(rawValue, ParseVector2);
        }

        /// <summary>
        /// 获取配置：DragonTreasureLevelMap
        /// 原始值：10,13,17,23,30,41,57,79,109,150,181,219,265,320,387,468,566,684,827,1000
        /// </summary>
        public static int[] GetDragonTreasureLevelMap(this ConfigComponent component)
        {
            string rawValue = component.GetString(DragonTreasureLevelMapKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：EliteAspectNumber
        /// 原始值：15|1-1,30|1-2,40|1-3,60|2-3,80|2-4,100|3-4,999|4
        /// </summary>
        public static Dictionary<int, Vector2Int> GetEliteAspectNumber(this ConfigComponent component)
        {
            string rawValue = component.GetString(EliteAspectNumberKey);
            return ParseDictionary(rawValue, int.Parse, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：EliteDropKeyProbability
        /// 原始值：35
        /// </summary>
        public static int GetEliteDropKeyProbability(this ConfigComponent component)
        {
            return component.GetInt(EliteDropKeyProbabilityKey);
        }

        /// <summary>
        /// 获取配置：EliteStrength
        /// 原始值：2-2.05,1.3-1.35
        /// </summary>
        public static Vector2[] GetEliteStrength(this ConfigComponent component)
        {
            string rawValue = component.GetString(EliteStrengthKey);
            return ParseArray(rawValue, ParseVector2);
        }

        /// <summary>
        /// 获取配置：EnergyPackRecoverTime
        /// 原始值：28800
        /// </summary>
        public static int GetEnergyPackRecoverTime(this ConfigComponent component)
        {
            return component.GetInt(EnergyPackRecoverTimeKey);
        }

        /// <summary>
        /// 获取配置：EnergyPackRecoverValue
        /// 原始值：30
        /// </summary>
        public static int GetEnergyPackRecoverValue(this ConfigComponent component)
        {
            return component.GetInt(EnergyPackRecoverValueKey);
        }

        /// <summary>
        /// 获取配置：FirstRoundInfoTemplate
        /// 原始值：1001,1001,2001,1001,1002,1,1001,1001,2,1002
        /// </summary>
        public static int[] GetFirstRoundInfoTemplate(this ConfigComponent component)
        {
            string rawValue = component.GetString(FirstRoundInfoTemplateKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：GemEchoAttrBonusPerLevel
        /// 原始值：1,1,1,1,3,3,3,3,5,5,5,5,7,7,7,7,9,9,9,9
        /// </summary>
        public static int[] GetGemEchoAttrBonusPerLevel(this ConfigComponent component)
        {
            string rawValue = component.GetString(GemEchoAttrBonusPerLevelKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：GemEchoBlessingBonusRate
        /// 原始值：5
        /// </summary>
        public static int GetGemEchoBlessingBonusRate(this ConfigComponent component)
        {
            return component.GetInt(GemEchoBlessingBonusRateKey);
        }

        /// <summary>
        /// 获取配置：GemEchoBlessingMax
        /// 原始值：100
        /// </summary>
        public static int GetGemEchoBlessingMax(this ConfigComponent component)
        {
            return component.GetInt(GemEchoBlessingMaxKey);
        }

        /// <summary>
        /// 获取配置：GemEchoMaxLevel
        /// 原始值：20
        /// </summary>
        public static int GetGemEchoMaxLevel(this ConfigComponent component)
        {
            return component.GetInt(GemEchoMaxLevelKey);
        }

        /// <summary>
        /// 获取配置：GoblinCrystalLoot
        /// 原始值：60,6,10
        /// </summary>
        public static int[] GetGoblinCrystalLoot(this ConfigComponent component)
        {
            string rawValue = component.GetString(GoblinCrystalLootKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：GoldenTicketRewardList
        /// 原始值：2003-1,2003-1,1002-400
        /// </summary>
        public static Vector2Int[] GetGoldenTicketRewardList(this ConfigComponent component)
        {
            string rawValue = component.GetString(GoldenTicketRewardListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：GrowUpAttrOptions
        /// 原始值：1001-2,1005-1,1002-30, 1001-2,1004-1,1002-30
        /// </summary>
        public static Vector2Int[] GetGrowUpAttrOptions(this ConfigComponent component)
        {
            string rawValue = component.GetString(GrowUpAttrOptionsKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：LevelNames
        /// 原始值：初入地牢,潮湿洞穴,城堡废墟, 落石通道, 三岔路口, 暗河渡口, 迷雾森林, 破碎石桥, 远古守卫, 深渊之下
        /// </summary>
        public static string[] GetLevelNames(this ConfigComponent component)
        {
            string rawValue = component.GetString(LevelNamesKey);
            return ParseArray(rawValue, s => s);
        }

        /// <summary>
        /// 获取配置：LevelUpExp
        /// 原始值：100,120,140,160,210,210,260,320,400,480,560,600
        /// </summary>
        public static int[] GetLevelUpExp(this ConfigComponent component)
        {
            string rawValue = component.GetString(LevelUpExpKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：LockedRoomFixedEvents
        /// 原始值：2002|0-1,2003|0-1,3001|1-1
        /// </summary>
        public static Dictionary<int, Vector2Int> GetLockedRoomFixedEvents(this ConfigComponent component)
        {
            string rawValue = component.GetString(LockedRoomFixedEventsKey);
            return ParseDictionary(rawValue, int.Parse, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：MineLevelWeights
        /// 原始值：1-300,2-240,3-100,4-20,5-4
        /// </summary>
        public static Vector2Int[] GetMineLevelWeights(this ConfigComponent component)
        {
            string rawValue = component.GetString(MineLevelWeightsKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：MineManorCoin
        /// 原始值：1,2,4,10,20
        /// </summary>
        public static int[] GetMineManorCoin(this ConfigComponent component)
        {
            string rawValue = component.GetString(MineManorCoinKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：MineTypeWeights
        /// 原始值：0-50,1-50,2-22,3-50,4-30
        /// </summary>
        public static Vector2Int[] GetMineTypeWeights(this ConfigComponent component)
        {
            string rawValue = component.GetString(MineTypeWeightsKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：MiningTime
        /// 原始值：15,30,45,80,120
        /// </summary>
        public static int[] GetMiningTime(this ConfigComponent component)
        {
            string rawValue = component.GetString(MiningTimeKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：MinionDropPotionProbability
        /// 原始值：10
        /// </summary>
        public static int GetMinionDropPotionProbability(this ConfigComponent component)
        {
            return component.GetInt(MinionDropPotionProbabilityKey);
        }

        /// <summary>
        /// 获取配置：MushroomPassiveRange
        /// 原始值：20001-29999
        /// </summary>
        public static Vector2Int GetMushroomPassiveRange(this ConfigComponent component)
        {
            return ParseVector2Int(component.GetString(MushroomPassiveRangeKey));
        }

        /// <summary>
        /// 获取配置：MysteryBoxCost
        /// 原始值：10,100,200,400,400,400
        /// </summary>
        public static int[] GetMysteryBoxCost(this ConfigComponent component)
        {
            string rawValue = component.GetString(MysteryBoxCostKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：ObsidianChestFreeInterval
        /// 原始值：82800
        /// </summary>
        public static int GetObsidianChestFreeInterval(this ConfigComponent component)
        {
            return component.GetInt(ObsidianChestFreeIntervalKey);
        }

        /// <summary>
        /// 获取配置：ObsidianChestLootList
        /// 原始值：5004-383,5005-225,5006-12
        /// </summary>
        public static Vector2Int[] GetObsidianChestLootList(this ConfigComponent component)
        {
            string rawValue = component.GetString(ObsidianChestLootListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：OmenStatuRange
        /// 原始值：3003-3009
        /// </summary>
        public static Vector2Int GetOmenStatuRange(this ConfigComponent component)
        {
            return ParseVector2Int(component.GetString(OmenStatuRangeKey));
        }

        /// <summary>
        /// 获取配置：PassiveTierWeights
        /// 原始值：50,50,30,20
        /// </summary>
        public static int[] GetPassiveTierWeights(this ConfigComponent component)
        {
            string rawValue = component.GetString(PassiveTierWeightsKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：RainbowChestLootList
        /// 原始值：5004-325,5005-215,5006-12,5007-8
        /// </summary>
        public static Vector2Int[] GetRainbowChestLootList(this ConfigComponent component)
        {
            string rawValue = component.GetString(RainbowChestLootListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：RareChestExtraLootList
        /// 原始值：1002-300-100,4003-3-100,4004-1-50,2002-2-25,2003-1-25,1007-1-25
        /// </summary>
        public static Vector3Int[] GetRareChestExtraLootList(this ConfigComponent component)
        {
            string rawValue = component.GetString(RareChestExtraLootListKey);
            return ParseArray(rawValue, ParseVector3Int);
        }

        /// <summary>
        /// 获取配置：REBlacksmithLuckyLoot
        /// 原始值：15-2001-1
        /// </summary>
        public static Vector3Int GetREBlacksmithLuckyLoot(this ConfigComponent component)
        {
            return ParseVector3Int(component.GetString(REBlacksmithLuckyLootKey));
        }

        /// <summary>
        /// 获取配置：RecoverTimePerEnergy
        /// 原始值：900
        /// </summary>
        public static int GetRecoverTimePerEnergy(this ConfigComponent component)
        {
            return component.GetInt(RecoverTimePerEnergyKey);
        }

        /// <summary>
        /// 获取配置：RecoverTimePerManure
        /// 原始值：480
        /// </summary>
        public static int GetRecoverTimePerManure(this ConfigComponent component)
        {
            return component.GetInt(RecoverTimePerManureKey);
        }

        /// <summary>
        /// 获取配置：RecoverTimePerMineTicket
        /// 原始值：1800
        /// </summary>
        public static int GetRecoverTimePerMineTicket(this ConfigComponent component)
        {
            return component.GetInt(RecoverTimePerMineTicketKey);
        }

        /// <summary>
        /// 获取配置：ReinforceLevelMax
        /// 原始值：500
        /// </summary>
        public static int GetReinforceLevelMax(this ConfigComponent component)
        {
            return component.GetInt(ReinforceLevelMaxKey);
        }

        /// <summary>
        /// 获取配置：RelicBoxLootList
        /// 原始值：30-897,40-92,70-10,110-1
        /// </summary>
        public static Vector2Int[] GetRelicBoxLootList(this ConfigComponent component)
        {
            string rawValue = component.GetString(RelicBoxLootListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：RelicBoxMythicLootList
        /// 原始值：70-10,110-1
        /// </summary>
        public static Vector2Int[] GetRelicBoxMythicLootList(this ConfigComponent component)
        {
            string rawValue = component.GetString(RelicBoxMythicLootListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：RelicPieceConversion
        /// 原始值：10
        /// </summary>
        public static int GetRelicPieceConversion(this ConfigComponent component)
        {
            return component.GetInt(RelicPieceConversionKey);
        }

        /// <summary>
        /// 获取配置：RelicUpgradePieceNum
        /// 原始值：100,40,20,20
        /// </summary>
        public static int[] GetRelicUpgradePieceNum(this ConfigComponent component)
        {
            string rawValue = component.GetString(RelicUpgradePieceNumKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：REMineLuckyLoot
        /// 原始值：25-1002-30
        /// </summary>
        public static Vector3Int GetREMineLuckyLoot(this ConfigComponent component)
        {
            return ParseVector3Int(component.GetString(REMineLuckyLootKey));
        }

        /// <summary>
        /// 获取配置：REPlantLuckyLoot
        /// 原始值：10-4003-1
        /// </summary>
        public static Vector3Int GetREPlantLuckyLoot(this ConfigComponent component)
        {
            return ParseVector3Int(component.GetString(REPlantLuckyLootKey));
        }

        /// <summary>
        /// 获取配置：RestEventHealPercent
        /// 原始值：20
        /// </summary>
        public static int GetRestEventHealPercent(this ConfigComponent component)
        {
            return component.GetInt(RestEventHealPercentKey);
        }

        /// <summary>
        /// 获取配置：RoundInfoTemplate
        /// 原始值：1001,1001,2001,1001,1001,1,1001,1001,2,1002
        /// </summary>
        public static int[] GetRoundInfoTemplate(this ConfigComponent component)
        {
            string rawValue = component.GetString(RoundInfoTemplateKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：RuneRoomFixedEvents
        /// 原始值：2-2001-1-1,2-3001-1-1
        /// </summary>
        public static Vector4Int[] GetRuneRoomFixedEvents(this ConfigComponent component)
        {
            string rawValue = component.GetString(RuneRoomFixedEventsKey);
            return ParseArray(rawValue, ParseVector4Int);
        }

        /// <summary>
        /// 获取配置：SeedGrowTime
        /// 原始值：8,15,40,80
        /// </summary>
        public static int[] GetSeedGrowTime(this ConfigComponent component)
        {
            string rawValue = component.GetString(SeedGrowTimeKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：SeedManorCoin
        /// 原始值：1,3,16,40
        /// </summary>
        public static int[] GetSeedManorCoin(this ConfigComponent component)
        {
            string rawValue = component.GetString(SeedManorCoinKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：SeedOutput1
        /// 原始值：1001-1000-1500-70,5001-1-2-50,1005-1-2-25
        /// </summary>
        public static Vector4Int[] GetSeedOutput1(this ConfigComponent component)
        {
            string rawValue = component.GetString(SeedOutput1Key);
            return ParseArray(rawValue, ParseVector4Int);
        }

        /// <summary>
        /// 获取配置：SeedOutput2
        /// 原始值：1001-3200-4000-50,5001-3-4-50,5002-1-2-15,5003-1-1-15
        /// </summary>
        public static Vector4Int[] GetSeedOutput2(this ConfigComponent component)
        {
            string rawValue = component.GetString(SeedOutput2Key);
            return ParseArray(rawValue, ParseVector4Int);
        }

        /// <summary>
        /// 获取配置：SeedOutput3
        /// 原始值：1001-15000-18000-100,1002-120-150-100,2001-2-2-100,2002-1-1-20,5004-3-3-40,5006-1-1-5
        /// </summary>
        public static Vector4Int[] GetSeedOutput3(this ConfigComponent component)
        {
            string rawValue = component.GetString(SeedOutput3Key);
            return ParseArray(rawValue, ParseVector4Int);
        }

        /// <summary>
        /// 获取配置：SeedOutput4
        /// 原始值：1001-35000-40000-100,1002-350-400-150,2002-1-2-150,2003-1-1-50,1007-1-1-50,5006-1-1-10
        /// </summary>
        public static Vector4Int[] GetSeedOutput4(this ConfigComponent component)
        {
            string rawValue = component.GetString(SeedOutput4Key);
            return ParseArray(rawValue, ParseVector4Int);
        }

        /// <summary>
        /// 获取配置：ShopChestEpicPityCount
        /// 原始值：10
        /// </summary>
        public static int GetShopChestEpicPityCount(this ConfigComponent component)
        {
            return component.GetInt(ShopChestEpicPityCountKey);
        }

        /// <summary>
        /// 获取配置：ShopChestSEpicPityCount
        /// 原始值：30
        /// </summary>
        public static int GetShopChestSEpicPityCount(this ConfigComponent component)
        {
            return component.GetInt(ShopChestSEpicPityCountKey);
        }

        /// <summary>
        /// 获取配置：ShopRelicEpicPityCount
        /// 原始值：10
        /// </summary>
        public static int GetShopRelicEpicPityCount(this ConfigComponent component)
        {
            return component.GetInt(ShopRelicEpicPityCountKey);
        }

        /// <summary>
        /// 获取配置：ShopRelicMythicPityCount
        /// 原始值：60
        /// </summary>
        public static int GetShopRelicMythicPityCount(this ConfigComponent component)
        {
            return component.GetInt(ShopRelicMythicPityCountKey);
        }

        /// <summary>
        /// 获取配置：ShopWishPityCount
        /// 原始值：4
        /// </summary>
        public static int GetShopWishPityCount(this ConfigComponent component)
        {
            return component.GetInt(ShopWishPityCountKey);
        }

        /// <summary>
        /// 获取配置：ShortcutGiftRewardList
        /// 原始值：1002-200,1001-5000,1002-50
        /// </summary>
        public static Vector2Int[] GetShortcutGiftRewardList(this ConfigComponent component)
        {
            string rawValue = component.GetString(ShortcutGiftRewardListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：SliverChestFreeInterval
        /// 原始值：14400
        /// </summary>
        public static int GetSliverChestFreeInterval(this ConfigComponent component)
        {
            return component.GetInt(SliverChestFreeIntervalKey);
        }

        /// <summary>
        /// 获取配置：SliverChestLootList
        /// 原始值：5003-100,5004-20
        /// </summary>
        public static Vector2Int[] GetSliverChestLootList(this ConfigComponent component)
        {
            string rawValue = component.GetString(SliverChestLootListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：StartGameEnergyCost
        /// 原始值：5
        /// </summary>
        public static int GetStartGameEnergyCost(this ConfigComponent component)
        {
            return component.GetInt(StartGameEnergyCostKey);
        }

        /// <summary>
        /// 获取配置：SweepBaseTimes
        /// 原始值：5
        /// </summary>
        public static int GetSweepBaseTimes(this ConfigComponent component)
        {
            return component.GetInt(SweepBaseTimesKey);
        }

        /// <summary>
        /// 获取配置：SweepValueRate
        /// 原始值：1.8
        /// </summary>
        public static float GetSweepValueRate(this ConfigComponent component)
        {
            return component.GetFloat(SweepValueRateKey);
        }

        /// <summary>
        /// 获取配置：TraderGiftList
        /// 原始值：1002-100,1006-10,2001-3,4003-1,1008-40
        /// </summary>
        public static Vector2Int[] GetTraderGiftList(this ConfigComponent component)
        {
            string rawValue = component.GetString(TraderGiftListKey);
            return ParseArray(rawValue, ParseVector2Int);
        }

        /// <summary>
        /// 获取配置：TrainPassiveRange
        /// 原始值：10001-19999
        /// </summary>
        public static Vector2Int GetTrainPassiveRange(this ConfigComponent component)
        {
            return ParseVector2Int(component.GetString(TrainPassiveRangeKey));
        }

        /// <summary>
        /// 获取配置：UnlockGemHoleRequiredNumber
        /// 原始值：0,40,200,400,1200
        /// </summary>
        public static int[] GetUnlockGemHoleRequiredNumber(this ConfigComponent component)
        {
            string rawValue = component.GetString(UnlockGemHoleRequiredNumberKey);
            return ParseArray(rawValue, int.Parse);
        }

        /// <summary>
        /// 获取配置：VideoAdCooldown
        /// 原始值：0
        /// </summary>
        public static int GetVideoAdCooldown(this ConfigComponent component)
        {
            return component.GetInt(VideoAdCooldownKey);
        }

        /// <summary>
        /// 获取配置：BossAttackStrength
        /// 原始值：0.7+min(8,max(0,x-2))*0.2+min(50,max(x-10,0))*0.01
        /// </summary>
        public static float GetBossAttackStrength(this ConfigComponent component, float x)
        {
            string formula = component.GetString(BossAttackStrengthKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：DefenseEfficiencyFactorFormula
        /// 原始值：1+(x-1)*0.18
        /// </summary>
        public static float GetDefenseEfficiencyFactorFormula(this ConfigComponent component, float x)
        {
            string formula = component.GetString(DefenseEfficiencyFactorFormulaKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：DragonTreasureRewardValue
        /// 原始值：50+(x-1)*30
        /// </summary>
        public static float GetDragonTreasureRewardValue(this ConfigComponent component, float x)
        {
            string formula = component.GetString(DragonTreasureRewardValueKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：EnemyEvasionFormula
        /// 原始值：min(x-1,10)*20+max(0,x-10)*5
        /// </summary>
        public static float GetEnemyEvasionFormula(this ConfigComponent component, float x)
        {
            string formula = component.GetString(EnemyEvasionFormulaKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：EvasionEfficiencyFactorFormula
        /// 原始值：1+(x-1)*0.12
        /// </summary>
        public static float GetEvasionEfficiencyFactorFormula(this ConfigComponent component, float x)
        {
            string formula = component.GetString(EvasionEfficiencyFactorFormulaKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GearUpgradeBlueprint
        /// 原始值：min(x,20)+ceil(max(x-20,0)/5)*5
        /// </summary>
        public static float GetGearUpgradeBlueprint(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GearUpgradeBlueprintKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GearUpgradeCoin
        /// 原始值：1000+(x/5)*500
        /// </summary>
        public static float GetGearUpgradeCoin(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GearUpgradeCoinKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GemEchoBaseCoinCost
        /// 原始值：20000+min(max(x,0),5)*29500+min(max(x-5,0),5)*59000+min(max(x-10,0),5)*218000+min(max(x-15,0),5)*436875
        /// </summary>
        public static float GetGemEchoBaseCoinCost(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GemEchoBaseCoinCostKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GemEchoBaseEchoStoneCost
        /// 原始值：400+min(max(x,0),5)*45+min(max(x-5,0),5)*100+min(max(x-10,0),5)*220+min(max(x-15,0),5)*375
        /// </summary>
        public static float GetGemEchoBaseEchoStoneCost(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GemEchoBaseEchoStoneCostKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GemEchoBaseRateFormula
        /// 原始值：max(0.001,0.5-min(5,max(0,x))*0.04-max(x-5,0)*0.0214)
        /// </summary>
        public static float GetGemEchoBaseRateFormula(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GemEchoBaseRateFormulaKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GemEchoBlessingGainFormula
        /// 原始值：max(1,20-x*2)
        /// </summary>
        public static float GetGemEchoBlessingGainFormula(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GemEchoBlessingGainFormulaKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GemEchoMinBlessingFormula
        /// 原始值：min(15,5*floor(x/5))
        /// </summary>
        public static float GetGemEchoMinBlessingFormula(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GemEchoMinBlessingFormulaKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：GrowUpCostFormula
        /// 原始值：1000+x*200+max(x-30,0)*300+max(x-60,0)*100+max(x-100,0)*100+max(x-1000,0)*1000
        /// </summary>
        public static float GetGrowUpCostFormula(this ConfigComponent component, float x)
        {
            string formula = component.GetString(GrowUpCostFormulaKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：LevelRoomCount
        /// 原始值：20+min(x*10,20)
        /// </summary>
        public static float GetLevelRoomCount(this ConfigComponent component, float x)
        {
            string formula = component.GetString(LevelRoomCountKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：LevelStrength_Atk
        /// 原始值：0.7+min(x-1,1)*0.5+min(300,max(0,x-2))*rnd(0.85,0.87)+max(0,x-10)*rnd(0.1,0.11)+min(max(0,x-22),50)*rnd(0.4,0.42)+max(0,x-100)*rnd(0.2,0.21)
        /// </summary>
        public static float GetLevelStrength_Atk(this ConfigComponent component, float x)
        {
            string formula = component.GetString(LevelStrength_AtkKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：LevelStrength_Hp
        /// 原始值：0.5+min(x-1,1)*0.6+min(300,max(0,x-2))*rnd(3,3.2)+max(0,x-10)*rnd(1.2,1.3)+max(0,x-50)*2.5f+max(0,x-30)*0.4+max(0,x-100)*0.35+max(0,x-150)*0.2
        /// </summary>
        public static float GetLevelStrength_Hp(this ConfigComponent component, float x)
        {
            string formula = component.GetString(LevelStrength_HpKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：RewardTotalValue
        /// 原始值：30+min(10,x-1)*2+min(30,max(0,x-10))*1+min(60,max(0,x-40))*0.5+min(100,max(0,x-100))*0.3+max(0,x-200)*0.12
        /// </summary>
        public static float GetRewardTotalValue(this ConfigComponent component, float x)
        {
            string formula = component.GetString(RewardTotalValueKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：RoomStrength
        /// 原始值：1+pow(x, 2)*2.3*p
        /// </summary>
        public static float GetRoomStrength(this ConfigComponent component, float x, float p)
        {
            string formula = component.GetString(RoomStrengthKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            paramDict["p"] = p;
            return EvaluateFormula(formula, paramDict);
        }

        /// <summary>
        /// 获取配置：ShopCoinInc
        /// 原始值：1+(x-1)*0.025
        /// </summary>
        public static float GetShopCoinInc(this ConfigComponent component, float x)
        {
            string formula = component.GetString(ShopCoinIncKey);
            var paramDict = new Dictionary<string, float>();
            paramDict["x"] = x;
            return EvaluateFormula(formula, paramDict);
        }

        #endregion

    }
}