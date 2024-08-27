using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SearchWindowManager : MonoBehaviour
{
    [SerializeField]
    private GameObject searchCanvas;  // 검색 창 Canvas

    [SerializeField]
    private TMP_InputField searchInputField;  // 검색어 입력 필드

    [SerializeField]
    private Button searchButton;  // 검색 버튼

    [SerializeField]
    private ScrollRect resultsScrollView;  // 검색 결과를 표시하는 Scroll View

    [SerializeField]
    private GameObject resultPrefab;  // 결과 항목 Prefab (클릭 가능)

    [SerializeField]
    private List<GameObject> navTargetObjects;  // 모든 목적지 오브젝트 목록

    [SerializeField]
    private PathUpdater pathUpdater;  // 목적지 설정 스크립트 참조

    private void Start()
    {
        // 검색 창 닫기
        searchCanvas.SetActive(false);

        // 검색 버튼 클릭 시 Search() 메소드 실행
        searchButton.onClick.AddListener(Search);
    }

    public void OpenSearchWindow()
    {
        searchCanvas.SetActive(true);  // 검색 창 열기
    }

    public void CloseSearchWindow()
    {
        searchCanvas.SetActive(false);  // 검색 창 닫기
    }

    private void Search()
    {
        string query = searchInputField.text.ToLower();  // 검색어 가져오기
        Debug.Log($"Searching for: {query}");
        ClearResults();

        foreach (GameObject target in navTargetObjects)
        {
            string targetName = target.name.ToLower();
            Debug.Log($"Checking target: {targetName}");

            if (targetName.Contains(query))
            {
                Debug.Log($"Match found: {targetName}");
                GameObject resultItem = Instantiate(resultPrefab, resultsScrollView.content);
                resultItem.GetComponentInChildren<TextMeshProUGUI>().text = target.name;

                resultItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SetTarget(target);
                    CloseSearchWindow();
                });

                // 추가된 항목의 RectTransform을 로그에 출력
                RectTransform resultRectTransform = resultItem.GetComponent<RectTransform>();
                Debug.Log($"Result item RectTransform - Width: {resultRectTransform.rect.width}, Height: {resultRectTransform.rect.height}");
            }
        }
    }

    private void ClearResults()
    {
        foreach (Transform child in resultsScrollView.content)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetTarget(GameObject target)
    {
        if (pathUpdater != null)
        {
            pathUpdater.SetTargetByObject(target);
        }
        else
        {
            Debug.LogError("PathUpdater is not assigned in SearchWindowManager.");
        }
    }
}