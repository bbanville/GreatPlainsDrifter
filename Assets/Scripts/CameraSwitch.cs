using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [Header("First Person Controller")]
    public KeyCode FirstPersonSwitch = KeyCode.Keypad1;
    public List<GameObject> firstPersonController;

    [Header("Third Person Controller")]
    public KeyCode ThirdPersonSwitch = KeyCode.Keypad2;
    public List<GameObject> thirdPersonController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(FirstPersonSwitch))
        {
            SwitchToFirstPerson(thirdPersonController);
            this.GetComponent<CameraSync>().controllerType = CameraSync.controllerTypes.firstPersonController;
        }
        if (Input.GetKeyDown(ThirdPersonSwitch))
        {
            SwitchToThirdPerson(firstPersonController);
            this.GetComponent<CameraSync>().controllerType = CameraSync.controllerTypes.thirdPersonController;
        }
    }

    public void SwitchToFirstPerson(List<GameObject> otherController)
    {
        foreach (GameObject controllerObj in otherController)
        {
            controllerObj.SetActive(false);
        }
        foreach (GameObject controllerObj in firstPersonController)
        {
            controllerObj.SetActive(true);
        }
    }

    public void SwitchToThirdPerson(List<GameObject> otherController)
    {
        foreach (GameObject controllerObj in otherController)
        {
            controllerObj.SetActive(false);
        }
        foreach (GameObject controllerObj in thirdPersonController)
        {
            controllerObj.SetActive(true);
        }
    }
}
