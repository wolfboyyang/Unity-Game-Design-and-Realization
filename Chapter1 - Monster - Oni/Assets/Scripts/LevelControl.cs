using UnityEngine;
using System.Collections;

public class LevelControl
{

    public GameObject oniGroupPrefab;
    public GameSceneControl gameSceneControl;
    public PlayerControl player;

    private float oniAppearLine;

    private float appearMargin = 15.0f;

    private int oniAppearNum = 1;
    private int comboCount = 0;

    public enum GroupType
    {
        None = -1,
        Slow = 0,
        Decelerate,
        Passing,
        Rapid,

        Normal
    }

    public GroupType groupType = GroupType.Normal;
    public GroupType nextGroupType = GroupType.Normal;

    private bool canDispath = false;
    public bool isRandom = true;
    private float nextLine = 50.0f;
    private float nextSpeed = OniGroupControl.SpeedMin;

    private int normalCount = 5;
    private int eventCount = 1;
    private GroupType eventType = GroupType.None;

    public const float IntervalMin = 20.0f;
    public const float IntervalMax = 50.0f;


    public void Create()
    {
        oniAppearLine = player.transform.position.x - 1.0f;
    }

    public void OnPlayerMissed()
    {
        oniAppearNum = 1;
        comboCount = 0;
    }

    public void OniAppearControl()
    {
        if (!canDispath)
        {
            if (isExclusiveGroup())
            {
                if (GameObject.FindGameObjectsWithTag("OniGroup").Length == 0)
                    canDispath = true;
            }
            else
                canDispath = true;

            if (canDispath)
            {
                switch (nextGroupType)
                {
                    case GroupType.Slow:
                        oniAppearLine = player.transform.position.x + 50.0f;
                        break;
                    case GroupType.Normal:
                        oniAppearLine = player.transform.position.x + nextLine;
                        break;
                    default:
                        oniAppearLine = player.transform.position.x + 10.0f;
                        break;
                }
            }
        }

        // too many onis in the scene
        if (gameSceneControl.oniGroupNum >= gameSceneControl.oniGroupAppearMax)
            return;
        // wait for player to pass the line
        if (player.transform.position.x <= oniAppearLine)
            return;

        groupType = nextGroupType;
        switch (groupType)
        {
            case GroupType.Slow:
                DispatchSlow();
                break;
            case GroupType.Decelerate:
                DispatchDecelerate();
                break;
            case GroupType.Passing:
                DispatchPassing();
                break;
            case GroupType.Rapid:
                DispatchRapid();
                break;
            case GroupType.Normal:
                DispatchNormal(nextSpeed);
                break;
        }

        oniAppearNum++;
        oniAppearNum = Mathf.Min(oniAppearNum, GameSceneControl.OniAppearNumMax);

        canDispath = false;
        comboCount++;
        gameSceneControl.oniGroupNum++;
        if (isRandom)
            SelectNextGroupType();
    }

    public bool isExclusiveGroup()
    {
        bool exclusive = false;
        switch (groupType)
        {
            case GroupType.Slow:
            case GroupType.Decelerate:
            case GroupType.Passing:
                exclusive = true;
                break;
        }
        switch (nextGroupType)
        {
            case GroupType.Slow:
            case GroupType.Decelerate:
            case GroupType.Passing:
                exclusive = true;
                break;
        }
        return exclusive;
    }

    public void SelectNextGroupType()
    {
        if (eventType != GroupType.None)
        {
            eventCount--;
            if (eventCount <= 0)
            {
                eventType = GroupType.None;
                normalCount = Random.Range(3, 7);
            }
        }
        else
        {
            normalCount--;
            if (normalCount <= 0)
            {
                eventType = (GroupType)Random.Range(0, 4);

                switch (eventType)
                {
                    default:
                    case GroupType.Slow:
                    case GroupType.Decelerate:
                    case GroupType.Passing:
                        eventCount = 1;
                        break;
                    case GroupType.Rapid:
                        eventCount = Random.Range(2, 4);
                        break;
                }
            }
        }

        if (eventType == GroupType.None)
        {
            float rate = Mathf.Clamp01(comboCount / 10.0f);

            nextSpeed = Mathf.Lerp(OniGroupControl.SpeedMax, OniGroupControl.SpeedMin, rate);
            nextLine = Mathf.Lerp(IntervalMax, IntervalMin, rate);
            nextGroupType = GroupType.Normal;
        }
        else
            nextGroupType = eventType;
    }

    public void DispatchNormal(float speed)
    {
        var appearPosition = player.transform.position;
        appearPosition.x += appearMargin;

        CreateOniGroup(appearPosition, speed, OniGroupControl.OniState.Normal);
    }

    public void DispatchSlow()
    {
        var appearPosition = player.transform.position;
        appearPosition.x += appearMargin;

        float rate = Mathf.Clamp01(comboCount / 10.0f);
        CreateOniGroup(appearPosition, OniGroupControl.SpeedMin*rate, OniGroupControl.OniState.Normal);
    }

    public void DispatchRapid()
    {
        var appearPosition = player.transform.position;
        appearPosition.x += appearMargin;

        float rate = Mathf.Clamp01(comboCount / 10.0f);
        CreateOniGroup(appearPosition, nextSpeed, OniGroupControl.OniState.Normal);
    }

    public void DispatchDecelerate()
    {
        var appearPosition = player.transform.position;
        appearPosition.x += appearMargin;

        float rate = Mathf.Clamp01(comboCount / 10.0f);
        CreateOniGroup(appearPosition, 9.0f, OniGroupControl.OniState.Decelerate);
    }

    public void DispatchPassing()
    {
        float speedLow = 2.0f;
        float speedRate = 2.0f;
        float speedHigh = (speedLow - player.runSpeed) / speedRate + player.runSpeed;

        float passingPoint = 0.5f;
        var appearPosition = player.transform.position;
        appearPosition.x += appearMargin;
        CreateOniGroup(appearPosition, speedHigh, OniGroupControl.OniState.Normal);

        appearPosition.x += appearMargin * Mathf.Lerp(speedRate-1.0f, 0.0f, passingPoint);
        CreateOniGroup(appearPosition, speedLow, OniGroupControl.OniState.Normal);
    }

    private void CreateOniGroup(Vector3 appearPosition, float speed, OniGroupControl.OniState state)
    {
        var position = appearPosition;

        var oniGroup = GameObject.Instantiate<GameObject>(oniGroupPrefab).GetComponent<OniGroupControl>();

        position.y = OniGroupControl.collisionSize / 2.0f;
        position.z = 0.0f;

        oniGroup.transform.position = position;
        oniGroup.gameSceneControl = gameSceneControl;
        oniGroup.player = player;
        oniGroup.runSpeed = speed;
        oniGroup.state = state;

        var basePosition = position;
        basePosition.x -= (OniGroupControl.collisionSize - OniControl.collisionSize) / 2.0f;
        basePosition.y = OniControl.collisionSize / 2.0f;

        oniGroup.CreateOnis(oniAppearNum, basePosition);
    }
}