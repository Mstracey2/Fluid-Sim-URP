using UnityEngine;
using static UnityEngine.Mathf;

public class GPUSort
{
    int sortKernel;
    int calculateOffsetsKernel;


    readonly ComputeShader sortCompute;
    ComputeBuffer indexBuffer;

    public GPUSort(ComputeShader algorithm)
    {
        sortCompute = algorithm;
    }

    public void SetBuffers(ComputeBuffer indexBuffer, ComputeBuffer offsetBuffer)
    {
        this.indexBuffer = indexBuffer;

        sortKernel = sortCompute.FindKernel("Sort");
        calculateOffsetsKernel = sortCompute.FindKernel("CalculateOffsets");

        sortCompute.SetBuffer(sortKernel, "Entries", indexBuffer);
        sortCompute.SetBuffer(calculateOffsetsKernel , "Offsets", offsetBuffer);
        sortCompute.SetBuffer(calculateOffsetsKernel ,"Entries", indexBuffer);
    }

    // Sorts given buffer of integer values using bitonic merge sort
    // Note: buffer size is not restricted to powers of 2 in this implementation
    public void Sort()
    {
        sortCompute.SetInt("numEntries", indexBuffer.count);

        // Launch each step of the sorting algorithm (once the previous step is complete)
        // Number of steps = [log2(n) * (log2(n) + 1)] / 2
        // where n = nearest power of 2 that is greater or equal to the number of inputs
        int numStages = (int)Log(NextPowerOfTwo(indexBuffer.count), 2);

        for (int stageIndex = 0; stageIndex < numStages; stageIndex++)
        {
            for (int stepIndex = 0; stepIndex < stageIndex + 1; stepIndex++)
            {
                // Calculate some pattern stuff
                int groupWidth = 1 << (stageIndex - stepIndex);
                int groupHeight = 2 * groupWidth - 1;
                sortCompute.SetInt("groupWidth", groupWidth);
                sortCompute.SetInt("groupHeight", groupHeight);
                sortCompute.SetInt("stepIndex", stepIndex);
                // Run the sorting step on the GPU
                sortCompute.Dispatch(sortKernel, NextPowerOfTwo((indexBuffer.count) / 2), 1,1);
            }
        }
    }


    public void SortAndCalculateOffsets()
    {
        Sort();

        sortCompute.Dispatch(calculateOffsetsKernel, indexBuffer.count, 1,1);
    }

}