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
        ClearResults();

        foreach (GameObject target in navTargetObjects)
        {
            if (target.name.ToLower().Contains(query))
            {
                GameObject resultItem = Instantiate(resultPrefab, resultsScrollView.content);
                resultItem.GetComponentInChildren<TextMeshProUGUI>().text = target.name;

                // ��� �׸� Ŭ�� �� ������ ����
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
        // ��ǥ ������ �����ϴ� ������ ���⿡ �߰��մϴ�.
        Debug.Log($"Target set to: {target.name}");
    }
}