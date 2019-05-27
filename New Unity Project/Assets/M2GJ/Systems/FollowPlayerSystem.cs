using System;
using Unity.Entities;
using MoonSlicer.Components;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics.Authoring;
using Unity.Physics;

namespace MoonSlicer.Systems
{
    //[UpdateAfter(typeof(PlayerSystem))]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class FollowPlayerSystem : ComponentSystem
    {
        EntityQuery playerQuery;
        EntityQuery cameraQuery;

        protected override void OnCreate()
        {
            playerQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerComponent>(), ComponentType.ReadOnly<LocalToWorld>());
            cameraQuery = EntityManager.CreateEntityQuery(typeof(FollowPlayerComponent), typeof(Transform));
        }

        protected override void OnUpdate()
        {

            //var playerComponents = playerQuery.ToComponentDataArray<PlayerComponent>(Allocator.Temp);
            using (var playerEntities = playerQuery.ToEntityArray(Allocator.TempJob))
            {
                if (playerEntities.Length == 0)
                    return;
                var playerComponent = EntityManager.GetComponentData<PlayerComponent>(playerEntities[0]);
                var playerTransform = EntityManager.GetComponentData<LocalToWorld>(playerEntities[0]);
                Entities.ForEach((
                    Entity entity,
                    ref FollowPlayerComponent follow,
                    Transform localToWorld) =>
                {
                    float3 playerPosition = playerTransform.Position + follow.TargetOffset;
                    quaternion playerRotation = math.mul(follow.RotationOffset, math.quaternion(playerTransform.Value));
                    float3 newPosition = playerPosition + math.mul(playerRotation, follow.Offset);
                    if (follow.LookAtPlayer)
                        localToWorld.rotation = quaternion.LookRotationSafe((playerPosition - newPosition), math.mul(playerRotation, math.up()));
                    else if (follow.CopyPlayerRotation)
                    {
                        if (follow.ForceCopyOffsetRaw)
                        {
                            localToWorld.rotation = math.mul(playerRotation, localToWorld.rotation);
                        }
                        else
                            localToWorld.rotation = playerRotation;
                    }
                    if (follow.ForceCopyOffsetRaw)
                    {
                        newPosition += math.mul(playerRotation, (float3)localToWorld.position);
                    }
                    if (follow.FollowPlayer)
                    {
                        //EntityManager.GetComponentData<PlayerComponent>(EntityManager.GetAllEntities())
                        //camera.CameraOffset.y = math.clamp(camera.CameraOffset.y + )
                        localToWorld.position = newPosition;
                        //UnityEngine.Vector3.SmoothDamp(localToWorld.position,
                        //playerPosition + math.mul(playerRotation, camera.CameraOffset),
                        //ref camera.CurrDampVelocity,
                        //camera.DampTime);
                        //math.float3(1, 1, 1))
                        //};
                    }
                });
            }
        }
    }
}