# JobsUsageLeoEcs
Simple example to show how to work with jobs system and LeoEcs

To get actual component by entity from `EcsPool.GetRawDenseItems()`(Components) you should:

- Get filtered entities by `EcsFilter.GetRawEntities()`(Entities)
- Get right indices by `EcsPool.GetRawSparseItems()`(Indices)
- Component = `Components[Indices[entity]]`