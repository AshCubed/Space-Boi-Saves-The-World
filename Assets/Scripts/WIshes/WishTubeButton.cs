using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishTubeButton : MonoBehaviour
{
    [SerializeField] WishTube myWishTube;
    [SerializeField] WishManager wishManager;
    [Header("Anim Data")]
    [SerializeField] private Animator animStation;
    [SerializeField] private string trigger;
    [SerializeField] private float waitTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animStation.SetTrigger(trigger);
            AudioManager.instance.PlaySounds("Push Button");
            LeanTween.delayedCall(waitTime, () => wishManager.GrantWish(myWishTube, myWishTube.GetWish()));
        }
    }
}
