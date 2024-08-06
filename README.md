# 유니티로 만든 실내 AR 네비게이션

#### 한신대학교 장준하통일관 3층, 4층 기준으로 맵 구현
<p style="float:left">
  <img src="https://github.com/user-attachments/assets/f7580a05-7a31-496c-a463-c8cf939a1f02" width="500">
  <img src="https://github.com/user-attachments/assets/f11f3daa-e47b-4fab-9d1e-1012fb728ad3" width="500">
</p>

# 3층, 4층 구분
    private bool IsOnFirstFloor(Vector3 position)
    {
        return position.y < -2.29f; // 예시: Y 좌표가 -2.29 이하인 경우 3층으로 간주
    }

    private bool IsOnSecondFloor(Vector3 position)
    {
        return position.y >= 1.2f; // 예시: Y 좌표가 1.2 이상인 경우 4층으로 간주
    }

# 엘리베이터 / 계단에 따른 경로 계산
#### 기본 설정은 계단

    private void UpdatePath(Vector3 targetPosition)
        {
            if (pathOptionDropdown.value == 1) // 엘리베이터 옵션이 선택된 경우
            {
                Vector3 elevatorEntry;
                Vector3 elevatorExit;
    
                if (IsOnFirstFloor(transform.position))
                {
                    elevatorEntry = elevatorEntry1F.transform.position;
                    elevatorExit = elevatorExit2F.transform.position;
                }
                else
                {
                    elevatorEntry = elevatorEntry2F.transform.position;
                    elevatorExit = elevatorExit1F.transform.position;
                }
    
                elevatorPathHandler.CalculatePathUsingElevator(transform.position, targetPosition, elevatorEntry, elevatorExit);
            }
            else
            {
                // 계단을 사용하는 기본 경로 계산
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
                if (path.corners.Length > 0)
                {
                    line.positionCount = path.corners.Length;
                    line.SetPositions(path.corners);
                }
                line.enabled = true;
            }
        }

# 엘리베이터 사용 시 경로
#### y좌표에 따라 엘리베이터 층 구분하고 엘리베이터 출발 지점 도착하면 2초후 도착 지점으로 이동
#### 그리고 다시 목적지 안내 시작
    private IEnumerator CalculatePathCoroutine(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
        {
            List<Vector3> completePath = new List<Vector3>();
    
            // 엘리베이터를 사용하는 경로 계산
            NavMeshPath pathToElevator = new NavMeshPath();
            NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
            completePath.AddRange(pathToElevator.corners);
    
            // 경로 표시
            UpdateLineRenderer(completePath);
    
            yield return new WaitForSeconds(2); // 엘리베이터 이동 시간
    
            // 엘리베이터 이동 후 목표 위치까지
            NavMeshPath pathFromElevator = new NavMeshPath();
            NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
            completePath.AddRange(pathFromElevator.corners);
    
            // 최종 경로 표시
            UpdateLineRenderer(completePath);
        }
