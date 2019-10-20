using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField]
    private LineRenderer body;

    [SerializeField]
    private Transform rightEye;

    [SerializeField]
    private Transform leftEye;

    [SerializeField]
    private GameObject pointPrefab;

    private List<Transform> points;

    // Start is called before the first frame update
    void Start()
    {
        //transform from local axis to world axis
        points = new List<Transform>();
        Vector3[] bodyPoints = new Vector3[body.positionCount];
        body.GetPositions(bodyPoints);
        for(var i = 0; i < body.positionCount; i++)
        {
            var worldPosition = transform.TransformPoint(bodyPoints[i]);
            bodyPoints[i] = worldPosition;
            points.Add(Instantiate(pointPrefab, bodyPoints[i], Quaternion.identity, transform).transform);
        }
        body.useWorldSpace = true;
        body.SetPositions(bodyPoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
