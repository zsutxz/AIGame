using UnityEngine;
using UnityEditor;

public class TaskTest : MonoBehaviour
{
    [MenuItem("AI/Command/Do Task")]
    static void DoTask()
    {
        for (int i = 0; i < 100; i++)
        {
            Vector3 position = Random.insideUnitSphere * 10;
            position.y = 0;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.name = "Cube " + i;
        }
    }
}