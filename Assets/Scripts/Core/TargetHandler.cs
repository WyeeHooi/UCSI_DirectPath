using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TargetHandler : MonoBehaviour
{

    [SerializeField]
    private NavigationController navigationController;
    [SerializeField]
    private TextAsset targetModelData;
    [SerializeField]
    private TMP_Dropdown targetDataDropdown;
    [SerializeField]
    private AutoCompleteComboBox autoCompleteComboBox;
    [SerializeField]
    private GameObject targetObjectPrefab;
    [SerializeField]
    private Transform[] targetObjectsParentTransforms;

    private List<TargetFacade> currentTargetItems = new List<TargetFacade>();

    private void Start()
    {
        GenerateTargetItems();
        FillDropdownWithTargetItems();
        FillAutoCompleteComboBox();
    }

    private void GenerateTargetItems()
    {
        IEnumerable<Target> targets = GenerateTargetDataFromSource();
        foreach (Target target in targets)
        {
            currentTargetItems.Add(CreateTargetFacade(target));
        }
    }

    private IEnumerable<Target> GenerateTargetDataFromSource()
    {
        return JsonUtility.FromJson<TargetWrapper>(targetModelData.text).TargetList;
    }

    private TargetFacade CreateTargetFacade(Target target)
    {
        GameObject targetObject = Instantiate(targetObjectPrefab, targetObjectsParentTransforms[target.FloorNumber], false);
        targetObject.SetActive(true);
        targetObject.name = $"{target.FloorNumber} - {target.Name}";
        targetObject.transform.localPosition = target.Position;
        targetObject.transform.localRotation = Quaternion.Euler(target.Rotation);

        TargetFacade targetData = targetObject.GetComponent<TargetFacade>();
        targetData.Name = target.Name;
        targetData.FloorNumber = target.FloorNumber;

        return targetData;
    }

    private void FillDropdownWithTargetItems()
    {
        List<TMP_Dropdown.OptionData> targetFacadeOptionData =
            currentTargetItems.Select(x => new TMP_Dropdown.OptionData
            {
                text = $"{x.Name}"
            }).ToList();

        targetDataDropdown.ClearOptions();
        targetDataDropdown.AddOptions(targetFacadeOptionData);
    }
    public void SetSelectedTargetPositionWithDropdown(int selectedValue)
    {
        navigationController.TargetPosition = GetCurrentlySelectedTarget(selectedValue);
    }
    public void FillAutoCompleteComboBox()
    {
        List<string> targetOptions = currentTargetItems.Select(x => $"{x.Name}").ToList();
        autoCompleteComboBox.SetAvailableOptions(targetOptions); // Set the available options for AutoCompleteComboBox
        //autoCompleteComboBox.onItemSelected.AddListener(SetSelectedTargetPositionWithAutoCompleteComboBox()); // Add listener for item selection
    }

    public void SetSelectedTargetPositionWithAutoCompleteComboBox(InputField _mainInput)
    {
        string selectedItem = _mainInput.text;
        TargetFacade target = GetCurrentTargetByTargetText(selectedItem);
        if (target != null)
        {
            int index = currentTargetItems.IndexOf(target);
            Debug.Log("Index of target: " + index);
            if (index != -1)
            {
                navigationController.TargetPosition = GetCurrentlySelectedTarget(index);
            }
        }
        
    }



   

    private Vector3 GetCurrentlySelectedTarget(int selectedValue)
    {
        Debug.Log("ISelectedvalue: " + selectedValue);
        if (selectedValue >= currentTargetItems.Count)
        {
            return Vector3.zero;
        }

        return currentTargetItems[selectedValue].transform.position;
    }

    public TargetFacade GetCurrentTargetByTargetText(string targetText)
    {
        return currentTargetItems.Find(x =>
            x.Name.ToLower().Equals(targetText.ToLower()));
    }
}