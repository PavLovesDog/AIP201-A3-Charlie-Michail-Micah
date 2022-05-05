using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    List<List<float>> flowFieldForces = new List<List<float>>(); // a list of lists!

    public int width = 25;
    public int height = 25;

    public int gridStartX;
    public int gridStartY;

    public float previousDirection;

    // Start is called before the first frame update
    void Start()
    {
        //previousDirection = 90.0f;// Random.Range(0, 360); // generate random direction for new row
        

        // Generate grid
        for (int y = gridStartY; y < gridStartY + height; ++y)
        {
            List<float> row = new List<float>();

            //List<float> column = new List<float>();

            for (int x = gridStartX; x < gridStartX + width; ++x)
            {
                float currentDirection = previousDirection + Random.Range(-0.1f, 0.1f);
                row.Add(currentDirection);
                //column.Add(current + 100);

                previousDirection = currentDirection; // base every value direction off the last value direction
            }

            flowFieldForces.Add(row);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        for (int y = gridStartY; y < gridStartY + height; ++y)
        {
            List<float> row = flowFieldForces[y - gridStartY];
            for (int x = gridStartX; x < gridStartX + width; ++x)
            {
                float yPos = y + 0.5f;
                float xPos = x + 0.5f;
    
                float forceAngle = row[x-gridStartX];
    
                //generate vector3 from angle, cosine of x, sine of y using mathf.deg2rad
                Vector3 forceDirection = new Vector3(Mathf.Cos(forceAngle * Mathf.Deg2Rad), Mathf.Sin(forceAngle * Mathf.Deg2Rad), 0.0f);
                forceDirection *= 0.5f;
                Debug.DrawRay(new Vector3(xPos, yPos, 0.0f), forceDirection, Color.cyan);
    
            }
    
        }
    }
    
    public Vector3 GetForceForPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x); // 0
        int y = Mathf.FloorToInt(position.y);

        List<float> row = flowFieldForces[y - gridStartY];
        float forceAngle = row[x - gridStartX];
        Vector3 forceDirection = new Vector3(Mathf.Cos(forceAngle * Mathf.Deg2Rad), Mathf.Sin(forceAngle * Mathf.Deg2Rad), 0.0f);

        return forceDirection;
    }
}
