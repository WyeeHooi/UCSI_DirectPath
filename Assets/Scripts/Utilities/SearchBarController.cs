using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchBarController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown targetDataDropdown;
    [SerializeField] private TMP_InputField searchBarInputField;

    private List<string> allTargetNames = new List<string>();
    private List<string> filteredTargetNames = new List<string>();

    private void Start()
    {
        GenerateTargetItems();
        PopulateDropdown();
    }

    private void GenerateTargetItems()
    {
        // Assuming you have already populated allTargetNames in the TargetHandler script
        // Example:
        // allTargetNames = currentTargetItems.Select(x => x.Name).ToList();
    }

    private void PopulateDropdown()
    {
        // Populate dropdown initially with all target names
        targetDataDropdown.ClearOptions();
        targetDataDropdown.AddOptions(allTargetNames);
    }

    public void OnSearchInputChanged(string searchText)
    {
        // Filter target names based on search text
        filteredTargetNames.Clear();
        foreach (string targetName in allTargetNames)
        {
            if (targetName.ToLower().Contains(searchText.ToLower()))
            {
                filteredTargetNames.Add(targetName);
            }
        }

        // Update dropdown options with filtered target names
        targetDataDropdown.ClearOptions();
        targetDataDropdown.AddOptions(filteredTargetNames);
    }
}
