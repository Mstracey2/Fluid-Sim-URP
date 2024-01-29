using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 44)]
public struct Particle
{
    public float pressure;
    public float density;
    public Vector3 currentForce;
    public Vector3 velocity;
    public Vector3 position;
}


public class SPH : MonoBehaviour
{
    [Header("Setup")]
    public float gravity;
    public bool showSpheres = true;
    public Vector3Int numToSpawn = new Vector3Int(10, 10, 10);
    private int totalParticles { get { return numToSpawn.x * numToSpawn.y * numToSpawn.z; } }
    public Vector3 boxSize = new Vector3(4, 10, 3);
    public Vector3 spawnCenter;
    public float particleRadius = 0.1f;
    public float spawnJitter = 0.2f;

    [Header("Particle Rendering")]
    public Mesh particleMesh;
    public float particleRenderSize = 8f;
    public Material material;

    [Header("Compute")]
    public ComputeShader shader;
    public Particle[] particles;

    [Header("Fluid Constants")]
    public float boundDamping = -0.3f;
    public float viscosity = -0.0003f;
    public float particleMass = 1f;
    public float gasConstant = 2f;
    public float restingDensity = 1f;
    public float timestep = -0.007f;

    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _particleBuffer;
    private int integrateKernel;
    private int forceKernel;

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
        _particleBuffer = new ComputeBuffer(totalParticles, 44);
        _particleBuffer.SetData(particles);
        FindKernelsAndSetBuffers();
        SetShaderVariables();
    }

    private void FixedUpdate()
    {
        shader.Dispatch(integrateKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(forceKernel, totalParticles / 100, 1, 1);
    }

    private void Update()
    {
        SetShaderVariables();
        material.SetFloat(SizeProperty, particleRenderSize);
        material.SetBuffer(ParticelsBufferProperty, _particleBuffer);
        if (showSpheres)
        {
            Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, new Bounds(Vector3.zero, boxSize), _argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
        }
            
    }

    private void SetShaderVariables()
    {
        shader.SetVector("boxSize", boxSize);
        shader.SetFloat("timestep", timestep);
        shader.SetInt("particleLength", totalParticles);
        shader.SetFloat("particleMass", particleMass);
        shader.SetFloat("viscosity", viscosity);
        shader.SetFloat("gasConstant", gasConstant);
        shader.SetFloat("restDensity", restingDensity);
        shader.SetFloat("boundDamping", boundDamping);
        shader.SetFloat("pi", Mathf.PI);
        shader.SetVector("boxSize", boxSize);
        shader.SetFloat("gravity", gravity);
        shader.SetFloat("radius", particleRadius);
        //shader.SetFloat("radius2", particleRadius * particleRadius);
        //shader.SetFloat("radius3", particleRadius * particleRadius * particleRadius);
        //shader.SetFloat("radius4", particleRadius * particleRadius * particleRadius * particleRadius);
        //shader.SetFloat("radius5", particleRadius * particleRadius * particleRadius * particleRadius * particleRadius);
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
        forceKernel = shader.FindKernel("ApplyComputeForces");
        shader.SetBuffer(integrateKernel, "_particles", _particleBuffer);
        shader.SetBuffer(forceKernel, "_particles", _particleBuffer);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnCenter, 0.1f);
        }
    }
}
