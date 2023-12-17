using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;

public class AnchorCreatedEvent : UnityEvent<Transform> { }

/* TODO 1. Enable ARCore Cloud Anchors API on Google Cloud Platform */
public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera = null;

    private ARAnchorManager arAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private string anchorIdToResolve;
    private AnchorCreatedEvent anchorCreatedEvent = null;
    public static ARCloudAnchorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        anchorCreatedEvent = new AnchorCreatedEvent();
        anchorCreatedEvent.AddListener((t) => CloudAnchorObjectPlacement.Instance.RecreatePlacement(t));
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position, arCamera.transform.rotation);
    }
    public void QueueAnchor(ARAnchor arAnchor)
    {
        pendingHostAnchor = arAnchor;
    }

    public void HostAnchor()
    {
        Debug.Log("HostAnchor call in progress");

        /* TODO 3.1 Get FeatureMapQuality */
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        Debug.Log(string.Format("Feature Map Quality: {0}", quality));

        if (quality != FeatureMapQuality.Insufficient)
        {
            /* TODO 3.2 Start the hosting process */
            HostCloudAnchorPromise cloudAnchor = arAnchorManager.HostCloudAnchorAsync(pendingHostAnchor, 365);

            /* Wait for the promise to solve (Hint! Pass the HostCloudAnchorPromise variable to the coroutine) */
            StartCoroutine(WaitHostingResult(cloudAnchor));
        }
    }

    public void Resolve()
    {
        Debug.Log("Resolve call in progress");

        /* TODO 5 Start the resolve process and wait for the promise */
        ResolveCloudAnchorPromise result = arAnchorManager.ResolveCloudAnchorAsync(anchorIdToResolve);
        StartCoroutine(WaitResolvingResult(result));
    }

    private IEnumerator WaitHostingResult(HostCloudAnchorPromise cloudAnchor)
    {
        /* TODO 3.3 Wait for the promise. Save the id if the hosting succeeded */
        yield return cloudAnchor;

        if(cloudAnchor.State == PromiseState.Cancelled)
        {
            yield break;
        }

        var result = cloudAnchor.Result;

        if (result.CloudAnchorState == CloudAnchorState.Success)
        {
            anchorIdToResolve = result.CloudAnchorId;
            Debug.Log("Anchor has been hosted!");
        }
        else
        {
            Debug.Log(string.Format("Error in hosting the anchor: {0} {1}", result.CloudAnchorState));
        }
    }

    private IEnumerator WaitResolvingResult(ResolveCloudAnchorPromise resolvePromise)
    {
        yield return resolvePromise;

        if (resolvePromise.State == PromiseState.Cancelled)
        {
            yield break;
        }

        var result = resolvePromise.Result;

        if (result.CloudAnchorState == CloudAnchorState.Success)
        {
            anchorCreatedEvent?.Invoke(result.Anchor.transform);
            Debug.Log("Anchor resolved successfully!");
        }
        else
        {
            Debug.Log("Error while resolving cloud anchor");
        }
    }

    void Update()
    {

    }
}
