using UnityEngine;
using System.Collections;

public class OniGroupControl : MonoBehaviour {

    public PlayerControl player;

    public GameSceneControl gameSceneControl;

    public GameObject[] oniPrefabs;

    public AudioClip[] defeatLevel1;
    public AudioClip[] defeatLevel2;
    public AudioClip[] defeatLevel3;

    public OniControl[] onis;

    public static float collisionSize = 2.0f;

    public const float SpeedMin = 2.0f;
    public const float SpeedMax = 10.0f;
    public const float LeaveSpeed = 10.0f;

    private int oniNum;
    private static int oniNumMax = 0;
    public float runSpeed = SpeedMin;
    public bool isPlayerHitted = false;

    public enum OniState
    {
        Normal = 0,
        Decelerate,
        Leave
    }

    public OniState state = OniState.Normal;

    public struct Decelerate
    {
        public bool isActvie;
        public float speedBase;
        public float timer;
    }

    public Decelerate decelerate;

    // Use this for initialization
    void Start () {
        decelerate.isActvie = false;
        decelerate.timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        SpeedControl();

        transform.rotation = Quaternion.identity;

        if(state == OniState.Leave)
        {
            // remove from scene when all onis are not visible
            bool isVisible = false;
            foreach(var oni in onis)
            {
                if (oni.GetComponent<Renderer>().isVisible)
                {
                    isVisible = true;
                    break;
                }
            }
            if (!isVisible)
                Destroy(gameObject);
        }

        transform.Translate(runSpeed * Time.deltaTime, 0, 0);
    }

    private void SpeedControl()
    {
        switch (state)
        {
            case OniState.Decelerate:
                {
                    const float decelerateStartDistance = 8.0f;

                    if (decelerate.isActvie)
                    {
                        float rate = 0.0f;
                        const float time0 = 0.7f;
                        const float time1 = 0.4f;
                        const float time2 = 2.0f;

                        const float speedMax = 30.0f;
                        const float speedMin = SpeedMin;

                        float time = decelerate.timer;

                        // Accelerate
                        if (time < time0)
                        {
                            rate = Mathf.Clamp01(time / time0);
                            rate = (Mathf.Sin(Mathf.Lerp(-Mathf.PI / 2.0f, Mathf.PI / 2.0f, rate)) + 1.0f) / 2.0f;

                            runSpeed = Mathf.Lerp(decelerate.speedBase, speedMax, rate);

                            SetOniMotionSpeed(2.0f);
                        }
                        else
                            time -= time0;

                        // decelerate to the same speed with player
                        if (time < time1)
                        {
                            rate = Mathf.Clamp01(time / time1);
                            rate = (Mathf.Sin(Mathf.Lerp(-Mathf.PI / 2.0f, Mathf.PI / 2.0f, rate)) + 1.0f) / 2.0f;

                            runSpeed = Mathf.Lerp(speedMax, PlayerControl.RunSpeedMax, rate);
                        }
                        else
                            time -= time1;

                        // decelerate to a very low speed
                        if (time < time2)
                        {
                            rate = Mathf.Clamp01(time / time2);
                            rate = (Mathf.Sin(Mathf.Lerp(-Mathf.PI / 2.0f, Mathf.PI / 2.0f, rate)) + 1.0f) / 2.0f;

                            runSpeed = Mathf.Lerp(PlayerControl.RunSpeedMax, speedMin, rate);

                            SetOniMotionSpeed(1.0f);
                        }
                        else
                        {
                            time -= time2;
                            runSpeed = speedMin;
                        }

                        decelerate.timer += Time.deltaTime;
                    }
                    else
                    {
                        float distance = transform.position.x - player.transform.position.x;
                        if(distance < decelerateStartDistance)
                        {
                            decelerate.isActvie = true;
                            decelerate.speedBase = runSpeed;
                            decelerate.timer = 0.0f;
                        }
                    }
                    break;
                }
            case OniState.Leave:
                {
                    runSpeed = LeaveSpeed + player.runSpeed;
                    break;
                }
        }
    }

    public void CreateOnis(int oniNum, Vector3 basePosition)
    {
        this.oniNum = oniNum;
        oniNumMax = Mathf.Max(oniNumMax, oniNum);

        onis = new OniControl[oniNum];
        Vector3 position;

        for (int i = 0; i < oniNum; i++)
        {
            var go = Instantiate<GameObject>(oniPrefabs[i % oniPrefabs.Length]);
            onis[i] = go.GetComponent<OniControl>();
            position = basePosition;

            if (i != 0)
            {
                Vector3 splatRange;
                splatRange.x = Mathf.Min(OniControl.collisionSize * (oniNum - 1),
                                         OniGroupControl.collisionSize);
                splatRange.z = splatRange.x / 2.0f;

                position.x += Random.Range(0.0f, splatRange.x);
                position.z += Random.Range(-splatRange.z, splatRange.z);
            }
            position.y = 0;

            onis[i].transform.position = position;
            onis[i].transform.parent = transform;

            onis[i].player = player;
            onis[i].waveAmplitude = (i + 1) * 0.1f;
            onis[i].waveAngleOffset = (i + 1) * Mathf.PI / 4.0f;
        }
    }

    private static int count;
    public void OnAttackFromPlayer()
    {
        gameSceneControl.AddDefeatNum(oniNum);

        Vector3 blowout = Vector3.right;
        Vector3 blowoutUp = Vector3.up;
        Vector3 blowoutXZ = Vector3.zero;

        float yAngle = 0.0f;
        float blowoutSpeed = 0.0f;
        float blowoutSpeedBase = 0.0f;
        float forwardBackAngle = 0.0f;
        float baseRadius = 1.0f;
        float yAngleCenter = 0.0f;
        float yAngleSwing = 0.0f;
        float arcLength = 0.0f;

        switch (gameSceneControl.evaluation)
        {
            default:
            case GameSceneControl.Evaluation.Okay:
                {
                    baseRadius = 0.3f;
                    blowoutSpeedBase = 10.0f;
                    forwardBackAngle = 40.0f;
                    yAngleCenter = 180.0f;
                    yAngleSwing = 10.0f;
                    break;
                }
            case GameSceneControl.Evaluation.Good:
                {
                    baseRadius = 0.3f;
                    blowoutSpeedBase = 10.0f;
                    forwardBackAngle = 0.0f;
                    yAngleCenter = 0.0f;
                    yAngleSwing = 60.0f;
                    break;
                }
            case GameSceneControl.Evaluation.Great:
                {
                    baseRadius = 0.5f;
                    blowoutSpeedBase = 15.0f;
                    forwardBackAngle = -20.0f;
                    yAngleCenter = 0.0f;
                    yAngleSwing = 30.0f;
                    break;
                }
        }

        forwardBackAngle += Random.Range(-5.0f, 5.0f);
        arcLength = (onis.Length - 1) * 30.0f;
        arcLength = Mathf.Min(arcLength, 120.0f);

        yAngle = yAngleCenter;
        yAngle += -arcLength / 2.0f;
        if (player.attackMotion == PlayerControl.AttackMotion.Right)
            yAngle += yAngleSwing;
        else
            yAngle -= yAngleSwing;

        foreach (var oni in onis)
        {
            blowoutUp = Vector3.up;
            blowoutXZ = Vector3.right * baseRadius;
            blowoutXZ = Quaternion.AngleAxis(yAngle, Vector3.up) * blowoutXZ;
            blowout = blowoutUp + blowoutXZ;
            blowout.Normalize();

            blowout = Quaternion.AngleAxis(forwardBackAngle, Vector3.forward) * blowout;
            blowoutSpeed = blowoutSpeedBase * Random.Range(0.8f, 1.2f);
            blowout *= blowoutSpeed;
            Vector3 angularVelocity = Vector3.Cross(Vector3.up, blowout);
            angularVelocity.Normalize();
            angularVelocity *= Mathf.PI * 8.0f * blowoutSpeed / 15.0f * Random.Range(0.5f, 1.5f);

            oni.AttackedFromPlayer(blowout, angularVelocity);

            yAngle += arcLength / (onis.Length - 1);
        }

        if(onis.Length > 0)
        {
            AudioClip[] defeatSE = null;
            if (onis.Length >= 1 && onis.Length < 3)
                defeatSE = defeatLevel1;
            else if (onis.Length >= 3 && onis.Length < 8)
                defeatSE = defeatLevel2;
            else
                defeatSE = defeatLevel3;
            if (defeatSE != null)
            {
                int index = Random.Range(0, defeatSE.Length);
                onis[0].GetComponent<AudioSource>().clip = defeatSE[index];
                onis[0].GetComponent<AudioSource>().Play();
            }
        }

        count++;
        Destroy(gameObject);
    }

    public void OnPlayerHitted()
    {
        gameSceneControl.AddScoreMax(oniNumMax);
        isPlayerHitted = true;
    }

    public void BeginLeave()
    {
        GetComponent<Collider>().enabled = false;
        state = OniState.Leave;
    }

    private void SetOniMotionSpeed(float speed)
    {
        foreach (var oni in onis)
            oni.SetMotionSpeed(speed);
    }
}
