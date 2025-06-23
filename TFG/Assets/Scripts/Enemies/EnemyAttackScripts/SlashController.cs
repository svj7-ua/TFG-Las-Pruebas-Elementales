using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashController : MonoBehaviour
{

    public float speed;
	[Tooltip("From 0% to 100%")]
	public List<GameObject> trails;
	private bool collided = false;
	private Rigidbody rb;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private LayerMask wallLayer;

	void Start () {
        rb = GetComponent <Rigidbody> ();
			
	}

	void FixedUpdate () {

        // Sets X and Z rotation to 0, so it doesn't rotate upwards or downwards
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        if (speed != 0 && rb != null)
            rb.position += transform.forward * (speed * Time.deltaTime);   
    }

	void OnTriggerEnter (Collider other) {

        // Checks if it collided with the player or a wall

        if ((playerLayer == (playerLayer | (1 << other.gameObject.layer)) || wallLayer == (wallLayer | (1 << other.gameObject.layer)))  && !collided)
        {
            Debug.Log($"Slash collided with {other.gameObject.name}!");
            collided = true;

            if (trails.Count > 0)
            {
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].transform.parent = null;
                    var ps = trails[i].GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Stop();
                        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                }
            }

            speed = 0;
            StartCoroutine(DestroyParticle(0f));
        }

	}

	public IEnumerator DestroyParticle (float waitTime) {

		if (transform.childCount > 0 && waitTime != 0) {
			List<Transform> tList = new List<Transform> ();

			foreach (Transform t in transform.GetChild(0).transform) {
				tList.Add (t);
			}		

			while (transform.GetChild(0).localScale.x > 0) {
				yield return new WaitForSeconds (0.01f);
				transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				for (int i = 0; i < tList.Count; i++) {
					tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				}
			}
		}
		
		yield return new WaitForSeconds (waitTime);
		Destroy (gameObject);
	}

}
