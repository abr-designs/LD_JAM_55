using System;
using System.Linq;
using Actors;
using Enums;
using UnityEngine;

namespace Managers
{
    public class ProcessorsManager : MonoBehaviour
    {
        [SerializeField]
        private PawnProcessor[] Processors;

        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            foreach (var processor in Processors)
            {
                processor.gameObject.SetActive(false);
            }
        }
        
        //Methods
        //============================================================================================================//
        
        public bool HasProcessor(COLOR outColor)
        {
            return Processors.Any(x => x.OutColor == outColor);
        }

        public void EnableProcessor(COLOR outColor)
        {
            if (HasProcessor(outColor) == false)
                throw new Exception();
        
            var processor = Processors.FirstOrDefault(x => x.OutColor == outColor);

            if (processor == null)
                throw new Exception();

            processor.gameObject.SetActive(true);
        }
    }
}
