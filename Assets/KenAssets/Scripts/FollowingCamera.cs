using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private Transform lookAt;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerProperty playerController = FindObjectOfType<PlayerProperty>();
        if (playerController != null)
        {
            follow = playerController.transform;

            // Look = Follow.Find("HeadTarget");
            lookAt = follow;
        }
    }
}
