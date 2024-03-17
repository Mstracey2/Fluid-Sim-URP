using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHSetup : MonoBehaviour
{
    public int particlesPerAxis;
    public Vector3[] spawnCenter;
    public float spawnJitter = 0.2f;

    public SPHParticleData standardFluidData;

    public List<SPHParticleData> particlePresets = new List<SPHParticleData>();

    private void Awake()
    {
       
    }

    public Particle[] SpawnParticlesInBox(int perAxis, Vector3 spawnPoint, SPHParticleData data)
    {
        Vector3Int numToSpawn = new Vector3Int(perAxis, perAxis, perAxis);
        List<Particle> newParticles = new List<Particle>();

        for (int x = 0; x < numToSpawn.x; x++)
        {
            for (int y = 0; y < numToSpawn.y; y++)
            {
                for (int z = 0; z < numToSpawn.z; z++)
                {
                    Vector3 spawnPos = spawnPoint + new Vector3(x * 0.05f, y * 0.05f, z * 0.05f);
                    spawnPos += Random.onUnitSphere * spawnJitter;
                    Particle p = new Particle
                    {
                        position = spawnPos,
                        staticDensityTarget = data.densityTarget,
                        staticPressureMulti = data.pressureForce,
                        staticNearPressureMulti = data.nearPressureForce,
                        staticViscosityMulti = data.viscosity
                    };

                    newParticles.Add(p);
                }
            }
        }
        return newParticles.ToArray();
    }

    public Particle[] ParticleSpawner(bool dynamicParticles)
    {
        if (dynamicParticles)
        {
            return SpawnParticlesInBox(particlesPerAxis, spawnCenter[0], standardFluidData);
        }
        else
        {
            Particle[] staticParticles = new Particle[particlesPerAxis];

            int spawnDivide = particlesPerAxis / particlePresets.Count;
            int arraylength = 0;

            foreach(SPHParticleData data in particlePresets)
            {
                Particle[] newParticleBunch = SpawnParticlesInBox(spawnDivide, spawnCenter[particlePresets.IndexOf(data)], data);
                arraylength += newParticleBunch.Length;
                staticParticles.CopyTo(newParticleBunch, arraylength);
            }

            return staticParticles;

        }
    }


    private void OnDrawGizmos()
    {
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        if (!Application.isPlaying)
        {
            foreach(Vector3 vect in spawnCenter)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(vect, 0.1f);
            }

        }
        Gizmos.matrix = matrix;
    }
}
