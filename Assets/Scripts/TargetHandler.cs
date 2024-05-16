/*using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetHandler : MonoBehaviour
{

    [SerializeField]
    private NavigationController navigationController; // �׺���̼� ��Ʈ�ѷ��� ����.

    [SerializeField]
    private TextAsset targetModelData; 

    [SerializeField]
    private TMP_Dropdown targetDataDropdown; // ��Ӵٿ� UI ��ҷ� Ÿ�� �ɼ��� ǥ��.

    [SerializeField]
    private GameObject targetObjectPrefab; // Ÿ�� ������Ʈ�� �����ϱ� ���� ������.

    [SerializeField]
    private Transform[] targetObjectsParentTransforms; // ���� �ٸ� ���� �ִ� Ÿ�� ������Ʈ�� ���� �θ� Ʈ������.

    private List<TargetFacade> currentTargetItems = new List<TargetFacade>(); // ������ Ÿ�� ������Ʈ�� �����ϴ� ����Ʈ.

    private void Start()
    {
        GenerateTargetItems(); // Ÿ�� �������� ����
        FillDropdownWithTargetItems(); // ��Ӵٿ��� Ÿ�� �ɼ����� ä��� �޼��� ȣ��.
    }

    // Ÿ�� ������ ����
    private void GenerateTargetItems()
    {
        IEnumerable<Target> targets = GenerateTargetDataFromSource();
        foreach (Target target in targets)
        {
            currentTargetItems.Add(CreateTargetFacade(target));
        }
    }

    // Ÿ�� ������ �������� �޼���.
    private IEnumerable<Target> GenerateTargetDataFromSource()
    {
        return JsonUtility.FromJson<TargetWrapper>(targetModelData.text).TargetList;
    }

    // Ÿ�� �������� �ν��Ͻ�ȭ�ϰ� �Ӽ��� �����ϴ� �޼���.
    //private TargetFacade CreateTargetFacade(Target target)
    //{
       // GameObject targetObject = Instantiate(targetObjectPrefab, targetObjectsParentTransforms[target.FloorNumber], false);
        //targetObject.SetActive(true);
        //targetObject.name = $"{target.FloorNumber} - {target.Name}";
        //targetObject.transform.localPosition = target.Position;
        //targetObject.transform.localRotation = Quaternion.Euler(target.Rotation);

        //TargetFacade targetData = targetObject.GetComponent<TargetFacade>();
        //targetData.Name = target.Name;
        //targetData.FloorNumber = target.FloorNumber;

        //return targetData;
    //}

    // ��Ӵٿ� UI�� Ÿ�� �ɼ����� ä��� �޼���.
    private void FillDropdownWithTargetItems()
    {
        List<TMP_Dropdown.OptionData> targetFacadeOptionData =
            currentTargetItems.Select(x => new TMP_Dropdown.OptionData
            {
                text = $"{x.FloorNumber} - {x.Name}"
            }).ToList();

        targetDataDropdown.ClearOptions();
        targetDataDropdown.AddOptions(targetFacadeOptionData);
    }

    // ��Ӵٿ�� ���õ� Ÿ���� ����� �� ȣ��Ǵ� �޼���.
    public void SetSelectedTargetPositionWithDropdown(int selectedValue)
    {
        navigationController.TargetPosition = GetCurrentlySelectedTarget(selectedValue);
    }

    // ���õ� Ÿ���� ��ġ�� �������� �޼���.
    private Vector3 GetCurrentlySelectedTarget(int selectedValue)
    {
        if (selectedValue >= currentTargetItems.Count)
        {
            return Vector3.zero;
        }

        return currentTargetItems[selectedValue].transform.position;
    }

    // Ÿ�� �ؽ�Ʈ�� ������� TargetFacade ��ü�� �������� �޼���.
    *//*public TargetFacade GetCurrentTargetByTargetText(string targetText)
    {
        return currentTargetItems.Find(x =>
            x.Name.ToLower().Equals(targetText.ToLower()));
    }*//*
}*/