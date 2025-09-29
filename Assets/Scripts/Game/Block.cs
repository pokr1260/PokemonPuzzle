using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;

public class Block : ExtendedBehavior
{
    [Header("AssetLoad Info")]
    public string assetBundleName;

    public string fileName;

    [Header("PokemonSetting")]
    // 포켓몬 도감번호, 이 번호로 어떤 포켓몬인지 정해진다.
    [Tooltip("포켓몬 도감번호")]
    [Range(1, 151)]
    public int pokemonNumber;  // 0은 미싱노

    [Header("Block Info")]
    [Tooltip("현재 블록 좌표 위치(Index)")]
    public Vector2Int indexPos;

    [Tooltip("상하 좌우로 미는 민감도, 숫자가 낮을수록 민감하다")]
    [Range(0,1)]
    public float swipeSensitive = 0.2f;

    [Tooltip("블록을 상하좌우로 움직일때의 속도값")]
    public float moveSpeed = 10.0f;

    [Tooltip("매치 블록 인식 기다리는 시간")]
    public float waitMatchedBlock = 0.25f;

    [Tooltip("블록의 마지막 이동의 smooth값 (작을수록 부드러움)")]
    [Range(0.001f,1.0f)]
    public float moveSmooth = 0.01f;

    // 매치가 되었다면 true / 매치가 이뤄지지 않았다면 false
    [SerializeField]
    [Tooltip("매치가 되었다면 true / 매치가 이뤄지지 않았다면 false")]
    private bool isMatched = false;

    private bool isMoveXEnd = false;
    private bool isMoveYEnd = false;

    //private static WWW loader;

    private Vector2 touchBeginePosition;    // 터치 시작 위치
    private Vector2 touchEndPosition;       // 터치 떼는 위치

    private float touchAngle = 0.0f;        // 터치 시작위치와 떼는 위치의 각도

    private static PuzzleZone puzzleZone;
    private Block changePositionBlock;      // 자리 교환할 블록

    private Vector2 moveTargetPos;          // 이동할 타겟좌표
    private Vector2 moveTempPos;            // 이동임시좌표
    private Vector2Int prevIndexPos;        // 이전 블록 좌표

    private Vector2 blockSize;              // 블록의 크기

    private bool isCallMouseDown;           // T : Down이 call 되었을 경우

    private void Awake()
    {
        if (null == puzzleZone)
        {
            puzzleZone = FindObjectOfType<PuzzleZone>();
        }

        isCallMouseDown = false;
    }

    public void Init(int pokemonNumber , int x, int y)
    {
        this.pokemonNumber = pokemonNumber;
        indexPos.x = x;
        indexPos.y = y;

        //transform.position = new Vector2(x, y);
        transform.localPosition = new Vector2(x, y + puzzleZone.dropDownOffset);

        InitSprite();
    }

    public void Init(int pokemonNumber, Vector2Int indexPos)
    {
        Init(pokemonNumber, indexPos.x, indexPos.y);
    }
    
    void InitSprite()
    {
        SpriteAtlas atlas = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(assetBundleName, fileName) as SpriteAtlas;
        if (null == atlas)
        {
            Debug.LogError("Block.cs / atlas is not found");
            return;
        }
        Sprite sprite = atlas.GetSprite(pokemonNumber.ToString("D3"));

        GetComponent<SpriteRenderer>().sprite = sprite;
        blockSize = GetComponent<SpriteRenderer>().size;    // 블록 사이즈 저장.
    }


    private void Update()
    {
        if (true == isMatched)
        {
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        }

        if (false == GameManager.Instance.isPause)
        {
            Move();
        }
    }

    private void Move()
    {
        // 타겟의 정보를 가져온다.
        moveTargetPos = indexPos;

        // 가로로 이동할 타겟과 현재 위치의 거리가 0.1f 이상인 경우 이동시킨다.
        if (Mathf.Abs(moveTargetPos.x - transform.localPosition.x) > 0.01f)
        {
            // 가로 이동이므로..
            moveTempPos = new Vector2(moveTargetPos.x, transform.localPosition.y);
            transform.localPosition = Vector2.Lerp(transform.localPosition, moveTempPos, moveSpeed * Time.deltaTime);
            isMoveXEnd = false;

            puzzleZone.FindAllMatches();
        }
        else    // 0.1f 이하의 경우 증감이 아닌 그냥 position을 set한다.
        {
            transform.localPosition = new Vector2(moveTargetPos.x, transform.localPosition.y);
            isMoveXEnd = true;

            puzzleZone.blockAllInfo[indexPos.x, indexPos.y] = this;
        }

        // 세로로 이동할 타겟과 현재 위치의 거리가 0.1f 이상인 경우 이동시킨다.
        if (Mathf.Abs(moveTargetPos.y - transform.localPosition.y) > 0.01f)
        {
            moveTempPos = new Vector2(transform.localPosition.x, moveTargetPos.y);
            transform.localPosition = Vector2.Lerp(transform.localPosition, moveTempPos, moveSpeed * Time.deltaTime);
            isMoveYEnd = false;

            puzzleZone.FindAllMatches();
        }
        else    // 0.1f 이하의 경우 증감이 아닌 그냥 position을 set한다.
        {
            transform.localPosition = new Vector2(transform.localPosition.x, moveTargetPos.y);
            isMoveYEnd = true;

            puzzleZone.blockAllInfo[indexPos.x, indexPos.y] = this;
        }
    }

    public bool IsMoveEnd()
    {
        return (true == isMoveXEnd && true == isMoveYEnd);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.GetGameState() == EGameState.WAITINPUT && false == isCallMouseDown)
        {
            //Debug.Log("Mouse Down : " + pokemonNumber.ToString());
            //Debug.Log("Mouse Down Click Pos : " + indexPos);

            touchBeginePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Mouse Down Input mousePosition : " + Input.mousePosition);
            Debug.Log("Mouse Down Click Pos : " + touchBeginePosition.ToString());
            isCallMouseDown = true;
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.GetGameState() == EGameState.WAITINPUT && true == isCallMouseDown)
        {
            //Debug.Log("Mouse Up : " + pokemonNumber.ToString());
            //Debug.Log("Mouse Up Click Pos : " + indexPos);

            touchEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Mouse Up Input mousePosition : " + Input.mousePosition);
            Debug.Log("Mouse Up Click Pos : " + touchEndPosition.ToString());
            CalculateTouchFromToAngle();
            isCallMouseDown = false;
        }
    }

    /*             90도
                    a
               135  ^  45
                 \  |  /
                  \ | /
                   \|/    
    180도 d --------|--------> b 0도
                   /|\
                  / | \
                 /  |  \
             -135   c  -45
                   -90도

       a , b, c, d 구역으로 나누어서 상하좌우 이동 범위를 정한다.

        a ( 45도 ~ 135도)   : 위
        b ( 45도 ~ -45도)   : 우측
        c (-45도 ~ -135도) : 아래
        d (135도 ~ -135도) : 왼쪽
    */

    void CalculateTouchFromToAngle()
    {
        // 좌우 이동거리가 swipeSensitive 이상일 경우 || 상하 이동거리가 swipeSensitive 이상일 경우
        if ( swipeSensitive < Mathf.Abs(touchBeginePosition.x - touchEndPosition.x) || swipeSensitive < Mathf.Abs(touchBeginePosition.y - touchEndPosition.y))
        {
            touchAngle = Mathf.Atan2(touchEndPosition.y - touchBeginePosition.y, touchEndPosition.x - touchBeginePosition.x) * Mathf.Rad2Deg;
            Debug.Log("touchAngle : " + touchAngle.ToString());
            MoveBlock();
            GameManager.Instance.SetGameState(EGameState.BLOCKMOVE);
        }
        else
        {
            GameManager.Instance.SetGameState(EGameState.WAITINPUT);
        }
    }

    void MoveBlock()
    {
        // (위의 주석에 나온 각도 범위 && 이동할 방향에서 밖으로 벗어나지 않을 조건)
        // 위
        if ((45 < touchAngle && touchAngle <= 135) && puzzleZone.puzzleHeight - 1 > indexPos.y)
        {
            // 블록이 매치되지 않을경우를 대비해 이전 좌표를 저장해둔다.
            prevIndexPos = indexPos;

            // 바꿀 대상의 블록을 가져온다.
            changePositionBlock = puzzleZone.blockAllInfo[indexPos.x, indexPos.y + 1];

            indexPos.y++;               // 처음터치한 블록 이동
            changePositionBlock.indexPos.y--;   // 처음 터치한 블록과 이동할 블록의 위치 변경

            Debug.Log("위로 이동");
            StartCoroutine(BlockMoveCheckCoroutine());
        }
        // 우측
        else if ((-45 < touchAngle && touchAngle <= 45) && puzzleZone.puzzleWidth - 1 > indexPos.x)
        {
            // 블록이 매치되지 않을경우를 대비해 이전 좌표를 저장해둔다.
            prevIndexPos = indexPos;

            // 바꿀 대상의 블록을 가져온다.
            changePositionBlock = puzzleZone.blockAllInfo[indexPos.x + 1, indexPos.y];

            indexPos.x++;
            changePositionBlock.indexPos.x--;

            Debug.Log("오른쪽 이동");
            StartCoroutine(BlockMoveCheckCoroutine());
        }
        // 아래
        else if ((-135 <= touchAngle && touchAngle < -45) && indexPos.y > 0)
        {
            // 블록이 매치되지 않을경우를 대비해 이전 좌표를 저장해둔다.
            prevIndexPos = indexPos;

            // 바꿀 대상의 블록을 가져온다.
            changePositionBlock = puzzleZone.blockAllInfo[indexPos.x, indexPos.y - 1];

            indexPos.y--;
            changePositionBlock.indexPos.y++;

            Debug.Log("아래 이동");
            StartCoroutine(BlockMoveCheckCoroutine());
        }
        // 왼쪽
        else if ((135 < touchAngle || touchAngle <= -135) && indexPos.x > 0)
        {
            // 블록이 매치되지 않을경우를 대비해 이전 좌표를 저장해둔다.
            prevIndexPos = indexPos;

            // 바꿀 대상의 블록을 가져온다.
            changePositionBlock = puzzleZone.blockAllInfo[indexPos.x - 1, indexPos.y];

            indexPos.x--;
            changePositionBlock.indexPos.x++;

            Debug.Log("왼쪽으로 이동");
            StartCoroutine(BlockMoveCheckCoroutine());
        }
        else
        {
            //Debug.Log("ERRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");
            //puzzleZone.gameState = GameState.eINPUTWAIT;
            //Debug.Log("MoveBlock / GameState : " + puzzleZone.gameState.ToString());
        }
    }

    // 블록 이동후 매치가 되어있으면 제거 매치가 되어있지 않으면 되돌아간다.
    private IEnumerator BlockMoveCheckCoroutine()
    {
        // 모든 Update가 한번 돌때까지 기다린뒤 ( Update에서 target을 업데이트 하기 때문 )
        yield return new WaitForEndOfFrame();
        // 타겟까지의 거리가 0일경우가 될때까지 기다린다.
        yield return new WaitUntil(() => { return IsMoveEnd(); });

        yield return new WaitForSeconds(waitMatchedBlock);

        if (null != changePositionBlock)
        {
            // 매치가 되지 않을 경우 원래대로 돌아간다.
            if (false == isMatched && false == changePositionBlock.isMatched)
            {
                changePositionBlock.indexPos = this.indexPos;
                indexPos = prevIndexPos;

                //yield return new WaitForEndOfFrame();
                //// 타겟까지의 거리가 0일경우가 될때까지 기다린다.
                //yield return new WaitUntil(() => { return IsMoveEnd(); });
                //
                //yield return new WaitForSeconds(0.2f);

                GameManager.Instance.SetGameState(EGameState.WAITINPUT);
            }
            else    // 매치가 된 경우
            {
                // 매치된 퍼즐 제거
                puzzleZone.DestroyMatchedBlocks();
            }
            changePositionBlock = null;
        }
    }

    public void SetIsMatched(bool value)
    {
        if (true == value)
        {
            isMatched = true;
            
        }
        else
        {
            isMatched = false;
        }
    }

    public bool GetIsMatched()
    {
        return isMatched;
    }

}
