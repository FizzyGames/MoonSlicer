using Unity.Entities;

namespace MoonSlicer.Components
{

    [InternalBufferCapacity(5)]
    public struct CollisionInfoElement : IBufferElementData
    {
        public static implicit operator CollisionInfo(CollisionInfoElement collisionInfoElement) => collisionInfoElement.Value;
        public static implicit operator CollisionInfoElement(CollisionInfo collisionInfo) => new CollisionInfoElement() { Value = collisionInfo };
        public CollisionInfo Value;
    }
    public struct CollisionInfo
    {
        public Entity other;
    }
}