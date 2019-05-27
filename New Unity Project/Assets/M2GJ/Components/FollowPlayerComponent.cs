using MoonSlicer.Systems;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace MoonSlicer.Components
{
    namespace Authoring
    {
        public class FollowPlayerComponent : ECSStoreMonoBehaviour<Components.FollowPlayerComponent>
        {
            public float3 EulerOffset;
            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentObject(entity, transform);
                Serialized.RotationOffset = quaternion.Euler(EulerOffset);
                base.Convert(entity, dstManager, conversionSystem);
            }

            ////todo: figure out late update in ECS
            //private void LateUpdate()
            //{
            //
            //    //float3 playerPosition = PlayerSingleton.Instance.PlayerPosition.Position + Serialized.CameraTargetOffset;
            //    //quaternion playerRotation = PlayerSingleton.Instance.PlayerPosition.Rotation;
            //    //transform.position = playerPosition + math.mul(playerRotation, Serialized.CameraOffset);
            //    //        //UnityEngine.Vector3.SmoothDamp(transform.position,
            //    //        //playerPosition + math.mul(playerRotation, Serialized.CameraOffset),
            //    //        //ref Serialized.CurrDampVelocity,
            //    //        //Serialized.DampTime);
            //    //transform.rotation =
            //    //        quaternion.LookRotation((playerPosition - (float3)transform.position), math.up());
            //}
        }
    }

    [Serializable]
    public struct FollowPlayerComponent : IComponentData
    {
        public bool FollowPlayer;
        public bool LookAtPlayer;
        public bool CopyPlayerRotation;
        public bool ForceCopyOffsetRaw;
        public float3 Offset;
        public float3 TargetOffset;
        public quaternion RotationOffset;
        public float DampTime;
        public UnityEngine.Vector3 CurrDampVelocity;
    }
}