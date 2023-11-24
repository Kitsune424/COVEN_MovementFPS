using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitanfallPlayerMovement
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform CameraFollowPoint;

        // Update is called once per frame
        void Update()
        {
            transform.position = CameraFollowPoint.position;
        }
    }
}

