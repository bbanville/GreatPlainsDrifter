using Opsive.UltimateCharacterController.StateSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveIn : MonoBehaviour
{
    [SerializeField] protected GameObject caveIn;

    [SerializeField] protected StateTrigger explosiveShake;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCaveIn()
    {
        caveIn.SetActive(true);
    }
}
