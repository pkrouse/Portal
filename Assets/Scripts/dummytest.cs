using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummytest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(pauseCR());
    }

    private IEnumerator pauseCR()
    {
        yield return new WaitForSeconds(5);
        float force = 200f;
        Rigidbody r = GetComponent<Rigidbody>();
        if (r == null)
        { Debug.LogError("No rigidbody found!!!"); }
        else
        {
            Debug.Log("Found the rigidbody");
        }
        r.AddForce(transform.forward * force, ForceMode.Impulse);
    }
}
