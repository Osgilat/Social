using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour
{
	public float moveSpeed = 20.0f; //speed of a bullet
	public Vector3 velocity; //velocity of a bullet
    public ParticleSystem explosionEffect; //explosion on collision
    public Transform diePoint; //point to transform player to, when he dies

    //Every tick
	private void FixedUpdate()
	{
		// transform bullet on the server
		transform.position += transform.forward * Time.deltaTime * moveSpeed;
	}

    //on collision with player do something
    void OnCollisionEnter(Collision collision)
    {
        //place where collision occured 
        var hit = collision.gameObject;
        //bullet collides only with player's gameObject
        if (hit.tag == "Player")
        {
            //play vfx on collision
            explosionEffect.Play();
            //set speed to 0
            moveSpeed = 0;
            //change player's state to stunned
            collision.gameObject.GetComponent<ShootAbility>().stunned = true;
            //set animator flag to "Died"
            collision.gameObject.GetComponentInChildren<Animator>().SetBool("Died", true);
            //set speed of gameObject to 0 
            collision.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
            //to be inside player after collision
            transform.position += transform.forward*1.5f;
            
        }
        
        this.GetComponent<SphereCollider>().enabled = false;

        Destroy(this.gameObject, 0.7f);
    }
}




