using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;

namespace Collisions
{
    public class AroundDetectionSystem : IEcsRunSystem
    {
        //Default: 14.05ms - 100000
        //Jobs: 3ms - 100000
        //Jobs + Burst: 0.98ms - 100000
        //With jobs constructing arrays takes more time than actual job
        //For 1000000 entities arrays construction takes 8.35ms, writing 3ms and job 1ms
        private readonly EcsCustomInject<Settings> _settings = default;
        private readonly EcsCustomInject<Transform> _player = default;
        private readonly EcsFilterInject<Inc<AABB>> _filter = default;
        private readonly EcsPoolInject<AABB> _aabbs = default;
        private readonly EcsPoolInject<Around> _around = default;

        //private static readonly ProfilerMarker Around = new(nameof(Around));
        private static readonly ProfilerMarker ArraysConstruct = new(nameof(ArraysConstruct));
        private static readonly ProfilerMarker Job = new(nameof(Job));
        private static readonly ProfilerMarker Writing = new(nameof(Writing));

        public void Run(IEcsSystems systems)
        {
            /*Around.Begin();
            var playerPosition = _player.Value.position;
            var settings = _settings.Value;

            foreach (var entity in _filter.Value)
            {
                ref var aabb = ref _aabbs.Value.Get(entity);

                if (Vector2.Distance(playerPosition, aabb.Position) <= settings.CloseDistance)
                {
                    if (_around.Value.Has(entity) == false)
                    {
                        _around.Value.Add(entity);
                    }
                }
                else
                {
                    if (_around.Value.Has(entity))
                    {
                        _around.Value.Del(entity);
                    }
                }
            }
            Around.End();*/
            ArraysConstruct.Begin();
            var entities = new NativeArray<int>(_filter.Value.GetRawEntities(), Allocator.TempJob);
            var indices = new NativeArray<int>(_aabbs.Value.GetRawSparseItems(), Allocator.TempJob);
            var aabb = new NativeArray<AABB>(_aabbs.Value.GetRawDenseItems(), Allocator.TempJob);
            var output = new NativeQueue<int>(Allocator.TempJob);
            ArraysConstruct.End();

            Job.Begin();
            var job = new CalculateCloseJob()
            {
                AABB = aabb,
                FilteredEntities = entities,
                Indices = indices,
                Output = output.AsParallelWriter(),
                Position = _player.Value.position,
                CloseDistance = _settings.Value.CloseDistance
            }.Schedule(entities.Length, 32);
            
            job.Complete();
            Job.End();
            
            Writing.Begin();
            while (output.Count > 0)
            {
                var entity = output.Dequeue();
                _around.Value.Add(entity);
            }
            Writing.End();


            entities.Dispose();
            indices.Dispose();
            aabb.Dispose();
            output.Dispose();
        }
        
        //Indices = pool.GetRawSparseItems()
        //Components = pool.GetRawDenseItems()
        //Component = Components[Indices[filteredEntity]]
        [BurstCompile]
        public struct CalculateCloseJob : IJobParallelFor
        {
            [ReadOnly] public float CloseDistance;
            [ReadOnly] public Vector2 Position;
            [ReadOnly] public NativeArray<int> FilteredEntities;
            [ReadOnly] public NativeArray<int> Indices;
            [ReadOnly] public NativeArray<AABB> AABB;
            [WriteOnly] public NativeQueue<int>.ParallelWriter Output;
            
            [BurstCompile]
            public void Execute(int index)
            {
                var entity = FilteredEntities[index];
                var aabb = AABB[Indices[entity]];

                if (Vector2.Distance(Position, aabb.Position) <= CloseDistance)
                {
                    Output.Enqueue(entity);
                }
            }
        }
    }
}