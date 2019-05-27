using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

//courtesy https://github.com/Unity-Technologies/EntityComponentSystemSamples
//modified for this use case
public class EntityTracker : MonoBehaviour, IConvertGameObjectToEntity
{
    private Entity EntityToTrack = Entity.Null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        EntityToTrack = entity;
    }
    private void Start()
    {
        StartCoroutine(LateCoroutine());
    }

    private IEnumerator LateCoroutine()
    {
        yield return null;
        while (isActiveAndEnabled)
        {
            yield return new WaitForEndOfFrame();
            if (EntityToTrack != Entity.Null)
            {
                try
                {
                    var em = World.Active.EntityManager;

                    //transform.position = em.GetComponentData<Translation>(EntityToTrack).Value;
                    //transform.rotation = em.GetComponentData<Rotation>(EntityToTrack).Value;
                    em.SetComponentData(EntityToTrack, new Translation() { Value = transform.position });
                    em.SetComponentData(EntityToTrack, new Rotation() { Value = transform.rotation });


                }
                catch
                {
                    // Dirty why to check for an Entity that no longer exists.
                    EntityToTrack = Entity.Null;
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
    }
}
