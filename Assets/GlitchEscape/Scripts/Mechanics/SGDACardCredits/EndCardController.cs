using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCardController : MonoBehaviour
{
    public SpriteRenderer blackCard;
    public SpriteRenderer endCard;
    private float timeElapsed;
    public float timeToFade;
    public float alphaStep;
    private bool fadeStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        timeElapsed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(!fadeStarted && timeElapsed > timeToFade)
        {
            fadeStarted = true;
            GetComponent<EndCardButton>().Activate();
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        Color alpha = blackCard.color;
        alpha.a -= alphaStep;
        blackCard.color = alpha;

        while(endCard.color.a > 0.01)
        {
            alpha = blackCard.color;
            alpha.a -= alphaStep;
            blackCard.color = alpha;
            yield return null;
        }
        StopCoroutine(FadeIn());
    }
}
