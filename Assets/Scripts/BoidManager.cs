using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public static BoidManager boidManager;
    public GameObject boidPrefab;
    public GameObject boundary;
    [Header("Flock Behaviour")]
    public int numberOfBoids = 80;
    public float flockBoundaryRadius = 20f;
    public float boundaryFactor = 5f;
    public float avoidFactor = 0.5f;
    public float cohesionFactor = 2f;
    public float alignmentFactor = 1f;
    public float avoidRange = 5f;
    public float minSpeed = 1f;
    public float viewRange = 25f;
    public float turnSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        boidManager = GetComponent<BoidManager>();
    }
    private void Start()
    {
        for(int x = 0; x < numberOfBoids; x ++)
        {
            Instantiate(boidPrefab, new Vector3(Random.Range(-flockBoundaryRadius, flockBoundaryRadius), Random.Range(-flockBoundaryRadius, flockBoundaryRadius), 0), Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        boundary.GetComponent<CircleCollider2D>().radius = flockBoundaryRadius;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector3.zero, flockBoundaryRadius);
    }
}
