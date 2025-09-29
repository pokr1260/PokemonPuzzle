using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public enum EGameState
{
    SCENELOAD
    , WAITINPUT
    , BLOCKMOVE
    , PARTICLE
    , SKILL
    , BATTLE
}

public class GameManager : Singleton<GameManager>
{
    

    public Player player;
    public Enemy enemy;
    public Pokemon enemyTarget;

    public PuzzleZone puzzleZone;

    public SkillSlot[] skillSlots = new SkillSlot[3];

    public List<Block> currentMatchedBlock = new List<Block>();

    public EGameState gameState;

    public Camera uiCamera;
    public GameObject uiCanvas;

    public RectTransform anchorChecker;

    private Vector2 screenToUICanvasRatio;

    public Vector2 leftBottomScreenPosition;
    public Vector2 rightTopScreenPosition;

    public Vector2 leftBottomWorldPosition;
    public Vector2 rightTopWorldPosition;

    private PuzzleZoneBG puzzleZoneBG;

    public GameObject filter;
    private MeshRenderer filterMeshRenderer;


    private AudioSource audioSource;

    public Pokemon pokemonPrefab = null;
    public GameObject pokemonHUDPrefab = null;
    public GameObject pokemonHUDRoot = null;

    public bool isDone = false;

    AudioClip notVeryEffectiveHit;  // 0.5배
    AudioClip normalHit;            // 1배
    AudioClip superEffectiveHit;    // 2.0배

    AudioClip pokemonDownSound;     // 포켓몬 기절 사운드

    
    public Material spritesDefaultMat;
    public Material outLineMat;

    public bool isPause = false;

    private void Start()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        gameState = EGameState.SCENELOAD;

        {
            if (null == player)
            {
                Debug.LogError("GameManager.cs / player is null");
                yield return null;
            }

            if (null == filter)
            {
                Debug.LogError("GameManager.cs / filter is null");
                yield return null;
            }

            if (null == pokemonPrefab)
            {
                Debug.LogError("GameManager.cs / pokemonPrefab is null");
                yield return null;
            }

            if (null == pokemonHUDPrefab)
            {
                Debug.LogError("GameManager.cs / pokemonHUDPrefab is null");
                yield return null;
            }

            if (null == pokemonHUDRoot)
            {
                Debug.LogError("GameManager.cs / pokemonHUDRoot is null");
                yield return null;
            }

            if (null == spritesDefaultMat)
            {
                Debug.LogError("GameManager.cs / spritesDefaultMat is null");
                yield return null;
            }

            if (null == outLineMat)
            {
                Debug.LogError("GameManager.cs / outLineMat is null");
                yield return null;
            }


            puzzleZone = GameObject.FindObjectOfType<PuzzleZone>();
            puzzleZoneBG = GameObject.FindObjectOfType<PuzzleZoneBG>();
            yield return new WaitForSeconds(0.1f);

            CalcScreenToUICanvasRatio();
            yield return new WaitForSeconds(0.1f);
            CheckAnchorScreenPosition();
            yield return new WaitForSeconds(0.1f);
            CheckAnchorWorldPosition();
            yield return new WaitForSeconds(0.1f);

            puzzleZone.Init();
            yield return new WaitForSeconds(0.1f);
            player.Init();
            yield return new WaitForSeconds(0.1f);

            filter.GetComponent<FillScreenFilter>().Init();
            filterMeshRenderer = filter.GetComponent<MeshRenderer>();

            audioSource = this.GetComponent<AudioSource>();
            yield return new WaitForSeconds(0.1f);

            // 적 임시 생성!
            {
                //int[] arr = { 1, 2, 3, 25, 10, 11, 12 };

                // 유령포켓몬들
                int[] arr = { 92, 93, 94 };
                enemy.Init(arr, 25);
                //SetTarget(enemy.enemyPokemons[0]);
                //SetTarget(enemy.currentWavePokemons[0]);
            }
            yield return new WaitForSeconds(0.1f);

            // 맞는 소리 로딩
            notVeryEffectiveHit = PokemonDataManager.Instance.GetPokemonEffectSound("hit_a");
            if (null == notVeryEffectiveHit)
            {
                Debug.LogError("Pokemon.cs / notVeryEffectiveHit is null");
                yield return null;
            }

            normalHit = PokemonDataManager.Instance.GetPokemonEffectSound("hit_b");
            if (null == normalHit)
            {
                Debug.LogError("Pokemon.cs / normalHit is null");
                yield return null;
            }

            superEffectiveHit = PokemonDataManager.Instance.GetPokemonEffectSound("hit_c");
            if (null == superEffectiveHit)
            {
                Debug.LogError("Pokemon.cs / superEffectiveHit is null");
                yield return null;
            }

            pokemonDownSound = PokemonDataManager.Instance.GetPokemonEffectSound("pokemon_down");
            if (null == superEffectiveHit)
            {
                Debug.LogError("Pokemon.cs / pokemonDownSound is null");
                yield return null;
            }

        }

        isDone = true;
        gameState = EGameState.WAITINPUT;
    }

    private void CheckAnchorScreenPosition()
    {
        if (null == anchorChecker)
        {
            Debug.LogError("GameManager.cs / anchorChecker is null");
            return;
        }

        // left bottom
        anchorChecker.anchorMin = new Vector2(0.0f, 0.0f);
        anchorChecker.anchorMax = new Vector2(0.0f, 0.0f);

        anchorChecker.anchoredPosition = new Vector2(0.0f, 0.0f);
        //Debug.Log(anchorChecker.localPosition);
        leftBottomScreenPosition = anchorChecker.localPosition;

        // right top
        anchorChecker.anchorMin = new Vector2(1.0f, 1.0f);
        anchorChecker.anchorMax = new Vector2(1.0f, 1.0f);

        anchorChecker.anchoredPosition = new Vector2(0.0f, 0.0f);
        //Debug.Log(anchorChecker.localPosition);
        rightTopScreenPosition = anchorChecker.localPosition;
    }

    private void CheckAnchorWorldPosition()
    {
        leftBottomWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        rightTopWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height));
    }


    public void SetGameState(EGameState state)
    {
        gameState = state;
    }

    public EGameState GetGameState()
    {
        return gameState;
    }

    public Pokemon[] GetPlayerPokemons()
    {
        return player.GetPlayerPokemons();
    }

    public Pokemon GetPlayerPokemon(int pokemonSlotIndex)
    {
        return player.GetPlayerPokemon(pokemonSlotIndex);
    }

    private void CalcScreenToUICanvasRatio()
    {
        CanvasScaler scaler = uiCanvas.GetComponent<CanvasScaler>();
        screenToUICanvasRatio.x = Screen.width / scaler.referenceResolution.x;
        screenToUICanvasRatio.y = Screen.height / scaler.referenceResolution.y;
    }

    public Vector2 GetScreenToUICanvasRatio()
    {
        return screenToUICanvasRatio;
    }

    public PuzzleZoneBG GetPuzzleZoneBG()
    {
        return puzzleZoneBG;
    }

    // 총 스크린 높이에서 PuzzleZone을 제외한 높이를 리턴한다.
    public float GetPuzzleZoneExcludeHeight()
    {
        return (rightTopScreenPosition.y - leftBottomScreenPosition.y) - puzzleZoneBG.GetTotalHeight();
    }

    public void SetFilterMeterialByPath(string path)
    {
        Material mat = Resources.Load(path, typeof(Material)) as Material;
        filterMeshRenderer.material = mat;
    }

    public void SetFilterMeterialByName(string name)
    {
        SetFilterMeterialByPath("Shader/Skill/" + name);
    }

    public void SetFilterMeterial(Material mat)
    {
        filterMeshRenderer.material = mat;
    }

    public Material GetMaterialByResources(string path)
    {
        Material mat = Resources.Load(path, typeof(Material)) as Material;

        return mat;
    }

    public void PlayEffectSound( AudioClip sound )
    {
        audioSource.clip = sound;
        audioSource.Play();
    }

    public bool isSoundEnd()
    {
        return audioSource.isPlaying;
    }

    // 블록 매치를 통한 기본 공격
    public void MatchBlockAttack(List<int> currentMatchedBlock)
    {
        Dictionary<int, int> pokemonBlockCount = new Dictionary<int, int>();

        // 총 폭파된 블럭 갯수를 구한다!
        for (int i = 0; i < currentMatchedBlock.Count; i++)
        {
            if (true == pokemonBlockCount.ContainsKey(currentMatchedBlock[i]))
            {
                pokemonBlockCount[currentMatchedBlock[i]]++;
            }
            else
            {
                pokemonBlockCount.Add(currentMatchedBlock[i], 1);
            }
        }

        Pokemon[] playerPokemons = player.GetPlayerPokemons();

        for (int i = 0; i < playerPokemons.Length; i++)
        {
            if (true == pokemonBlockCount.ContainsKey(playerPokemons[i].pokemonNumber))
            {
               playerPokemons[i].Attack(enemyTarget, pokemonBlockCount[playerPokemons[i].pokemonNumber]);
            }
        }

        return;
    }

    public void PlayHitSound(float hitPower)
    {
        // 2배
        if (2.0f <= hitPower)
        {
            Debug.Log("효과는 굉장했다!");
            audioSource.clip = superEffectiveHit;
        }
        // 1배
        else if (1.0f <= hitPower && hitPower < 2.0)
        {
            Debug.Log("효과는 평범했다");
            audioSource.clip = normalHit;
        }
        // 0.5배
        else if (0.0f < hitPower && hitPower < 1.0f)
        {
            Debug.Log("효과는 별로였다.");
            audioSource.clip = notVeryEffectiveHit;
        }
        // 0배
        else
        {
            Debug.Log("효과는 발생하지 않았다.");
            // 소리 출력x
        }

        audioSource.Play();
    }

    public void PlayPokemonDownSound()
    {
        audioSource.clip = pokemonDownSound;
        audioSource.Play();
    }

    public void SetTarget(Pokemon target)
    {
        if (null != this.enemyTarget)
        {
            this.enemyTarget.SetMeterial(spritesDefaultMat);
        }
        this.enemyTarget = target;
        this.enemyTarget.SetMeterial(outLineMat);
    }

    // 남은 적 포켓몬중 알아서 타겟을 잡아준다.
    public void DeadPokemon()
    {
        // 다음 타겟 설정
        Pokemon enemyTarget =enemy.GetLiveEnemyPokemon();

        if (null != enemyTarget)
        {
            this.SetTarget(enemyTarget);
        }
        else
        {
            // next wave
            enemy.NextWave();
        }
    }

}
