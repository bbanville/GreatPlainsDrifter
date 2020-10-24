using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSync : MonoBehaviour
{
    public enum controllerTypes { firstPersonController, thirdPersonController }
    public controllerTypes controllerType = controllerTypes.firstPersonController;

    [Header("Controllers")]
    public GameObject firstPersonController;
    public GameObject thirdPersonController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (controllerType == controllerTypes.thirdPersonController)
        {
            firstPersonController.transform.position = thirdPersonController.transform.position;
            thirdPersonController.transform.rotation = firstPersonController.transform.rotation;
        }
        else if (controllerType == controllerTypes.thirdPersonController)
        {
            thirdPersonController.transform.position = firstPersonController.transform.position;
            thirdPersonController.transform.rotation = firstPersonController.transform.rotation;
        }
    }
}
