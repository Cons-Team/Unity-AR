// 문수가 작성한 코드 터치 시 이동되는 그런건가? 아무튼
// 니가 작성한거 아니니까 헷갈리지마 임마..

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private GameObject navTargetObject;
    
    private NavMeshPath path;
    private LineRenderer line;

    private bool lineToggle = false;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began)) {
            lineToggle = !lineToggle;
        }
        if (lineToggle)
        {
            NavMesh.CalculatePath(transform.position, navTargetObject.transform.position, NavMesh.AllAreas, path);
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
            line.enabled = true;
        }
    }

    // 시작지 좌표
    public void set_source(string xyz)
    {
        List<string> coordinate = new List<string>(xyz.Split(','));

        transform.position = new Vector3(float.Parse(coordinate[0]), float.Parse(coordinate[1]), 0);
    }


    // 도착지 좌표
    public void set_destination(string xyz)
    {
        List<string> coordinate = new List<string>(xyz.Split(','));

        navTargetObject.transform.position = new Vector3(float.Parse(coordinate[0]), float.Parse(coordinate[1]), 39);
    }
}
