using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    
    public void UpdateProgress(float progress)
    {
        // Clamp progress between 0 and 1
        progress = Mathf.Clamp01(progress);
        fillImage.transform.localScale = new Vector3(progress, 1, 1);
    }
}
