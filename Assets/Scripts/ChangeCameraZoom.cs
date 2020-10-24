using BrendonBanville.Controllers;
using System.Collections;
using UnityEngine;

public class ChangeCameraZoom : MonoBehaviour
{
    [SerializeField] ThirdPersonCamera tpCamera;
    [SerializeField] GameObject playerModel;
    
    public enum zoomTypes { ZoomOut, ZoomIn }
    public zoomTypes zoomType = zoomTypes.ZoomOut;

    private float defaultDistance = 2.6f;

    void Start()
    {
        defaultDistance = tpCamera.defaultDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (zoomType == zoomTypes.ZoomIn)
            {
                if (other.GetComponentInChildren<SkinnedMeshRenderer>())
                {
                    playerModel = other.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
                }
                
                StartCoroutine(ZoomInFirstPerson(0.4f));
            }
            else if (zoomType == zoomTypes.ZoomOut)
            {
                if (other.GetComponentInChildren<SkinnedMeshRenderer>())
                {
                    playerModel = other.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
                }

                StartCoroutine(ZoomOutFirstPerson(0.2f));
            }
        }
    }

    IEnumerator ZoomInFirstPerson(float timeToZoom)
    {
        tpCamera.occlusionOn = false;
        tpCamera.defaultDistance = Mathf.Lerp(0.0f, tpCamera.defaultDistance, Time.deltaTime);

        yield return new WaitForSeconds(timeToZoom);

        playerModel.SetActive(false);
    }

    IEnumerator ZoomOutFirstPerson(float timeToZoom)
    {
        tpCamera.occlusionOn = false;
        tpCamera.defaultDistance = Mathf.Lerp(defaultDistance, 0.0f, Time.deltaTime);

        yield return new WaitForSeconds(timeToZoom);

        playerModel.SetActive(true);
    }

    IEnumerator SetOcclusion(float timeToWait, bool state)
    {
        yield return new WaitForSeconds(timeToWait);

        tpCamera.occlusionOn = state;
    }
}
