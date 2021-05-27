using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    private Zone _zone;

    private List<ZoneBehaviour> _behaviourUnits;
    private readonly List<Vector3> _behaviourUnitsPos = new List<Vector3>();
    
    
    private void Start()
    {
        _zone = GetComponent<Zone>();
        if (_zone == null)
            throw new Exception("No Zone attached to ZoneTrigger");
        
        UpdateBehavioursLists();
    }

    private void UpdateBehavioursLists()
    {
        _behaviourUnits = FindObjectsOfType<ZoneBehaviour>().ToList();
        
        _behaviourUnitsPos.Clear();
        for (int i = 0; i < _behaviourUnits.Count; i++)
        {
            _behaviourUnitsPos.Add(_behaviourUnits[i].transform.position);
        }

    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _behaviourUnits.Count; i++)
        {
            CheckUnitIsInsideZoneAndAffect(i);
        }
    }

    private void CheckUnitIsInsideZoneAndAffect(int unitNumber)
    {
        _behaviourUnits[unitNumber].Active = _zone.CheckPointIsInsideShape(_behaviourUnitsPos[unitNumber]);
    }



}
