using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class ObjectGenerator : MonoBehaviour
{
    private AudioSource audioSource;
    public List<MoveToTargetTask> objectsToGenerate; // List of GameObjects to generate
    public bool useRotation = false;
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

    public Vector3 GenerationBounds
    {
        get { return transform.localScale; }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(GenerateObjects());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, GenerationBounds); // Draw boundaries as a wire cube Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SpawnAreaPosition, targetSpawnAreaSize); // Draw boundaries as a wire cube Gizmo
    }

    IEnumerator GenerateObjects()
    {
        while (true)
        {
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

            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360)
            );


            // Instantiate a random object from the list at the calculated position
            GameObject newObject = Instantiate(objectsToGenerate[Random.Range(0, objectsToGenerate.Count)].gameObject, transform.position + randomPosition, Quaternion.identity);
            MoveToTargetTask targetTask = newObject.GetComponent<MoveToTargetTask>();
            targetTask.targetTransform = randomTargetPosition;
            if (useRotation)
            {
                targetTask.useRotation = true;
                targetTask.targetRotation = randomRotation;
            } else
            {
                targetTask.useRotation = false;
            }

            // Wait for the object to complete its task (modify this condition according to your needs)
            yield return new WaitUntil(() => newObject.GetComponent<MoveToTargetTask>().TaskIsComplete);
            audioSource.Play();

            Destroy(newObject); // Destroy the object after it's complete, you can modify this behavior as needed
        }
    }
}
