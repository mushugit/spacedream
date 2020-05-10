using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoDestroyingParticleSystem : MonoBehaviour
{
    private List<ParticleSystem> particleSystems;

    [SerializeField]
    private bool includeChildParticlesSystems = true;

    void Start()
    {
        var localParticlesSystems = GetComponents<ParticleSystem>();
        particleSystems = localParticlesSystems.ToList();
        if (includeChildParticlesSystems)
        {
            particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (particleSystems != null)
        {
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem.IsAlive())
                {
                    return;
                }
            }
            Destroy(gameObject);
        }
    }
}
