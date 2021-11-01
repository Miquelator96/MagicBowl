using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShieldPlayer : NetworkBehaviour 
{
	public GameObject audioOnline;
	public GameObject particleSys;
	
	public void PlayShield()
	{
		if (isLocalPlayer)
		{
			GetComponent<PlayerController>().PlaySpellAnimation();
			CmdPlayShield(transform.position + new Vector3(0, 0.1f, 0));
		}
	}

	[Command]
	public void CmdPlayShield(Vector3 position)
	{
		RpcPlayShield(position);
	}

	[ClientRpc]
	public void RpcPlayShield(Vector3 position)
	{
		if (!(GetComponent<PlayerController>().isStunned))
		{
			GameObject audio = Instantiate(audioOnline) as GameObject;
			audio.GetComponent<AudioPlayerOnline> ().playshield ();

			// Set the owner of the particle system so that the spell doesn't affect the player
			GameObject particleSysNetwork = Instantiate(particleSys) as GameObject;
			particleSysNetwork.transform.parent = transform;
			particleSysNetwork.transform.position = position;
			foreach (Transform child in particleSysNetwork.transform)
			{
				child.GetComponent<ParticleSystem>().Play();
			}
		}
	}

}