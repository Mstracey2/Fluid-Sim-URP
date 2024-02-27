using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;

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
    public Vector3 boxSize = new Vector3(4, 10, 3);
    public Vector3 boxCentre = new Vector3(4, 10, 3);
    public Vector3 spawnCenter;
    public float particleRadius = 0.1f;
    public float spawnJitter = 0.2f;
    public float timestep = -0.007f;
    public int numOfParticleCalc;
    private int totalParticles { get { return numToSpawn.x * numToSpawn.y * numToSpawn.z; } }

    [Header("Particle Rendering")]
    public Mesh particleMesh;
    public float particleRenderSize = 8f;
    public Material material;
    public Gradient colourGradient;
    public int resolution;
    public int particleMaxVelocity;
    Color[] colourMap;
    Texture2D texture;

    [Header("Mouse")]
    public GameObject mouseSphereRef;
    public float pushPullForce;
    private Vector3 mouseRefCentre;
    public float mouseRadius;
    private bool pull;
    private bool push;

    [Header("Fluid Constants")]
    public float particleMass = 1f;
    public float densityTarget;
    public float pressureForce;
    public float nearPressureForce;
    public float predictionIteration;
    public float viscosity;

    [Header("Compute")]
    public ComputeShader shader;
    public Particle[] particles;

    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _particleBuffer;

    //Kernals
    private int externalKernel;
    private int detectBoundsKernel;
    private int densityKernel;
    private int pressureKernel;
    private int forceKernel;
    private int viscosityKernel;
    LineRenderer lineRend;
    public float lineRendMulti;

    private static readonly int SizeProperty = Shader.PropertyToID("_size");
    private static readonly int ParticelsBufferProperty = Shader.PropertyToID("_particlesBuffer");

    private void Awake()
    {
        
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 16;
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
        CalculateBoxVertices();
        mouseRefCentre = mouseSphereRef.transform.position;
        boxSize = transform.localScale;
        boxCentre = transform.localPosition;
        material.SetFloat(SizeProperty, particleRenderSize);
        material.SetBuffer(ParticelsBufferProperty, _particleBuffer);
        if (showSpheres)
        {
            Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, new Bounds(Vector3.zero, boxSize), _argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
        }
            
    }
    private void CalculateBoxVertices()
    {
        var trans = this.transform;
        var min = trans.position - transform.localScale * 0.5f;
        var max = trans.position + transform.localScale * 0.5f;

        lineRend.SetPosition(0,new Vector3(min.x, min.y, min.z));
        lineRend.SetPosition(1, new Vector3(min.x, min.y, max.z));
        lineRend.SetPosition(3, new Vector3(min.x, max.y, min.z));
        lineRend.SetPosition(2,new Vector3(min.x, max.y, max.z));
        lineRend.SetPosition(4, new Vector3(min.x, min.y, min.z));
        lineRend.SetPosition(5,new Vector3(max.x, min.y, min.z));
        lineRend.SetPosition(6, new Vector3(max.x, max.y, min.z));
        lineRend.SetPosition(7, new Vector3(min.x, max.y, min.z));
        lineRend.SetPosition(8, new Vector3(max.x, max.y, min.z));
        lineRend.SetPosition(9, (new Vector3(max.x, max.y, max.z)));
        lineRend.SetPosition(10,(new Vector3(min.x, max.y, max.z)));
        lineRend.SetPosition(11, (new Vector3(max.x, max.y, max.z)));
        lineRend.SetPosition(12, (new Vector3(max.x, min.y, max.z)));
        lineRend.SetPosition(13, (new Vector3(min.x, min.y, max.z)));
        lineRend.SetPosition(14, (new Vector3(max.x, min.y, max.z)));
        lineRend.SetPosition(15, (new Vector3(max.x, min.y, min.z)));
    }
    private void SimulateParticles(float frames)
    {
        float timeStepper = frames / numOfParticleCalc * timestep;
        SetComputeVariables(timeStepper);

        shader.Dispatch(detectBoundsKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(externalKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(densityKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(pressureKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(viscosityKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(forceKernel, totalParticles / 100, 1, 1);
    }

    private void SetComputeVariables(float time)
    {
        produceColourGradientMap();
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
        shader.SetFloat("predictionIteration", predictionIteration);
        shader.SetFloat("viscosityMulti", viscosity);
        shader.SetVector("boxCentre", boxCentre);
        shader.SetMatrix("worldMatrix", transform.localToWorldMatrix);
        shader.SetMatrix("localMatrix", transform.worldToLocalMatrix);
        material.SetFloat("maxVel", particleMaxVelocity);
        shader.SetVector("mousePos", mouseRefCentre);
        shader.SetFloat("mouseRadius", mouseRadius);
        shader.SetFloat("pushPullForce", pushPullForce);
        shader.SetBool("push", push);
        shader.SetBool("pull", pull);
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
        externalKernel = shader.FindKernel("CalculateExternalForces");
        detectBoundsKernel = shader.FindKernel("DetectBounds");
        densityKernel = shader.FindKernel("CalculateDensity");
        pressureKernel = shader.FindKernel("CalculatePressure");
        viscosityKernel = shader.FindKernel("CalculateViscosity");
        forceKernel = shader.FindKernel("ApplyForces");

        shader.SetBuffer(externalKernel, "_particles", _particleBuffer);
        shader.SetBuffer(detectBoundsKernel, "_particles", _particleBuffer);
        shader.SetBuffer(densityKernel, "_particles", _particleBuffer);
        shader.SetBuffer(pressureKernel, "_particles", _particleBuffer);
        shader.SetBuffer(viscosityKernel, "_particles", _particleBuffer);
        shader.SetBuffer(forceKernel, "_particles", _particleBuffer);

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

    public void PushParticles(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            push = true;
        }
        else if(context.canceled)
        {
            push = false;
        }
    }

    public void PullParticles(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pull = true;
        }
        else if (context.canceled)
        {
            pull = false;
        }
    }

    private void produceColourGradientMap()
    {
        texture = new Texture2D(resolution, 1);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        colourMap = new Color[resolution];
        for(int i = 0; i< colourMap.Length; i++)
        {
            float t = i / (colourMap.Length - 1f);
            colourMap[i] = colourGradient.Evaluate(t);
        }

        texture.SetPixels(colourMap);
        texture.Apply();
        material.SetTexture("ColourMap", texture);
    }
}
