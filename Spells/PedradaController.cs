using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PedradaController : NetworkBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image buttonImg;
    private Image anchorImg;
    private Image cooldownImg;
    private Vector3 inputVector;

    public Sprite pressedImg;
    private Sprite releasedImg;

    public float angle;
    public Vector3 shootDirection;

    private bool spellIsLocked = false;
    public float lockTime;
    private float timeLocked;

    private Transform character;
    private GameObject HUDRange;
    private GameObject HUDAOE;

    private float range = 3;
    // Use this for initialization
    void Start()
    {
        buttonImg = gameObject.GetComponent<Image>();
        anchorImg = transform.parent.GetComponent<Image>();
        cooldownImg = transform.parent.Find("Cooldown").GetComponent<Image>();

        angle = 0.0f;
        shootDirection = Vector3.zero;

        releasedImg = buttonImg.sprite;

        character = transform.parent.transform.parent.transform.parent;
        HUDRange = character.GetChild(4).GetChild(0).gameObject;
        HUDAOE = character.GetChild(4).GetChild(2).gameObject;

        timeLocked = lockTime;
    }

    // Update is called once per frame
    void Update()
    {
        HUDAOE.transform.eulerAngles = new Vector3(90, angle, 0);
        HUDAOE.transform.position = character.transform.position + new Vector3(Mathf.Cos(angle*Mathf.PI/180)*3,0.1f, 
            -Mathf.Sin(angle * Mathf.PI / 180) * 3);

        shootDirection.x = inputVector.x;
        shootDirection.z = inputVector.z;

        shootDirection = shootDirection.normalized * 3;

        // helper.transform.position = transform.parent.transform.parent.transform.parent.transform.position + new Vector3(0, 0.3f, 0);

        if (spellIsLocked)
        {
            LockSpell();
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (!spellIsLocked)
        {
            HUDRange.transform.localScale = new Vector3(range, range, range);
            buttonImg.sprite = pressedImg;
            anchorImg.enabled = true;

            HUDRange.SetActive(true);
            HUDAOE.SetActive(true);
            OnDrag(ped);
        }
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        if (!spellIsLocked)
        {
            buttonImg.sprite = releasedImg;
            buttonImg.rectTransform.anchoredPosition = Vector3.zero;
            anchorImg.enabled = false;

            HUDRange.SetActive(false);
            HUDAOE.SetActive(false);

            inputVector = Vector3.zero;
            character.GetComponent<PedradaPlayer>().PlayPedrada();

            spellIsLocked = true;
            cooldownImg.enabled = true;
            GetComponent<AudioPlayer>().playpedrada();

        }
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        if (!spellIsLocked)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(anchorImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
            {
                pos.x = (pos.x / anchorImg.rectTransform.sizeDelta.x);
                pos.y = (pos.y / anchorImg.rectTransform.sizeDelta.y);

                inputVector = new Vector3(pos.x * 4, 0, -pos.y * (1 / 0.7f));

                if (inputVector.magnitude > 1)
                {
                    inputVector = inputVector.normalized * 1.2f;
                }

                buttonImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (anchorImg.rectTransform.sizeDelta.x / 6),
                    -inputVector.z * (anchorImg.rectTransform.sizeDelta.y / 3f));

                angle = Mathf.Atan2(inputVector.z, inputVector.x) * 180 / Mathf.PI;
            }
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