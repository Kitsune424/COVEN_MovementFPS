using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] Transform CameraFollowPoint;

    // Update is called once per frame
    void Update()
    {
        transform.position = CameraFollowPoint.position;
    }
}
