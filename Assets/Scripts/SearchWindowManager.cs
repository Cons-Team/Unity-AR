using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SearchWindowManager : MonoBehaviour
{
    [SerializeField] private GameObject searchCanvas; // 검색 창 Canvas
    [SerializeField] private TMP_InputField searchInputField; // 검색어 입력 필드
    [SerializeField] private Button searchButton; // 검색 버튼
    [SerializeField] private ScrollRect resultsScrollView; // 검색 결과를 표시하는 Scroll View
    [SerializeField] private GameObject resultPrefab; // 결과 항목 Prefab
    [SerializeField] private List<GameObject> navTargetObjects; // 모든 목적지 오브젝트 목록
    [SerializeField] private PathUpdater pathUpdater; // 목적지 설정 스크립트 참조

    private void Start()
    {
        searchCanvas.SetActive(false); // 검색 창 초기에는 닫아두기
        searchButton.onClick.AddListener(Search); // 검색 버튼 클릭 시 Search 메소드 실행
    }

    // 검색 창 열기
    public void OpenSearchWindow() => searchCanvas.SetActive(true);

    // 검색 창 닫기
    public void CloseSearchWindow() => searchCanvas.SetActive(false);

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

    // 검색 결과 항목 생성
    private void CreateResultItem(GameObject target)
    {
        GameObject resultItem = Instantiate(resultPrefab, resultsScrollView.content);
        TextMeshProUGUI resultLabel = resultItem.GetComponentInChildren<TextMeshProUGUI>();

        if (resultLabel) resultLabel.text = target.name; // 결과의 텍스트 설정
        else
        {
            Debug.LogError("결과 프리팹에 TextMeshProUGUI 컴포넌트가 없습니다.");
            return;
        }

        Button resultButton = resultItem.GetComponent<Button>();
        if (resultButton)
        {
            resultButton.onClick.AddListener(() =>
            {
                SetTarget(target); // 클릭 시 대상 설정
                CloseSearchWindow(); // 검색 창 닫기
            });
        }
        else Debug.LogError("결과 프리팹에 Button 컴포넌트가 없습니다.");
    }

    // 이전 검색 결과 삭제
    private void ClearResults()
    {
        foreach (Transform child in resultsScrollView.content)
        {
            Destroy(child.gameObject);
        }
    }

    // 선택된 대상 설정
    private void SetTarget(GameObject target)
    {
        if (pathUpdater) pathUpdater.SetTargetByObject(target);
        else Debug.LogError("SearchWindowManager에 PathUpdater가 할당되지 않았습니다.");
    }
}