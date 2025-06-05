using Unity.Mathematics.Geometry;
using UnityEngine;

public class TP_Thor : MonoBehaviour
{
    public GameObject Pen;
    public GameObject Hand;
    public float minDist = 0.1f;
    public float dampingRadius = 2f;
    public float speed = 0.01f;
    bool returning=false;
    public Vector2 minMaxDamping;
    public bool bbb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(Pen.transform.position, Hand.transform.position);
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            Return();
        }
        bbb = Pen.GetComponent<Rigidbody>().isKinematic;
        
        if (!bbb)
        {

     
        if (dist < dampingRadius)
        {
            Pen.GetComponent<Rigidbody>().angularDamping = minMaxDamping.x;
        }
        else
        {
            Pen.GetComponent<Rigidbody>().angularDamping = minMaxDamping.y;
            }
        }
        
        if (returning)
        {
            if (dist > minDist)
            {
                Pen.transform.position = Vector3.Lerp(Pen.transform.position, Hand.transform.position, Time.deltaTime * speed);
            }
            else {
                Pen.GetComponent<Rigidbody>().isKinematic = false;
                returning= false;
            }
        }
    }
    [ContextMenu("Return")]
    public void Return()
    {
        Pen.GetComponent<Rigidbody>().isKinematic = true;
        returning = true;

    }
}
