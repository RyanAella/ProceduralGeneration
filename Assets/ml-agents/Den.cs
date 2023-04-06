using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Den : MonoBehaviour
{
    public Vector3 position;
    public bool occupied;
    public int monthsUntilDissapear;

    public Den (Vector3 position, bool occupied)
    {
        this.position = position;
        this.occupied = occupied;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
