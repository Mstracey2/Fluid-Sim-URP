using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;

/*SPH
 * 
 * Script that holds the primary functionality for the SPH Sim.
 * 
 * Calls the setup for the sim, 
 * along with dispatching compute shader kernels on a time step.
 */


[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 120)]
public struct Particle
{
    public Vector3 pressure;
    public Vector2 density;
    public Vector3 viscosity;
    public Vector3 external;    //e.g gravity
    public Vector3 velocity;
    public Vector3 position;
    public Vector3 positionPrediction;  //prediction makes particles react to pressure changes earlier
    public uint3 hashData;

    /* Whenever particles require a default value setting, these are the data types that are passed through.
     * Primarily used for multiple fluid demonstration, holding 
    */
    public float defaultDensityTarget;
    public float defaultPressureMulti;
    public float defaultNearPressureMulti;
    public float defaultViscosityMulti;
    public Vector3 colour;
};


public class SPH : MonoBehaviour
{
    #region Variables
    [Header("Setup")]
    public bool fixedTimestep;              //Pauses the sim
    private float particleRadius = 0.7f;
    private int numOfParticleCalc = 3;      //calculations per frame
    private float timestep;
    int threads = 100;


    [Header("Mouse")]
    public GameObject mouseSphereRef;       //mouse object
    private Vector3 mouseRefCentre;         //mouse object pos


    [Header("Compute Shaders")]
    public ComputeShader SPHComputeshader;
    public ComputeShader sortingAlgorithm;  //Seb Lagues sorting Shader for spatial hashing

    [Header("SPH systems")]
    public SPHSetup particleSetter;
    public SPHRendering rendering;

    // buffer Data
    public Particle[] particles;
    private uint3[] hashDataVect;
    private uint[] offsetHashData;
    private int totalParticles;

    /*
     * Below are the buffers for the compute shader, used for the particle data and mesh arguments, 
     * along with spatial hash data
     */
    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _particleBuffer;
    private ComputeBuffer _hashData;
    private ComputeBuffer _offsetHashData;

    private GPUSort bufferSorter;       //Seb Lagues sorting algorithm for spatial hashing
    private EasyCompute computeHelp;

    //Kernals
    private int externalKernel;
    private int densityKernel;
    private int pressureKernel;
    private int forceKernel;
    private int viscosityKernel;
    private int spatialHashKernel;
    #endregion

    /*
     * I wanted to make compute shader variable finding more secure rather than finding through a string input.
     * This enum holds the names of all the variables, so scripts elsewhere (UI), can pick from this list.
     */
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

    #region FrameSteps
    private void Awake()
    {
        computeHelp = new EasyCompute(SPHComputeshader);    //simplifies the buffer setting

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
            //timestepper needs to be updated every frame due to Time.fixedDeltaTime
            float timeStepper = Time.fixedDeltaTime / timestep;
            SPHComputeshader.SetFloat("timestep", timeStepper);

            //do particle calculations multiple times a frame for more accuracy. (high amount causes poorer performance)
            for (int i = 0; i < numOfParticleCalc; i++)     
            {
                SimulateParticles();
            }
        }
    }

    private void Update()
    {
        //used for colour gradient (passing through the values required)
        rendering.SetShaderProperties(_particleBuffer);
        
        //setting mouse position
        mouseRefCentre = mouseSphereRef.transform.position;
        SPHComputeshader.SetVector("mousePos", mouseRefCentre);
    }
    #endregion

    
    /// <summary>
    /// This function dispatches the kernels.
    /// On each call, the hash data is collected and sorts the hash buffers (hash data and offset)
    /// </summary>
    private void SimulateParticles()
    {
        computeHelp.Dispatcher(new int[] { externalKernel, spatialHashKernel }, new Vector3Int(totalParticles / threads, 1, 1));

        bufferSorter.SortAndCalculateOffsets();

        computeHelp.Dispatcher(new int[] { densityKernel,pressureKernel,viscosityKernel,forceKernel }, new Vector3Int(totalParticles / threads, 1, 1));
    }

    #region Setting GPU variables
    /// <summary>
    /// a function called to change GPU floats, passing through its name and value
    /// </summary>
    /// <param name="variable"></param> this will be one of the names from the enum
    /// <param name="value"></param>
    public void SetGpuFloat(string variable, float value)
    {
        SPHComputeshader.SetFloat(variable, value);
    }

    public void SetGpuTimeStep(float value)
    {
        timestep = numOfParticleCalc * value;
    }

    //Requried for bounds changing.
    public void SetGpuMatrix()
    {
        SPHComputeshader.SetMatrix("worldMatrix", transform.localToWorldMatrix);
        SPHComputeshader.SetMatrix("localMatrix", transform.worldToLocalMatrix);
        rendering.CalculateBoxVertices();   //updates the line renderer
    }
    #endregion

    private void FindKernelsAndSetBuffers()
    {
       

        externalKernel = SPHComputeshader.FindKernel("CalculateExternalForces");
        densityKernel = SPHComputeshader.FindKernel("CalculateDensity");
        pressureKernel = SPHComputeshader.FindKernel("CalculatePressure");
        viscosityKernel = SPHComputeshader.FindKernel("CalculateViscosity");
        forceKernel = SPHComputeshader.FindKernel("ApplyForces");
        spatialHashKernel = SPHComputeshader.FindKernel("GetSpacialHash");

        //particles buffers (all kernels)
        computeHelp.BufferMaker(new int[] { externalKernel, densityKernel,pressureKernel, viscosityKernel, forceKernel, spatialHashKernel}, "_particles", _particleBuffer);

        //hash data buffers (hash and primary calculation kernels)
        computeHelp.BufferMaker(new int[] { densityKernel, pressureKernel, viscosityKernel, spatialHashKernel }, "hashData", _hashData);

        //hash offset buffers (hash and primary calculation kernels)
        computeHelp.BufferMaker(new int[] { densityKernel, pressureKernel, viscosityKernel, spatialHashKernel }, "hashOffsetData", _offsetHashData);

        //Used to sort the hash data and offset buffers, those buffers are passed through here
        bufferSorter = new GPUSort(sortingAlgorithm);
        bufferSorter.SetBuffers(_hashData, _offsetHashData);

    }

   /*
    * This function sets the compute buffers for the sim, called at awake.
    * SPH setup is called from here to setup the particles
    * Also called when resetting the particles (switching to multiple fluid demo)
    */
    public void SetParticles()
    {
        //used to allow the GPU to check whether its allowing changable behaviour or default particle settings
        SPHComputeshader.SetBool("staticFluidMultipliers", particleSetter.MultipleFluids);

        //if particles have already been set up.
        if (particles.Length > 0)
        {
            ReleaseBuffers();   //clear buffers
        }

        particles = particleSetter.ParticleSpawner();   //set up particles

        totalParticles = particles.Length;

        //args and particle buffer setup
        _argsBuffer = rendering.CreateMeshArgsBuffer(totalParticles);
        _particleBuffer = new ComputeBuffer(totalParticles, 120);
        _particleBuffer.SetData(particles);
        

        //hash data buffer setup
        hashDataVect = new uint3[totalParticles];
        offsetHashData = new uint[totalParticles];
        _hashData = new ComputeBuffer(totalParticles, 12);
        _hashData.SetData(hashDataVect);
        _offsetHashData = new ComputeBuffer(totalParticles, 4);
        _offsetHashData.SetData(offsetHashData);
        for (int i = 0; i < offsetHashData.Length; i++)
        {
            offsetHashData[i] = (uint)totalParticles;
        }

        FindKernelsAndSetBuffers();
        
        //This determines whether the particles are rendered by gradient or their appropriote default colour (water = blue, syrup = orange)
        if (particleSetter.MultipleFluids)
        {
            rendering.SetShaderFloat("gradientChoice", 4);
        }
        else
        {
            rendering.SetShaderFloat("gradientChoice", 1);
        }
    }

    #region Mouse Functions
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
    #endregion

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
