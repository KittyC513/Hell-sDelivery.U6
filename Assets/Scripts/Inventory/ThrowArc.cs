using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class ThrowArc : MonoBehaviour
{
    private float showArc;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float curveLength;
    private float maxCurveLength;
    [SerializeField] private float minimumCurveLength = 1.5f;
    [SerializeField] private int lineSegments = 10;
    private Vector3[] segments;

    //line variables
    private Vector3 lineStartPos;
    public Vector3 velocity;
    private float gravity;

    private CustomGravity customGravity;

    private bool showThrowArc;
    

    private void Start()
    {
        segments = new Vector3[lineSegments];
        maxCurveLength = curveLength;
        lineRenderer.positionCount = lineSegments;

        //check for a custom gravity component to set the gravity
        CustomGravity temp = GetComponent<CustomGravity>();
    }


    public void ShowThrowArc(Vector3 _velocity, Vector3 startPos, float percent, float _gravity)
    {
        velocity = _velocity;
        lineStartPos = startPos;

        velocity = _velocity;
        gravity = _gravity;
        curveLength = Mathf.Lerp(minimumCurveLength, maxCurveLength, percent);

        showThrowArc = true;
    }

    public void StopThrowArc()
    {
        showThrowArc = false;
    }

    private void Update()
    {
        if (showThrowArc)
        {
            //set the start position
            segments[0] = lineStartPos;
            lineRenderer.SetPosition(0, lineStartPos);

            for (int i = 1; i < lineSegments; i++)
            {
                float time = (i * Time.fixedDeltaTime * curveLength);

                gravity = 9.81f;
                Vector3 gravityVel = 0.5f * (gravity*Vector3.down) * Mathf.Pow(time, 2);

                //Vector3 timeVel = (velocity + gravityVel) * time;

                segments[i] = segments[0] + velocity * time + gravityVel;
                lineRenderer.SetPosition(i, segments[i]);
            }

            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
   
    }
}
