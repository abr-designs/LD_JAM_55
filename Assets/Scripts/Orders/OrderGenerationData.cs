using System;
using UnityEngine;

namespace Orders
{
#if UNITY_EDITOR
    [Serializable]
    public class OrderGenerationData
    {
        [SerializeField] private string name;
        [SerializeField] private OrderProfileScriptableObject profile;
        [SerializeField, Min(1)] private int ordersToGenerate;

        public Order[] GenerateOrders()
        {
            return profile.GenerateOrders(ordersToGenerate);
        }
    }
#endif
}