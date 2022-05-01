using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//--------------------- This is a script I am reworking now. When done, we should have our own collision system --------------------

public class RotatedRect : MonoBehaviour
{
	
	public Rect collisionRect;
	public float rotation;
	public Vector2 origin;
	
	public RotatedRect(Rect theRect, float theInitialRotation) {
		this.collisionRect = theRect;
		this.rotation = theInitialRotation;
		
		//Calculate the Rectangles origin. We assume the center of the Rectangle will
        //be the point that we will be rotating around and we use that for the origin
		this.origin = new Vector2((int)theRect.width / 2, (int)theRect.height /2);
	}
	
	/// <summary>
    /// Used for changing the X and Y position of the RotatedRectangle
    /// </summary>
    /// <param name="theXPositionAdjustment"></param>
    /// <param name="theYPositionAdjustment"></param> 
	public void ChangePosition(int theXPositionAdjustment, int theYPositionAdjustment) {
		this.collisionRect.x += theXPositionAdjustment;
		this.collisionRect.y += theYPositionAdjustment;
	}
	
	/// <summary>
    /// This intersects method can be used to check a standard Unity Rectangle
    /// object and see if it collides with a Rotated Rectangle object
    /// </summary>
    /// <param name="theRectangle"></param>
    /// <returns></returns>
    public bool Intersects(Rect theRect) {
		return this.Intersects(new RotatedRect(theRect, 0.0f));
	}
	
	/// <summary>
    /// Check to see if two Rotated Rectangles have collided
    /// </summary>
    /// <param name="theRectangle"></param>
    /// <returns></returns>
    public bool Intersects(RotatedRect theRect) {
		//Calculate the Axis we will use to determine if a collision has occurred
        //Since the objects are rectangles, we only have to generate 4 Axis (2 for
        //each rectangle) since we know the other 2 on a rectangle are parallel.
		ArrayList aRectAxis = new ArrayList();
		aRectAxis.Add(this.UpperRightCorner() - this.UpperLeftCorner());
		aRectAxis.Add(this.UpperRightCorner() - this.LowerRightCorner());
		aRectAxis.Add(theRect.UpperLeftCorner() - theRect.LowerLeftCorner());
		aRectAxis.Add(theRect.UpperLeftCorner() - theRect.UpperRightCorner());
		
		//Cycle through all of the Axis we need to check. If a collision does not occur
        //on ALL of the Axis, then a collision is NOT occurring. We can then exit out 
        //immediately and notify the calling function that no collision was detected. If
        //a collision DOES occur on ALL of the Axis, then there is a collision occurring
        //between the rotated rectangles. We know this to be true by the Seperating Axis Theorem
		foreach(Vector2 aAxis in aRectAxis) {
			if(!this.IsAxisCollision(theRect, aAxis)) {
				return false;
			}
		}
		
		return true;
		
	}
	
	/// <summary>
    /// Determines if a collision has occurred on an Axis of one of the
    /// planes parallel to the Rectangle
    /// </summary>
    /// <param name="theRectangle"></param>
    /// <param name="aAxis"></param>
    /// <returns></returns>
    private bool IsAxisCollision(RotatedRect theRect, Vector2 aAxis) {
		//Project the corners of the Rectangle we are checking on to the Axis and
        //get a scalar value of that project we can then use for comparison
		List<float> aRectAScalars = new List<float>();
		aRectAScalars.Add(this.GenerateScalar(theRect.UpperLeftCorner(), aAxis));
		aRectAScalars.Add(this.GenerateScalar(theRect.UpperRightCorner(), aAxis));
		aRectAScalars.Add(this.GenerateScalar(theRect.LowerLeftCorner(), aAxis));
		aRectAScalars.Add(this.GenerateScalar(theRect.LowerRightCorner(), aAxis));
		
		//Project the corners of the current Rectangle on to the Axis and
        //get a scalar value of that project we can then use for comparison
		List<float> aRectBScalars = new List<float>();
		aRectBScalars.Add(this.GenerateScalar(this.UpperLeftCorner(), aAxis));
		aRectBScalars.Add(this.GenerateScalar(this.UpperRightCorner(), aAxis));
		aRectBScalars.Add(this.GenerateScalar(this.LowerLeftCorner(), aAxis));
		aRectBScalars.Add(this.GenerateScalar(this.LowerRightCorner(), aAxis));
		
		//Get the Maximum and Minimum Scalar values for each of the Rectangles
		//int aRectAMinimum = aRectAScalars.Min()
		float aRectAMinimum = aRectAScalars[0];
		foreach(float temp in aRectAScalars) {
			if(temp < aRectAMinimum) {
				aRectAMinimum = temp;
			}
		}
		//int aRectAMaximum = aRectAScalars.Max();
		float aRectAMaximum = aRectAScalars[0];
		foreach(float temp in aRectAScalars) {
			if(temp > aRectAMaximum) {
				aRectAMaximum = temp;
			}
		}
		//int aRectBMinimum = aRectBScalars.Min()
		float aRectBMinimum = aRectBScalars[0];
		foreach(float temp in aRectBScalars) {
			if(temp < aRectBMinimum) {
				aRectBMinimum = temp;
			}
		}
		//int aRectBMaximum = aRectBScalars.Max();
		float aRectBMaximum = aRectBScalars[0];
		foreach(float temp in aRectBScalars) {
			if(temp > aRectBMaximum) {
				aRectBMaximum = temp;
			}
		}
		
		//If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
        //then we are detecting a collision between the rectangles on this Axis
		if(aRectBMinimum <= aRectAMaximum && aRectBMaximum >= aRectAMaximum) {
			return true;
		} else if(aRectAMinimum <= aRectBMaximum && aRectAMaximum >= aRectBMaximum) {
			return true;
		}

		return false;
	}
	
	/// <summary>
    /// Generates a scalar value that can be used to compare where corners of 
    /// a rectangle have been projected onto a particular axis. 
    /// </summary>
    /// <param name="theRectCorner"></param>
    /// <param name="theAxis"></param>
    /// <returns></returns>
    private float GenerateScalar(Vector2 theRectCorner, Vector2 theAxis) {
		//Using the formula for Vector projection. Take the corner being passed in
        //and project it onto the given Axis
		float aNumerator = (theRectCorner.x * theAxis.x) + (theRectCorner.y * theAxis.y);
        float aDenominator = (theAxis.x * theAxis.x) + (theAxis.y * theAxis.y);
        float aDivisionResult = aNumerator / aDenominator;
        Vector2 aCornerProjected = new Vector2(aDivisionResult * theAxis.x, aDivisionResult * theAxis.y);
		
		//Now that we have our projected Vector, calculate a scalar of that projection
        //that can be used to more easily do comparisons
		float aScalar = (theAxis.x * aCornerProjected.x) + (theAxis.y * aCornerProjected.y);
		return aScalar;
	}
	
	/// <summary>
    /// Rotate a point from a given location and adjust using the Origin we
    /// are rotating around
    /// </summary>
    /// <param name="thePoint"></param>
    /// <param name="theOrigin"></param>
    /// <param name="theRotation"></param>
    /// <returns></returns>
	private Vector2 RotatePoint(Vector2 thePoint, Vector2 theOrigin, float theRotation) {
		Vector2 aTranslatedPoint = new Vector2();
        aTranslatedPoint.x = (float)(theOrigin.x + (thePoint.x - theOrigin.x) * Mathf.Cos(theRotation)
            - (thePoint.y - theOrigin.y) * Mathf.Sin(theRotation));
        aTranslatedPoint.y = (float)(theOrigin.y + (thePoint.y - theOrigin.y) * Mathf.Cos(theRotation)
            + (thePoint.x - theOrigin.x) * Mathf.Sin(theRotation));
        return aTranslatedPoint;
	}
	
	public Vector2 UpperLeftCorner()
    {
        Vector2 aUpperLeft = new Vector2(this.collisionRect.xMin, this.collisionRect.yMin);
        aUpperLeft = this.RotatePoint(aUpperLeft, aUpperLeft + this.origin, this.rotation);
        return aUpperLeft;
    }

    public Vector2 UpperRightCorner()
    {
        Vector2 aUpperRight = new Vector2(this.collisionRect.xMax, this.collisionRect.yMin);
        aUpperRight = this.RotatePoint(aUpperRight, aUpperRight + new Vector2(-this.origin.x, this.origin.y), this.rotation);
        return aUpperRight;
    }

    public Vector2 LowerLeftCorner()
    {
        Vector2 aLowerLeft = new Vector2(this.collisionRect.xMin, this.collisionRect.yMax);
        aLowerLeft = this.RotatePoint(aLowerLeft, aLowerLeft + new Vector2(this.origin.x, -this.origin.y), this.rotation);
        return aLowerLeft;
    }

    public Vector2 LowerRightCorner()
    {
        Vector2 aLowerRight = new Vector2(this.collisionRect.xMax, this.collisionRect.yMax);
        aLowerRight = this.RotatePoint(aLowerRight, aLowerRight + new Vector2(-this.origin.x, -this.origin.y), this.rotation);
        return aLowerRight;
    }

    public float x
    {
        get { return this.collisionRect.x; }
    }

    public float y
    {
        get { return this.collisionRect.y; }
    }

    public float width
    {
        get { return this.collisionRect.width; }
    }

    public float height
    {
        get { return this.collisionRect.height; }
    }
}
