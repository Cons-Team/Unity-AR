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
        ClearResults();  // 이전 검색 결과 삭제

        foreach (GameObject target in navTargetObjects)
        {
            if (target.name.ToLower().Contains(query))
            {
                // Prefab을 content에 생성하여 자동으로 배치되도록 함
                GameObject resultItem = Instantiate(resultPrefab, resultsScrollView.content);
                TextMeshProUGUI resultLabel = resultItem.GetComponentInChildren<TextMeshProUGUI>();

                if (resultLabel != null)
                {
                    resultLabel.text = target.name;  // 결과의 텍스트 설정
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component is missing in the resultPrefab.");
                    continue;  // 컴포넌트가 없으면 다음으로 넘어감
                }

                Button resultButton = resultItem.GetComponent<Button>();
                if (resultButton != null)
                {
                    resultButton.onClick.AddListener(() =>
                    {
                        SetTarget(target);  // 클릭 시 대상 설정
                        CloseSearchWindow();  // 창 닫기
                    });
                }
                else
                {
                    Debug.LogError("Button component is missing in the resultPrefab.");
                }
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