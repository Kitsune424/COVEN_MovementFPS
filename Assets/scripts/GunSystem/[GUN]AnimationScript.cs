using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUN_AnimationScript : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Aiming
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("enterADS");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetTrigger("exitADS");
        }
    }
}
