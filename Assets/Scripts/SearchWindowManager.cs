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
        ClearResults();  // ���� �˻� ��� ����

        foreach (GameObject target in navTargetObjects)
        {
            if (target.name.ToLower().Contains(query))
            {
                // Prefab�� content�� �����Ͽ� �ڵ����� ��ġ�ǵ��� ��
                GameObject resultItem = Instantiate(resultPrefab, resultsScrollView.content);
                TextMeshProUGUI resultLabel = resultItem.GetComponentInChildren<TextMeshProUGUI>();

                if (resultLabel != null)
                {
                    resultLabel.text = target.name;  // ����� �ؽ�Ʈ ����
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component is missing in the resultPrefab.");
                    continue;  // ������Ʈ�� ������ �������� �Ѿ
                }

                Button resultButton = resultItem.GetComponent<Button>();
                if (resultButton != null)
                {
                    resultButton.onClick.AddListener(() =>
                    {
                        SetTarget(target);  // Ŭ�� �� ��� ����
                        CloseSearchWindow();  // â �ݱ�
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