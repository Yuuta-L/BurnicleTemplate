//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2026-07-16 11:33:25.418
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using BurnicleFramework.Runtime;


namespace BurnicleFramework.Generated
{
    /// <summary>
    /// Item表。
    /// </summary>
    public class DRItem : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取物品名。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取和水晶的价值比。
        /// </summary>
        public float ValueRatio
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取物品多语言key。
        /// </summary>
        public string ItemNameKey
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取物品描述key。
        /// </summary>
        public string ItemDesKey
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取图标资源名。
        /// </summary>
        public string IconAssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取默认品质。
        /// </summary>
        public Template.Tiers DefaultTier
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否显示在背包中。
        /// </summary>
        public bool ShowInInventory
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否完美品质。
        /// </summary>
        public bool IsPerfect
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableParser.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableParser.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
			Name = columnStrings[index++];
			float.TryParse(columnStrings[index++],out var tempValueRatio); ValueRatio = tempValueRatio;
			ItemNameKey = columnStrings[index++];
			ItemDesKey = columnStrings[index++];
			IconAssetName = columnStrings[index++];
			DefaultTier = columnStrings[index++].EnumParse<Template.Tiers>();
			bool.TryParse(columnStrings[index++],out var tempShowInInventory); ShowInInventory = tempShowInInventory;
			bool.TryParse(columnStrings[index++],out var tempIsPerfect); IsPerfect = tempIsPerfect;
            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    Name = binaryReader.ReadString();
                    ValueRatio = binaryReader.ReadSingle();
                    ItemNameKey = binaryReader.ReadString();
                    ItemDesKey = binaryReader.ReadString();
                    IconAssetName = binaryReader.ReadString();
					DefaultTier = (Template.Tiers)binaryReader.Read7BitEncodedInt32();
                    ShowInInventory = binaryReader.ReadBoolean();
                    IsPerfect = binaryReader.ReadBoolean();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
