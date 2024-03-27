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



    /******* Search Bar Funtionality *******/

    // This method populates the 'AutoCompleteComboBox' (the Unity Element) with options based on the 'currentTargetItems' list.
    // 'currentTargetItems' list is a collection of objects (destination points) with each storing the destination name and position value 

    public void FillAutoCompleteComboBox()
    {
        List<string> targetOptions = currentTargetItems.Select(x => $"{x.Name}").ToList();
        // fetching all the Name property of each object in the 'currentTargetItems' list, and store them again into a list 'targetOptions'.
        // the 'Select' LINQ method is used specifically on each object 'x' Name attribute which are then collected into a new list using ToList()
        // 'x' represents an object of the currentTargetItems list 
        
        autoCompleteComboBox.SetAvailableOptions(targetOptions);
        // Sets the available options for the 'AutoCompleteComboBox' Unity element using the targetOptions list.

        // The available options for the 'AutoCompleteComboBox' is essentially to provide a set of options
        // that are available for selection based on the user's keyword search.

        // SetAvailableOptions method is called from another C# file by passing the destination Name list
        // and get populated at the 'AutoCompleteCombobox' itempanel
    }


    // This method reads the user's target location based on the text entered in the input field.
    // Then, identify the target location position value on the virtual map
    public void SetSelectedTargetPositionWithAutoCompleteComboBox(InputField _mainInput)
    {        
        string selectedItem = _mainInput.text;
        // Retrieves the text entered in the input field.
        
        TargetFacade target = GetCurrentTargetByTargetText(selectedItem);
        // Finds the TargetFacade object corresponding to the selected item text.

        if (target != null)
        {
            
            int index = currentTargetItems.IndexOf(target);
            // Retrieves the index of the target object in the currentTargetItems list.

            if (index != -1)
            {                
                navigationController.TargetPosition = GetCurrentlySelectedTarget(index);
                // retrieve the target's position value using the index retrieved from the currentTargetItems list.
            }
        }
    }
    /******* Search Bar Funtionality Ends Here*******/



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