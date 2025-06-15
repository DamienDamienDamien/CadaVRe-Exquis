using UnityEngine;

public class Color_Random : MonoBehaviour
{
    public GameObject target;
    float minDis = 0.1f;
    bool isClose = false; // Flag to check if the target is close
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogError("Script attaché à : " + gameObject.name);
        target = GameObject.Find("Tip"); // Find the target object in the scene
    }

    // Update is called once per frame
    void Update()
    {
        float Distance = Vector3.Distance(transform.position, target.transform.position);
        if (Distance<minDis&& !isClose)
        {
            isClose = true; // Set the flag to true if the target is close
            target.transform.parent.GetComponent<PencilCylinder>().ChangeColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))); ;
        }
        else
        {
            isClose = false; // Reset the flag if the target is not close
        }
    }
}
