using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 108)]
public struct Particle
{
    public Vector3 pressure;
    public Vector2 density;
    public Vector3 external;
    public Vector3 velocity;
    public Vector3 position;
    public Vector3 positionPrediction;
    public uint3 hashData;

    public float staticDensityTarget;
    public float staticPressureMulti;
    public float staticNearPressureMulti;
    public float staticViscosityMulti;
    public Vector3 colour;
};


public class SPH : MonoBehaviour
{
    [Header("Setup")]
    public bool fixedTimestep;
    private float particleRadius = 0.7f;
    private int numOfParticleCalc = 3;
    private float timestep;
    


    [Header("Mouse")]
    public GameObject mouseSphereRef;
    private Vector3 mouseRefCentre;


    [Header("Compute Shaders")]
    public ComputeShader SPHComputeshader;
    public ComputeShader sortingAlgorithm;

    [Header("SPH systems")]
    public SPHSetup particleSetter;
    public SPHRendering rendering;

    // buffer Data
    public Particle[] particles;
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
        mousePos,
        mouseRadius,
        pushPullForce,
        gradientChoice,
        maxVel,
        _size,
        gravity,
        timestep,
        boundDamping,
        NA,
    }

    private void Awake()
    {
        Screen.fullScreen = true;
        SetGpuTimeStep(0.9f);

        SetParticles();

        //constant GPU values.
        SPHComputeshader.SetFloat("radius", particleRadius);
        SPHComputeshader.SetInt("particleLength", totalParticles);
        SPHComputeshader.SetFloat("pi", Mathf.PI);
        
       
    }

    private void FixedUpdate()
    {
        if (fixedTimestep)
        {
            float timeStepper = Time.fixedDeltaTime / timestep;
            SPHComputeshader.SetFloat("timestep", timeStepper);

            for (int i = 0; i < numOfParticleCalc; i++)
            {
                SimulateParticles();
            }

        }
    }

    private void Update()
    {
        
        rendering.SetShaderProperties(_particleBuffer);
        
        //setting mouse position
        mouseRefCentre = mouseSphereRef.transform.position;
        SPHComputeshader.SetVector("mousePos", mouseRefCentre);
    }
   

    int thread = 100;

    private void SimulateParticles()
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

    public void SetGpuFloat(string variable, float value)
    {
        Debug.Log(value);
        SPHComputeshader.SetFloat(variable, value);
    }

    public void SetGpuTimeStep(float value)
    {
        timestep = numOfParticleCalc * value;
    }

    public void SetGpuMatrix()
    {
        SPHComputeshader.SetMatrix("worldMatrix", transform.localToWorldMatrix);
        SPHComputeshader.SetMatrix("localMatrix", transform.worldToLocalMatrix);
        rendering.CalculateBoxVertices();
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

    public void SetParticles()
    {
        SPHComputeshader.SetBool("staticFluidMultipliers", particleSetter.MultipleFluids);
        if (particles.Length > 0)
        {
            ReleaseBuffers();
           
        }
        particles = particleSetter.ParticleSpawner();
        totalParticles = particles.Length;

        _argsBuffer = rendering.CreateMeshArgsBuffer(totalParticles);

        _particleBuffer = new ComputeBuffer(totalParticles, 108);
        _particleBuffer.SetData(particles);

        hashDataVect = new uint3[totalParticles];
        offsetHashData = new uint[totalParticles];

        for (int i = 0; i < offsetHashData.Length; i++)
        {
            offsetHashData[i] = (uint)totalParticles;
        }
        _hashData = new ComputeBuffer(totalParticles, 12);
        _hashData.SetData(hashDataVect);
        _offsetHashData = new ComputeBuffer(totalParticles, 4);
        _offsetHashData.SetData(offsetHashData);
        FindKernelsAndSetBuffers();
        
        if (particleSetter.MultipleFluids)
        {
            rendering.SetShaderFloat("gradientChoice", 4);
        }
        else
        {
            rendering.SetShaderFloat("gradientChoice", 1);
        }
    }

    public void PushParticles(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SPHComputeshader.SetBool("push", true);
        }
        else if(context.canceled)
        {
            SPHComputeshader.SetBool("push", false);
        }
    }

    public void PullParticles(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SPHComputeshader.SetBool("pull", true);
        }
        else if (context.canceled)
        {
            SPHComputeshader.SetBool("pull", false);
        }
    }

    void OnDestroy()
    {
        ReleaseBuffers();
    }

    public void ReleaseBuffers()
    {
        _argsBuffer.Release();
        _particleBuffer.Release();
        _offsetHashData.Release();
        _hashData.Release();
        bufferSorter.ReleaseBuffers();
    }

}
