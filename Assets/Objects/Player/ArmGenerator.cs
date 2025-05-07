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
            action();
        }
    }

    void CreateArmHighLevel()
    {
        //left ARM

        shoulder_left = Instantiate(jointPrefab);
        shoulder_left.transform.parent = arm_left.transform; // Set parent first
        shoulder_left.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Adjust local position
        shoulder_left.transform.localPosition = new Vector3(0, 0, 0); // Adjust local position

        elbow_left = Instantiate(jointPrefab);
        elbow_left.transform.parent = shoulder_left.transform; // Set parent first
        elbow_left.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Adjust local position
        elbow_left.transform.localPosition = new Vector3(0, -4.2f, 0); // Adjust local position
        elbow_left.transform.localScale = new Vector3(1f, 1f, 1f); // Adjust local position

        hand_left = Instantiate(handPrefab);
        hand_left.transform.parent = elbow_left.transform; // Set parent first
        hand_left.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Adjust local position
        hand_left.transform.localPosition = new Vector3(0, -4.2f, 0); // Adjust local position

        //right ARM
        shoulder_right = Instantiate(jointPrefab);
        shoulder_right.transform.parent = arm_right.transform; // Set parent first
        shoulder_right.transform.localRotation = Quaternion.Euler(320, 0, 0); // Adjust local position
        shoulder_right.transform.localPosition = new Vector3(0, 0, 0); // Adjust local position

        elbow_right = Instantiate(jointPrefab);
        elbow_right.transform.parent = shoulder_right.transform; // Set parent first
        elbow_right.transform.localRotation = Quaternion.Euler(320, 0, 0); // Adjust local position
        elbow_right.transform.localPosition = new Vector3(0, -4.2f, 0); // Adjust local position
        elbow_right.transform.localScale    = new Vector3(1f, 1f, 1f); // Adjust local position

        hand_right = Instantiate(handPrefab);
        hand_right.transform.parent = elbow_right.transform; // Set parent first
        hand_right.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Adjust local position
        hand_right.transform.localPosition = new Vector3(0, -4.2f, 0); // Adjust local position

        main_pose();
    }


    void action() // on key E  is pressed
    {
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

    public void main_pose()
    {
        //shoulder left
        arm_left.transform.GetChild(0).transform.localRotation = Quaternion.Euler(320, 0, 0);
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.localRotation = Quaternion.Euler(320, 0, 0);
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation = Quaternion.Euler(0, 180, 0);

        //shoulder left
        arm_right.transform.GetChild(0).transform.localRotation = Quaternion.Euler(320, 0, 0);
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.localRotation = Quaternion.Euler(320, 0, 0);
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    public void grab_pose()
    {
        //shoulder left
        arm_left.transform.GetChild(0).transform.localRotation = Quaternion.Euler(-90, 0, 0);
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.localRotation = Quaternion.Euler(-40, -15, 0);
        arm_left.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation = Quaternion.Euler(0, 200, 0);

        //shoulder left
        arm_right.transform.GetChild(0).transform.localRotation = Quaternion.Euler(-90, 0, 0);
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.localRotation = Quaternion.Euler(-40, 15, 0);
        arm_right.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.localRotation = Quaternion.Euler(0, -200, 0);
    }


}

