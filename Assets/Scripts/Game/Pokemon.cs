using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokemonDefine;
using UnityEngine.U2D;

using System.Reflection;

public class Pokemon : MonoBehaviour
{
    [Header("Pokemon Info")]
    [Range(1, 151)]
    public int pokemonNumber = 0;       // 설정 안할시 미싱노

    [Tooltip("포켓몬 타입1")]
    public PokemonDefine.PokemonType pokemonType1;

    [Tooltip("포켓몬 타입2")]
    public PokemonDefine.PokemonType pokemonType2;

    [Tooltip("포켓몬 상태")]
    public PokemonDefine.PokemonStatus pokemonStatus = PokemonStatus.NORMAL;

    [Tooltip("포켓몬 현재 체력")]
    public float hp;

    [Tooltip("포켓몬 전체 제력")]
    public float totalHp;

    [Tooltip("포켓몬 레벨")]
    public int level;

    [Tooltip("포켓몬 공격력")]
    public float attack;

    // 포켓몬 이름
    public string pokemonName;

    // 스킬
    public int skillCount = 2;

    public Skill[] pokemonSkills;

    public GameObject[] pokemonSkillGameobject;

    [Range(0,100)]
    public float skillGauge = 0.0f;

    public SkillSlot skillSlot = null;

    public HUD hud;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public float deadAlphaSpeed = 1.0f;

    private bool isPlayerPokemon;

    // 포켓몬 울음소리
    private AudioClip pokemonCry;
    public float deadPokemonCryPitch = 0.95f;

    private AudioSource audioSource;

    public void Init( int pokemonNumber , bool isPlayerPokemon , int level , int pokemonSlot , SkillSlot skillSlot )
    {
        if (null == this.spriteRenderer)
        {
            this.spriteRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        }

        this.level = level;
        this.pokemonNumber = pokemonNumber;
        this.isPlayerPokemon = isPlayerPokemon;
        InitStats();
        InitSkill(isPlayerPokemon, skillSlot);
        InitSprite(isPlayerPokemon);
        InitPosition(pokemonSlot, isPlayerPokemon);

        // 울음 소리
        pokemonCry = PokemonDataManager.Instance.GetPokemonCrySound(this.pokemonNumber);

        audioSource = GetComponent<AudioSource>();
        if (null == audioSource)
        {
            Debug.LogError("Pokemons.cs / audioSource is null");
            return;
        }
        audioSource.clip = pokemonCry;

    }

    private void InitStats()
    {
        PokemonBaseStats stats = PokemonDataManager.Instance.GetPokemonBaseStatData(pokemonNumber);
        SetStats(stats);
    }

    //스텟 세팅
    public void SetStats(PokemonBaseStats stats)
    {
        hp = stats.hp;
        totalHp = stats.hp;
        pokemonName = stats.name;
        pokemonType1 = PokemonDataManager.Instance.GetPokemonType(stats.type1);
        pokemonType2 = PokemonDataManager.Instance.GetPokemonType(stats.type2);
        attack = stats.attack;

        return;
    }

    //// 스킬 변경
    //public void SetSkill(int slot, string skillName)
    //{
    //    pokemonSkills[slot] = PokemonDataManager.Instance.GetPokemonSkillData(skillName);

    //    return;
    //}

    private void InitSkill(bool isPlayerPokemon , SkillSlot skillSlot)
    {
        if (true == isPlayerPokemon)
        {
            this.skillSlot = skillSlot;
        }

        pokemonSkillGameobject = new GameObject[skillCount];
        pokemonSkills = new Skill[skillCount];
        for (int i = 0; i < pokemonSkillGameobject.Length; i++)
        {
            pokemonSkillGameobject[i] = new GameObject();
            pokemonSkillGameobject[i].transform.parent = this.transform;
        }


        string[] skillList = PokemonDataManager.Instance.GetPokemonSkillLearnData(pokemonNumber);

        /* 플레이어포켓몬인 경우 포켓몬 저장이 구현되면 저장된 포켓몬정보에서 스킬을 가져오도록 수정할것. >>>>>*/
        int[] skillIndex = new int[2];
        skillIndex[0] = Random.Range(0, skillList.Length);
        skillIndex[1] = 0;
        

        while (true)
        {
            skillIndex[1] = Random.Range(0, skillList.Length);
            if (skillIndex[0] != skillIndex[1])
            {
                break;
            }
        }
        /* 플레이어포켓몬인 경우 포켓몬 저장이 구현되면 저장된 포켓몬정보에서 스킬을 가져오도록 수정할것. <<<<<*/

        string[] skillName = new string[2];

        for (int i = 0; i < skillName.Length; i++)
        {
            skillName[i] = skillList[skillIndex[i]];
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        for (int i = 0; i < pokemonSkillGameobject.Length; i++)
        {
            System.Type t = assembly.GetType(skillName[i]);

            Skill s = pokemonSkillGameobject[i].AddComponent(t) as Skill;
            pokemonSkills[i] = s;

            //Debug.Log(skillName[i]);

            s.Init(this, skillName[i], pokemonNumber);
        }

    }

    private void InitSprite(bool isPlayerPokemon)
    {
        Sprite sprite = PokemonDataManager.Instance.GetPokemonSprite(pokemonNumber);

        // null이면 기본 세팅인 안농이 뜰것이다.
        if (null != sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        if (true == isPlayerPokemon)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void InitPosition(int pokemonSlot, bool isPlayerPokemon)
    {
        // 비례식을 이용해서 높이를 구한다.
        float height = Screen.height;
        float UIHeight = GameManager.Instance.rightTopScreenPosition.y - GameManager.Instance.leftBottomScreenPosition.y;
        float puzzleZoneHeight = (height * GameManager.Instance.GetPuzzleZoneBG().GetTotalHeight()) / UIHeight;

        float center = ((Screen.height - puzzleZoneHeight) / 2.0f) + puzzleZoneHeight;

        if (true == isPlayerPokemon)
        {
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, center, 0.0f));
            this.transform.position += new Vector3(1.5f, 0.0f, 0.0f);
        }
        else
        {
            // 적 포켓몬
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, center, 0.0f));
            this.transform.position += new Vector3(-1.5f, 0.0f, 0.0f);
        }

        if (1 == pokemonSlot)
        {
            transform.position -= new Vector3(0.0f, 2.0f, 0.0f);
        }
        else if (2 == pokemonSlot)
        {
            transform.position += new Vector3(0.0f, 2.0f, 0.0f);
        }
    }

    public Skill[] GetPokemonSkillData()
    {
        return pokemonSkills;
    }

    public void Attack(Pokemon target, int blockCount)
    {
        if (null == target)
        {
            Debug.LogError("Pokemon.cs / void Attack(Pokemon target, int blockCount) / target is null");
            return;
        }

        Debug.Log(this.pokemonName+"이(가) " + target.pokemonName + "에게 " + blockCount + " 만큼 공격!");

        StartCoroutine(AttackMotionCo());
        target.Defence(this.attack* 0.01f * blockCount);
    }

    public IEnumerator AttackMotionCo()
    {
        Vector3 direction = new Vector3(1.0f,0.0f,0.0f);
        if (true != spriteRenderer.flipX)
        {
            direction = direction * -1.0f;
        }

        this.transform.position += direction * 0.2f;
        yield return new WaitForSeconds(0.1f);
        this.transform.position -= direction * 0.2f;
    }


    public void DoSkill(int skillSlotIndex)
    {
        Skill s = pokemonSkillGameobject[skillSlotIndex].GetComponent<Skill>();

        // 임시로 0번 포켓몬으로 설정..
        StartCoroutine(s.DoSkill(GameManager.Instance.enemyTarget));
    }

    // 평타 공격을 받았을시
    public void Defence(float power)
    {
        Debug.Log(this.pokemonName + "는(은) " + power + "의 피해를 입었다.");
        StartCoroutine(DefenceMotionCo(0.5f));

        hp -= power;

        if (hp <= 0)
        {
            hp = 0.0f;
            StartCoroutine(DeadAnimation());
        }

        hud.SetAnimateHP(hp, totalHp);

    }

    public IEnumerator DefenceMotionCo(float hitPower)
    {
        GameManager.Instance.PlayHitSound(hitPower);
        this.spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        yield return new WaitForSeconds(0.1f);
        this.spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.1f);
        this.spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        yield return new WaitForSeconds(0.1f);
        this.spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    // 스킬로 인한 데미지
    public void Damage(PokemonType skillType, int skillDamage)
    {
        TypeResult result = PokemonTypeManager.Instance.TypeCompare(skillType, this.pokemonType1 | this.pokemonType2);

        float resultValue = result.resultValue;

        StartCoroutine(DefenceMotionCo(resultValue));

        // 데미지
        float resultDamage = resultValue * skillDamage;

        if ((hp - resultDamage) <= 0.0f)
        {
            hp = 0.0f;
            StartCoroutine(DeadAnimation());
        }

        hud.SetAnimateHP(hp, totalHp);

        // 상태이상
        switch (result.status)
        {
            case PokemonStatus.NORMAL:          // 노멀 (아무이상없음)
                break;
            case PokemonStatus.FREEZE:          // 얼음
                break;
            case PokemonStatus.PARALYSIS:       // 마비
                break;
            case PokemonStatus.BURN:            // 화상
                break;
            case PokemonStatus.SLEEP:           // 잠듦
                break;
            case PokemonStatus.POISONING:       // 독
                break;
            case PokemonStatus.FAINT:           // 기절
                break;
            default:
                break;
        }
    }

    IEnumerator DeadAnimation()
    {
        GameManager.Instance.isPause = true;

        // HUD 애니메이션이 모두 끝나면!
        yield return new WaitUntil(() => { return (true == this.hud.isAnimationFinish); });

        yield return new WaitForSeconds(0.25f);

        // 포켓몬 울음소리 재생(약간 느리게)
        PlayPokemonCry(deadPokemonCryPitch);
        yield return new WaitUntil(() => { return false == isPlayingPokemonCry(); });
        // 포켓몬 다운 소리 재생
        GameManager.Instance.PlayPokemonDownSound();

        float alpha = 1.0f;
        while (true)
        {
            alpha -= Time.deltaTime * deadAlphaSpeed;
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            yield return null;

            if (alpha <= 0)
            {
                break;
            }
            
        }

        hud.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        GameManager.Instance.DeadPokemon();
        GameManager.Instance.isPause = false;
    }

    public void PlayPokemonCry(float pitch = 1.0f)
    {
        audioSource.pitch = pitch;
        audioSource.Play();
    }

    public bool isPlayingPokemonCry()
    {
        return audioSource.isPlaying;
    }

    private void OnMouseDown()
    {
    }

    // 공격 타겟!
    private void OnMouseUp()
    {
        // 적 포켓몬인 경우에만 타겟을 정한다!!
        if (false == isPlayerPokemon)
        {
            Debug.Log("선택된 포켓몬 : " + this.pokemonName);
            GameManager.Instance.SetTarget(this);
        }
    }

    public void SetMeterial( Material material )
    {
        spriteRenderer.material = material;
    }


    private void OnDisable()
    {
        if (null != hud)
        {
            this.hud.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (null != hud)
        {
            this.hud.gameObject.SetActive(true);
        }
    }


}