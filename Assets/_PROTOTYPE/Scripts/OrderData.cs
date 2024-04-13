using System;
using UnityEngine;

namespace _PROTOTYPE.Scripts
{
    [Serializable]
    public struct OrderData
    {
        public COLOR color;
        [Min(0)]
        public int count;
    }

    [Serializable]
    public class Order
    {
        [SerializeField]
        private string name;

        [SerializeField, Min(0)]
        public int reward;

        [SerializeField, Min(0f)]
        public float orderTime;

        public OrderData[] orderDatas;

    }
}