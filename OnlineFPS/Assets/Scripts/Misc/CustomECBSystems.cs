using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PredictedSimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class PostPredictionPreTransformsECBSystem : EntityCommandBufferSystem
{
    public unsafe struct Singleton : IComponentData, IECBSingleton
    {
        internal UnsafeList<EntityCommandBuffer>* pendingBuffers;
        internal Allocator allocator;

        public EntityCommandBuffer CreateCommandBuffer(WorldUnmanaged world)
        {
            return EntityCommandBufferSystem.CreateCommandBuffer(ref *pendingBuffers, allocator, world);
        }

        public void SetPendingBufferList(ref UnsafeList<EntityCommandBuffer> buffers)
        {
            pendingBuffers = (UnsafeList<EntityCommandBuffer>*)UnsafeUtility.AddressOf(ref buffers);
        }

        public void SetAllocator(Allocator allocatorIn)
        {
            allocator = allocatorIn;
        }
    }
    
    protected override unsafe void OnCreate()
    {
        base.OnCreate();

        ref UnsafeList<EntityCommandBuffer> pendingBuffers = ref *m_PendingBuffers;
        this.RegisterSingleton<Singleton>(ref pendingBuffers, World.Unmanaged);
    }
}
