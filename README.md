# 유니티로 만든 실내 AR 네비게이션

#### 한신대학교 장준하통일관 3층, 4층 기준으로 맵 구현
<p style="float:left">
  <img src="https://github.com/user-attachments/assets/f7580a05-7a31-496c-a463-c8cf939a1f02" width="300">
  <img src="https://github.com/user-attachments/assets/f11f3daa-e47b-4fab-9d1e-1012fb728ad3" width="410">
</p>

# 구현된 앱 화면
<p style="float:left">
  <img src="https://github.com/user-attachments/assets/0b57d7f4-4a50-4b40-b351-23c1b7a6a452" width="300">
  <img src="https://github.com/user-attachments/assets/837bbfed-c9c8-4b17-9f6a-1d3290b7cb87" width="300">
  <img src="https://github.com/user-attachments/assets/c30804fb-447e-4b95-8b13-1a06a771b7d3" width="300">
</p>

# 3층, 4층 구분
    // 현재 위치가 3층에 있는지 확인
    private bool IsOnFirstFloor(Vector3 position)
    {
        return position.y < -2.29f; // Y 좌표 기준으로 3층 판별
    }

    // 현재 위치가 4층에 있는지 확인
    private bool IsOnSecondFloor(Vector3 position)
    {
        return position.y >= 1.2f; // Y 좌표 기준으로 4층 판별
    }

# 엘리베이터 / 계단에 따른 경로 계산
#### 기본 설정은 계단
##### toggle로 했을 경우, 업데이트가 안되는 문제가 발생 -> dropdown으로 할 수 밖에 없었음.

    // 경로를 업데이트하고 경로 옵션에 따라 계단 또는 엘리베이터 경로를 설정
    private void UpdatePath(Vector3 targetPosition)
    {
        if (pathOptionDropdown.value == 1) // 엘리베이터 옵션이 선택된 경우
        {
            Vector3 elevatorEntry = IsOnFirstFloor(transform.position) ? elevatorEntry1F.transform.position : elevatorEntry2F.transform.position;
            Vector3 elevatorExit = IsOnFirstFloor(transform.position) ? elevatorExit2F.transform.position : elevatorExit1F.transform.position;

            // 엘리베이터를 사용하여 경로 계산
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
    // 엘리베이터를 이용한 경로 계산 메서드
    public void CalculatePathUsingElevator(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        // 기존 코루틴이 실행 중이면 중지
        if (elevatorCoroutine != null)
        {
            StopCoroutine(elevatorCoroutine);
        }
        // 새로운 경로 계산 코루틴 시작
        elevatorCoroutine = StartCoroutine(CalculatePathCoroutine(startPosition, targetPosition, elevatorEntry, elevatorExit));
    }

    // 엘리베이터 경로를 계산하는 코루틴
    private IEnumerator CalculatePathCoroutine(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        List<Vector3> completePath = new List<Vector3>();

        // 시작 지점에서 엘리베이터 입구까지 경로 계산
        NavMeshPath pathToElevator = new NavMeshPath();
        NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
        completePath.AddRange(pathToElevator.corners);

        // 경로 표시 업데이트
        UpdateLineRenderer(completePath);

        // 엘리베이터 이동 대기 시간
        yield return new WaitForSeconds(2);

        // 엘리베이터 출구에서 목표 지점까지 경로 계산
        NavMeshPath pathFromElevator = new NavMeshPath();
        NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
        completePath.AddRange(pathFromElevator.corners);

        // 최종 경로 표시 업데이트
        UpdateLineRenderer(completePath);
    }

# 경로 안내 화살표 렌더러 투명도 조절 기능
#### 슬라이더를 이용해 투명도 조절
<p style="float:left">
  <img src="https://github.com/user-attachments/assets/9f7fcb81-ff65-430a-9971-66139cb711be" width="300">
  <img src="https://github.com/user-attachments/assets/b02756ac-04bf-43b0-9232-c592e665be1a" width="300">
</p>

    // LineRenderer의 투명도를 설정하는 메서드
    public void SetLineOpacity(float opacity)
    {
        lineOpacity = opacity;

        if (lineRenderer != null && lineRenderer.material != null)
        {
            Color materialColor = lineRenderer.material.color;
            materialColor.a = opacity; // 알파 값을 변경하여 투명도 조절
            lineRenderer.material.color = materialColor;
        }
    }

# 검색기능
#### 검색후 목적지 변경까지 완료
#### 이진 검색을 활용하여 검색어와 가장 가까운 시작 위치를 찾아 특정 위치를 찾아낸 다음, 그 주변에서 추가적인 일치 항목들을 검토
#### (장점) 전체 데이터를 모두 검색하지 않고도 정확한 결과를 더 빠르게 얻을 수 있다.
<img src="https://github.com/user-attachments/assets/8a2706af-56f3-4d14-b10a-80d790d79023" width="300">
<img src="https://github.com/user-attachments/assets/5643286b-654c-4e34-b10e-73d03a0f516d" width="300">

    // 검색 기능 수행
    private void Search()
    {
        string query = searchInputField.text.ToLower(); // 입력된 검색어 가져오기
        ClearResults(); // 이전 검색 결과 삭제

        // navTargetObjects를 이름 기준으로 정렬
        List<GameObject> sortedTargets = new List<GameObject>(navTargetObjects);
        sortedTargets.Sort((a, b) => a.name.ToLower().CompareTo(b.name.ToLower()));

        // 이진 검색으로 검색어에 근접한 위치 찾기
        int index = BinarySearch(sortedTargets, query);
        if (index != -1) FindAllMatches(sortedTargets, query, index); // 근처 항목들에서 모든 일치 검색
    }

    // 이진 검색 메서드
    private int BinarySearch(List<GameObject> sortedTargets, string query)
    {
        int left = 0, right = sortedTargets.Count - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            string midName = sortedTargets[mid].name.ToLower();

            // 검색어를 포함하는지 확인
            if (midName.Contains(query)) return mid;
            else if (midName.CompareTo(query) < 0) left = mid + 1; // 검색어보다 작으면 오른쪽 탐색
            else right = mid - 1; // 검색어보다 크면 왼쪽 탐색
        }
        return -1; // 검색어를 찾지 못함
    }

    // 이진 검색 위치를 기준으로 좌우 범위를 확장하여 모든 부분 일치 항목 찾기
    private void FindAllMatches(List<GameObject> sortedTargets, string query, int startIndex)
    {
        // 왼쪽으로 확장하여 일치 항목 찾기
        for (int i = startIndex; i >= 0; i--)
        {
            if (sortedTargets[i].name.ToLower().Contains(query)) CreateResultItem(sortedTargets[i]);
            else break; // 일치하지 않으면 종료
        }

        // 오른쪽으로 확장하여 일치 항목 찾기
        for (int i = startIndex + 1; i < sortedTargets.Count; i++)
        {
            if (sortedTargets[i].name.ToLower().Contains(query)) CreateResultItem(sortedTargets[i]);
            else break; // 일치하지 않으면 종료
        }
    }


# 비콘 연동 플레이어 위치 변경
#### PlayerSetting Version 0.1 -> 1로 변경
    // Android에서 좌표 수신
    public void GetLocation()
    {
        // Android의 GetLocation 메소드로부터 좌표 수신
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        string location = ajo.Call<string>("GetLocation");

        string[] locationVector3 = location.Split(',');

        // Indicator(플레이어) 위치 할당
        indicator.transform.position = new Vector3(float.Parse(locationVector3[0]), float.Parse(locationVector3[1]), float.Parse(locationVector3[2]));

        // Android에서 Log 출력 (좌표 수신 확인용)
        ajo.Call("SendToastFromUnity", location);

        // Android에서 Log 출력 (Indicator에 좌표가 저장됐는지 확인용)
        string playerLocation = indicator.transform.position.ToString();
        ajo.Call("SendToastFromUnity", playerLocation);
    }

# 도착시간 계산
#### 이동 거리와 이동 속도, 엘리베이터 대기 시간을 고려하여 도착 시간을 예측 -> 두 가지 경우를 고려 ( 엘리베이터 / 계단 )
    public void UpdateArrivalTimeWithElevator(float elevatorWaitTime, List<Vector3> completePath)
    {
        if (completePath == null || completePath.Count == 0)
        {
            Debug.LogWarning("경로가 비어있거나 null입니다.");
            return;
        }
    
        // 전체 거리 계산
        float totalDistance = 0f;
        for (int i = 0; i < completePath.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(completePath[i], completePath[i + 1]);
        }
    
        // 이동 시간 계산
        float travelTime = totalDistance / movementSpeed;
        float totalTravelTime = travelTime + elevatorWaitTime;
    
        // 도착 예정 시간 계산
        int minutes = Mathf.FloorToInt(totalTravelTime / 60);
        int seconds = Mathf.FloorToInt(totalTravelTime % 60);
    
        // 도착 예정 시간 텍스트 업데이트
        if (arrivalTimeText != null)
        {
            arrivalTimeText.text = $"도착 예정 시간: {minutes}분 {seconds}초 후";
        }
    
        // 디버그 로그 추가
        string finalDestination = completePath.Count > 0 ? $"({completePath[completePath.Count - 1].x}, {completePath[completePath.Count - 1].y}, {completePath[completePath.Count - 1].z})" : "없음";
        Debug.Log($"도착 예정 시간 업데이트: 목표 지점: {finalDestination}, 총 거리: {totalDistance}, 이동 시간: {travelTime}초, 엘리베이터 대기 시간: {elevatorWaitTime}초, 도착 예정 시간: {minutes}분 {seconds}초 후");
    }

# 미구현 기능들
#### 1. 2D 지도 전환
#### 2. 이벤트 + 상점 정보 표시
#### 3. 도움벨
