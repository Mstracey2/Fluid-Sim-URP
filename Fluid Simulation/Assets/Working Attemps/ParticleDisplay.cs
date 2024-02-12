using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDisplay : MonoBehaviour
{
    Material mat;
    public Gradient colourGradient;
    public int resolution;
    public SPH sph;
    public Color col;
    Texture2D newGradientTexture;

	public void ProduceGradientColour(Material mat)
    {
		TextureFromGradient(ref newGradientTexture, resolution, colourGradient);
		mat.SetTexture("_BaseColor", newGradientTexture);
    }

	public static void TextureFromGradient(ref Texture2D texture, int width, Gradient gradient, FilterMode filterMode = FilterMode.Bilinear)
	{
		if (texture == null)
		{
			texture = new Texture2D(width, 1);
		}
		else if (texture.width != width)
		{
			texture.Reinitialize(width, 1);
		}
		if (gradient == null)
		{
			gradient = new Gradient();
			gradient.SetKeys(
				new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.black, 1) },
				new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
			);
		}
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = filterMode;

		Color[] cols = new Color[width];
		for (int i = 0; i < cols.Length; i++)
		{
			float t = i / (cols.Length - 1f);
			cols[i] = gradient.Evaluate(t);
		}
		texture.SetPixels(cols);
		texture.Apply();
	}
}
