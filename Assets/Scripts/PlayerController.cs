using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject CameraBoundsCube;

    [SerializeField]
    public string ForwardsBackwardsMovementKey = "Vertical";

    [SerializeField]
    public string LeftRightMovementKey = "Horizontal";

    public float SpeedHorizontal = 10.0f;
    public float SpeedVertical = 10.0f;

    public float FallOffSpeed = 10.0f;

    private Vector3 startingPosition = new Vector3();

    private Vector3 leftOverVelocity = new Vector3();

    private Vector2 vMinBounds;
    private Vector2 vMaxBounds;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;

        Debug.Assert(CameraBoundsCube);
        if(CameraBoundsCube)
        {
            vMinBounds = new Vector2(CameraBoundsCube.transform.position.x - (CameraBoundsCube.transform.localScale.x / 2),
                                     CameraBoundsCube.transform.position.z - (CameraBoundsCube.transform.localScale.z / 2));
            vMaxBounds = new Vector2(CameraBoundsCube.transform.position.x + (CameraBoundsCube.transform.localScale.x / 2),
                                     CameraBoundsCube.transform.position.z + (CameraBoundsCube.transform.localScale.z / 2));
        }
        else
        {
            vMinBounds = new Vector2(-50f, -50f);
            vMinBounds = new Vector2(50f,50f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Service.Game.CurrentState == WerewolfGame.GameState.PlayerInvestigateDay
            || Service.Game.CurrentState == WerewolfGame.GameState.PlayerInvestigateNight)
        {
            if (Input.GetButton(ForwardsBackwardsMovementKey) || Input.GetButton(LeftRightMovementKey))
            {
                float vert = Input.GetAxis(ForwardsBackwardsMovementKey) * SpeedVertical;
                float hori = Input.GetAxis(LeftRightMovementKey) * SpeedHorizontal;

                leftOverVelocity += new Vector3(hori, 0, vert) * Time.deltaTime;

                //leftOverVelocity = new Vector3(hori, 0, vert);

                //vert *= Time.deltaTime;
                //hori *= Time.deltaTime;

                //transform.position += new Vector3(hori, 0, vert);
            }
            else
            {
                //transform.position += leftOverVelocity * Time.deltaTime;
                //leftOverVelocity = Vector3.Lerp(leftOverVelocity, Vector3.zero, 10 * Time.deltaTime);
            }

            if(transform.position.x <= vMinBounds.x)
            {
                if (leftOverVelocity.x < 0)
                {
                    leftOverVelocity.x = 0;
                }
            }
            else if (transform.position.x >= vMaxBounds.x)
            {
                if (leftOverVelocity.x > 0)
                {
                    leftOverVelocity.x = 0;
                }
            }

            if (transform.position.z <= vMinBounds.y)
            {
                if (leftOverVelocity.z < 0)
                {
                    leftOverVelocity.z = 0;
                }
            }
            else if (transform.position.z >= vMaxBounds.y)
            {
                if (leftOverVelocity.z > 0)
                {
                    leftOverVelocity.z = 0;
                }
            }



            transform.position += leftOverVelocity;
            leftOverVelocity = Vector3.Lerp(leftOverVelocity, Vector3.zero, FallOffSpeed * Time.deltaTime);

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.Log(hit.transform.gameObject.name);
                }
            }
        }
    }
}
