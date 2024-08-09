using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    private const float CLOSE_DISTANCE = 1;

    // A list of gameobjects (gos).
    private List<GameObject> chickenWaypoints = new List<GameObject>();

    // Current target.
    private GameObject currentTarget = null;

    // Current Speed.
    private float currentSpeed = 5;

    // A reference to the animator.
    private Animator anim;

    // Hashes to animator parameters.
    private int walkHash = Animator.StringToHash("Walk");


    // Start is called before the first frame update
    void Start()
    {

        string objectTag = "Waypoint" + name;

        // Debug.Log("game object name is: " +  name + " ~ ~ ~ " + objectTag);

        // Find all the chicken waypoints in the level.
        GameObject[] GameObjectsWithChickenWaypointTag;

        GameObjectsWithChickenWaypointTag = GameObject.FindGameObjectsWithTag(objectTag);
        foreach (GameObject waypoint in GameObjectsWithChickenWaypointTag)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();
            if (tmpWaypointMan)
            {
                chickenWaypoints.Add(waypoint);
            }
        }

        currentTarget = FindClosest();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget != null)
        {
            // Determine the direction to the current target.
            Vector3 direction = currentTarget.transform.position - transform.position;
            direction.y = 0;
            // Calculates the length of the relative position vector.
            float distance = direction.magnitude;
            // Face in the right direction.
            if (direction.magnitude > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = rotation;
            }
            // Calculate the normalised direction to the target from a game object.
            Vector3 normDirection = direction / distance;
            // Move the game object.
            transform.position = transform.position + normDirection * currentSpeed * Time.deltaTime;
            anim.SetInteger(walkHash, 1);
            // Change target.
            if (distance < CLOSE_DISTANCE)
            {
                // Select the next target.
                VisGraphWaypointManager tmpWaypointMan =
                currentTarget.GetComponent<VisGraphWaypointManager>();
                if (tmpWaypointMan)
                {
                    if (tmpWaypointMan.connections.Count == 0)
                    {
                        currentTarget = null;
                    }
                    if (tmpWaypointMan.connections.Count == 1)
                    {
                        currentTarget = tmpWaypointMan.connections[0].ToNode;
                    }
                    if (tmpWaypointMan.connections.Count > 1)
                    {
                        int rndIdx = Random.Range(0, tmpWaypointMan.connections.Count);
                        currentTarget = tmpWaypointMan.connections[rndIdx].ToNode;
                    }
                }
            }
        }
    }

    private GameObject FindClosest()
    {
        GameObject closest = null;
        float distanceSqr = Mathf.Infinity;
        foreach (GameObject go in chickenWaypoints)
        {
            if (go != null)
            {
                // Get a vector to the gameobject.
                Vector3 direction = go.transform.position - transform.position;
                // Determine the distance squared of the vector.
                float tmpDistanceSqr = direction.sqrMagnitude;
                if (tmpDistanceSqr < distanceSqr)
                {
                    closest = go;
                    distanceSqr = tmpDistanceSqr;
                }
            }
        }
        return closest;
    }
}

