using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to display text above the node.
using UnityEditor;

public class VisGraphWaypointManager : MonoBehaviour
{
    // Allow you to set the waypoint text colour.
    [SerializeField]
    private enum waypointTextColour { Blue, Cyan, Yellow, Black, White, Magenta, Red };
#pragma warning disable

    [SerializeField]
    private waypointTextColour WaypointTextColour = waypointTextColour.Blue;
#pragma warning restore

    // List of all connections from this node.
    [SerializeField]
    public List<VisGraphConnection> connections = new List<VisGraphConnection>();

    public List<VisGraphConnection> Connections
    {
        get { return connections; }
    }

    // Allow you to set a waypoint as a start or goal.
    [SerializeField]
    private enum waypointPropsList { Standard, Start, Goal };

#pragma warning disable
    [SerializeField]
    private waypointPropsList WaypointType = waypointPropsList.Standard;

#pragma warning restore
    // Controls if the node type is displayed in the Unity editor.
    private const bool displayType = false;

    // Used to determine if the waypoint is selected.
    private bool ObjectSelected = false;

    // Text displayed above the node.
    private const bool displayText = true;
    private string infoText = "";
    private Color infoTextColor;

    public int score;

    public GameObject collectablePrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Assign a random score between 0 and 3 when the game starts
        score = Random.Range(0, 4);

        // Spawn collectables based on the score
        if (collectablePrefab != null)
        {
            SpawnCollectables(score);
        }
        else
        {
            //Debug.LogWarning("No Collectable Prefab assigned for " + gameObject.name);
        }
    }


    private void SpawnCollectables(int numberOfCollectables)
    {
        Debug.LogWarning(gameObject.name + " got numberOfCollectables=" + numberOfCollectables);

        for (int i = 0; i < numberOfCollectables; i++)
        {
            Vector3 randomPosition = GetRandomPositionNearWaypoint();
            Quaternion rotation = Quaternion.Euler(0, Random.Range(-162, -142), 0);

            GameObject collectable = Instantiate(collectablePrefab, randomPosition, rotation);
            //collectable.transform.localScale = Vector3.one * 1.1f;

            Debug.Log("Collectable" + collectable.name + "spawned at: " + randomPosition);

            // Add an animation? or a script to handle the collectable's behavior
            //Animator collectableAnimator = collectable.GetComponent<Animator>();
            //if (collectableAnimator != null)
            //{
            //    collectableAnimator.SetTrigger("Idle");  
            //}
        }
    }

    private Vector3 GetRandomPositionNearWaypoint()
    {
        // Generate a random position near the waypoint within a small radius
        float radius = 2.0f;  
        Vector2 randomOffset = Random.insideUnitCircle * radius;

        return new Vector3(transform.position.x + randomOffset.x, transform.position.y, transform.position.z + randomOffset.y);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Text displayed above the waypoint.
        infoText = "";

        if (displayType)
        {
#pragma warning disable
            infoText = "Type: " + WaypointType.ToString() + " / ";
#pragma warning restore
        }

        infoText += gameObject.name + "\n Connections: " + Connections.Count;

        infoText += "\n Score: " + score;

        switch (WaypointTextColour)
        {
            case waypointTextColour.Blue:
                infoTextColor = Color.blue;
                break;
            case waypointTextColour.Cyan:
                infoTextColor = Color.cyan;
                break;
            case waypointTextColour.Yellow:
                infoTextColor = Color.yellow;
                break;
            case waypointTextColour.Black:
                infoTextColor = Color.black;
                break;
            case waypointTextColour.White:
                infoTextColor = Color.white;
                break;
            case waypointTextColour.Magenta:
                infoTextColor = Color.magenta;
                break;
            case waypointTextColour.Red:
                infoTextColor = Color.red;
                break;
        }
        DrawWaypointAndConnections(ObjectSelected);
        if (displayText)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = infoTextColor;
            Handles.Label(transform.position + Vector3.up * 1, infoText, style);
        }
        ObjectSelected = false;
    }
    // Draws debug objects when an object is selected.
    void OnDrawGizmosSelected()
    {
        ObjectSelected = true;
    }


    // Draws debug objects for the waypoint and connections.
    private void DrawWaypointAndConnections(bool ObjectSelected)
    {
        Color WaypointColor = Color.yellow;
        Color ArrowHeadColor = Color.blue;
        if (ObjectSelected)
        {
            WaypointColor = Color.red;
            ArrowHeadColor = Color.magenta;
        }
        // Draw a yellow sphere at the transform's position
        Gizmos.color = WaypointColor;
        Gizmos.DrawSphere(transform.position, 0.2f);
        // Draw all the connections.
        for (int i = 0; i < Connections.Count; i++)
        {
            if (Connections[i].ToNode != null)
            {
                if (Connections[i].ToNode.Equals(gameObject))
                {
                    infoText = "WARNING - Connection to SELF at element: " + i;
                    infoTextColor = Color.red;
                }
                Vector3 direction = Connections[i].ToNode.transform.position - transform.position;
                DrawConnection(i, transform.position, direction, ArrowHeadColor);
                if (ObjectSelected)
                {
                    // Draw spheres along the line.
                    Gizmos.color = ArrowHeadColor;
                    float dist = direction.magnitude;
                    float pos = dist * 0.1f;
                    Gizmos.DrawSphere(transform.position +
                    (direction.normalized * pos), 0.3f);
                    pos = dist * 0.2f;
                    Gizmos.DrawSphere(transform.position +
                    (direction.normalized * pos), 0.3f);
                    pos = dist * 0.3f;
                    Gizmos.DrawSphere(transform.position +
                    (direction.normalized * pos), 0.3f);
                }
            }
            else
            {
                infoText = "WARNING - Connection is missing at element: " + i;
                infoTextColor = Color.red;
            }
        }
    }

    // This arrow method is based on the example here: https://gist.github.com/MatthewMaker/5293052
    public void DrawConnection(float ConnectionsIndex, Vector3 pos, Vector3 direction,
    Color ArrowHeadColor, float arrowHeadLength = 0.5f, float arrowHeadAngle = 40.0f)
    {
        Debug.DrawRay(pos, direction, Color.blue);
        Vector3 right = Quaternion.LookRotation(direction) *
        Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) *
        Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Debug.DrawRay(pos + direction.normalized +
        (direction.normalized * (0.1f * ConnectionsIndex)),
        right * arrowHeadLength, ArrowHeadColor);
        Debug.DrawRay(pos + direction.normalized +
        (direction.normalized * (0.1f * ConnectionsIndex)),
        left * arrowHeadLength, ArrowHeadColor);
    }
}
