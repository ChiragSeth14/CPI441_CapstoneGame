using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public float lerpSpeed = 1.0f;

    private Vector3 offset;

    private Vector3 targetPos;

    private void Start()
    {
        if (target == null) return;

        offset = transform.position - target.position;

        // Ensure camera stays behind
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    private void Update()
    {
        if (target == null) return;

        targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }

}
