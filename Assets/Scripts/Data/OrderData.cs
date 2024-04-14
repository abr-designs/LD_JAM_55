using System;
using Enums;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Order
    {
        [Serializable]
        public struct OrderData
        {
            public COLOR color;
            [Min(0)]
            public int count;
        }
        
        [SerializeField]
        private string name;

        [SerializeField, Min(0)]
        public int collectibleDrops;

        [SerializeField, Min(0f)]
        public float orderTime;

        public OrderData[] orderDatas;

    }
}