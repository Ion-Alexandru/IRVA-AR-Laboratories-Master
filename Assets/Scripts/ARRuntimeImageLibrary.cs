using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;

/* TODO 1 Create in Unity an image database with minimum 3 images */
/* TODO 2 Augment the database */
public class ARRuntimeImageLibrary : MonoBehaviour
{
    private ARTrackedImageManager trackImageManager;
    public GameObject m_PlacedPrefab;

    void Start()
    {
        /* TODO 3.1 Download minimum one image from the internet */
        var url = "https://m.media-amazon.com/images/I/712jjwwyvTL._AC_UF894,1000_QL80_.jpg";
        StartCoroutine(DownloadImage(url));
    }

    /* Function which downloads and creates an image database */
    IEnumerator DownloadImage(string url)
    {
        Texture2D imageToAdd;

        /* We will use UnityWebRequest API to download the image */
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            /* Downloaded image */
            imageToAdd = ((DownloadHandlerTexture)request.downloadHandler).texture;

            /* TODO 3.2 Destroy the previous ARTrackedImageManager component. 
             * Hint! What's the difference between Destroy() and DestroyImmediate()? */
            DestroyImmediate(trackImageManager);

            /* TODO 3.3 Attach a new ARTrackedImageManager component */
            trackImageManager = gameObject.AddComponent<ARTrackedImageManager>();

            /* TODO 3.4 Create a new runtime library */
            var library = trackImageManager.CreateRuntimeLibrary();

            /* TODO 3.5 Add the image to the database*/
            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(imageToAdd, "Image Name", 0.5f /* 50 cm */);
            }

            /* Set maximum number of moving images */
            trackImageManager.requestedMaxNumberOfMovingImages = 3;

            /* TODO 3.6 Set the new library as the reference library */
            trackImageManager.referenceLibrary = library;

            /* TODO 3.7 Enable the new ARTrackedImageManager component */
            trackImageManager.enabled = true;

            /* Attach event handling */
            trackImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            /* TODO 3.8 Instantiate a new object in scene so that it always fallows the tracked image
             * Hint! Use SetParent() method */

            GameObject newObject = Instantiate(m_PlacedPrefab, trackedImage.transform.position, trackedImage.transform.rotation);

            newObject.transform.SetParent(trackedImage.transform);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            /* Handle update event */
        }

        foreach (ARTrackedImage removedImage in eventArgs.removed)
        {
            /* Handle removed event */
        }
    }

    XRImageTrackingSubsystem CreateImageTrackingSubsystem()
    {
        // Get all available plane subsystems
        var descriptors = new List<XRImageTrackingSubsystemDescriptor>();
        Debug.Log(descriptors.Count);
        SubsystemManager.GetSubsystemDescriptors(descriptors);

        // Find one that supports boundary vertices
        foreach (var descriptor in descriptors)
        {
            if (descriptor.supportsMutableLibrary)
            {
                // Create this plane subsystem
                return descriptor.Create();
            }
        }

        return null;
    }
}
