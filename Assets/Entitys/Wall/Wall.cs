using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Structure
{
    // Start is called before the first frame update
    protected override void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        isBroken = false;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        navMeshObstacle.enabled = !isBroken && status == Status.Complete;
        // navMeshObstacle.enabled = false;
        blendShapeUpdate();
        base.Update();
    }

    void blendShapeUpdate()
    {
        float brokenweight = skinnedMeshRenderer.GetBlendShapeWeight(0) + (isBroken ? 5f : -5f);
        brokenweight = Mathf.Clamp(brokenweight, 0f, 100f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, brokenweight);
    }

    public bool isBroken;

    protected override void killed(Entity attacked = null)
    {
        isBroken = true;
        status = Status.Constaracting;
        base.killed();
        // callBuilder();
    }

    SkinnedMeshRenderer skinnedMeshRenderer;

    public override void construction(int h)
    {
        base.construction(h);
        if (status == Status.Complete)
        {
            isBroken = false;
        }
    }
}
