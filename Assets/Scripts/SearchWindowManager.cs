using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SearchWindowManager : MonoBehaviour
{
    [SerializeField] private GameObject searchCanvas; // �˻� â Canvas
    [SerializeField] private TMP_InputField searchInputField; // �˻��� �Է� �ʵ�
    [SerializeField] private Button searchButton; // �˻� ��ư
    [SerializeField] private ScrollRect resultsScrollView; // �˻� ����� ǥ���ϴ� Scroll View
    [SerializeField] private GameObject resultPrefab; // ��� �׸� Prefab
    [SerializeField] private List<GameObject> navTargetObjects; // ��� ������ ������Ʈ ���
    [SerializeField] private PathUpdater pathUpdater; // ������ ���� ��ũ��Ʈ ����

    private void Start()
    {
        searchCanvas.SetActive(false); // �˻� â �ʱ⿡�� �ݾƵα�
        searchButton.onClick.AddListener(Search); // �˻� ��ư Ŭ�� �� Search �޼ҵ� ����
    }

    // �˻� â ����
    public void OpenSearchWindow() => searchCanvas.SetActive(true);

    // �˻� â �ݱ�
    public void CloseSearchWindow() => searchCanvas.SetActive(false);

    // �˻� ��� ����
    private void Search()
    {
        string query = searchInputField.text.ToLower(); // �Էµ� �˻��� ��������
        ClearResults(); // ���� �˻� ��� ����

        // navTargetObjects�� �̸� �������� ����
        List<GameObject> sortedTargets = new List<GameObject>(navTargetObjects);
        sortedTargets.Sort((a, b) => a.name.ToLower().CompareTo(b.name.ToLower()));

        // ���� �˻����� �˻�� ������ ��ġ ã��
        int index = BinarySearch(sortedTargets, query);
        if (index != -1) FindAllMatches(sortedTargets, query, index); // ��ó �׸�鿡�� ��� ��ġ �˻�
    }

    // ���� �˻� �޼���
    private int BinarySearch(List<GameObject> sortedTargets, string query)
    {
        int left = 0, right = sortedTargets.Count - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            string midName = sortedTargets[mid].name.ToLower();

            // �˻�� �����ϴ��� Ȯ��
            if (midName.Contains(query)) return mid;
            else if (midName.CompareTo(query) < 0) left = mid + 1; // �˻���� ������ ������ Ž��
            else right = mid - 1; // �˻���� ũ�� ���� Ž��
        }
        return -1; // �˻�� ã�� ����
    }

    // ���� �˻� ��ġ�� �������� �¿� ������ Ȯ���Ͽ� ��� �κ� ��ġ �׸� ã��
    private void FindAllMatches(List<GameObject> sortedTargets, string query, int startIndex)
    {
        // �������� Ȯ���Ͽ� ��ġ �׸� ã��
        for (int i = startIndex; i >= 0; i--)
        {
            if (sortedTargets[i].name.ToLower().Contains(query)) CreateResultItem(sortedTargets[i]);
            else break; // ��ġ���� ������ ����
        }

        // ���������� Ȯ���Ͽ� ��ġ �׸� ã��
        for (int i = startIndex + 1; i < sortedTargets.Count; i++)
        {
            if (sortedTargets[i].name.ToLower().Contains(query)) CreateResultItem(sortedTargets[i]);
            else break; // ��ġ���� ������ ����
        }
    }

    // �˻� ��� �׸� ����
    private void CreateResultItem(GameObject target)
    {
        GameObject resultItem = Instantiate(resultPrefab, resultsScrollView.content);
        TextMeshProUGUI resultLabel = resultItem.GetComponentInChildren<TextMeshProUGUI>();

        if (resultLabel) resultLabel.text = target.name; // ����� �ؽ�Ʈ ����
        else
        {
            Debug.LogError("��� �����տ� TextMeshProUGUI ������Ʈ�� �����ϴ�.");
            return;
        }

        Button resultButton = resultItem.GetComponent<Button>();
        if (resultButton)
        {
            resultButton.onClick.AddListener(() =>
            {
                SetTarget(target); // Ŭ�� �� ��� ����
                CloseSearchWindow(); // �˻� â �ݱ�
            });
        }
        else Debug.LogError("��� �����տ� Button ������Ʈ�� �����ϴ�.");
    }

    // ���� �˻� ��� ����
    private void ClearResults()
    {
        foreach (Transform child in resultsScrollView.content)
        {
            Destroy(child.gameObject);
        }
    }

    // ���õ� ��� ����
    private void SetTarget(GameObject target)
    {
        if (pathUpdater) pathUpdater.SetTargetByObject(target);
        else Debug.LogError("SearchWindowManager�� PathUpdater�� �Ҵ���� �ʾҽ��ϴ�.");
    }
}