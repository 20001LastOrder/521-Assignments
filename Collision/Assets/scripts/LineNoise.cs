using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LineNoise : MonoBehaviour
{
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

    [SerializeField]
    private LineRenderer approximator;

    [SerializeField]
    private LineRenderer lineApproximator;

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

    [SerializeField]
    private float groundLevel = -4.5f;

    void Start()
    {


        var positions = CreateStone();
        var stoneBottomPoints = positions.GetRange(numPointsWidth + numPointsHeight, numPointsWidth + numPointsHeight);

        //create perlin noise
        PerlinNoiseGenerator generator = new PerlinNoiseGenerator(Utils.RandomInt(), noiseLength, 8, 0, 0.5f);
        DrawPillar(generator, stoneBottomPoints, leftPillarLeft, -1);
        DrawPillar(generator, stoneBottomPoints, leftPillarRight, 1);
        DrawPillar(generator, stoneBottomPoints, rightPillarLeft, -1);
        DrawPillar(generator, stoneBottomPoints, rightPillarRight, 1);
        List<Vector3> approximators = new List<Vector3>();
        approximator.positionCount = 13;

        approximators.AddRange(GetApproxPoints(positions.GetRange(0, numPointsWidth - 1), 4));
        approximators.AddRange(GetApproxPoints(positions.GetRange(numPointsWidth, numPointsHeight - 1), 2));
        approximators.AddRange(GetApproxPoints(positions.GetRange(numPointsWidth + numPointsHeight, numPointsWidth - 1), 4));
        approximators.AddRange(GetApproxPoints(positions.GetRange(2* numPointsWidth + numPointsHeight, numPointsHeight - 1), 2));
        approximators.Add(approximators[0]);
        approximator.SetPositions(approximators.ToArray());

        lineApproximator.positionCount = 3;

        Vector3[] lineApproximators = new Vector3[rightPillarRight.positionCount];
        rightPillarRight.GetPositions(lineApproximators);

        lineApproximator.transform.position = rightPillarRight.transform.position;
        var bottomLineApproximator = new List<Vector3>(lineApproximators);
        bottomLineApproximator = GetApproxPoints(bottomLineApproximator, 3);
        lineApproximator.SetPositions(bottomLineApproximator.ToArray());

        approximators = approximators.Select(p => transform.TransformPoint(p)).ToList();
        bottomLineApproximator = bottomLineApproximator.Select(p => rightPillarRight.transform.TransformPoint(p)).ToList();

        EnvironmentManager.Instance.RegisterCollider(approximators);
        EnvironmentManager.Instance.RegisterCollider(bottomLineApproximator);

    }

    public List<Vector3> GetApproxPoints(List<Vector3> points, int approximatorNum)
    {
        List<Vector3> approxs = new List<Vector3>();
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

    private void DrawPillar(PerlinNoiseGenerator g, List<Vector3> stoneBottomPoints, LineRenderer pillar, int noiseDirection)
    {
        var pillarTop = pillar.transform.localPosition;
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

        // +1 to make sure that the noise does not go out of the bound
        var pillarPositions = VerticalNoise(g, 0, new Vector3(0, 0, 0), numPointsWidth, pillarTopHeight - groundLevel, 0.5f, -1, noiseDirection);

        pillar.positionCount = numPointsWidth;
        pillar.SetPositions(pillarPositions.ToArray());
    }

    private List<Vector3> CreateStone()
    {
        PerlinNoiseGenerator generator = new PerlinNoiseGenerator(Utils.RandomInt(), noiseLength * 4, 8, 0, 1, 2, 24);

        var positions = new List<Vector3>();
        var start = topStone.GetPosition(0);
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
