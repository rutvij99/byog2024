using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text titleText;
    [SerializeField] private Image Fill;
    [SerializeField] private Image Diff;

    private Coroutine diffRoutine;

    public void SetTitle(string title)
    {
        titleText.text = title;
    }
    
    public void UpdateHealth(float perc)
    {
        if (diffRoutine != null) 
            StopCoroutine(diffRoutine);

        Fill.fillAmount = perc;
        diffRoutine = StartCoroutine(DiffCoroutine());
    }

    private IEnumerator DiffCoroutine()
    {
        yield return new WaitForSeconds(1f);
        float timeStep = 0;
        float start = Diff.fillAmount;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime / 0.25f;
            Diff.fillAmount = Mathf.Lerp(start, Fill.fillAmount, timeStep);
            yield return new WaitForEndOfFrame();
        }
    }
}
