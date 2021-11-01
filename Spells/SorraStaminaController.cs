using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SorraStaminaController : NetworkBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Image buttonImg;
    private Image cooldownImg;

    public Sprite pressedImg;
    private Sprite releasedImg;

    private Transform character;

    private bool spellIsLocked = false;
    public float lockTime;
    private float timeLocked;

    // Use this for initialization
    void Start()
    {
        buttonImg = gameObject.GetComponent<Image>();
        cooldownImg = transform.parent.Find("Cooldown").GetComponent<Image>();
        
        releasedImg = buttonImg.sprite;

        character = transform.parent.transform.parent.transform.parent;

        timeLocked = lockTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (spellIsLocked)
        {
            LockSpell();
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (!spellIsLocked)
        {
            buttonImg.sprite = pressedImg;
        }
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        if (!spellIsLocked)
        {
            buttonImg.sprite = releasedImg;
            character.GetComponent<SorraStaminaPlayer>().PlaySorrastamina();
            spellIsLocked = true;
			cooldownImg.enabled = true;
            GetComponent<AudioPlayer>().playshield();
        }
    }

    private void LockSpell()
    {
        // Basic timer.
        if (timeLocked > 0)
        {
            timeLocked -= Time.deltaTime;
			cooldownImg.fillAmount = timeLocked / lockTime;
        }
        else
        {
            // Stun time is over.
			cooldownImg.enabled = false;
            timeLocked = lockTime;
            spellIsLocked = false;
        }
    }
}