using UnityEngine;

public class Rigidbody2DExtension {

    public static void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        Vector3 dir = (body.transform.position - explosionPosition);
        float calc = 1 - (dir.magnitude / explosionRadius);
        if (calc <= 0)
            return;
        body.AddForce(dir.normalized * explosionForce * calc);
    }
}
