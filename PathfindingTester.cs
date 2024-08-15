using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class PathfindingTester : MonoBehaviour
{
    // The A* manager.
    private AStarManager AStarManager = new AStarManager();

    // List of possible waypoints.
    private List<GameObject> Waypoints = new List<GameObject>();

    // List of waypoint map connections. Represents a path.
    private List<Connection> ConnectionArray = new List<Connection>();

    // The start and end nodes.
    [SerializeField]
    private GameObject start;

    [SerializeField]
    private GameObject end;

    // Debug line offset.
    Vector3 OffSet = new Vector3(0, 0.3f, 0);

    [SerializeField]
    private GameObject worldObject = null;

    [SerializeField]
    private bool isMovable = true;

    // Timer.
    private float timer = 0;

    // Distance.
    private float totalDistance = 0;
    private Vector3 lastPosition = new Vector3();
    private bool outputTimeAndDist = true;

    // Movement variables.
    private float currentSpeed = 8;
    private int currentTarget = 0;
    private Vector3 currentTargetPos;
    private int moveDirection = 1;
    private bool agentMove = true;

    private int totalScore = 0;
    private HashSet<VisGraphWaypointManager> visitedWaypoints = new HashSet<VisGraphWaypointManager>();


    //void Awake()
    //{
    //    PrefabStoreManager.AddPrefabName(gameObject.name);
    //}


    // Start is called before the first frame update
    void Start()
    {
        if (start == null || end == null)
        {
            Debug.Log("No start or end waypoints.");
            return;
        }

        VisGraphWaypointManager tmpWpM = start.GetComponent<VisGraphWaypointManager>();

        if (tmpWpM == null)
        {
            Debug.Log("Start is not a waypoint.");
            return;
        }

        tmpWpM = end.GetComponent<VisGraphWaypointManager>();

        if (tmpWpM == null)
        {
            Debug.Log("End is not a waypoint.");
            return;
        }

        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();
            if (tmpWaypointMan)
            {
                Waypoints.Add(waypoint);
            }
        }

        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();

            // Loop through a waypoints connections.
            foreach (VisGraphConnection aVisGraphConnection in tmpWaypointMan.Connections)
            {
                if (aVisGraphConnection.ToNode != null)
                {
                    Connection aConnection = new Connection();
                    aConnection.FromNode = waypoint;
                    aConnection.ToNode = aVisGraphConnection.ToNode;
                    AStarManager.AddConnection(aConnection);
                }
                else
                {
                    Debug.Log("Warning, " + waypoint.name + " has a missing to node for a connection!");
                }
            }
        }

        lastPosition = transform.position;

        // Run A Star...
        // ConnectionArray stores all the connections in the route to the goal / end node.
        ConnectionArray = AStarManager.PathfindAStar(start, end);

        if (ConnectionArray.Count == 0)
        {
            Debug.Log("Warning, A* did not return a path between the start and end node.");
        }

    }
    // Draws debug objects in the editor and during editor play (if option set).

    void OnDrawGizmos()
    {
        // Draw path.
        foreach (Connection aConnection in ConnectionArray)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet), (aConnection.ToNode.transform.position + OffSet));
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!isMovable)
        {
            return;  // If not movable, don't execute the movement logic
        }

        if (agentMove)
        {

            // Timer and distance.
            outputTimeAndDist = true;
            Vector3 tmpDir = lastPosition - transform.position;
            float tmpDistance = tmpDir.magnitude;
            totalDistance += tmpDistance;

            //Debug.Log("distance: " + TotalDistance);
            lastPosition = transform.position;
            timer += Time.deltaTime;

            // Check if close to world object.
            if (worldObject != null)
            {
                float distToObject =
                Vector3.Distance(transform.position, worldObject.transform.position);

                if (distToObject < 7)
                {
                    return;
                }
            }

            ////////////
            // Check if the agent has passed the waypoint
            VisGraphWaypointManager currentWaypoint = ConnectionArray[currentTarget].ToNode.GetComponent<VisGraphWaypointManager>();

            if (currentWaypoint != null)
            {
                float distanceToWaypoint = Vector3.Distance(transform.position, currentWaypoint.transform.position);
                if (distanceToWaypoint < 1.0f && !visitedWaypoints.Contains(currentWaypoint))
                {
                    totalScore += currentWaypoint.score;
                    visitedWaypoints.Add(currentWaypoint);

                    PrefabStoreManager.AddAgentScores(name, totalScore);

                    // Handle collectable collection
                    Collider[] hitColliders = Physics.OverlapSphere(currentWaypoint.transform.position, 1.5f); // 1.5f is the search radius
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.CompareTag("Collectable"))
                        {
                            Destroy(hitCollider.gameObject); // Destroy the collectable
                            Debug.Log("Destroyed collectable: " + hitCollider.name);
                        }
                    }

                    Debug.Log("Node: " + currentWaypoint.name + ", Score: " + currentWaypoint.score + ", totalScore=" + totalScore);        
                }
            }
            ////////////

            // Determine the direction to first node in the array.
            if (moveDirection > 0)
            {
                currentTargetPos = ConnectionArray[currentTarget].ToNode.transform.position;
            }
            else
            {
                currentTargetPos = ConnectionArray[currentTarget].FromNode.transform.position;
            }

            // Clear y to avoid up/down movement. Assumes flat surface.
            currentTargetPos.y = transform.position.y;
            Vector3 direction = currentTargetPos - transform.position;

            // Calculate the length of the relative position vector
            float distance = direction.magnitude;

            // Face in the right direction.
            direction.y = 0;

            if (direction.magnitude > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = rotation;
            }

            // Calculate the normalised direction to the target from a game object.
            Vector3 normDirection = direction / distance;

            // Move the game object.
            transform.position = transform.position + normDirection * currentSpeed * Time.deltaTime;

            // Check if close to current target.
            if (distance < 1)
            {
                // Close to target, so move to the next target in the list (if there is one).
                currentTarget += moveDirection;

                if (currentTarget == ConnectionArray.Count)
                {
                    // No next target. Change direction and undo target change.
                    moveDirection = -1;
                    currentTarget += moveDirection;
                }

                if (currentTarget == -1)
                {
                    // No next target. Change direction and undo target change.
                    moveDirection = 1;
                    currentTarget += moveDirection;
                    // Uncomment the line below if you would like the agent to stop after one trip.
                    agentMove = false;
                }
            }
        }
        else
        {
            if (outputTimeAndDist == true)
            {
                Debug.Log(name + "'s Time: " + timer);
                Debug.Log(name + "'s Distance: " + totalDistance);

                //PrefabStoreManager.AddAgentTimeAndDistance(name, ((int)timer), totalDistance);

                totalDistance = 0;
                timer = 0;
                outputTimeAndDist = false;

                // Trigger Game Over
                GameOver();
            }
        }
    }


    void GameOver()
    {
        Debug.Log("Game Over: Fox has returned to the endpoint.");

        // Find the fox by name 
        GameObject fox = GameObject.Find("Fox");

        if (fox != null)
        {
            // Get the Animator component from the fox
            Animator foxAnimator = fox.GetComponent<Animator>();

            if (foxAnimator != null)
            {

                // Trigger the final animation(s)
                foxAnimator.Play("Fox_Attack_Tail");
            }
            else
            {
                Debug.LogWarning("No Animator component found on Fox.");
            }
        }
        else
        {
            Debug.LogWarning("Fox object not found in the scene.");
        }
    }


    private void OnGUI()
    {

        GUI.color = Color.red;

        float yPosition = 20; // Starting y position for the labels

        for (int i = 0; i < PrefabStoreManager.agentNames.Count; i++)
        {
            //string displayText = PrefabStoreManager.agentNames[i] + "'s Time: " + PrefabStoreManager.agentTimes[i] + " / Distance: " + PrefabStoreManager.agentDistances[i];
            string displayText = PrefabStoreManager.agentNames[i] + "'s "  +
                                " Total Score: " + PrefabStoreManager.agentTotalScores[i];

            GUI.Label(new Rect(10, yPosition, Screen.width, Screen.height), displayText);

            yPosition += 20; // Increment the y position for the next label
        }

        if (totalDistance == 0)
        {
            GUI.color = Color.red;
            GUIStyle style = new GUIStyle();
            style.fontSize = 30;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(0, yPosition, Screen.width, Screen.height), "Game Over!", style);
        }

    }
}