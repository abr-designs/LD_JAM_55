using System;
using System.Linq;
using Actors;
using UnityEngine;

namespace Managers
{
    public class SpawnersManager : MonoBehaviour
    {
        [SerializeField]
        private PawnSpawner[] _spawners;

        public bool HasLockedSpawners()
        {
            return _spawners.Any(x => x.IsActive == false);
        }

        public void UnlockNextSpawner()
        {
            if (HasLockedSpawners() == false)
                throw new Exception();
            
            var spawner = _spawners.FirstOrDefault(x => x.IsActive == false);

            if (spawner == null)
                throw new Exception();
            
            spawner.SetActive(true);
        }
    }
}