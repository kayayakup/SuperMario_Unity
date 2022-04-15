using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallow : MonoBehaviour
{
    public Transform target;
    public Transform leftBounds;
    public Transform rightBounds;
    public float smoothDampTime = 0.15f;
    private Vector3 smoothDampVelocity = Vector3.zero; 
    private float camwidth, camHeight, levelMinx, levelMaxX;
    // Use this for initialization
    void Start () {
        camHeight = Camera.main.orthographicSize * 2;
        camwidth = camHeight * Camera.main.aspect;

        float leftBoundsWidth = leftBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;                    
        float rightBoundsWidth = rightBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x/2;

        levelMinx=leftBounds.position.x+leftBoundsWidth+(camwidth/2);
        levelMaxX=rightBounds.position.x-rightBoundsWidth-(camwidth/2);
    }
    void Update () {
        if (target) {
            float targetX = Mathf.Max (levelMinx, Mathf.Min(levelMaxX, target.position.x));
            float x = Mathf.SmoothDamp(transform.position.x, targetX, ref smoothDampVelocity.x, smoothDampTime);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
