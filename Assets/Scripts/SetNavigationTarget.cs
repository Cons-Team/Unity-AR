using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private TMP_InputField mainSearchField;
    [SerializeField]
    private GameObject modalPanel;
    [SerializeField]
    private TMP_InputField modalSearchField;
    [SerializeField]
    private GameObject modalSuggestionPanel;
    [SerializeField]
    private GameObject suggestionButtonPrefab;
    [SerializeField]
    private List<GameObject> navTargetObjects; // 여러 네비게이션 목적지 리스트
    [SerializeField]
    private GameObject player; // 플레이어 오브젝트의 참조를 받을 변수
    [SerializeField]
    private Button searchbtn; // 모달 패널을 보이게 하는 버튼

    private NavMeshPath path;
    private LineRenderer line;
    private Vector3 lastPosition;
    private List<GameObject> suggestionButtons = new List<GameObject>();

    private void Start()
    {
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();

        // 초기 경로 계산
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[0].transform.position);
        }

        // 모달 창 버튼에 이벤트 리스너 추가
        searchbtn.onClick.AddListener(OpenModal);

        // 모달 검색 필드에 이벤트 리스너 추가
        modalSearchField.onValueChanged.AddListener(OnModalSearchValueChanged);

        // 안드로이드 코드
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        string coordinate = androidJavaObject.Call<string>("GetCoordinate");
        UpdatePlayerPosition(coordinate);
    }

    private void OpenModal()
    {
        modalPanel.SetActive(true);
        modalSearchField.text = "";
        UpdateSuggestions("");
    }

    private void CloseModal()
    {
        modalPanel.SetActive(false);
    }

    private void OnModalSearchValueChanged(string searchText)
    {
        UpdateSuggestions(searchText);
    }

    private void UpdateSuggestions(string searchText)
    {
        // 이전에 생성된 제안 버튼 제거
        foreach (GameObject button in suggestionButtons)
        {
            Destroy(button);
        }
        suggestionButtons.Clear();

        // 검색어와 일치하는 제안 생성
        foreach (GameObject target in navTargetObjects)
        {
            if (target.name.ToLower().Contains(searchText.ToLower()))
            {
                GameObject suggestionButton = Instantiate(suggestionButtonPrefab, modalSuggestionPanel.transform);
                suggestionButton.GetComponentInChildren<TextMeshProUGUI>().text = target.name;
                suggestionButton.GetComponent<Button>().onClick.AddListener(() => OnSuggestionSelected(target));
                suggestionButtons.Add(suggestionButton);
            }
        }
    }

    private void OnSuggestionSelected(GameObject target)
    {
        mainSearchField.text = target.name;
        UpdatePath(target.transform.position);
        CloseModal();
    }

    private void UpdatePath(Vector3 targetPosition)
    {
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
            line.enabled = true;
        }
    }

    private void Update()
    {
        if (navTargetObjects.Count > 0)
        {
            if (Vector3.Distance(transform.position, lastPosition) > 0.1f)
            {
                lastPosition = transform.position;
                UpdatePath(navTargetObjects[0].transform.position);
            }
        }
    }

    // 받은 좌표에 따라 플레이어의 위치를 업데이트하는 메서드
    private void UpdatePlayerPosition(string coordinate)
    {
        string[] coordinates = coordinate.Split(',');
        Debug.Log("좌표 크기 : " + coordinates.Length);
        if (coordinates.Length == 3)
        {
            if (
                float.TryParse(coordinates[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(coordinates[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(coordinates[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float z))
            {
                // 플레이어 오브젝트의 위치를 업데이트
                if (player != null) // 플레이어 오브젝트가 null이 아닌 경우에만 위치를 업데이트
                    player.transform.position = new Vector3(x, y, z);
                else
                    Debug.LogError("플레이어 설정이 안됐습니다.");
            }
            else
            {
                Debug.LogError("좌표를 구문 분석하지 못했습니다.");
            }
        }
        else
        {
            Debug.LogError("잘못된 좌표 형식입니다.");
        }
    }
}