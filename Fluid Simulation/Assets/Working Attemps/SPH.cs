using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 68)]
public struct Particle
{
    public Vector3 pressure;
    public Vector2 density;
    public Vector3 external;
    public Vector3 velocity;
    public Vector3 position;
    public Vector3 positionPrediction;
}


public class SPH : MonoBehaviour
{
    [Header("Setup")]
    public float gravity;
    public float boundDamping = 0.2f;
    public bool showSpheres = true;
    public bool fixedTimestep;
    public Vector3Int numToSpawn = new Vector3Int(10, 10, 10);
    private int totalParticles { get { return numToSpawn.x * numToSpawn.y * numToSpawn.z; } }
    public Vector3 boxSize = new Vector3(4, 10, 3);
    public Vector3 spawnCenter;
    public float particleRadius = 0.1f;
    public float spawnJitter = 0.2f;
    public float timestep = -0.007f;
    public int numOfParticleCalc;

    [Header("Particle Rendering")]
    public Mesh particleMesh;
    public float particleRenderSize = 8f;
    public Material material;

    [Header("Compute")]
    public ComputeShader shader;
    public Particle[] particles;

    [Header("Fluid Constants")]
    public float particleMass = 1f;
    
    public float densityTarget;
    public float pressureForce;
    public float nearPressureForce;
    public float disNum;
    public float viscosity;
    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _particleBuffer;
    public ParticleDisplay pDisplay;
    //Kernals
    private int integrateKernel;
    private int densityKernel;
    private int pressureKernel;
    private int forceKernel;
    private int viscosityKernel;

    private static readonly int SizeProperty = Shader.PropertyToID("_size");
    private static readonly int ParticelsBufferProperty = Shader.PropertyToID("_particlesBuffer");

    private void Awake()
    {
        SpawnParticlesInBox();
        uint[] args =
        {
            particleMesh.GetIndexCount(0),
            (uint)totalParticles,
            particleMesh.GetIndexStart(0),
            particleMesh.GetBaseVertex(0),
            0
        };
        _argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        _argsBuffer.SetData(args);
        _particleBuffer = new ComputeBuffer(totalParticles, 68);
        _particleBuffer.SetData(particles);
        FindKernelsAndSetBuffers();
    }

    private void FixedUpdate()
    {
        if (fixedTimestep)
        {
            SimulateParticles(Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        material.SetFloat(SizeProperty, particleRenderSize);
        material.SetBuffer(ParticelsBufferProperty, _particleBuffer);
        //pDisplay.ProduceGradientColour(material);
        if (showSpheres)
        {
            Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, new Bounds(Vector3.zero, boxSize), _argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
        }
            
    }

    private void SimulateParticles(float frames)
    {
        float timeStepper = frames / numOfParticleCalc * timestep;
        SetComputeVariables(timeStepper);

        shader.Dispatch(integrateKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(densityKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(pressureKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(viscosityKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(forceKernel, totalParticles / 100, 1, 1);
    }

    private void SetComputeVariables(float time)
    {
        shader.SetVector("boxSize", boxSize);
        shader.SetFloat("timestep", time);
        shader.SetInt("particleLength", totalParticles);
        shader.SetFloat("particleMass", particleMass);
        shader.SetFloat("pi", Mathf.PI);
        shader.SetVector("boxSize", boxSize);
        shader.SetFloat("gravity", gravity);
        shader.SetFloat("radius", particleRadius);
        shader.SetFloat("boundDamping", boundDamping);
        shader.SetFloat("densityTarget", densityTarget);
        shader.SetFloat("pressureMulti", pressureForce);
        shader.SetFloat("nearPressureMulti", nearPressureForce);
        shader.SetFloat("disNum", disNum);
        shader.SetFloat("viscosityMulti", viscosity);
    }

    private void SpawnParticlesInBox()
    {
        Vector3 spawnPoint = spawnCenter;
        List<Particle> _particles = new List<Particle>();
        for (int x = 0; x < numToSpawn.x; x++)
        {
            for (int y = 0; y < numToSpawn.y; y++)
            {
                for (int z = 0; z < numToSpawn.z; z++)
                {
                    Vector3 spawnPos = spawnPoint + new Vector3(x * particleRadius * 2, y * particleRadius * 2, z * particleRadius * 2);
                    spawnPos += Random.onUnitSphere * particleRadius * spawnJitter;
                    Particle p = new Particle
                    {
                        position = spawnPos
                    };
                    _particles.Add(p);
                }
            }
        }
        particles = _particles.ToArray();
    }

    private void FindKernelsAndSetBuffers()
    {
        integrateKernel = shader.FindKernel("Integrate");
        densityKernel = shader.FindKernel("CalculateDensity");
        pressureKernel = shader.FindKernel("CalculatePressure");
        viscosityKernel = shader.FindKernel("CalculateViscosity");
        forceKernel = shader.FindKernel("ApplyForces");

        shader.SetBuffer(integrateKernel, "_particles", _particleBuffer);
        shader.SetBuffer(densityKernel, "_particles", _particleBuffer);
        shader.SetBuffer(pressureKernel, "_particles", _particleBuffer);
        shader.SetBuffer(viscosityKernel, "_particles", _particleBuffer);
        shader.SetBuffer(forceKernel, "_particles", _particleBuffer);

    }

    private void OnDrawGizmos()
    {
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, boxSize);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnCenter, 0.1f);
        }
        Gizmos.matrix = matrix;
    }
}
