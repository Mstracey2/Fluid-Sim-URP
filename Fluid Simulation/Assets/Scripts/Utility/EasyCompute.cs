using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyCompute
{
    ComputeShader shader;

    public EasyCompute(ComputeShader shader)
    {
        this.shader = shader;
    }

    public void BufferMaker(int[] kernelList, string structName, ComputeBuffer buffer )
    {
        foreach(int kernel in kernelList)
        {
            shader.SetBuffer(kernel,structName, buffer);
        }
    }

    public void Dispatcher(int[] kernelList, Vector3Int threads)
    {
        foreach (int kernel in kernelList)
        {
            shader.Dispatch(kernel, threads.x, threads.y, threads.z);
        }
    }
}
