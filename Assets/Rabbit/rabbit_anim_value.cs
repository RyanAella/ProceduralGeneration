using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rabbit_anim_value : MonoBehaviour
{

    [SerializeField, Range(0f, 1f)]
    private float speed;

    [SerializeField, Range(-1f, 1f)]
    private float look_LR, look_UD;

    [SerializeField]
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speed", speed);
        animator.SetFloat("left_right", look_LR);
        animator.SetFloat("up_down", look_UD);
        
    }
}
