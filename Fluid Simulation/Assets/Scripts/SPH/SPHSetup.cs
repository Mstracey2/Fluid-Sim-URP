using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*SPH SETUP
 * 
 * Script that holds the primary setup functionality for the SPH Sim.
 *
 *spawns particles depending on multi or single fluid behaviour
 */

public class SPHSetup : MonoBehaviour
{
    #region Variables
    public int particlesPerAxis;
    public float spawnJitter = 0.2f;        //small value to disrupt particle spawn pattern
    public bool MultipleFluids;

    [Header("Dynamic Single fluid Data")]
    public Vector3 standardSpawnCenter;          //default spawn
    public SPHParticleData standardFluidData;    //default fluid preset

    [Header("Static multi fluid Data")]
    public List<SPHParticleData> particlePresets = new List<SPHParticleData>();
    public Vector3[] spawnCenter;

    #endregion

    #region Particle Spawning
    /// <summary>
    /// Function that will set particle positions and spawn them in a grid based format
    /// </summary>
    /// <param name="perAxis"></param>           //particles per axis
    /// <param name="spawnPoint"></param>        //starting spawn pos
    /// <param name="data"></param>              // the chosen preset (default if the fluid can be dynamically changed)
    /// <returns></returns>
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
                    spawnPos += UnityEngine.Random.onUnitSphere * spawnJitter;  //adds a small jitter to the position

                    //creates a particle with the data of the preset passed through
                    Particle p = new Particle
                    {
                        position = spawnPos,
                        defaultDensityTarget = data.densityTarget,
                        defaultPressureMulti = data.pressureForce,
                        defaultNearPressureMulti = data.nearPressureForce,
                        defaultViscosityMulti = data.viscosity,
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
        /*
         * If the particles are to remain as a single fluid setup,
         * there is no requirement for any fancy set up, so the particles are set up with default spawn and preset data,
         * 
         * else, particles are split up into different locations and set (for the most part) evenly to different presets.
         */
        if (!MultipleFluids)
        {
            return SpawnParticlesInBox(particlesPerAxis, standardSpawnCenter, standardFluidData);
        }
        else
        {
            int totalParticles = particlesPerAxis * particlesPerAxis * particlesPerAxis;
            List<Particle> total = new List<Particle>();

            //how many particles per preset
            int spawnDivide = (totalParticles / particlePresets.Count); 

            //cubed rooted to get how many per axis for each preset (rounded down to whole number)
            double cubedRt = Math.Pow(Convert.ToDouble(spawnDivide), 0.3333333333333333);

            //spawn particles with the newly calculated divide per preset.
            foreach(SPHParticleData data in particlePresets)
            {
                Particle[] newParticleBunch = SpawnParticlesInBox((int)cubedRt, spawnCenter[particlePresets.IndexOf(data)], data);
                total.AddRange(newParticleBunch);
            }

            // particles left out due to rounding down
            RandomizeRemainderParticles(total,totalParticles);
            return total.ToArray();

        }
    }

    /// <summary>
    /// cube rooting means a likely decimal number, meaning not all particles will be left out due to rounding down.
    /// The remainder particles are randomized to any location and preset.
    /// although not the best solution, the result is hardly noticeable as its a small amount of particles.
    /// </summary>
    /// <param name="particleList"></param>
    /// <param name="target"></param>
    void RandomizeRemainderParticles(List<Particle> particleList, int target)
    {
        for (int i = particleList.Count; i != target; i++)
        {
            int randomized = UnityEngine.Random.Range(0, particlePresets.Count);    //random preset
            Particle p = new Particle
            {
                position = spawnCenter[randomized],
                defaultDensityTarget = particlePresets[randomized].densityTarget,
                defaultPressureMulti = particlePresets[randomized].pressureForce,
                defaultNearPressureMulti = particlePresets[randomized].nearPressureForce,
                defaultViscosityMulti = particlePresets[randomized].viscosity,
                colour = new Vector3(particlePresets[randomized].colour.r, particlePresets[randomized].colour.g, particlePresets[randomized].colour.b)
            };
            particleList.Add(p);
        }
    }


    #endregion

    public void SetMultiFluids()
    {
        MultipleFluids = !MultipleFluids;
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
