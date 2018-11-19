using UnityEngine;

/// <summary>
/// Class CameraExtensions. Extends the Camera with a function that returns the camera's extents as bounds.
/// </summary>
public static class CameraExtensions
{
    /// <summary>
    /// Returns the camera's extents as bounds for an orthographic camera.
    /// </summary>
    /// <param name="camera">The camera.</param>
    /// <returns>Bounds.</returns>
    public static Bounds OrthographicBounds(this Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}
