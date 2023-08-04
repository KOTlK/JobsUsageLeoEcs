using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Collisions
{
    public class DrawSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<Transform> _player = default;
        private readonly EcsFilterInject<Inc<AABB, Around>> _filter = default;
        private readonly EcsPoolInject<AABB> _aabbs = default;  
        
        public void Run(IEcsSystems systems)
        {
            var playerPosition = _player.Value.position;
            
            foreach (var entity in _filter.Value)
            {
                ref var aabb = ref _aabbs.Value.Get(entity);
                Debug.DrawLine(playerPosition, aabb.Position);
            }
        }
    }
}