using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    private Image progressBar;

    void Awake()
    {
        progressBar = transform.GetComponent<Image>();
    }

    void Update()
    {
        progressBar.fillAmount = Loader.GetLoadingProgress();
    }
}
