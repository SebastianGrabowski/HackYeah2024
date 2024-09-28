using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FormationData", menuName = "ScriptableObjects/FormationData", order = 1)]
public class FormationData : ScriptableObject
{

    [System.Serializable]
    public class Grid
    {
        public bool[] Values = new bool[100];
    }

    public Grid MyGrid;

}
