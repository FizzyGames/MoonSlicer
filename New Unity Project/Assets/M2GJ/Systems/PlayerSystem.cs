using MoonSlicer.Components;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

namespace MoonSlicer.Systems
{
    //TODO: replace this with ECS workflow
    //      once you actually figure out what that looks like
    public class PlayerPosition
    {
        public float3 Position;
        public quaternion Rotation;
    }
    public class PlayerSingleton
    {
        private static PlayerSingleton _instance;
        public static PlayerSingleton Instance => _instance != null ? _instance : _instance = new PlayerSingleton();

        public PlayerPosition PlayerPosition = new PlayerPosition();
    }
    //[UpdateAfter(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
    public class PlayerSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (MovementStateInfo movementInfo,
                 ref PlayerComponent player,
                 ref PhysicsCollider collider,
                 ref PhysicsVelocity velocity,
                 ref Translation position,
                 ref Rotation rotation) =>
                {
                    player.DirectionalInput.x = Input.GetAxis("Horizontal");
                    player.DirectionalInput.y = Input.GetAxis("Vertical");
                    player.HitLightAttack = Input.GetButton("LightAttack");
                    player.HitHeavyAttack = Input.GetButton("HeavyAttack");
                    player.HitDodge = Input.GetButton("Dodge");
                    player.HitSuper = Input.GetButton("Super");
                    player.AimHorizontal = Input.GetAxis("Mouse X");
                    player.AimVertical = Input.GetAxis("Mouse Y");
                    player.CurrAimDegrees += player.AimHorizontal * 0.1f;

                    Cursor.lockState = CursorLockMode.Locked;


                    PlayerSingleton.Instance.PlayerPosition.Position = position.Value;

                    rotation = new Rotation()
                    {
                        Value = quaternion.Euler(0, player.CurrAimDegrees, 0)
                    };
                    velocity.Linear = math.mul(rotation.Value, new float3(player.DirectionalInput.x, 0, player.DirectionalInput.y)) * movementInfo.Speed;
                    PlayerSingleton.Instance.PlayerPosition.Rotation = rotation.Value;
                    velocity.Linear.y = -0.01f;
                });
        }

        //protected override JobHandle OnUpdate(JobHandle inputDeps)
        //{
        //throw new NotImplementedException();
        //}
    }
}