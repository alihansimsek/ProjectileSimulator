using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private Stack<GameObject> ballPool;
    public GameObject ball;
    public GameObject spawnPoint;
    public Slider speedSlider;
    public Slider heightSlider;

    private Vector3 destination = new Vector3(0,0.5f,0);
    private float animationSpeed = 1;
    private float jumpHeight = 1;
    private Transform target;
    private bool spawnPointSet = false;
    private int poolLimit = 16;
    private int bonusBallLimit = 8;
    private int bonusBallInUse = 0;

    // Start is called before the first frame update
    void Start()
    {
        ballPool = new Stack<GameObject>();
        fillPool();
        Debug.Log(ballPool);
        target = transform.Find("Target");
        animationSpeed = speedSlider.value;
        jumpHeight = heightSlider.value;
        heightSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        speedSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("point: " + hit.point + ", collider: " + hit.collider);
                    destination = hit.point;

                if (spawnPointSet)
                {
                    if (ballPool.Count > 0)
                    {
                        GameObject newBall = ballPool.Pop();
                        newBall.transform.position = spawnPoint.transform.position;
                        newBall.SetActive(true);
                        target.transform.position = new Vector3(destination.x, target.transform.position.y, destination.z);
                        newBall.GetComponent<Jump>().JumpToDestiantion(destination, animationSpeed, jumpHeight);
                        spawnPointSet = false;
                        bonusBallInUse = 0;
                    }
                    else if(bonusBallInUse < bonusBallLimit)
                    {
                        bonusBallInUse++;
                        GameObject newBall = Instantiate<GameObject>(ball, spawnPoint.transform.position, Quaternion.identity);
                        target.transform.position = new Vector3(destination.x, target.transform.position.y, destination.z);
                        newBall.GetComponent<Jump>().JumpToDestiantion(destination, animationSpeed, jumpHeight);
                        spawnPointSet = false;
                    }

                }

                else
                {
                    spawnPoint.transform.position = destination;
                    spawnPointSet = true;
                }
            }
        }
    }
    public void ValueChangeCheck()
    {
        animationSpeed = speedSlider.value;
        jumpHeight = heightSlider.value;
    }

    private void fillPool()
    {
        for(int i = 0; i < poolLimit; i++)
        {
            GameObject newBall = Instantiate(ball);
            newBall.SetActive(false);
            ballPool.Push(newBall);
        }
    }

    public void returnToPool(GameObject ball)
    {
        if(ballPool.Count < poolLimit)
        {
            ballPool.Push(ball);
        }
        else
        {
            Destroy(ball);
            bonusBallInUse--;
        }
    }
}
