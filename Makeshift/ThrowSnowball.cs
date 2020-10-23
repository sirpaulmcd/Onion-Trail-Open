using System.Collections;
using UnityEngine;

namespace EGS
{
/// <summary>
/// A small, hacky script to periodically throw snowballs at a target.
/// This is not for use in the actual game -- it is just a means of testing the
/// WarAndPeace system.
/// </summary> 
public class ThrowSnowball : MonoBehaviour
{
    // Use "... = default" to avoid "Field ... is never assigned to" messages.
    [SerializeField] private GameObject target = default;
    [SerializeField] private float velocity = default;
    [SerializeField] private float cooldownSeconds = default;
    [SerializeField] private float knockback = default;

    private void Start()
    {
        StartCoroutine(createAndThrowSnowball());
    }

    /// <summary>
    /// Throws a snowball at the target repeatedly.
    /// </summary>
    private IEnumerator createAndThrowSnowball()
    {
        while(true)
        {
            // Wait a moment before throwing the snowball.
            // This is at the beginning rather than the end to prevent snowballs
            // from being created while Unity is loading the scene.
            yield return new WaitForSeconds(cooldownSeconds);

            // Perform some basic calculations for the trajectory...
            Vector3 casterPos = this.transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 direction = Vector3.Normalize(targetPos - casterPos);

            // Ready the snow. Place it in the "Snowballs" GameObject to prevent
            // cluttering the scene with objects.
            GameObject snowballContainer = GameObject.Find("Snowballs");
            if(snowballContainer == null) 
            {
                snowballContainer = new GameObject("Snowballs");
            }
            GameObject snowball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            snowball.transform.parent = snowballContainer.transform;
            Rigidbody rb = snowball.AddComponent<Rigidbody>();
            snowball.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            // Make it hurt...
            WarAndPeace wap = snowball.AddComponent<WarAndPeace>();
            wap.attacker = this.gameObject;
            wap.criticalHitMultiplier = 1;
            wap.criticalHitChance = 0;
            wap.heal = false;
            wap.knockback = this.knockback;
            wap.knockbackOrigin = this.gameObject;
            wap.minimumDamage = 10;
            wap.maximumDamage = 50;
            wap.name = "snowball";

            // Pew pew pew!
            snowball.transform.position = casterPos + direction;
            rb.useGravity = false;
            rb.velocity = direction * velocity;
        }
    }
}
}