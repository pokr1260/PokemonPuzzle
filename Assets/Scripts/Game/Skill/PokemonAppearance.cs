using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonAppearance : MonoBehaviour
{
    [Header("Backgrounds")]
    [Tooltip("배경 게임오브젝트")]
    public GameObject bg;

    [Header("Lines")]
    [Tooltip("뒤에 지나갈 라인들")]
    public Transform[] lines;

    [Tooltip("라인 지나갈 속도")]
    public float lineSpeed;

    [Tooltip("라인의 너비")]
    public float lineWidth;
    [Tooltip("라인의 높이")]
    public float lineHeight;

    [Header("Pokemon")]
    [Tooltip("등장포켓몬")]
    public GameObject pokemon;
    
    [Tooltip("포켓몬 번호")]
    public int pokemonNumber;

    [Header("Others")]
    [Tooltip("몇초뒤에 disable할 것인가?")]
    public float disableTime;

    private float rangeMinX;
    private float rangeMaxX;
    private float rangeMinY;
    private float rangeMaxY;


    public void Init(int pokemonNumber)
    {
        if (null == bg)
        {
            Debug.LogError("PokemonAppearance.cs / bg is null");
            return;
        }

        if (lines.Length <= 0)
        {
            Debug.LogError("PokemonAppearance.cs / lines length is 0");
            return;
        }

        if (null == pokemon)
        {
            Debug.LogError("PokemonAppearance.cs / pokemon is null");
            return;
        }

        if (pokemonNumber <= 0)
        {
            Debug.LogError("PokemonAppearance.cs / pokemonNumber is wrong");
            return;
        }
        this.pokemonNumber = pokemonNumber;

        // 크기 조절은 FillScreenSprite 가 해준다. 위치할 좌표만 잘 찍어주자

        // 등장씬 위치 조절
        {
            float height = Screen.height;
            float UIHeight = GameManager.Instance.rightTopScreenPosition.y - GameManager.Instance.leftBottomScreenPosition.y;
            float puzzleZoneHeight = (height * GameManager.Instance.GetPuzzleZoneBG().GetTotalHeight()) / UIHeight;

            float center = ((Screen.height - puzzleZoneHeight) / 2.0f) + puzzleZoneHeight;

            Vector3 result = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, center, 0.0f));
            result.x = 0.0f;

            this.transform.position = result;
        }

        // 포켓몬 이미지 띄우기
        { 
            Sprite sprite = PokemonDataManager.Instance.GetPokemonSprite(pokemonNumber);

            pokemon.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        // 배경 가속 라인 세팅(흰줄)
        {
            FillScreenSprite fillScreenSprite = GetComponent<FillScreenSprite>();

            Vector2[] startPos = new Vector2[lines.Length];

            // 가로는 스크린 가득 차 있으므로 스크린 좌우 끝을 기준으로 삼는다.
            rangeMinX = GameManager.Instance.leftBottomWorldPosition.x;
            rangeMaxX = GameManager.Instance.rightTopWorldPosition.x;

            //float bgWorldHeight = Mathf.Abs(GameManager.Instance.leftBottomWorldPosition.y - Camera.main.ScreenToWorldPoint(new Vector3(0.0f, fillScreenSprite.targetSize.y, 0.0f)).y);

            // Sprite Pixels Per Unit만큼 나누어 준다.
            float bgWorldHeight = fillScreenSprite.targetSize.y / lines[0].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
            float bgWorldHeightHalf = bgWorldHeight / 2.0f;

            rangeMinY = this.transform.position.y - bgWorldHeightHalf;
            rangeMaxY = this.transform.position.y + bgWorldHeightHalf;

            for (int i = 0; i < lines.Length; i++)
            {
                startPos[i].x = Random.Range(rangeMinX, rangeMaxX);
                startPos[i].y = Random.Range(rangeMinY, rangeMaxY);

                lines[i].position = startPos[i];
                lines[i].localScale = new Vector2(lineWidth, lineHeight);
            }
        }

        // 포켓몬 사운드 재생
        {
            AudioClip cry = PokemonDataManager.Instance.GetPokemonCrySound(pokemonNumber);

            GetComponent<AudioSource>().clip = cry;
            GetComponent<AudioSource>().Play();
        }

    }

    /*
    {
        if (null == bg)
        {
            Debug.LogError("PokemonAppearance.cs / bg is null");
            return;
        }

        if (lines.Length <= 0)
        {
            Debug.LogError("PokemonAppearance.cs / lines length is 0");
            return;
        }

        if (null == pokemon)
        {
            Debug.LogError("PokemonAppearance.cs / pokemon is null");
            return;
        }

        if (pokemonNumber <= 0)
        {
            Debug.LogError("PokemonAppearance.cs / pokemonNumber is wrong");
            return;
        }
        this.pokemonNumber = pokemonNumber;

        thisRectTransform = GetComponent<RectTransform>();

        float absoluteScreenHeight = Camera.main.orthographicSize * 2.0f;
        float absoluteScreenWidth = absoluteScreenHeight / Screen.height * Screen.width;

        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Camera.main.pixelHeight, 0.0f));

        // 등장씬 위치 조절. ( 등장씬은 스킬패널의 위쪽 부분에서 중앙부분에서 등장할 것이다. )
        {
            //PuzzleZoneBG puzzleZoneBG = GameObject.FindObjectOfType<PuzzleZoneBG>();
            // 퍼즐존의 높이만큼 이동한다.
            thisRectTransform.anchoredPosition = new Vector2(0.0f, GameManager.Instance.GetPuzzleZoneBG().GetTotalHeight());

            float height = GameManager.Instance.GetPuzzleZoneExcludeHeight();
            float halfHeight = height / 2.0f;
            float resultHeight = thisRectTransform.anchoredPosition.y + halfHeight;

            thisRectTransform.anchoredPosition = new Vector2(0.0f, resultHeight);
        }

        // 포켓몬 이미지 띄우기
        {
            Sprite sprite = PokemonDataManager.Instance.GetPokemonSprite(pokemonNumber);

            pokemon.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        // 포켓몬 크기 조절
        {
            float scale = GameManager.Instance.uiCanvas.GetComponent<CanvasScaler>().referencePixelsPerUnit;
            pokemon.GetComponent<RectTransform>().localScale *= scale;
        }

        // 배경 가속 라인 세팅(흰줄)
        {
            lineWidth = lines[0].rect.width;
            lineHeight = lines[0].rect.height;

            Vector2[] startPos = new Vector2[lines.Length];

            rangeMinX = GameManager.Instance.leftBottomScreenPosition.x;
            rangeMaxX = GameManager.Instance.rightTopScreenPosition.x;

            rangeMinY = 0.0f;
            rangeMaxY = bg.GetComponent<RectTransform>().rect.height;

            for (int i = 0; i < lines.Length; i++)
            {
                startPos[i].x = Random.Range(rangeMinX, rangeMaxX);
                startPos[i].y = Random.Range(rangeMinY + lineHeight, rangeMaxY - lineHeight);

                lines[i].anchoredPosition = startPos[i];
            }
        }

        // 포켓몬 사운드 재생
        {
            AudioClip cry = PokemonDataManager.Instance.GetPokemonCrySound(pokemonNumber);

            GetComponent<AudioSource>().clip = cry;
            GetComponent<AudioSource>().Play();
        }

    }
    */

    private void Update()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            // 범위 밖으로 나갈경우
            if (rangeMaxX + (lineWidth / 100.0f) < lines[i].position.x)
            {
                float x = rangeMinX - (lineWidth / 100.0f);
                float y = Random.Range(rangeMinY, rangeMaxY);

                lines[i].position = new Vector2(x, y);
            }

            Vector2 addPosition = lines[i].position;
            addPosition.x = addPosition.x + (lineSpeed * Time.deltaTime);

            lines[i].position = addPosition;
        }
    }

    /*
    private void Update()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            // 범위 밖으로 나갈경우
            if ( rangeMaxX < lines[i].anchoredPosition.x )
            {
                float x = rangeMinX - lineWidth;
                float y = Random.Range(rangeMinY + lineHeight, rangeMaxY - lineHeight);

                lines[i].anchoredPosition = new Vector2(x, y);
            }

            Vector2 addPosition = lines[i].anchoredPosition;
            addPosition.x = addPosition.x + (lineSpeed * Time.deltaTime);

            lines[i].anchoredPosition = addPosition;
        }
    }
    */

    public IEnumerator TimerActiveOffCo()
    {
        yield return new WaitForSeconds(disableTime);
        this.gameObject.SetActive(false);
    }
}
