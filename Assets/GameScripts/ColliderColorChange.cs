using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderColorChange : MonoBehaviour
{

    public MeshCreationSettings CreationSettings;
    public TerrainGeneratorColormix Generator;
    public GameManager Manager;


    private void OnTriggerEnter(Collider other) {
        
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player") {
            Generator.HeighGradient0 = CreationSettings.HeighGradient;
            Generator.ColorMesh();
            Manager.TriggerChange();
            Destroy(this.gameObject);
        }
    }

}
