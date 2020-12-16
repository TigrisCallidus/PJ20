using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderBehaviour : MonoBehaviour
{

    public GameManager Manager;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player") {
            Manager.GoOut();
        }
    }


}
