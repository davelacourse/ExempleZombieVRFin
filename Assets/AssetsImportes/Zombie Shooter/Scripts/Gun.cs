using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public float speed = 40;
    public GameObject bullet;
    public Transform barrel;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public void Fire()
    {
        FireServerRPC();   
    }

    // Comme seulement le serveur peut instancier un objet réseau on appelle la méthode dans un ServerRPC
    [ServerRpc(RequireOwnership = false)]
    public void FireServerRPC()
    {
        GameObject spawnedBullet = Instantiate(bullet, barrel.position, barrel.rotation);

        // Instancie sur le réseau le bullet
        spawnedBullet.GetComponent<NetworkObject>().Spawn(true);

        spawnedBullet.GetComponent<Rigidbody>().velocity = speed * barrel.forward;
        audioSource.PlayOneShot(audioClip);
        // Cette méthode détruit le gameobject automatiquement pour tout le monde
        Destroy(spawnedBullet, 2);
    }
}


