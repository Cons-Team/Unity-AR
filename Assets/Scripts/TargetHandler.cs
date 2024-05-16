/*using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetHandler : MonoBehaviour
{

    [SerializeField]
    private NavigationController navigationController; // 네비게이션 컨트롤러의 참조.

    [SerializeField]
    private TextAsset targetModelData; 

    [SerializeField]
    private TMP_Dropdown targetDataDropdown; // 드롭다운 UI 요소로 타겟 옵션을 표시.

    [SerializeField]
    private GameObject targetObjectPrefab; // 타겟 오브젝트를 생성하기 위한 프리팹.

    [SerializeField]
    private Transform[] targetObjectsParentTransforms; // 서로 다른 층에 있는 타겟 오브젝트를 위한 부모 트랜스폼.

    private List<TargetFacade> currentTargetItems = new List<TargetFacade>(); // 생성된 타겟 오브젝트를 저장하는 리스트.

    private void Start()
    {
        GenerateTargetItems(); // 타겟 아이템을 생성
        FillDropdownWithTargetItems(); // 드롭다운을 타겟 옵션으로 채우는 메서드 호출.
    }

    // 타겟 정보를 추출
    private void GenerateTargetItems()
    {
        IEnumerable<Target> targets = GenerateTargetDataFromSource();
        foreach (Target target in targets)
        {
            currentTargetItems.Add(CreateTargetFacade(target));
        }
    }

    // 타겟 정보를 가져오는 메서드.
    private IEnumerable<Target> GenerateTargetDataFromSource()
    {
        return JsonUtility.FromJson<TargetWrapper>(targetModelData.text).TargetList;
    }

    // 타겟 프리팹을 인스턴스화하고 속성을 설정하는 메서드.
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

    // 드롭다운 UI를 타겟 옵션으로 채우는 메서드.
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

    // 드롭다운에서 선택된 타겟이 변경될 때 호출되는 메서드.
    public void SetSelectedTargetPositionWithDropdown(int selectedValue)
    {
        navigationController.TargetPosition = GetCurrentlySelectedTarget(selectedValue);
    }

    // 선택된 타겟의 위치를 가져오는 메서드.
    private Vector3 GetCurrentlySelectedTarget(int selectedValue)
    {
        if (selectedValue >= currentTargetItems.Count)
        {
            return Vector3.zero;
        }

        return currentTargetItems[selectedValue].transform.position;
    }

    // 타겟 텍스트를 기반으로 TargetFacade 객체를 가져오는 메서드.
    *//*public TargetFacade GetCurrentTargetByTargetText(string targetText)
    {
        return currentTargetItems.Find(x =>
            x.Name.ToLower().Equals(targetText.ToLower()));
    }*//*
}*/