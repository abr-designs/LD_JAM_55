using UnityEngine;

namespace Data
{
    public static class GlobalMults
    {
        public static float SpawnerRegenTimeMult = 1f;
        public static float DeliveryResetTimeMult = 1f;
        public static float TickleTimeMult = 1f;
        public static float RewardAmountMult = 1f;
        public static float ProcessingTime = 1f;
        
        public static float SpawnerRegenTimeMultDelta = -0.2f;
        public static float DeliveryResetTimeMultDelta = 0.2f;
        public static float TickleTimeMultDelta = 0.30f;
        public static float RewardAmountMultDelta = 0.25f;
        public static float ProcessTimeMultDelta = -0.2f;

        //TODO Might need to see the values
        public static readonly string SpawnerText = $"Add 1 new Crystal";
        public static readonly string SpawnerRegenTimeText = $"Reduce crystal Growth time by {Mathf.Abs(SpawnerRegenTimeMultDelta):P0}";
        public static readonly string DeliveryResetTimeText = $"Extend delivery downtime by {Mathf.Abs(DeliveryResetTimeMultDelta):P0}";
        public static readonly string TickleTimeText = $"Increase Crystal tickle quality by {Mathf.Abs(TickleTimeMultDelta):P0}";
        public static readonly string RewardAmountText = $"Increase rewards earned by {Mathf.Abs(RewardAmountMultDelta):P0}";
        public static readonly string ProcessingTimeText = $"Reduce Refining Time by {Mathf.Abs(RewardAmountMultDelta):P0}";


        public static void ResetValues()
        {
            SpawnerRegenTimeMult = 1f;
            DeliveryResetTimeMult = 1f;
            TickleTimeMult = 1f;
            RewardAmountMult = 1f;
            ProcessingTime = 1f;
        }
        
    }
}