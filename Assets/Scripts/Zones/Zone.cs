using UnityEngine;

public abstract class Zone : MonoBehaviour
{

    public abstract bool CheckPointIsInsideShape(Vector3 pointPos);
}
