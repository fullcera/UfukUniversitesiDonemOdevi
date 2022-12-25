using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EKTemplate;
using DG.Tweening;
public class PlayerController : MonoBehaviour
{
    public Transform player;

    public float speedHorizontal = 5f;
    public float speed = 5f;

    public bool canMove = false;

    public Animator playerAnim;
    public Transform pizzaPoint;

    public Animator[] juries;
    public GameObject[] crosses;
    public GameObject[] tics;
    public GameObject[] winParticles;
    public GameObject[] loseParticles;
    public Transform camFinish;
    public Transform pizzaFinish;

    public Animator camAnim;

    private bool rightleft = false;

    private int i;

    public GameObject pizza;
    public GameObject tepsi;
    private int collectedWrongObjects;
    public ParticleSystem confetti;

    public List<Transform> pizzaPointList = new List<Transform>();

    #region Singleton
    public static PlayerController instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    void Start()
    {
        LevelManager.instance.startEvent.AddListener(OnGameStart);
    }

    public void OnGameStart()
    {
        i = 0;
        canMove = true;
        playerAnim.SetBool("Walk", true);
    }

    void Update()
    {
        if (canMove)
        {
            Move();
            if (InputManager.instance.input.x > 0f && Input.GetMouseButton(0))
            {
                transform.DOLocalRotate(new Vector3(0, transform.position.y + 80, 0), 0.4f).SetEase(Ease.Linear);
            }
            else
            {
                transform.DOLocalRotate(new Vector3(0, 0, 0f), 0.4f).SetEase(Ease.Linear);
            }
            if (InputManager.instance.input.x < 0f && Input.GetMouseButton(0))
            {
                transform.DOLocalRotate(new Vector3(0, transform.position.y - 50, 0f), 0.4f).SetEase(Ease.Linear);
            }
            else
            {
                transform.DOLocalRotate(new Vector3(0, 0, 0f), 0.4f).SetEase(Ease.Linear);
            }
        }
    }
    private void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        if (rightleft == false)
        {
            Vector3 temp = player.localPosition;
            temp.x += InputManager.instance.input.x * Time.deltaTime * speedHorizontal;
            temp.x = Mathf.Clamp(temp.x, -4.85f, 4.85f);
            player.localPosition = temp;
        }
    }

    IEnumerator Win_Sequence()
    {
        tepsi.transform.DOMove(pizzaFinish.position, 1.25f);
        CameraManager.instance.enabled = false;
        camAnim.enabled = true;
        camAnim.SetBool("Cam", true);
        confetti.Play();
        
        for (int i = 0; i < tics.Length; i++)
        {
            tics[i].SetActive(true);
        }

        for (int i = 0; i < winParticles.Length; i++)
        {
            winParticles[i].SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < juries.Length; i++)
        {
            juries[i].SetBool("Finish", true);
        }

        LevelManager.instance.Success();
    }
    IEnumerator Lose_Sequence()
    {
        tepsi.transform.DOMove(pizzaFinish.position, 1.25f);
        CameraManager.instance.enabled = false;
        camAnim.enabled = true;
        camAnim.SetBool("Cam", true);

        for (int i = 0; i < crosses.Length; i++)
        {
            crosses[i].SetActive(true);
        }

        for (int i = 0; i < loseParticles.Length; i++)
        {
            loseParticles[i].SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < juries.Length; i++)
        {
            juries[i].SetBool("Finish", true);
        }

        LevelManager.instance.Fail();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            speed = 0f;
            canMove = false;
            playerAnim.SetBool("Walk", false);
            if (i == 0)
            {
                StartCoroutine(Lose_Sequence());
            }
            if (collectedWrongObjects >= 3)
            {
                StartCoroutine(Lose_Sequence());
            }
            else
            {
                StartCoroutine(Win_Sequence());
            }
        }
        else if (other.CompareTag("Trap"))
        {
            canMove = false;
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.DOMoveZ(transform.localPosition.z - 6f, .35f).SetEase(Ease.Linear);
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(0.35f);
                canMove = true;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (other.CompareTag("Tomato"))
        {
            if (i < 16)
            {
                collectedWrongObjects++;
                other.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                rightleft = true;
                GameObject newTomato = Instantiate(Resources.Load("prefabs/TomatoPart"), pizzaPoint.position, Quaternion.Euler
                    (90, other.transform.rotation.y, other.transform.rotation.z)) as GameObject;
                newTomato.transform.DOMove(pizzaPointList[i].position, .06f).SetEase(Ease.Linear).OnComplete(() => {
                rightleft = false;
                newTomato.transform.SetParent(transform.GetChild(2).transform);
                    i++;
                });
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Cheese"))
        {
            if (i < 16)
            {
                other.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                rightleft = true;
                GameObject newTomato = Instantiate(Resources.Load("prefabs/CheesePart"), pizzaPoint.position, Quaternion.Euler
                    (90, other.transform.rotation.y, other.transform.rotation.z)) as GameObject;
                newTomato.transform.DOMove(pizzaPointList[i].position, .06f).SetEase(Ease.Linear).OnComplete(() => {
                rightleft = false;
                newTomato.transform.SetParent(transform.GetChild(2).transform);
                    i++;
                });
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Pepper"))
        {
            if (i < 16)
            {
                collectedWrongObjects++;
                other.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                rightleft = true;
                GameObject newTomato = Instantiate(Resources.Load("prefabs/PepperPart"), pizzaPoint.position, Quaternion.Euler
                    (90, other.transform.rotation.y, other.transform.rotation.z)) as GameObject;
                newTomato.transform.DOMove(pizzaPointList[i].position, .06f).SetEase(Ease.Linear).OnComplete(() => {
                rightleft = false;
                newTomato.transform.SetParent(transform.GetChild(2).transform);
                    i++;
                });
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Sucuk"))
        {
            if (i < 16)
            {
                other.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                rightleft = true;
                GameObject newTomato = Instantiate(Resources.Load("prefabs/SucukPart"), pizzaPoint.position, Quaternion.Euler
                    (other.transform.rotation.x, other.transform.rotation.y, other.transform.rotation.z)) as GameObject;
                newTomato.transform.DOMove(pizzaPointList[i].position, .06f).SetEase(Ease.Linear).OnComplete(() => {
                    rightleft = false;
                    newTomato.transform.SetParent(transform.GetChild(2).transform);
                    i++;
                });
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Mushroom"))
        {
            if (i < 16)
            {
                other.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                rightleft = true;
                GameObject newTomato = Instantiate(Resources.Load("prefabs/MushroomPart"), pizzaPoint.position, Quaternion.Euler
                    (other.transform.rotation.x, other.transform.rotation.y, other.transform.rotation.z)) as GameObject;
                newTomato.transform.DOMove(pizzaPointList[i].position, .06f).SetEase(Ease.Linear).OnComplete(() => {
                    rightleft = false;
                    newTomato.transform.SetParent(transform.GetChild(2).transform);
                    i++;
                });
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Olive"))
        {
            if (i < 16)
            {
                other.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                rightleft = true;
                GameObject newOlive = Instantiate(Resources.Load("prefabs/OlivePart"), pizzaPoint.position, Quaternion.Euler
                    (90, other.transform.rotation.y, other.transform.rotation.z)) as GameObject;
                newOlive.transform.DOMove(pizzaPointList[i].position, .06f).SetEase(Ease.Linear).OnComplete(() => {
                    rightleft = false;
                    newOlive.transform.SetParent(transform.GetChild(2).transform);
                    i++;
                });
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Nutella"))
        {
            if (i < 16)
            {
                collectedWrongObjects++;
                other.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                pizza.GetComponent<Renderer>().material.DOColor(new Color(0.245283f, 0.1411534f, 0.08908863f, 1), .6f);
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
