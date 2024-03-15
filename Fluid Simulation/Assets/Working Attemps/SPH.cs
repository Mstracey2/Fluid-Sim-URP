using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using Unity.Mathematics;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 76)]
public struct Particle
{
    public Vector3 pressure;
    public Vector2 density;
    public Vector3 external;
    public Vector3 velocity;
    public Vector3 position;
    public Vector3 positionPrediction;
    public uint3 hashData;
}


public class SPH : MonoBehaviour
{
    [Header("Setup")]
    public float gravity;
    public float boundDamping = 0.2f;

    public bool fixedTimestep;
    public float particleRadius = 0.1f;
    public float timestep = -0.007f;
    public int numOfParticleCalc;


   

    [Header("Mouse")]
    public GameObject mouseSphereRef;
    public float pushPullForce;
    private Vector3 mouseRefCentre;
    public float mouseRadius;
    private bool pull;
    private bool push;

    [Header("Fluid Constants")]
    public float densityTarget;
    public float pressureForce;
    public float nearPressureForce;
    public float predictionIteration;
    public float viscosity;

    [Header("Compute Shaders")]
    public ComputeShader shader;
    public ComputeShader sortingAlgorithm;

    [Header("SPH systems")]
    public SPHSetup particleSetter;
    public SPHRendering rendering;

    // buffer Data
    private Particle[] particles;
    private uint3[] hashDataVect;
    private uint[] offsetHashData;
    private int totalParticles;

    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _particleBuffer;
    private ComputeBuffer _hashData;
    private ComputeBuffer _offsetHashData;

    private GPUSort bufferSorter;

    //Kernals
    private int externalKernel;
    private int densityKernel;
    private int pressureKernel;
    private int forceKernel;
    private int viscosityKernel;
    private int spatialHashKernel;



    

    private void Awake()
    {


        particles = particleSetter.SpawnParticlesInBox();
        totalParticles = particles.Length;

        _argsBuffer = rendering.CreateMeshArgsBuffer(totalParticles);

        _particleBuffer = new ComputeBuffer(totalParticles, 80);
        _particleBuffer.SetData(particles);

        hashDataVect = new uint3[totalParticles];
        offsetHashData = new uint[totalParticles];

        for(int i = 0;  i < offsetHashData.Length; i++)
        {
            offsetHashData[i] = (uint)totalParticles;
        }

        _hashData = new ComputeBuffer(totalParticles, 12);
        _hashData.SetData(hashDataVect);
        _offsetHashData = new ComputeBuffer(totalParticles, 4);
        _offsetHashData.SetData(offsetHashData);
        
        FindKernelsAndSetBuffers();

    }

    private void FixedUpdate()
    {
        if (fixedTimestep)
        {
            float timeStepper = Time.fixedDeltaTime / numOfParticleCalc * timestep;
            SetComputeVariables(timeStepper);
            for (int i = 0; i < numOfParticleCalc; i++)
            {

                SimulateParticles(Time.fixedDeltaTime);
            }

        }
    }

    private void Update()
    {
        mouseRefCentre = mouseSphereRef.transform.position;

            
    }
   

    int thread = 100;

    private void SimulateParticles(float frames)
    {
        float timeStepper = Time.fixedDeltaTime / numOfParticleCalc * timestep;
        SetComputeVariables(timeStepper);

        shader.Dispatch(externalKernel, totalParticles / thread, 1, 1);

        shader.Dispatch(spatialHashKernel, totalParticles / thread, 1, 1);
        bufferSorter.SortAndCalculateOffsets();

        
        shader.Dispatch(densityKernel, totalParticles / thread, 1 , 1);
        shader.Dispatch(pressureKernel, totalParticles / thread, 1 , 1);
        shader.Dispatch(viscosityKernel, totalParticles / thread, 1 , 1);
        shader.Dispatch(forceKernel, totalParticles / thread, 1 , 1);

        //_particleBuffer.GetData(particles);
        //_hashData.GetData(hashDataVect);
        //_offsetHashData.GetData(offsetHashData);
    }

    private void SetComputeVariables(float time)
    {
        shader.SetMatrix("worldMatrix", transform.localToWorldMatrix);
        shader.SetMatrix("localMatrix", transform.worldToLocalMatrix);
        shader.SetFloat("timestep", time);
        shader.SetInt("particleLength", totalParticles);
        shader.SetFloat("pi", Mathf.PI);
        shader.SetFloat("gravity", gravity);
        shader.SetFloat("radius", particleRadius);
        shader.SetFloat("boundDamping", boundDamping);
        shader.SetFloat("densityTarget", densityTarget);
        shader.SetFloat("pressureMulti", pressureForce);
        shader.SetFloat("nearPressureMulti", nearPressureForce);
        shader.SetFloat("predictionIteration", predictionIteration);
        shader.SetFloat("viscosityMulti", viscosity);
        shader.SetVector("mousePos", mouseRefCentre);
        shader.SetFloat("mouseRadius", mouseRadius);
        shader.SetFloat("pushPullForce", pushPullForce);
        shader.SetBool("push", push);
        shader.SetBool("pull", pull);
        shader.SetFloat("gradientChoice", gradientType);
        material.SetFloat("maxVel", particleMaxVelocity);
        material.SetFloat("gradientType", gradientType);
    }

    public void SetGpuBool(string gpuName, bool value)
    {
        shader.SetBool(gpuName, value);
    }

    public void SetGpuInt(string gpuName, int value)
    {
        shader.SetInt(gpuName, value);
    }

    public void SetGpuVector(string gpuName, Vector3 value)
    {
        shader.SetVector(gpuName, value);
    }

   

    private void FindKernelsAndSetBuffers()
    {
        externalKernel = shader.FindKernel("CalculateExternalForces");
        densityKernel = shader.FindKernel("CalculateDensity");
        pressureKernel = shader.FindKernel("CalculatePressure");
        viscosityKernel = shader.FindKernel("CalculateViscosity");
        forceKernel = shader.FindKernel("ApplyForces");
        spatialHashKernel = shader.FindKernel("GetSpacialHash");
       //detectBoundsKernel = shader.FindKernel("DetectBounds");


        shader.SetBuffer(externalKernel, "_particles", _particleBuffer);
        shader.SetBuffer(densityKernel, "_particles", _particleBuffer);
        shader.SetBuffer(pressureKernel, "_particles", _particleBuffer);
        shader.SetBuffer(viscosityKernel, "_particles", _particleBuffer);
        shader.SetBuffer(forceKernel, "_particles", _particleBuffer);
        shader.SetBuffer(spatialHashKernel, "_particles", _particleBuffer);
        //shader.SetBuffer(detectBoundsKernel, "_particles", _particleBuffer);

        shader.SetBuffer(spatialHashKernel, "hashData", _hashData);
        shader.SetBuffer(densityKernel, "hashData", _hashData);
        shader.SetBuffer(pressureKernel, "hashData", _hashData);
        shader.SetBuffer(viscosityKernel, "hashData", _hashData);

        shader.SetBuffer(spatialHashKernel, "hashOffsetData", _offsetHashData);
        shader.SetBuffer(densityKernel, "hashOffsetData", _offsetHashData);
        shader.SetBuffer(pressureKernel, "hashOffsetData", _offsetHashData);
        shader.SetBuffer(viscosityKernel, "hashOffsetData", _offsetHashData);


        bufferSorter = new GPUSort(sortingAlgorithm);
        bufferSorter.SetBuffers(_hashData, _offsetHashData);
         
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

    void OnDestroy()
    {
        _argsBuffer.Release();
        _particleBuffer.Release();
        _offsetHashData.Release();
        _hashData.Release();
    }
}
