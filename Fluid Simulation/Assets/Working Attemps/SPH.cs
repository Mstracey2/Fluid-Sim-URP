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
    public int gradientType;

    [Header("Compute Shaders")]
    public ComputeShader SPHComputeshader;
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


    public enum GPUVariables
    {
        densityTarget,
        pressureMulti,
        nearPressureMulti,
        viscosityMulti,
        predictionIteration,
    }



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
        SPHComputeshader.SetInt("particleLength", totalParticles);
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
        rendering.SetShaderProperties(_particleBuffer, gradientType);
        mouseRefCentre = mouseSphereRef.transform.position;   
    }
   

    int thread = 100;

    private void SimulateParticles(float frames)
    {
        SPHComputeshader.Dispatch(externalKernel, totalParticles / thread, 1, 1);
        SPHComputeshader.Dispatch(spatialHashKernel, totalParticles / thread, 1, 1);
        bufferSorter.SortAndCalculateOffsets();

        SPHComputeshader.Dispatch(densityKernel, totalParticles / thread, 1 , 1);
        SPHComputeshader.Dispatch(pressureKernel, totalParticles / thread, 1 , 1);
        SPHComputeshader.Dispatch(viscosityKernel, totalParticles / thread, 1 , 1);
        SPHComputeshader.Dispatch(forceKernel, totalParticles / thread, 1 , 1);

        //_particleBuffer.GetData(particles);
        //_hashData.GetData(hashDataVect);
        //_offsetHashData.GetData(offsetHashData);
    }

    private void SetComputeVariables(float time)
    {
        
        SPHComputeshader.SetMatrix("worldMatrix", transform.localToWorldMatrix);
        SPHComputeshader.SetMatrix("localMatrix", transform.worldToLocalMatrix);
        SPHComputeshader.SetFloat("timestep", time);
       
        SPHComputeshader.SetFloat("pi", Mathf.PI);
        SPHComputeshader.SetFloat("gravity", gravity);
        SPHComputeshader.SetFloat("radius", particleRadius);
        SPHComputeshader.SetFloat("boundDamping", boundDamping);
        
        SPHComputeshader.SetVector("mousePos", mouseRefCentre);
        SPHComputeshader.SetFloat("mouseRadius", mouseRadius);
        SPHComputeshader.SetFloat("pushPullForce", pushPullForce);
        SPHComputeshader.SetBool("push", push);
        SPHComputeshader.SetBool("pull", pull);
        SPHComputeshader.SetFloat("gradientChoice", gradientType);

        //SPHComputeshader.SetFloat("densityTarget", densityTarget);
        //SPHComputeshader.SetFloat("pressureMulti", pressureForce);
        //SPHComputeshader.SetFloat("nearPressureMulti", nearPressureForce);
        //SPHComputeshader.SetFloat("predictionIteration", predictionIteration);
        //SPHComputeshader.SetFloat("viscosityMulti", viscosity);
       

        rendering.ProduceColourGradientMap();

    }

    public void SetGpuBool(GPUVariables variable, bool value)
    {
        SPHComputeshader.SetBool(nameof(variable), value);
    }

    public void SetGpuInt(GPUVariables variable, int value)
    {
        SPHComputeshader.SetInt(nameof(variable), value);
    }

    public void SetGpuFloat(GPUVariables variable, float value)
    {
        SPHComputeshader.SetFloat(nameof(variable), value);
    }

    public void SetGpuVector(GPUVariables variable, Vector3 value)
    {
        SPHComputeshader.SetVector(nameof(variable), value);
    }

   

    private void FindKernelsAndSetBuffers()
    {
        externalKernel = SPHComputeshader.FindKernel("CalculateExternalForces");
        densityKernel = SPHComputeshader.FindKernel("CalculateDensity");
        pressureKernel = SPHComputeshader.FindKernel("CalculatePressure");
        viscosityKernel = SPHComputeshader.FindKernel("CalculateViscosity");
        forceKernel = SPHComputeshader.FindKernel("ApplyForces");
        spatialHashKernel = SPHComputeshader.FindKernel("GetSpacialHash");
       //detectBoundsKernel = shader.FindKernel("DetectBounds");


        SPHComputeshader.SetBuffer(externalKernel, "_particles", _particleBuffer);
        SPHComputeshader.SetBuffer(densityKernel, "_particles", _particleBuffer);
        SPHComputeshader.SetBuffer(pressureKernel, "_particles", _particleBuffer);
        SPHComputeshader.SetBuffer(viscosityKernel, "_particles", _particleBuffer);
        SPHComputeshader.SetBuffer(forceKernel, "_particles", _particleBuffer);
        SPHComputeshader.SetBuffer(spatialHashKernel, "_particles", _particleBuffer);
        //shader.SetBuffer(detectBoundsKernel, "_particles", _particleBuffer);

        SPHComputeshader.SetBuffer(spatialHashKernel, "hashData", _hashData);
        SPHComputeshader.SetBuffer(densityKernel, "hashData", _hashData);
        SPHComputeshader.SetBuffer(pressureKernel, "hashData", _hashData);
        SPHComputeshader.SetBuffer(viscosityKernel, "hashData", _hashData);

        SPHComputeshader.SetBuffer(spatialHashKernel, "hashOffsetData", _offsetHashData);
        SPHComputeshader.SetBuffer(densityKernel, "hashOffsetData", _offsetHashData);
        SPHComputeshader.SetBuffer(pressureKernel, "hashOffsetData", _offsetHashData);
        SPHComputeshader.SetBuffer(viscosityKernel, "hashOffsetData", _offsetHashData);


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
