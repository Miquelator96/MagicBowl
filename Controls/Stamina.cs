using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Stamina : NetworkBehaviour
{
    public float maxStamina;
    [SyncVar(hook = "OnChangeStamina")]
    public float currentStamina;
    public float recoverySpeed;
    public Image staminaBar;
    private PlayerController playerController;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {   
        // if (!isServer)
        //     return;

        if (currentStamina < maxStamina)
        {
            currentStamina += recoverySpeed * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }
    }

    public void TakeDamage(int amount)
    {
        // if (!isServer)
        //     return;

        currentStamina -= amount;
        if (currentStamina < 0)
        {
            currentStamina = 0;
            if (isLocalPlayer) GetComponent<PlayerController>().isStunned = true;
        }
    }

    public void StopStun()
    {
        // if (!isServer)
        //     return;

        currentStamina = 10;
        if (isLocalPlayer) GetComponent<PlayerController>().isStunned = false;
    }

    void OnChangeStamina(float stamina)
    {
        staminaBar.fillAmount = stamina / maxStamina;
    }
}
