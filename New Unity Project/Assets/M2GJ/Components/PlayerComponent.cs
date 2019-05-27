using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MoonSlicer.Components
{
    namespace Authoring
    {
        public class PlayerComponent : MonoBehaviour, IConvertGameObjectToEntity//ECSStoreMonoBehaviour<Components.PlayerInputs>
        {
            public int MaxHealth;
            public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                var inputs = new Components.PlayerComponent();
                dstManager.AddBuffer<CollisionInfoElement>(entity);
                dstManager.AddComponentData(entity, new TeamComponent() { TeamIndex = Team.Player });
                dstManager.AddComponentData(entity, new HealthState() { MaxHealth = MaxHealth, Health = MaxHealth });


                dstManager.AddComponentData(entity, inputs);
            }
        }
    }

    [Serializable]
    public struct PlayerComponent : IComponentData
    {
        public float2 DirectionalInput;
        public bool HitDodge;
        public bool HitLightAttack;
        public bool HitHeavyAttack;
        public bool HitSuper;
        public int ComboStage;
        public float SuperGauge;
        public float AimHorizontal;
        public float AimVertical;
        public float CurrAimDegrees;
        //public bool IsJumping;
        //public bool Grounded;
    }
}