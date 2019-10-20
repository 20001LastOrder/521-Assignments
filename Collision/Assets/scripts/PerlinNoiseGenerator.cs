using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator
{
    private System.Random r;

    private List<List<Vector2>> octaves;

    private float noiseLength;

    public PerlinNoiseGenerator(int seed, float noiseLength, int numOctaves, float min, float max, int increasingPoints=2, int initialPoints = 6, float maxDecay = 0.5f)
    {
        r = new System.Random(seed);
        octaves = new List<List<Vector2>>();
        this.noiseLength = noiseLength;
 
        //create octaves
        for(var i = 0; i < numOctaves; i++)
        {
            octaves.Add(CreateOctave(noiseLength, min, max, initialPoints));
            max *= maxDecay;
            initialPoints *= increasingPoints;
        }
    }

    // use linear interplotation
    private float Interplotation(float y1, float y2, float increament)
    {
        return y1 * (1 - increament) + y2 * increament; 
    }

    private List<Vector2> CreateOctave(float noiseLength, float min, float max, int numPoints)
    {
        List<Vector2> points = new List<Vector2>();
        float increament =(float)noiseLength / (numPoints - 1);

        for(var i = 0; i < numPoints; i++)
        {
            var x = i * increament;
            var y = min + ((float)r.NextDouble()) * (max - min);
            points.Add(new Vector2(x, y));
        }

        return points;
    }

    private float GetValue(float x, List<Vector2> octave)
    {
        for(var i = 1; i < octave.Count; i++)
        {
            if(x <= octave[i].x)
            {
                var increament = (x - octave[i-1].x) / (octave[i].x - octave[i-1].x); //normalize increment
                var value = Interplotation(octave[i-1].y, octave[i].y, increament);
                return value;
            }
        }

        return -1;
    }

    public float GetNoiseValue(float x)
    {
        float value = 0;

        foreach(var octave in octaves)
        {
            value += GetValue(x, octave);
        }

        return value;
    }
}
