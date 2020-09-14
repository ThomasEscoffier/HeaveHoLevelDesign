using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(LineRenderer) )]
public class CurvedLineRenderer : MonoBehaviour 
{
	[SerializeField]
	protected float lineSegmentSize = 0.15f;
	[SerializeField]
	protected Transform[] linePoints;
	[SerializeField] 
	protected SpriteRenderer colorForRope;
	
	private Vector3[] linePositions = new Vector3[0];
	private LineRenderer line;

	AnimationCurve curveX = new AnimationCurve();
	AnimationCurve curveY = new AnimationCurve();
	AnimationCurve curveZ = new AnimationCurve();
	
	Keyframe[] keysX;
	Keyframe[] keysY;
	Keyframe[] keysZ;
	
	List<Vector3> lineSegments = new List<Vector3>();
	
	
	private void Awake()
	{
		line = GetComponent<LineRenderer>();
		line.sharedMaterial = new Material(line.sharedMaterial)
		{
			color = colorForRope.color
		};
		int length = linePoints.Length;
		
		//find curved points in children
		linePositions = new Vector3[length];

		//create keyframe sets
		keysX = new Keyframe[length];
		keysY = new Keyframe[length];
		keysZ = new Keyframe[length];
	}

	// Update is called once per frame
	public void Update () 
	{
		GetPoints();
		SetPointsToLine();
	}

	void GetPoints()
	{
		//add positions
		for( int i = 0; i < linePoints.Length; i++ )
		{
			linePositions[i] = linePoints[i].transform.position;
		}
	}

	void SetPointsToLine()
	{
		//get smoothed values
		Vector3[] smoothedPoints = SmoothLine( linePositions, lineSegmentSize );

		//set line settings
		line.positionCount = smoothedPoints.Length;
		line.SetPositions( smoothedPoints );
	}
	
	Vector3[] SmoothLine( Vector3[] inputPoints, float segmentSize )
	{
		//set keyframes
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			keysX[i] = new Keyframe( i, inputPoints[i].x );
			keysY[i] = new Keyframe( i, inputPoints[i].y );
			keysZ[i] = new Keyframe( i, inputPoints[i].z );
		}

		//apply keyframes to curves
		curveX.keys = keysX;
		curveY.keys = keysY;
		curveZ.keys = keysZ;

		//smooth curve tangents
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			curveX.SmoothTangents( i, 0 );
			curveY.SmoothTangents( i, 0 );
			curveZ.SmoothTangents( i, 0 );
		}

		//list to write smoothed values to
		lineSegments.Clear();

		//find segments in each section
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			//add first point
			lineSegments.Add( inputPoints[i] );

			//make sure within range of array
			if( i+1 < inputPoints.Length )
			{
				//find distance to next point
				float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i+1]);

				//number of segments
				int segments = (int)(distanceToNext / segmentSize);

				//add segments
				for( int s = 1; s < segments; s++ )
				{
					//interpolated time on curve
					float time = s/(float)segments + i;

					//sample curves to find smoothed position
					Vector3 newSegment = new Vector3( curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time) );

					//add to list
					lineSegments.Add( newSegment );
				}
			}
		}

		return lineSegments.ToArray();
	}
}
