using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private float offset = .05f;
    [SerializeField]
    private GameObject collideSound;

    List<Vector3> colliding = new List<Vector3>();

    private void Awake()
    {
        Ball.Collided += NewCollision;
    }

    private void OnDestroy()
    {
        Ball.Collided -= NewCollision;
        
    }

    // Receive a new collision detection and check if the same collision already has been detected then store it
    private void NewCollision(Vector3 pos)
    {
        if (colliding.Where(x => (x - pos).magnitude < offset).ToArray().Length == 0)
            colliding.Add(pos);
    }

    private void Update()
    {
        // If the collision list ain't empty empty it by playing the collision sound at the right position
        while (colliding.Count != 0)
        {
            var sound = Instantiate(collideSound, colliding[0], Quaternion.identity);
            sound.GetComponent<AudioSource>().Play();
            Destroy(sound, .25f);
            colliding.RemoveAt(0);
        }
    }
}
