using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour
{

    public Vector3 TargetPosition { get; set; } = Vector3.zero; // 목표 지점의 위치를 설정하고 가져오는 속성.

    public NavMeshPath CalculatedPath { get; private set; } // 계산된 경로를 가져오는 속성.

    private void Start()
    {
        CalculatedPath = new NavMeshPath(); // 계산된 경로 초기화.
    }

    private void Update()
    {
        if (TargetPosition != Vector3.zero)
        { // 목표 지점이 설정되어 있으면
            NavMesh.CalculatePath(transform.position, TargetPosition, NavMesh.AllAreas, CalculatedPath); // 현재 위치에서 목표 지점까지의 경로 계산.
        }
    }
}