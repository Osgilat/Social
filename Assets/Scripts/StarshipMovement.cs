using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipMovement : MonoBehaviour {

    Transform startTransform;
    public float speed;
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public bool closeViewWithShip = false;

    private void Start()
    {
        startTransform = transform;
    }

    

    // Update is called once per frame
    void Update () {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
        if((targetPosition - transform.position).sqrMagnitude < 0.01)
        {
            speed = 0;
            transform.position = startPosition;
        }
    }
}
