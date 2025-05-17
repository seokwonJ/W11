using UnityEngine;

public class FollowCrossHairSniper : FollowCrossHair
{
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        Vector3 reverseDir = -dirToPlayer;
        float angle = Mathf.Atan2(reverseDir.y, reverseDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }
}
