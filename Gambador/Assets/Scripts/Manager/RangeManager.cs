using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeManager
{
    private GameObject player;
    private Projector rangeProjector;
    private float rangeOrigin;
    private bool canUpdate =true;
    public float RangeRadius;

    [HideInInspector] public delegate void RangeEventManager();
    [HideInInspector] public static event RangeEventManager OnRangeUpdateEvent;

    public RangeManager()
    {
        player = GameObject.Find("Player");
        rangeProjector = player.transform.Find("Range").Find("RangeProjector").GetComponent<Projector>();

        RangeRadius = Config.Range;
        rangeOrigin = RangeRadius;
        rangeProjector.orthographicSize = RangeRadius;
    }

    public void OnRangeUpdateTrigger()
    {
        OnRangeUpdateEvent?.Invoke();
    }

    public void UpdateRangeSmoothly(float newRange)
    {
       
        GameManager.singleton.StartCoroutine(UpdateRangeCoroutine(newRange));
        
    }

    IEnumerator UpdateRangeCoroutine(float addRange)
    {
        if(RangeRadius+addRange <= Config.MaxRange)
        {
            OnRangeUpdateTrigger();
            while (!canUpdate)
            {
                yield return 0;
            }
            float newRange = RangeRadius + addRange;
            canUpdate = false;
            float ratio = 0;
            while (RangeRadius != newRange)
            {
                ratio += 1.5f;
                RangeRadius = Mathf.Lerp(RangeRadius, newRange, ratio * Time.deltaTime * Config.TimeScale);
                rangeProjector.orthographicSize = RangeRadius;
                yield return 0;
            }
            canUpdate = true;
        }
    }
}
