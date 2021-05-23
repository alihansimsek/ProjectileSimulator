using UnityEngine;
 
public class Jump : MonoBehaviour
{
    private float animationSpeed = 1;
    private float jumpHeight = 1;
    private float maxHeight = 1;
    public int destroyCountdown = 10;
    public float bounceTreshold = 1; //smaller values means more bounce
    public float weightiness = 2; //it must be above 1 for object to ever fall after a bounce

    /// true if the gameObject is jumping
    private bool move;
    private float time;
    private float destroyTimer = 0.0f;
    private float floorOffset = 0.5f;
    private Vector3 diff, origin, destiny, speed;
    private SceneController sceneController;

    private void Start()
    {
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
    }
    private void Update()
    {
        //movement
        if (move)
        {
            time += Time.deltaTime * animationSpeed;
            transform.position = new Vector3(origin.x + speed.x * time, origin.y + speed.y * time + 0.5f * Physics.gravity.y * Mathf.Pow(time, 2), origin.z + speed.z * time);
 
            //when we pass the objective point
            if ((diff.x > 0 && transform.position.x >= destiny.x) || (diff.x < 0 && transform.position.x <= destiny.x))
            {
                //reset time and stop moving
                time = 0;
                move = false;
                if ((maxHeight/weightiness) >= bounceTreshold)
                {
                    Vector3 newDestination = new Vector3(destiny.x + diff.x/weightiness , 0 , destiny.z + diff.z/weightiness );
                    JumpToDestiantion(newDestination, animationSpeed, jumpHeight / weightiness);
                }
            }
        }
        if(destroyTimer > destroyCountdown)
        {
            gameObject.SetActive(false);
            destroyTimer = 0.0f;
            sceneController.returnToPool(gameObject);
        }
        destroyTimer += Time.deltaTime;
    }
 
    public void JumpToDestiantion(Vector3 destination, float animSpeed, float jHeight)
    {
        animationSpeed = animSpeed;
        jumpHeight = jHeight;
        time = 0;
        move = true;
        origin = transform.position;
        destiny = new Vector3(destination.x, destination.y + floorOffset, destination.z);
        diff = destiny - origin;

        CalculateSpeed();

    }
    private void CalculateSpeed()
    {
        maxHeight = diff.y > 0 ? diff.y + jumpHeight : jumpHeight;
 
        var speedY = Mathf.Sqrt(-2 * Physics.gravity.y * maxHeight);
        speed = new Vector3
        (
            diff.x * Physics.gravity.y / (-speedY - Mathf.Sqrt(Mathf.Abs(Mathf.Pow(speedY, 2) + 2 * Physics.gravity.y * diff.y))),
            speedY, diff.z * Physics.gravity.y / (-speedY - Mathf.Sqrt(Mathf.Abs(Mathf.Pow(speedY, 2) + 2 * Physics.gravity.y * diff.y)))
        );
    }
}