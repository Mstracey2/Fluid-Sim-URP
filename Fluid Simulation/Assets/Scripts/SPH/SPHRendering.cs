using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*SPH RENDERING
 * 
 * Script that holds the primary functionality for the SPH particle gradient and box rendering.
 * 
 * Holds functionality to produce colour gradients to be sampled in the shader.
 * 
 */

public class SPHRendering : MonoBehaviour
{
    #region Variables
    LineRenderer lineRend;

    public Transform transRef;  //centre box reference

    [Header("Particle Rendering")]
    public Mesh particleMesh;
    public Material material;
    public int resolution;  //resolution of colour sampling

    Color[] colourMap;
    Texture2D texture;
    public bool showSpheres = true;
    private Vector3 boxSize = new Vector3(4, 10, 3);

    private static readonly int ParticlesBufferProperty = Shader.PropertyToID("_particlesBuffer");

    private ComputeBuffer argsRendRef;

    #endregion

    #region frameSteps
    private void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 16;

    }

    // Update is called once per frame
    void Update()
    {
        if (showSpheres)
        {
            Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, new Bounds(Vector3.zero, boxSize), argsRendRef, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
        }

    }
    #endregion

    /// <summary>
    /// Creates a pattern to produce a box with the line renderer
    /// </summary>
    public void CalculateBoxVertices()
    {
        var trans = transRef;
        var min = trans.localPosition - trans.localScale * 0.5f;
        var max = trans.localPosition + trans.localScale * 0.5f;

        //position pattern
        lineRend.SetPosition(0, new Vector3(min.x, min.y, min.z));
        lineRend.SetPosition(1, new Vector3(min.x, min.y, max.z));
        lineRend.SetPosition(3, new Vector3(min.x, max.y, min.z));
        lineRend.SetPosition(2, new Vector3(min.x, max.y, max.z));
        lineRend.SetPosition(4, new Vector3(min.x, min.y, min.z));
        lineRend.SetPosition(5, new Vector3(max.x, min.y, min.z));
        lineRend.SetPosition(6, new Vector3(max.x, max.y, min.z));
        lineRend.SetPosition(7, new Vector3(min.x, max.y, min.z));
        lineRend.SetPosition(8, new Vector3(max.x, max.y, min.z));
        lineRend.SetPosition(9, (new Vector3(max.x, max.y, max.z)));
        lineRend.SetPosition(10, (new Vector3(min.x, max.y, max.z)));
        lineRend.SetPosition(11, (new Vector3(max.x, max.y, max.z)));
        lineRend.SetPosition(12, (new Vector3(max.x, min.y, max.z)));
        lineRend.SetPosition(13, (new Vector3(min.x, min.y, max.z)));
        lineRend.SetPosition(14, (new Vector3(max.x, min.y, max.z)));
        lineRend.SetPosition(15, (new Vector3(max.x, min.y, min.z)));
    }


    public void ProduceColourGradientMap(Gradient gradient)
    {
        //texture setup
        texture = new Texture2D(resolution, 1);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        colourMap = new Color[resolution];
        for (int i = 0; i < colourMap.Length; i++)
        {
            float t = i / (colourMap.Length - 1f);
            colourMap[i] = gradient.Evaluate(t);
        }

        texture.SetPixels(colourMap);
        texture.Apply();
        material.SetTexture("ColourMap", texture);
    }


    public ComputeBuffer CreateMeshArgsBuffer(int particleNum)
    {
        uint[] args =
        {
            particleMesh.GetIndexCount(0),
            (uint)particleNum,
            particleMesh.GetIndexStart(0),
            particleMesh.GetBaseVertex(0),
            0
        };
        ComputeBuffer argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        argsRendRef = argsBuffer;
        return argsBuffer;
    }

    #region Set GPU Variables
    public void SetShaderFloat(string variable, float value)
    {
        material.SetFloat(variable, value);
    }
    public void SetShaderProperties(ComputeBuffer particleBuffer)
    {
        material.SetBuffer(ParticlesBufferProperty, particleBuffer);
    }
    #endregion
}
