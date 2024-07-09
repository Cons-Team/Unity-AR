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
    private List<GameObject> navTargetObjects; // ���� �׺���̼� ������ ����Ʈ
    [SerializeField]
    private GameObject player; // �÷��̾� ������Ʈ�� ������ ���� ����
    [SerializeField]
    private Button searchbtn; // ��� �г��� ���̰� �ϴ� ��ư

    private NavMeshPath path;
    private LineRenderer line;
    private Vector3 lastPosition;
    private List<GameObject> suggestionButtons = new List<GameObject>();

    private void Start()
    {
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();

        // �ʱ� ��� ���
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[0].transform.position);
        }

        // ��� â ��ư�� �̺�Ʈ ������ �߰�
        searchbtn.onClick.AddListener(OpenModal);

        // ��� �˻� �ʵ忡 �̺�Ʈ ������ �߰�
        modalSearchField.onValueChanged.AddListener(OnModalSearchValueChanged);

        // �ȵ���̵� �ڵ�
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
        // ������ ������ ���� ��ư ����
        foreach (GameObject button in suggestionButtons)
        {
            Destroy(button);
        }
        suggestionButtons.Clear();

        // �˻���� ��ġ�ϴ� ���� ����
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

    // ���� ��ǥ�� ���� �÷��̾��� ��ġ�� ������Ʈ�ϴ� �޼���
    private void UpdatePlayerPosition(string coordinate)
    {
        string[] coordinates = coordinate.Split(',');
        Debug.Log("��ǥ ũ�� : " + coordinates.Length);
        if (coordinates.Length == 3)
        {
            if (
                float.TryParse(coordinates[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(coordinates[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(coordinates[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float z))
            {
                // �÷��̾� ������Ʈ�� ��ġ�� ������Ʈ
                if (player != null) // �÷��̾� ������Ʈ�� null�� �ƴ� ��쿡�� ��ġ�� ������Ʈ
                    player.transform.position = new Vector3(x, y, z);
                else
                    Debug.LogError("�÷��̾� ������ �ȵƽ��ϴ�.");
            }
            else
            {
                Debug.LogError("��ǥ�� ���� �м����� ���߽��ϴ�.");
            }
        }
        else
        {
            Debug.LogError("�߸��� ��ǥ �����Դϴ�.");
        }
    }
}