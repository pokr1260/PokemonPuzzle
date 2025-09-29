using System.Collections;
using System.Collections.Generic;
using System.Linq;                  // ToArray
using UnityEngine;


public class PuzzleZone : MonoBehaviour
{
    // 퍼즐 공간의 너비
    [Tooltip("퍼즐 공간의 너비")]
    [Range(1, 7)]
    public int puzzleWidth = 6;

    // 퍼즐공간의 높이
    [Tooltip("퍼즐공간의 높이")]
    [Range(1, 7)]
    public int puzzleHeight = 7;

    [Tooltip("블록 프리팹을 넣어주세요.")]
    public GameObject blockPrefab;

    [Tooltip("퍼즐 지역을 바닥에서 떨어뜨릴 거리 (px)")]
    public float bottomOffset = 0.0f;

    [Tooltip("퍼즐이 떨어지기 시작할 위치")]
    public float dropDownOffset = 100.0f;

    [Tooltip("매치되고 파괴 되기까지 기다리는 시간.")]
    public float destroyBlockWaitTime = 1.0f;

    public GameObject mask;

    public GameObject destroyEffect;

    public GameObject skillGaugeEffect;

    public GameObject backgroundImage;

    public SkillSlot[] skillSlots = new SkillSlot[3];

    // 플레이어의 포켓몬
    private HashSet<int> playerPokemonNumbers = new HashSet<int>();

    // 블록에 등장시킬 포켓몬 종류
    private HashSet<int> blockPokemons = new HashSet<int>();

    // 블록 정보 [y,x]
    public Block[,] blockAllInfo = null;

    private Player player = null;

    //[SerializeField]
    //private List<Block> currentMatchedBlock = new List<Block>();

    [SerializeField]
    private List<int> currentMatchedBlock = new List<int>();

    private Rect puzzleZoneRect;

    private void Awake()
    {
        if (null == destroyEffect)
        {
            Debug.LogError("destroyEffect is null");
        }

        if (null == blockPrefab)
        {
            Debug.LogError("blockPrefab is null");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init()
    {
        blockAllInfo = new Block[puzzleWidth, puzzleHeight];

        // 플레이어 포켓몬 도감 번호를 가져온다.
        // 이 번호를 토대로 퍼즐 블록을 생성한다.
        // 가지고 있는 플레이어 포켓몬의 퍼즐 블록이 포함되어 생성되어야 한다.
        player = GameObject.Find("Player").GetComponent<Player>();

        SetupBlockPokemons();

        // 인스펙터에서 블록설정이 안되어있는 경우 그냥 종료
        if (null == blockPrefab)
        {
            Debug.LogError("blockPrefab is null, check Inspector");
            return;
        }

        // 블럭 생성
        CreateBlock();

        // 스크린 좌표를 월드 포지션으로 변환
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));

        transform.position = screenToWorldPoint;
        transform.position *= -1.0f;

        // 중앙 설정
        Vector2 blockSize = blockPrefab.GetComponent<SpriteRenderer>().size;
        // 앵커포인트가 중앙에 위치하므로 0.5배만큼 좌하단으로 이동시킨다.
        {
            // 좌 하단으로 이동
            {
                Vector3 tempPosition = transform.position;
                tempPosition.x += blockSize.x * 0.5f;
                tempPosition.y += blockSize.y * 0.5f;

                transform.position = tempPosition;
            }

            // 중앙으로 이동
            {
                /*
                 
                ^
                |
                |
                |
                |
                |
                |□□□□□□□□□□□□□□
                |-------------------->

                |------a------|--b--|
                |                   |
                |-----------c-------|

                중앙으로 이동할려면 c - a = b 가 되고 b의 0.5배만큼 오른쪽으로 이동하면 중앙으로 이동한다.

                c == (screenToWorldPoint.x * 2.0f)
                a == (blockSize.x * puzzleWidth)
                b == ((screenToWorldPoint.x * 2.0f) - (blockSize.x * puzzleWidth))
                 */

                Vector3 tempPosition = transform.position;

                tempPosition.x = tempPosition.x + (((screenToWorldPoint.x * 2.0f) - (blockSize.x * puzzleWidth)) / 2.0f);

                transform.position = tempPosition;
            }

            // 세로 오프셋
            {
                Vector3 heightScreenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, bottomOffset, 0.0f));

                Vector3 tempPosition = transform.position;
                tempPosition.y = heightScreenToWorldPoint.y;    // 설정한 포지션을 넣어준다.
                tempPosition.y += (blockSize.y * 0.5f);         // 블록 pivot이 중앙에 있으므로 그만큼 바닥을 가리키도록 더해준다.

                transform.position = tempPosition;
            }

        }

        // pivot이 완전한 좌하단이 아니고 blockSize의 각각 반만큼 이동해있기 때문에 이것을 빼준다.
        puzzleZoneRect.Set(transform.position.x - (blockSize.x * 0.5f), transform.position.y - (blockSize.y * 0.5f), blockSize.x * puzzleWidth, blockSize.y * puzzleHeight);
        //Debug.Log(puzzleZoneRect);

        //블럭 마스크 설정
        {
            // 마스크 크기 설정
            mask.transform.localScale = new Vector2(puzzleWidth * 100.0f, puzzleHeight * 100.0f);

            // 마스크 위치 설정
            mask.transform.position = puzzleZoneRect.center;
        }

        // 배경이미지 높이 설정
        backgroundImage.GetComponent<PuzzleZoneBG>().SetHeight(puzzleZoneRect);

    }

    // 블록에 등장시킬 포켓몬들을 설정한다.
    private void SetupBlockPokemons()
    {
        int[] getPlayerPokemonNumbers = player.GetPlayerPokemonNumbers();

        for (int i = 0; i < getPlayerPokemonNumbers.Length; i++)
        {
            // 포켓몬 도감번호 1 ~ 151
            if (0 < getPlayerPokemonNumbers[i] && getPlayerPokemonNumbers[i] < 152)
            {
                //플레이어가 들고있는 포켓몬정보를 가져온다.
                playerPokemonNumbers.Add(getPlayerPokemonNumbers[i]);
                blockPokemons.Add(getPlayerPokemonNumbers[i]);
            }
        }

        // 퍼즐에 등장시킬 포켓몬을 미리 정한다.
        while (blockPokemons.Count < 5)
        {
            int getValue = Random.Range(1, 151);
            blockPokemons.Add(getValue);    // 이미 존재하면 다음 루프에서 구한다.
        }
    }

    private void CreateBlock()
    {
        int[] asArray = blockPokemons.ToArray();    //using System.Linq;
        
        //가로
        for (int i = 0; i < puzzleWidth; i++)
        {
            //세로
            for (int j = 0; j < puzzleHeight; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);

                // Block의 Init 호출
                int maxIterations = 0;
                int pokemonNumber = asArray[Random.Range(0, asArray.Length)];

                // 맨 처음 타일 생성시 매치되는 타일이 생성되지 않도록 하기 위함 (100번 넘게 돌렸는데도 그러면 어쩔수 없..)
                while (true == MatchesAt(i, j, pokemonNumber) && maxIterations < 100)
                {
                    pokemonNumber = asArray[Random.Range(0, asArray.Length)];
                    maxIterations++;
                }

                GameObject block = Instantiate(blockPrefab, tempPosition, Quaternion.identity, this.transform);
                blockAllInfo[i, j] = block.GetComponent<Block>();
                blockAllInfo[i, j].Init(pokemonNumber, i, j);
            }
        }
    }

    public bool IsBlockMoveAllEnd()
    {
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                Block tempBlock = blockAllInfo[i, j];
                if ( null != tempBlock )
                {
                    if (false == tempBlock.IsMoveEnd())
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public bool IsBlockMoveEnd(List<Block> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].IsMoveEnd())
            {
                return false;
            }
        }
        return true;
    }

    
    // x,y 에 있는 블록이 매치인가 아닌가를 리턴
    private bool MatchesAt(int x, int y, int pokemonNumber)
    {
        if (1 < x)
        {
            if (blockAllInfo[x - 1, y].pokemonNumber == pokemonNumber && pokemonNumber == blockAllInfo[x - 2, y].pokemonNumber)
            {
                return true;
            }
        }

        if (1 < y)
        {
            if (blockAllInfo[x, y - 1].pokemonNumber == pokemonNumber && pokemonNumber == blockAllInfo[x, y - 2].pokemonNumber)
            {
                return true;
            }
        }
        return false;
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitUntil(() => { return (false == GameManager.Instance.isPause); });

        yield return new WaitForSeconds(0.1f);
        //전 블럭을 돈다.
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                // 현재 위치의 block을 받아온다.
                Block currentBlock = blockAllInfo[i, j];

                if (null != currentBlock)
                {
                    // 좌우 매치 확인
                    {
                        // 좌우 범위를 벗어나지 않으며
                        if (0 < i && i < puzzleWidth - 1)   // 양 끝을 기준으로 좌우 검사를 할 필요가 없다.
                        {
                            Block leftBlock = blockAllInfo[i - 1, j];
                            Block rightBlock = blockAllInfo[i + 1, j];

                            if (null != leftBlock && null != rightBlock)
                            {
                                // 나란히 좌우로 세 블록이 같은 포켓몬인 경우
                                if (leftBlock.pokemonNumber == currentBlock.pokemonNumber && currentBlock.pokemonNumber == rightBlock.pokemonNumber)
                                {
                                    yield return new WaitUntil(() => { return IsBlockMoveAllEnd(); });
                                    // 모든 이동을 마쳤을때 매치 표시를 남긴다.
                                    //if (false == currentMatchedBlock.Contains(leftBlock))
                                    //{
                                    //    currentMatchedBlock.Add(leftBlock);
                                    //}
                                    leftBlock.SetIsMatched(true);

                                    //if (false == currentMatchedBlock.Contains(rightBlock))
                                    //{
                                    //    currentMatchedBlock.Add(rightBlock);
                                    //}
                                    rightBlock.SetIsMatched(true);

                                    //if (false == currentMatchedBlock.Contains(currentBlock))
                                    //{
                                    //    currentMatchedBlock.Add(currentBlock);
                                    //}
                                    currentBlock.SetIsMatched(true);
                                }
                            }
                        }
                    }

                    // 상하 매치 확인
                    {
                        // 상하 범위를 벗어나지 않으며
                        if (0 < j && j < puzzleHeight - 1)  // 양 끝을 기준으로 상하 검사를 할 필요가 없다.
                        {
                            Block upBlock = blockAllInfo[i, j+1];
                            Block downBlock = blockAllInfo[i, j-1];

                            if (null != upBlock && null != downBlock)
                            {
                                // 나란히 상하로 세 블록이 같은 포켓몬인 경우
                                if (upBlock.pokemonNumber == currentBlock.pokemonNumber && currentBlock.pokemonNumber == downBlock.pokemonNumber)
                                {
                                   yield return new WaitUntil(() => { return IsBlockMoveAllEnd(); });
                                    // 모든 이동을 마쳤을때 매치 표시를 남긴다.
                                    //if (false == currentMatchedBlock.Contains(upBlock))
                                    //{
                                    //    currentMatchedBlock.Add(upBlock);
                                    //}
                                    upBlock.SetIsMatched(true);

                                    //if (false == currentMatchedBlock.Contains(upBlock))
                                    //{
                                    //    currentMatchedBlock.Add(downBlock);
                                    //}

                                    downBlock.SetIsMatched(true);
                                    //if (false == currentMatchedBlock.Contains(upBlock))
                                    //{
                                    //    currentMatchedBlock.Add(currentBlock);
                                    //}
                                    currentBlock.SetIsMatched(true);
                                }
                            }

                        }
                    }
                }
            }
        }
    }

    public void DestroyMatchedBlocks()
    {
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                if (null != blockAllInfo[i, j])
                {
                    if (true == blockAllInfo[i, j].GetIsMatched())
                    {
                        // 파티클 생성
                        GameObject destroyParticle = Instantiate(destroyEffect, new Vector2(i, j), Quaternion.identity, this.transform);
                        destroyParticle.transform.localPosition = new Vector2(i, j);

                        // 스킬 게이지 이펙트 파티클
                        for (int ii = 0; ii < skillSlots.Length; ii++)
                        {
                            if (skillSlots[ii].pokemonNumber == blockAllInfo[i,j].pokemonNumber)
                            {
                                GameObject skillGaugeEffectParticle = Instantiate(skillGaugeEffect, new Vector2(i, j), Quaternion.identity, this.transform);
                                skillGaugeEffectParticle.transform.localPosition = new Vector2(i, j);

                                skillGaugeEffectParticle.GetComponent<MoveTarget>().Init(skillSlots[ii].GetGaugeHandleWorldPosition());

                                // 스킬 게이지 추가
                                skillSlots[ii].AddGauge();

                                break;
                            }
                        }

                        // 매치된 블록 리스트에 추가
                        currentMatchedBlock.Add(blockAllInfo[i, j].pokemonNumber);

                        Destroy(blockAllInfo[i, j].gameObject);
                        blockAllInfo[i, j] = null;
                    }
                }
            }
        }

        // 매치된 블록을 전달해서 어택
        GameManager.Instance.MatchBlockAttack(currentMatchedBlock);

        currentMatchedBlock.Clear();
        // 빈블록이 존재할시 아래로 내려간다.
        StartCoroutine(DropBlock());
    }

    // 블록을 아래로 떨어뜨리기
    private IEnumerator DropBlock()
    {
        //yield return new WaitUntil(() => { return IsBlockMoveAllEnd(); });

        int blankBlockCount = 0;
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)  // 세로의 빈공간을 세서 얼마나 아래로 내려가는지 확인할것.
            {
                if (null == blockAllInfo[i,j])
                {
                    blankBlockCount++;          // 빈 공간 발견시 갯수를 센다.
                }
                else if (0 < blankBlockCount)
                {
                    blockAllInfo[i, j].indexPos.y -= blankBlockCount;   // 발견한 빈 공간만큼 아래로 내린다.
                    blockAllInfo[i, j] = null;
                }
            }
            blankBlockCount = 0;
        }

        yield return new WaitUntil(() => { return IsBlockMoveAllEnd(); });
        // 블록 채우기
        StartCoroutine(FillPuzzleZoneCoroutine());

    }

    // 퍼즐구역 채우는 코루틴
    private IEnumerator FillPuzzleZoneCoroutine()
    {
        yield return new WaitUntil(() => { return (false == GameManager.Instance.isPause); });
        GameManager.Instance.SetGameState(EGameState.BLOCKMOVE);
        // 빈 공간을 매울 블록을 생성
        RefillPuzzleZone();

        // 빈 공간을 채우고나서 다 떨어질때까지 기다린다.
        yield return new WaitUntil(() => { return IsBlockMoveAllEnd(); });
        // 매칭 확인전 블럭 폭파 딜레이만큼 기다린다.
        yield return new WaitForSeconds(destroyBlockWaitTime);

        // 매치 확인
        while (true == IsMatchedBlock())
        {
            // 파괴한다.
            DestroyMatchedBlocks();
            
            yield return new WaitForSeconds(0.1f);
        }
        
        GameManager.Instance.SetGameState(EGameState.WAITINPUT);
    }

    private void RefillPuzzleZone()
    {
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                if (null == blockAllInfo[i,j])  // 날아간 빈 블록 자리가 있다면
                {
                    // 새 블록을 만들어서 채워준다.
                    Vector2 tempPosition = new Vector2(i, j + dropDownOffset);

                    GameObject newBlock = Instantiate(blockPrefab, tempPosition, Quaternion.identity, this.transform);

                    int[] asArray = blockPokemons.ToArray();
                    int pokemonNumber = asArray[Random.Range(0, asArray.Length)];

                    newBlock.GetComponent<Block>().Init(pokemonNumber, i, j);

                    blockAllInfo[i, j] = newBlock.GetComponent<Block>();
                }
            }
        }
    }

    // 전 블럭을 검사해서 하나라도 매칭된 것이 있는지 확인 (T/F)
    private bool IsMatchedBlock()
    {
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                if (null != blockAllInfo[i,j])
                {
                    if (true == blockAllInfo[i,j].GetIsMatched()) // 하나라도 매치되는 것이 있다면
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
