using UnityEngine;

public class ArmGenerator : MonoBehaviour
{
    [SerializeField] GameObject jointPrefab;
    [SerializeField] GameObject handPrefab;
    [SerializeField] GameObject arm_left;
    [SerializeField] GameObject arm_right;

    private GameObject shoulder_left;
    private GameObject elbow_left;
    private GameObject hand_left;

    private GameObject shoulder_right;
    private GameObject elbow_right;
    private GameObject hand_right;

    public float speed = 5;
    private float time = 0;
    public float countdownTime = 10;
    

    void Awake()
    {
        CreateArmHighLevel();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Action();
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            LeftArmUp();
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            LeftArmDown();
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            RightArmUp();
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            RightArmDown();
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Move("FORWARD");
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move("BACKWARDS");
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move("LEFT");
            countdownTime = 20;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move("RIGHT");
            countdownTime = 10;
        }
        

    }

    // Update is called once per frame
        void FixedUpdate()
    {
        time += Time.deltaTime;

        if (countdownTime > 0) // wait before stand by
        {
            countdownTime -= Time.deltaTime;
        }

        countdownTime -= Time.deltaTime;

        if (!Input.anyKey && countdownTime < 0)
        {
            //StandBy();
        }
    }

    void CreateArmHighLevel()
    {
        //left ARM

        shoulder_left = Instantiate(jointPrefab);
        elbow_left = Instantiate(jointPrefab);
        hand_left = Instantiate(handPrefab);

        hand_left.transform.parent = elbow_left.transform;
        elbow_left.transform.parent = shoulder_left.transform;
        shoulder_left.transform.parent = arm_left.transform;

        shoulder_left.transform.position = new Vector3(0, 0, 0);
        elbow_left.transform.position = new Vector3(0, -0.5f, 0);
        hand_left.transform.position = new Vector3(0, -1f, 0);

        shoulder_left.transform.position = arm_left.transform.position;

        //right ARM

        shoulder_right = Instantiate(jointPrefab);
        elbow_right = Instantiate(jointPrefab);
        hand_right = Instantiate(handPrefab);

        hand_right.transform.parent = elbow_right.transform;
        elbow_right.transform.parent = shoulder_right.transform;
        shoulder_right.transform.parent = arm_right.transform;

        shoulder_right.transform.position = new Vector3(0, 0, 0);
        elbow_right.transform.position = new Vector3(0, -0.5f, 0);
        hand_right.transform.position = new Vector3(0, -1f, 0);

        shoulder_right.transform.position = arm_right.transform.position;

        //shoulder left
        arm_left.transform.GetChild(0).transform.localRotation = Quaternion.Euler(-90, 0, 0);
        //shoulder left
        arm_right.transform.GetChild(0).transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    void StandBy()
    {
        //shoulder left
        arm_left.transform.GetChild(0).transform.localRotation =
            Quaternion.AngleAxis(20 * Mathf.Sin(speed * time), new Vector3(1, 0, 0));

        //elbow left
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(45 * Mathf.Sin(speed * time + Mathf.PI / 4), new Vector3(1, 0, 0));

        //hand left
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(90 * Mathf.Sin(speed * time + Mathf.PI / 4), new Vector3(1, 0, 0));

        //shoulder right
        arm_right.transform.GetChild(0).transform.localRotation =
            Quaternion.AngleAxis(5 * Mathf.Sin(speed * time), new Vector3(1, 0, 0));

        //elbow right
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(10 * Mathf.Sin(speed * time + Mathf.PI / 4), new Vector3(1, 0, 0));

        //hand right
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(180 * Mathf.Sin(speed * time + Mathf.PI / 4), new Vector3(0, 1, 0));
    }

    void Action() // on key E  is pressed
    {
        Debug.Log("E was pressed");


        //shoulder left
        arm_left.transform.GetChild(0).transform.localRotation = Quaternion.Euler(-90, 0, 0);
        //shoulder left
        arm_right.transform.GetChild(0).transform.localRotation = Quaternion.Euler(-90, 0, 0);

        //elbow left
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(5 * Mathf.Sin(10 * speed * time), new Vector3(1, 0, 0));
        //elbow left
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(5 * Mathf.Sin(10 * speed * time), new Vector3(1, 0, 0));

        //hand left
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(180 * Mathf.Sin(speed * time + Mathf.PI / 4), new Vector3(0, 1, 0));

        //hand right
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation =
            Quaternion.AngleAxis(180 * Mathf.Sin(speed * time + Mathf.PI / 4), new Vector3(0, 1, 0));
    }

    void LeftArmUp()
    {
        
    }

    void LeftArmDown()
    {

    }

    void RightArmUp()
    {
        
    }

    void RightArmDown()
    {

    }

    void Jump()
    {

    }

    void Move(string action)
    {
        switch (action)
        {
            case "FORWARD":
                break;
            case "BACKWARDS":
                break;
            case "LEFT":
                break;
            case "RIGHT":
                break;

        }
    }

    void WrongKey()
    {

    }

}

