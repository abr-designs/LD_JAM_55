using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Enums;
using Managers;
using UnityEngine;

namespace Orders
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Order Profile", menuName = "ScriptableObjects/Order Profile", order = 1)]
    public class OrderProfileScriptableObject : ScriptableObject
    {
        [Serializable]
        public struct ColorData
        {
            public bool useColor;
            public COLOR color;
            public Vector2Int countRange;
            public AnimationCurve changeCurve;

            public int GetOrderCount(float t)
            {
                return Mathf.RoundToInt(Mathf.Lerp(countRange.x, countRange.y, changeCurve.Evaluate(t)));
            }
        }
        
        [Header("Colors")]
        public ColorData redData;
        public ColorData blueData;
        public ColorData greenData;
        public ColorData purpleData;

        [Header("Time")]
        public Vector2 timeRange;
        public AnimationCurve changeTimeCurve;
        
        [Header("Rewards")]
        public Vector2Int rewardsRange;
        public AnimationCurve changeRewardsCurve;

        //============================================================================================================//

        public Order[] GenerateOrders(int count)
        {
            var outOrders = new Order[count];

            for (int i = 0; i < count; i++)
            {
                var dt = (i + 1f) / count;
                var orderDatas = new List<Order.OrderData>(GameManager.COLOR_COUNT);
                for (int ii = 0; ii < GameManager.COLOR_COUNT; ii++)
                {
                    var colorData = GetColorData(ii);
                    if(colorData.useColor == false)
                        continue;

                    var orderCount = colorData.GetOrderCount(dt);
                    
                    if(orderCount == 0)
                        continue;
                    
                    orderDatas.Add(new Order.OrderData
                    {
                        color = colorData.color,
                        count = orderCount
                    });
                }

                if (orderDatas.Count == 0)
                    throw new Exception();
                
                outOrders[i] = new Order
                {
                    collectibleDrops = Mathf.RoundToInt(Mathf.Lerp(rewardsRange.x, rewardsRange.y, changeRewardsCurve.Evaluate(dt))),
                    orderTime = Mathf.Lerp(timeRange.x, timeRange.y, changeTimeCurve.Evaluate(dt)),
                    orderDatas = orderDatas.ToArray()
                };
            }

            return outOrders;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ColorData GetColorData(int index)
        {
            switch (index)
            {
                case 0:
                    return redData;
                case 1:
                    return blueData;
                case 2:
                    return greenData;
                case 3:
                    return purpleData;
                default:
                    throw new NotImplementedException();
            }
        }
        //============================================================================================================//
    }
#endif
}