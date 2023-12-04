using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;

public class ObjectGeneratorArea : MonoBehaviour
{
    public ObjectGenerator generator;
    private Orbital orbital;
    public Vector3 currentPlayAreaOffset;

    public float OrbitalSpeed => orbital.MoveLerpTime;
    
    private void Start()
    {
        orbital = GetComponent<Orbital>();
    }
    
    public void SetPosition(Vector3 position)
    {
        currentPlayAreaOffset = position;
        orbital.WorldOffset = position;
        orbital.enabled = true;
    }

    void ResetGenerator()
    {
        generator.Reset();
    }

    public void ResetPlayArea()
    {
        orbital.WorldOffset = currentPlayAreaOffset;
        orbital.enabled = true;
        Invoke(nameof(ResetGenerator), 0.7f); // required or the old move task position is kept
    }
}
