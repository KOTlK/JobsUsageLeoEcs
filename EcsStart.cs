using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Collisions
{
    public class EcsStart : MonoBehaviour
    {
        [SerializeField] private Transform _max;
        [SerializeField] private Transform _min;
        [SerializeField] private Transform _player;
        [SerializeField] private Settings _settings;
        [SerializeField] private int _count;
        
        private IEcsSystems _systems;

        private void Start()
        {
            var world = new EcsWorld();
            _systems = new EcsSystems(world);

            var pool = world.GetPool<AABB>();

            Vector2 RandomPosition()
            {
                var x = Random.Range(_min.position.x, _max.position.x);
                var y = Random.Range(_min.position.y, _max.position.y);

                return new Vector2(x, y);
            }
            
            for (var i = 0; i < _count; i++)
            {
                var entity = world.NewEntity();
                ref var aabb = ref pool.Add(entity);

                aabb.Position = RandomPosition();
                Debug.DrawRay(aabb.Position, Vector3.up, Color.blue, 10f);
            }
            

            _systems
                .DelHere<Around>()
                .Add(new AroundDetectionSystem())
                .Add(new DrawSystem())
                .Inject(_player, _settings)
                .Init();
        }

        private void Update()
        {
            _systems.Run();
        }
    }
}