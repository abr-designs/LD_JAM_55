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

        public OrderData[] orderDatas;

    }
}