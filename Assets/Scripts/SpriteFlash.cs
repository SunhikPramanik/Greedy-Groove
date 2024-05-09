using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CallCoroutine(float fl, Color fc, int nof)
    {
        StartCoroutine(FlashCoroutine(fl, fc, nof));
    }

    public IEnumerator FlashCoroutine(float flashDuration, Color flashColor, int numberOfFlashes)
    {
        Color startColor = spriteRenderer.color;
        float elapsedFlashTime = 0;
        float elapsedFlashPercentage = 0;

        while(elapsedFlashTime < flashDuration)
        {
            elapsedFlashTime += Time.deltaTime;
            elapsedFlashPercentage = elapsedFlashTime / flashDuration;

            if(elapsedFlashPercentage > 1)
            {
                elapsedFlashPercentage = 1;
            }


            float pingPongPercentage = Mathf.PingPong(elapsedFlashPercentage * 2 * numberOfFlashes, 1);
            spriteRenderer.color = Color.Lerp(startColor, flashColor, pingPongPercentage);


            yield return null;
        }
    }
}
