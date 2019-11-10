using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Create noise for the stonehange
public class LineNoise : MonoBehaviour
{
    //Renders for different components of the stonehange
    [SerializeField]
    private LineRenderer topStone;
    [SerializeField]
    private LineRenderer leftPillarLeft;
    [SerializeField]
    private LineRenderer leftPillarRight;
    [SerializeField]
    private LineRenderer rightPillarLeft;
    [SerializeField]
    private LineRenderer rightPillarRight;

    // The renderer for the approximator (used for debug)
    [SerializeField]
    private LineRenderer approximator;
    [SerializeField]
    private LineRenderer rightPillarApproximator;
    [SerializeField]
    private LineRenderer leftPillarApproximator;

    // characteristics of the noise
    [SerializeField]
    private float modifyScale = 1f;

    [SerializeField]
    private float noiseLength = 1.0f;

    [SerializeField]
    int numPointsWidth = 50;

    [SerializeField]
    private int numPointsHeight = 10;

    [SerializeField]
    private float height = 1.0f;

    [SerializeField]
    private float width = 5.0f;

    // approximator-related
    [SerializeField]
    private int numPillarApproximator = 3;
    [SerializeField]
    private int numHorizontalApproximator = 4;
    [SerializeField]
    private int numVerticalApproximator = 2;

    private float groundLevel;

    void Start()
    {
        groundLevel = EnvironmentManager.Instance.GroundLevel;

        // create the top stone with perlin nose
        var positions = CreateStone();
        var stoneBottomPoints = positions.GetRange(numPointsWidth + numPointsHeight, numPointsWidth + numPointsHeight);

        //create the bottom stones with perlin noise
        PerlinNoiseGenerator generator = new PerlinNoiseGenerator(Utils.RandomInt(), noiseLength, 8, 0, 0.5f);
        DrawPillar(generator, stoneBottomPoints, leftPillarLeft, -1);
        DrawPillar(generator, stoneBottomPoints, leftPillarRight, 1);
        DrawPillar(generator, stoneBottomPoints, rightPillarLeft, -1);
        DrawPillar(generator, stoneBottomPoints, rightPillarRight, 1);

        // Create approximator of the stones for collider
        List<Vector3> approximators = new List<Vector3>();
        approximator.positionCount = 13;
        approximators.AddRange(GetApproxPoints(positions.GetRange(0, numPointsWidth - 1), numHorizontalApproximator));
        approximators.AddRange(GetApproxPoints(positions.GetRange(numPointsWidth, numPointsHeight - 1), numVerticalApproximator));
        approximators.AddRange(GetApproxPoints(positions.GetRange(numPointsWidth + numPointsHeight, numPointsWidth - 1), numHorizontalApproximator));
        approximators.AddRange(GetApproxPoints(positions.GetRange(2* numPointsWidth + numPointsHeight, numPointsHeight - 1), numVerticalApproximator));
        approximators.Add(approximators[0]);
        approximator.SetPositions(approximators.ToArray());

        // create collider for bottom of the stonehange
        var rightPillarApproximatorPositions = CreatePillarCollider(rightPillarRight, rightPillarApproximator);
        var leftPillarApproximatorPositions = CreatePillarCollider(leftPillarLeft, leftPillarApproximator);

        // Since stonehange is create in local axis space, translate them into world space
        approximators = approximators.Select(p => transform.TransformPoint(p)).ToList();
        rightPillarApproximatorPositions = rightPillarApproximatorPositions.Select(p => rightPillarRight.transform.TransformPoint(p)).ToList();
        leftPillarApproximatorPositions = leftPillarApproximatorPositions.Select(p => leftPillarLeft.transform.TransformPoint(p)).ToList();

        // register the colliders
        EnvironmentManager.Instance.RegisterCollider(approximators);
        EnvironmentManager.Instance.RegisterCollider(rightPillarApproximatorPositions);
        EnvironmentManager.Instance.RegisterCollider(leftPillarApproximatorPositions);
    }

    // approximate the lower part of the stonehange to create a collider
    private List<Vector3> CreatePillarCollider(LineRenderer pillar, LineRenderer approximator)
    {
        approximator.positionCount = numPillarApproximator;
        Vector3[] lineApproximators = new Vector3[pillar.positionCount];
        pillar.GetPositions(lineApproximators);

        approximator.transform.position = pillar.transform.position;
        var bottomLineApproximator = new List<Vector3>(lineApproximators);
        bottomLineApproximator = GetApproxPoints(bottomLineApproximator, 3);
        //make the approximator higher to make sure it covers the bottom of the stonehange regardless of the noise
        bottomLineApproximator[0] += new Vector3(0, 1, 0);
        approximator.SetPositions(bottomLineApproximator.ToArray());
        return bottomLineApproximator;
    }

    // create an approximator for a line shape by sampling the original shape with equal spaces between points
    public List<Vector3> GetApproxPoints(List<Vector3> points, int approximatorNum)
    {
        List<Vector3> approxs = new List<Vector3>();

        // reserve the first two points to make sure the approximator is close to the original shape
        approxs.Add(points[0]);
        approxs.Add(points[1]);

        if(approximatorNum <= 2)
        {
            return approxs;
        }

        int increment = (points.Count - 1) / (approximatorNum - 2);
        for (var i = 1; i < approximatorNum-1; i++)
        {
            approxs.Add(points[i * increment]);
        }

        return approxs;
    }

    /* create the bottom stone (simulated by two lines) with perlin noise
     * stoneBottomPoints: the bottom line of the top stone
      */
    private void DrawPillar(PerlinNoiseGenerator g, List<Vector3> stoneBottomPoints, LineRenderer pillar, int noiseDirection)
    {
        var pillarTop = pillar.transform.localPosition;
        
        // find the suitable position at the bottom of the top stone to start draw (to make them always connected) 
        for (var i = 0; i < stoneBottomPoints.Count; i++)
        {
            // x value decreses
            if (stoneBottomPoints[i].x < pillarTop.x)
            {
                pillarTop = Vector3.Lerp(stoneBottomPoints[i - 1], stoneBottomPoints[i], stoneBottomPoints[i - 1].x - pillarTop.x);
                break;
            }
        }

        pillar.transform.localPosition = pillarTop;
        var pillarTopHeight = this.transform.TransformPoint(pillarTop).y;
        var pillarPositions = VerticalNoise(g, 0, new Vector3(0, 0, 0), numPointsWidth, pillarTopHeight - groundLevel, 0.5f, -1, noiseDirection);

        pillar.positionCount = numPointsWidth;
        pillar.SetPositions(pillarPositions.ToArray());
    }

    // Create the top stone
    private List<Vector3> CreateStone()
    {
        PerlinNoiseGenerator generator = new PerlinNoiseGenerator(Utils.RandomInt(), noiseLength * 4, 8, 0, 1, 2, 24);

        var positions = new List<Vector3>();
        var start = topStone.GetPosition(0);

        // create for each direction, for each direction, start at the end of the previous direction
        positions.AddRange(HorizontalNoise(generator, 0, start, numPointsWidth, modifyScale, 1, 1));
        positions.AddRange(VerticalNoise(generator, noiseLength, positions[positions.Count - 1], numPointsHeight, height, modifyScale, -1, 1));
        positions.AddRange(HorizontalNoise(generator, 2 * noiseLength, positions[positions.Count - 1], numPointsWidth, modifyScale, -1, -1));
        var newHeight = Mathf.Abs(positions[positions.Count - 1].y - topStone.GetPosition(0).y);
        positions.AddRange(VerticalNoise(generator, 3 * noiseLength, positions[positions.Count - 1], numPointsHeight - 1, newHeight, modifyScale, 1, -1));
        positions.Add(topStone.GetPosition(0));

        topStone.positionCount = numPointsWidth * 2 + numPointsHeight * 2;
        topStone.SetPositions(positions.ToArray());
        return positions;
    }

    // create a horizonal lin with perlin noise modifying the height
    private List<Vector3> HorizontalNoise(PerlinNoiseGenerator g, float origin, Vector3 start, int numPoints, float noiseHeight, int direction, int noiseDirection)
    {
        var scale = (numPoints - 1) / width;
        List<Vector3> points = new List<Vector3>();
        points.Add(start);
        for(var i = 1; i < numPoints; i++)
        {
            var xPosition = start.x + direction * (i / scale);

            // - 0.1 to make sure that the x value does not go out of bound
            float height = noiseHeight * noiseDirection * g.GetNoiseValue(origin + direction*(xPosition / width) * (noiseLength - 0.1f));
            points.Add(new Vector3(xPosition, start.y + height, 0.0f));
        }
        return points;
    }

    // create a verticle line with perlin noise modifying the width
    private List<Vector3> VerticalNoise(PerlinNoiseGenerator g, float origin, Vector3 start, int numPoints, float length, float noiseHeight, int direction, int noiseDirection)
    {
        var scale = (numPoints - 1) / length;
        List<Vector3> points = new List<Vector3>
        {
            start
        };
        for (var i = 1; i < numPoints; i++)
        {
            var yPosition = start.y + direction * (i / scale);

            // - 0.1 to make sure that the y value does not go out of bound
            float width = noiseHeight * noiseDirection * g.GetNoiseValue(origin + (direction)*(yPosition / length) * (noiseLength - 0.1f));
            points.Add(new Vector3(start.x + width, yPosition, 0.0f));
        }
        return points;
    }
}
