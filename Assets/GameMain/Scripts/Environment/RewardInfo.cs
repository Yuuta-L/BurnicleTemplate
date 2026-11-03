using System.Collections.Generic;
using BurnicleFramework.Generated;
using GameFramework;
using UnityEngine;

namespace Template
{
    public class RewardInfo : IReference
    {
        public WealthTypes RewardType;
        public int RewardId;
        public Tiers RewardTier;
        public int RewardLevel;
        public int Quantity;
        /// <summary>
        /// 是否为碎片形式：为 true 时表示该奖励是 RewardType 对应物品的碎片。
        /// </summary>
        public bool IsFragment;

        public static RewardInfo Create(WealthTypes rewardType, int rewardId, Tiers rewardTier, int rewardLevel, int quantity, bool isFragment = false)
        {
            var info = ReferencePool.Acquire<RewardInfo>();
            info.RewardType = rewardType;
            info.RewardId = rewardId;
            info.RewardTier = rewardTier;
            info.RewardLevel = rewardLevel;
            info.Quantity = quantity;
            info.IsFragment = isFragment;
            return info;
        }
        
        public static RewardInfo Create(EnumItem itemId, int quantity, bool isFragment = false)
        {
            return Create(WealthTypes.Item, (int)itemId, default, 0, quantity, isFragment);
        }
        
        public static RewardInfo Create(RewardInfo other, int quantity)
        {
            var info = Create(other.RewardType, other.RewardId, other.RewardTier, other.RewardLevel, quantity);
            info.IsFragment = other.IsFragment;
            return info;
        }

        public RewardCollection EnsureItems()
        {
            var rewards = new RewardCollection();
            rewards.AddReward(this);
            return rewards;
        }
        
        public void Clear()
        {
            RewardType = default;
            RewardId = 0;
            RewardTier = default;
            RewardLevel = 0;
            Quantity = 0;
            IsFragment = false;
        }
    }

    public class RewardCollection
    {
        public List<RewardInfo> Rewards = new List<RewardInfo>();
        public bool IsEmpty => Rewards.Count <= 0;

        public RewardCollection()
        {
        }
        
        public RewardCollection(Vector2Int[] items)
        {
            foreach (var item in items)
            {
                AddItem(item.x, item.y);
            }
        }
        
        public void AddReward(RewardInfo rewardInfo)
        {
            var existing = Rewards.Find(r => r.RewardType == rewardInfo.RewardType && r.RewardId == rewardInfo.RewardId && r.IsFragment == rewardInfo.IsFragment);
            if (existing != null)
            {
                existing.Quantity += rewardInfo.Quantity;
                ReferencePool.Release(rewardInfo);
                return;
            }

            Rewards.Add(rewardInfo);
        }

        public void RemoveReward(RewardInfo rewardInfo)
        {
            Rewards.Remove(rewardInfo);
        }
        
        public void AddItem(EnumItem itemType,int quantity)
        {
            AddItem((int)itemType, quantity);
        }
        
        public void AddItem(int itemId,int quantity)
        {
            foreach (var rewardInfo in Rewards)
            {
                if (rewardInfo.RewardType != WealthTypes.Item)
                {
                    continue;
                }
                
                if (rewardInfo.RewardId == itemId)
                {
                    rewardInfo.Quantity += quantity;
                    return;
                }
            }

            Rewards.Add(RewardInfo.Create(WealthTypes.Item, itemId, default, 0, quantity));
        }

        public void Combine(RewardCollection other)
        {
            var items = other.Rewards.ToArray();
            foreach (var rewardInfo in items)
            {
                AddReward(rewardInfo);
            }
            other.Rewards.Clear();
        }
        
        public void Clear()
        {
            foreach (var rewardInfo in Rewards)
            {
                ReferencePool.Release(rewardInfo);
            }
            
            Rewards.Clear();
        }

        public override string ToString()
        {
            var str = "";
            foreach (var rewardInfo in Rewards)
            {
                str += $"{rewardInfo.RewardType}-{rewardInfo.RewardId}-{rewardInfo.RewardTier}-{rewardInfo.Quantity}\n";
            }
            return str;
        }
    }
}