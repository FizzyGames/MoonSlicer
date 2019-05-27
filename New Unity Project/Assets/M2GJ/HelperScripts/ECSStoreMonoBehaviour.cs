using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace MoonSlicer
{
    public abstract class ECSStoreMonoBehaviour<T> : MonoBehaviour, IConvertGameObjectToEntity
        where T : struct, IComponentData
    {
        public T Serialized;
        public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, Serialized);
        }
    }

    public abstract class ECSStoreMonoBehaviourShared<T> : MonoBehaviour, IConvertGameObjectToEntity
        where T : struct, ISharedComponentData
    {
        public T Serialized;
        public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddSharedComponentData(entity, Serialized);
        }
    }
}