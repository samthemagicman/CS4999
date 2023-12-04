using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class ObjectGenerator : MonoBehaviour
{
    public UnityEvent<MoveToTargetTask> newObjectGenerated = new UnityEvent<MoveToTargetTask>();
    private AudioSource audioSource;
    public MoveToTargetTask objectsToGenerate; // List of GameObjects to generate

    public TaskConfiguration taskConfiguration = new TaskConfiguration(true, false, false, 6, 0.1f, 0.1f);

    public bool autogenerate = true;

    [SerializeField]
    private Vector3 _spawnAreaPosition;
    public Vector3 SpawnAreaPosition
    {
        get
        {
            if (useRelativeTargetPosition)
            {
                return _spawnAreaPosition + transform.position;
            }
            else
            {
                return _spawnAreaPosition;
            }
        }
        set { _spawnAreaPosition = value; }
    }

    public Vector3 targetSpawnAreaSize;
    public bool useRelativeTargetPosition = true;
    private GameObject currentObject;

    public Vector3 GenerationBounds
    {
        get { return transform.localScale; }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (autogenerate)
        {
            CreateObject();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, GenerationBounds); // Draw boundaries as a wire cube Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SpawnAreaPosition, targetSpawnAreaSize); // Draw boundaries as a wire cube Gizmo
    }

    private void CreateObject()
    {
        DestroyCurrentObject();
        // Calculate random position within the object's bounds
        Vector3 randomPosition = new Vector3(
            Random.Range(-GenerationBounds.x / 2, GenerationBounds.x / 2),
            Random.Range(-GenerationBounds.y / 2, GenerationBounds.y / 2),
            Random.Range(-GenerationBounds.z / 2, GenerationBounds.z / 2)
        );

        Vector3 randomTargetPosition = new Vector3(
            Random.Range(-targetSpawnAreaSize.x / 2, targetSpawnAreaSize.x / 2),
            Random.Range(-targetSpawnAreaSize.y / 2, targetSpawnAreaSize.y / 2),
            Random.Range(-targetSpawnAreaSize.z / 2, targetSpawnAreaSize.z / 2)
        );
        randomTargetPosition += SpawnAreaPosition;

        Vector3 randomTargetScale = new Vector3(1, 1, 1) * Random.Range(1.5f, 0.5f);

        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );


        // Instantiate a random object from the list at the calculated position
        currentObject = Instantiate(objectsToGenerate.gameObject, transform.position + randomPosition, Quaternion.identity);
        MoveToTargetTask targetTask = currentObject.GetComponent<MoveToTargetTask>();
        targetTask.targetTransform = randomTargetPosition;
        targetTask.targetRotation = randomRotation;
        targetTask.targetScale = randomTargetScale;

        targetTask.configuration = taskConfiguration;
        
        targetTask.onCompleted.AddListener(() =>
        {
            if (autogenerate)
            {
                Generate();
            }
            else
            {
                DestroyCurrentObject();
            }
            audioSource.Play();
        });
        
        newObjectGenerated.Invoke(targetTask);
    }
    
    public void DestroyCurrentObject() {
        if (currentObject)
        {
            Destroy(currentObject);
        }
    }
    
    public void Generate()
    {
        CreateObject();
    }

    public void Reset()
    {
        CreateObject();
    }
}
