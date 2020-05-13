using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAnimation : MonoBehaviour
{
    [SerializeField] List<Animation> forwardAnimations;
    [SerializeField] List<Animation> backwardsAnimations;
    [SerializeField] List<Animation> upwardsAnimations;
    [SerializeField] List<Animation> downwardsAnimations;
    Animator anim;
    List<Animation> currentList;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentList = forwardAnimations;
    }

    private void Update()
    {
        if (anim.IsInTransition(0))
        {
            GetRandomAnimation();
        }
    }

    void GetRandomAnimation()
    {
        anim.Play(currentList[Random.Range(0, currentList.Count - 1)].name);


    }
}
