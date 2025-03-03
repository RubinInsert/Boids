using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidAgent : MonoBehaviour
{
    BoidManager boidManager;
    public Vector2 velocity;
    List<Collider2D> closeProxBoids;
    List<Collider2D> inSightBoids;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake()
    {
        boidManager = BoidManager.boidManager;
        //velocity = transform.up; // Change with random values
        float angle = Random.Range(0f, 360f);
        // Create a unit vector and multiply by a random speed
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(boidManager.minSpeed, boidManager.minSpeed + 2.0f);
        closeProxBoids = new List<Collider2D>();
        inSightBoids = new List<Collider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (boidManager == null) return;

        closeProxBoids.Clear();
        inSightBoids.Clear();

        closeProxBoids.AddRange(Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), boidManager.avoidRange, LayerMask.GetMask("Boid")));
        inSightBoids.AddRange(Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), boidManager.viewRange, LayerMask.GetMask("Boid")));

        velocity += (Cohesion() + BoundaryEnforcement() + Separation() + Alignment()) * Time.deltaTime;

        if (velocity.magnitude < boidManager.minSpeed)
        {
            velocity = velocity.normalized * boidManager.minSpeed;
        }

        float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(velocity.y, velocity.x) - 90f; // -90f to ensure birds were pointing in the direction of travel
        float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * boidManager.turnSpeed);
        transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        transform.position += (Vector3)velocity * Time.deltaTime * 5;
    }
    Vector2 BoundaryEnforcement()
    {
        float distanceFromBoundary = Vector2.Distance(Vector2.zero, (Vector2)transform.position);
        if (distanceFromBoundary > boidManager.flockBoundaryRadius)
        {
            Vector2 directionToCenter = (Vector2.zero - (Vector2)transform.position).normalized;
            float urgencyFactor = Mathf.Clamp01((distanceFromBoundary - boidManager.flockBoundaryRadius) / boidManager.flockBoundaryRadius);
            return directionToCenter * boidManager.boundaryFactor * (1 - urgencyFactor);
        }
        return Vector2.zero;
    }
    Vector2 Alignment()
    {
        if (inSightBoids.Count <= 1) return Vector2.zero;

        Vector2 avgFlockVel = Vector2.zero;
        inSightBoids.ForEach((inSightBoid) =>
        {
            if (inSightBoid.transform == this.transform) return;
            BoidAgent boid = inSightBoid.GetComponent<BoidAgent>();
            if (boid != null)
            {
                avgFlockVel += boid.velocity;
            }
        });
        avgFlockVel /= (inSightBoids.Count - 1);
        return ((avgFlockVel.normalized - velocity).normalized * boidManager.alignmentFactor);
    }
    Vector2 Separation()
    {
        Vector2 directionAway = Vector2.zero;
        Vector2 separationForce = Vector2.zero;
        closeProxBoids.ForEach((closeProxBoid) =>
        {
            if (closeProxBoid.transform == this.transform) return;
            directionAway = (Vector2)transform.position - (Vector2)closeProxBoid.transform.position;
            if (directionAway.magnitude > 0)
            {
                separationForce += directionAway.normalized / directionAway.magnitude;
            }
        });
        return separationForce * boidManager.avoidFactor;
    }
    Vector2 Cohesion()
    {
        if (inSightBoids.Count == 0) return Vector2.zero;

        Vector2 avgFlockPos = Vector2.zero;
        inSightBoids.ForEach((inSightBoid) =>
        {
            if (inSightBoid.transform == this.transform) return;
            avgFlockPos += new Vector2(inSightBoid.transform.position.x, inSightBoid.transform.position.y);
        });
        avgFlockPos /= inSightBoids.Count;
        return (avgFlockPos - new Vector2(transform.position.x, transform.position.y)).normalized * boidManager.cohesionFactor;
    }
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, boidManager.avoidRange);
    }
}
