# 유니티로 만든 실내 AR 네비게이션

#### 한신대학교 장준하통일관 3층, 4층 기준으로 맵 구현
<p style="float:left">
  <img src="https://github.com/user-attachments/assets/f7580a05-7a31-496c-a463-c8cf939a1f02" width="300">
  <img src="https://github.com/user-attachments/assets/f11f3daa-e47b-4fab-9d1e-1012fb728ad3" width="410">
</p>

# 구현된 앱 화면
<img src="https://github.com/user-attachments/assets/0b57d7f4-4a50-4b40-b351-23c1b7a6a452" width="300">

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
##### toggle로 했을 경우, 업데이트가 안되는 문제가 발생 -> dropdown으로 할 수 밖에 없었음.

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

# 경로 안내 화살표 렌더러 투명도 조절 기능
<p style="float:left">
  <img src="https://github.com/user-attachments/assets/9f7fcb81-ff65-430a-9971-66139cb711be" width="300">
  <img src="https://github.com/user-attachments/assets/b02756ac-04bf-43b0-9232-c592e665be1a" width="300">
</p>
#### 슬라이더를 이용해 투명도 조절

    // 라인의 투명도를 설정하는 메서드
    public void SetLineOpacity(float opacity)
    {
        lineOpacity = opacity;
        if (lineRenderer != null && lineRenderer.material != null)
        {
            Color materialColor = lineRenderer.material.color;
            materialColor.a = opacity; // 알파 채널을 사용하여 투명도 조절
            lineRenderer.material.color = materialColor;
        }

    }

# 검색기능
#### 새로운 창 띄우기 + 현재 위치 띄우는 것 까진 됐는데
#### 자동완성검색 로직이 미완성
<img src="https://github.com/user-attachments/assets/8a2706af-56f3-4d14-b10a-80d790d79023" width="300">
