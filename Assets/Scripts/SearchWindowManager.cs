using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SearchWindowManager : MonoBehaviour
{
    [SerializeField]
    private GameObject searchCanvas;  // �˻� â Canvas

    [SerializeField]
    private TMP_InputField searchInputField;  // �˻��� �Է� �ʵ�

    [SerializeField]
    private Button searchButton;  // �˻� ��ư

    [SerializeField]
    private ScrollRect resultsScrollView;  // �˻� ����� ǥ���ϴ� Scroll View

    [SerializeField]
    private GameObject resultPrefab;  // ��� �׸� Prefab (Ŭ�� ����)

    [SerializeField]
    private List<GameObject> navTargetObjects;  // ��� ������ ������Ʈ ���

    [SerializeField]
    private PathUpdater pathUpdater;  // ������ ���� ��ũ��Ʈ ����

    private void Start()
    {
        // �˻� â �ݱ�
        searchCanvas.SetActive(false);

        // �˻� ��ư Ŭ�� �� Search() �޼ҵ� ����
        searchButton.onClick.AddListener(Search);
    }

    public void OpenSearchWindow()
    {
        searchCanvas.SetActive(true);  // �˻� â ����
    }

    public void CloseSearchWindow()
    {
        searchCanvas.SetActive(false);  // �˻� â �ݱ�
    }

    private void Search()
    {
        string query = searchInputField.text.ToLower();  // �˻��� ��������
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

                // �߰��� �׸��� RectTransform�� �α׿� ���
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