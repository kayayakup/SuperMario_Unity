using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemyAI : MonoBehaviour
{
    public float gravity;
    public Vector2 velocity;
    public bool isWalkingLeft = true;

    public LayerMask floorMask;
    public LayerMask wallMask;

    private bool shouldDie = false;
    private float deathTimer = 0;

    public float timeBeforeDestroy = 1.0f;

    private bool grounded = false;

    private enum EnemyState
    {
        walking,
        falling,
        dead
    }

    private EnemyState state = EnemyState.falling;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        Fall();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyPosition();

        CheckCrushed();
    }

    public void Crush()
    {
        state = EnemyState.dead;
        GetComponent<Animator>().SetBool("isCrushed", true);
        GetComponent<Collider2D>().enabled = false;
        shouldDie = true;
    }

    void CheckCrushed()
    {
        if (shouldDie)
        {
            if (deathTimer <= timeBeforeDestroy)
            {
                deathTimer += Time.deltaTime;
            }
            else
            {
                shouldDie = false;
                Destroy(this.gameObject);
            }
        }
    }

    void UpdateEnemyPosition()
    {
        if (state != EnemyState.dead)
        {
            Vector3 pos = transform.localPosition;
            Vector3 scale = transform.localScale;

            if (state == EnemyState.falling)
            {
                pos.y += velocity.y * Time.deltaTime;
                velocity.y -= gravity * Time.deltaTime;
            }

            if (state == EnemyState.walking)
            {
                if (isWalkingLeft)
                {
                    pos.x -= velocity.x * Time.deltaTime;
                    scale.x = -1;
                }
                else{
                    pos.x += velocity.x * Time.deltaTime;
                    scale.x = 1;
                }
            }

            if (velocity.y <= 0)
            {
                pos = CheckGround(pos);
            }

            CheckWalls(pos, scale.x);

            transform.localPosition = pos;
            transform.localScale = scale;
        }
    }

    Vector3 CheckGround(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y - .5f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y - .5f);
        Vector2 originRight = new Vector2(pos.x + 0.5f - 0.2f, pos.y - .5f);

        RaycastHit2D groundLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D groundMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D groundRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        if (groundLeft.collider != null || groundMiddle.collider != null || groundRight.collider != null)
        {
            RaycastHit2D hitRay = groundLeft;
            if (groundLeft)
            {
                hitRay = groundLeft;
            }
            else if (groundMiddle)
            {
                hitRay = groundMiddle;
            }
            else if (groundRight)
            {
                hitRay = groundRight;
            }

            if (hitRay.collider.tag == "Player")
            {
                SceneManager.LoadScene("GameOver");
            }

            pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + .5f;
            grounded = true;
            velocity.y = 0;
            state = EnemyState.walking;
        }
        else
        {
            if (state != EnemyState.falling)
            {
                Fall();
            }
        }
        return pos;
    }

    void CheckWalls(Vector3 pos, float direction)
    {
        Vector2 origintop = new Vector2(pos.x + direction * 0.4f, pos.y + .5f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * 0.4f, pos.y);
        Vector2 originButtom = new Vector2(pos.x + direction * 0.4f, pos.y - .5f + 0.2f);

        RaycastHit2D wallTop = Physics2D.Raycast(origintop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(originButtom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        if (wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null)
        {
            RaycastHit2D hitRay = wallTop;
            if (wallTop)
            {
                hitRay = wallTop;
            }
            else if (wallMiddle)
            {
                hitRay = wallMiddle;
            }
            else if (wallBottom)
            {
                hitRay = wallBottom;
            }

            if (hitRay.collider.tag == "Player")
            {
                SceneManager.LoadScene("GameOver");
            }
            isWalkingLeft = !isWalkingLeft;
        }
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    void Fall()
    {
        velocity.y = 0;
        state = EnemyState.falling;
        grounded = false;

    }
}
