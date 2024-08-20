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
        ClearResults();

        foreach (GameObject target in navTargetObjects)
        {
            if (target.name.ToLower().Contains(query))
            {
                GameObject resultItem = Instantiate(resultPrefab, resultsScrollView.content);
                resultItem.GetComponentInChildren<TextMeshProUGUI>().text = target.name;

                // 결과 항목 클릭 시 목적지 설정
                resultItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SetTarget(target);
                    CloseSearchWindow();
                });
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
        // 목표 지점을 설정하는 로직을 여기에 추가합니다.
        Debug.Log($"Target set to: {target.name}");
    }
}