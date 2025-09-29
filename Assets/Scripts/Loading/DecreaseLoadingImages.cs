using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecreaseLoadingImages : MonoBehaviour
{
    public Image progressHandleImage;

    public Image bgImage;

    public float alpha { get; private set; } = 1.0f;

    public float alphaSpeed = 0.1f;

    private void Start()
    {
        if (null == bgImage)
        {
            Debug.LogError("LoadingBG.cs / bgImage is null");
            return;
        }

        if (null == progressHandleImage)
        {
            Debug.LogError("LoadingBG.cs / progressHandleImage is null");
            return;
        }

        StartCoroutine(DecreaseAlpha());

    }


    public IEnumerator DecreaseAlpha()
    {
        yield return new WaitUntil( () => { return (true == GameManager.Instance.isDone); } );

        while (0.0f <= alpha)
        {
            alpha -= Time.deltaTime * alphaSpeed;

            progressHandleImage.color = new Color(1.0f,1.0f,1.0f, alpha);
            bgImage.color = new Color(0.0f,0.0f,0.0f, alpha);

            yield return new WaitForSeconds(0.1f);
        }
        
    }

    private void Update()
    {
        if (alpha < 0.0f)
        {
            Destroy(this.transform.root.gameObject);
        }
    }

}
