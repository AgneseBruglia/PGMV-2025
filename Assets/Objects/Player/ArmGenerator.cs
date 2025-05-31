using UnityEngine;
using System.Collections;
using System.Collections.Generic;



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

    public float duration = 1f;

    public enum ArmSide { Left, Right }
    public enum ArmAction { Raise, Lower }


    private Dictionary<(ArmSide, ArmAction), bool> isMoving = new();


    public class TargetPosition
    {
        public Quaternion targetShoulderRot;
        public Quaternion targetUpperArmRot;
        public Quaternion targetLowerArmRot;

        public TargetPosition(Quaternion targetShoulderRot, Quaternion targetUpperArmRot, Quaternion targetLowerArmRot)
        {
            this.targetShoulderRot = targetShoulderRot;
            this.targetUpperArmRot = targetUpperArmRot;
            this.targetLowerArmRot = targetLowerArmRot;
        }
    }


    void Awake()
    {
        CreateArmHighLevel();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoving[(ArmSide.Left, ArmAction.Raise)] = false;
        isMoving[(ArmSide.Left, ArmAction.Lower)] = false;
        isMoving[(ArmSide.Right, ArmAction.Raise)] = false;
        isMoving[(ArmSide.Right, ArmAction.Lower)] = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && (!isMoving[(ArmSide.Left, ArmAction.Lower)] || !isMoving[(ArmSide.Right, ArmAction.Lower)])) // to main pose
        {
            StartCoroutine(
                moveArm(
                    arm_left,
                    ArmSide.Left,
                    ArmAction.Lower,
                    new TargetPosition(
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(0, 180, 0)
                    )
                )
            );

            StartCoroutine(
                moveArm(
                    arm_right,
                    ArmSide.Right,
                    ArmAction.Lower,
                    new TargetPosition(
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(0, 180, 0)
                    )
                )
            );
        }

        if (Input.GetKeyDown(KeyCode.I) && !isMoving[(ArmSide.Left, ArmAction.Raise)])
        {
            StartCoroutine(
                moveArm(
                    arm_left,
                    ArmSide.Left,
                    ArmAction.Raise,
                    new TargetPosition(
                        Quaternion.Euler(-90, 0, 0),
                        Quaternion.Euler(-40, -15, 0),
                        Quaternion.Euler(0, 200, 0)
                    )
                )
            );
        }

        if (Input.GetKeyDown(KeyCode.K) && !isMoving[(ArmSide.Left, ArmAction.Lower)])
        {
            StartCoroutine(
                moveArm(
                    arm_left,
                    ArmSide.Left,
                    ArmAction.Lower,
                    new TargetPosition(
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(0, 180, 0)
                    )
                )
            );
        }

        if (Input.GetKeyDown(KeyCode.O) && !isMoving[(ArmSide.Right, ArmAction.Raise)])
        {
            StartCoroutine(
                moveArm(
                    arm_right,
                    ArmSide.Right,
                    ArmAction.Raise,
                    new TargetPosition(
                        Quaternion.Euler(-90, 0, 0),
                        Quaternion.Euler(-40, -15, 0),
                        Quaternion.Euler(0, 200, 0)
                    )
                )
            );
        }

        if (Input.GetKeyDown(KeyCode.L) && !isMoving[(ArmSide.Right, ArmAction.Lower)])
        {
            StartCoroutine(
                moveArm(
                    arm_right,
                    ArmSide.Right,
                    ArmAction.Lower,
                    new TargetPosition(
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(320, 0, 0),
                        Quaternion.Euler(0, 180, 0)
                    )
                )
            );
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

    IEnumerator moveArm(GameObject arm, ArmSide side, ArmAction action, TargetPosition target)
    {
        Transform shoulder = arm.transform.GetChild(0);
        Transform upperArm = shoulder.GetChild(1);
        Transform lowerArm = upperArm.GetChild(1);

        // if already in position, do nothing
        if (
            shoulder.transform.localRotation == target.targetShoulderRot
            && upperArm.transform.localRotation == target.targetUpperArmRot
            && lowerArm.transform.localRotation == target.targetLowerArmRot
        )
        {
            isMoving[(side, action)] = false;
            yield break;
        }

        isMoving[(side, action)] = true;

        AudioSource audio = arm.GetComponent<AudioSource>();
        audio.loop = true;
        audio.Play();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            shoulder.localRotation = Quaternion.Slerp(shoulder.transform.localRotation, target.targetShoulderRot, t);
            upperArm.localRotation = Quaternion.Slerp(upperArm.transform.localRotation, target.targetUpperArmRot, t);
            lowerArm.localRotation = Quaternion.Slerp(lowerArm.transform.localRotation, target.targetLowerArmRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation is exact
        shoulder.localRotation = target.targetShoulderRot;
        upperArm.localRotation = target.targetUpperArmRot;
        lowerArm.localRotation = target.targetLowerArmRot;

        audio.loop = false;
        audio.GetComponent<AudioSource>().Stop();

        isMoving[(side, action)] = false;
    }

    public void main_pose()
    {
        StartCoroutine(
            moveArm(
                arm_left,
                ArmSide.Left,
                ArmAction.Lower,
                new TargetPosition(
                    Quaternion.Euler(320, 0, 0),
                    Quaternion.Euler(320, 0, 0),
                    Quaternion.Euler(0, 180, 0)
                )
            )
        );

        StartCoroutine(
            moveArm(
                arm_right,
                ArmSide.Right,
                ArmAction.Lower,
                new TargetPosition(
                    Quaternion.Euler(320, 0, 0),
                    Quaternion.Euler(320, 0, 0),
                    Quaternion.Euler(0, 180, 0)
                )
            )
        );
    }

    public void grab_pose()
    {
        StartCoroutine(
            moveArm(
                arm_left,
                ArmSide.Left,
                ArmAction.Raise,
                new TargetPosition(
                    Quaternion.Euler(-90, 0, 0),
                    Quaternion.Euler(-40, 15, 0),
                    Quaternion.Euler(0, 200, 0)
                )
            )
        );

        StartCoroutine(
            moveArm(
                arm_right,
                ArmSide.Right,
                ArmAction.Raise,
                new TargetPosition(
                    Quaternion.Euler(-90, 0, 0),
                    Quaternion.Euler(-40, 15, 0),
                    Quaternion.Euler(0, -200, 0)
                )
            )
        );
    }


}

