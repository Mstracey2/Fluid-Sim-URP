using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHSetup : MonoBehaviour
{
    public int particlesPerAxis;
    private Vector3Int numToSpawn;
    public Vector3 spawnCenter;
    public float spawnJitter = 0.2f;

    private void Awake()
    {
       
    }

    public Particle[] SpawnParticlesInBox()
    {
        numToSpawn = new Vector3Int(particlesPerAxis, particlesPerAxis, particlesPerAxis);
        Vector3 spawnPoint = spawnCenter;
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
                        position = spawnPos
                    };
                    newParticles.Add(p);
                }
            }
        }
        return newParticles.ToArray();
    }


    private void OnDrawGizmos()
    {
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnCenter, 0.1f);
        }
        Gizmos.matrix = matrix;
    }
}
