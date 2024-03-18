using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHSetup : MonoBehaviour
{
    public int particlesPerAxis;
    public float spawnJitter = 0.2f;
    public bool MultipleFluids;

    [Header("Dynamic Single fluid Data")]
    public Vector3 standardSpawnCenter;
    public SPHParticleData standardFluidData;

    [Header("Static multi fluid Data")]
    public List<SPHParticleData> particlePresets = new List<SPHParticleData>();
    public Vector3[] spawnCenter;

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
                    spawnPos += UnityEngine.Random.onUnitSphere * spawnJitter;
                    Particle p = new Particle
                    {
                        position = spawnPos,
                        staticDensityTarget = data.densityTarget,
                        staticPressureMulti = data.pressureForce,
                        staticNearPressureMulti = data.nearPressureForce,
                        staticViscosityMulti = data.viscosity,
                        colour = new Vector3(data.colour.r,data.colour.g,data.colour.b) 
                    };

                    newParticles.Add(p);
                }
            }
        }
        return newParticles.ToArray();
    }

    public Particle[] ParticleSpawner()
    {
        if (!MultipleFluids)
        {
            return SpawnParticlesInBox(particlesPerAxis, standardSpawnCenter, standardFluidData);
        }
        else
        {
            int totalParticles = particlesPerAxis * particlesPerAxis * particlesPerAxis;
            List<Particle> total = new List<Particle>();
            int spawnDivide = (totalParticles / particlePresets.Count);
            double cubedRt = Math.Pow(Convert.ToDouble(spawnDivide), 0.3333333333333333);

            foreach(SPHParticleData data in particlePresets)
            {
                Particle[] newParticleBunch = SpawnParticlesInBox((int)cubedRt, spawnCenter[particlePresets.IndexOf(data)], data);
                total.AddRange(newParticleBunch);
            }
            //RoundListToThread(total);
            RandomizeRemainderParticles(total,totalParticles);
            return total.ToArray();

        }
    }

    void RandomizeRemainderParticles(List<Particle> particleList, int target)
    {
        for(int i = particleList.Count; i != target; i++)
        {
            int randomized = UnityEngine.Random.Range(0, particlePresets.Count);
            Particle p = new Particle
            {
                position = spawnCenter[randomized],
                staticDensityTarget = particlePresets[randomized].densityTarget,
                staticPressureMulti = particlePresets[randomized].pressureForce,
                staticNearPressureMulti = particlePresets[randomized].nearPressureForce,
                staticViscosityMulti = particlePresets[randomized].viscosity,
                colour = new Vector3(particlePresets[randomized].colour.r, particlePresets[randomized].colour.g, particlePresets[randomized].colour.b)
            };
            particleList.Add(p);
        }
    }

    void RoundListToThread(List<Particle> particleList)
    {
        float tar = (particleList.Count / 1000);
        int targetAmount = (int)Math.Floor(tar);
        targetAmount = targetAmount * 1000;
        int count = targetAmount;
        while(particleList.Count != targetAmount)
        {
            particleList.RemoveAt(count);
            count--;
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
