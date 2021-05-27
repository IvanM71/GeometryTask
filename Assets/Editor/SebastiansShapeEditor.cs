using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.VirtualTexturing;

/*
 * Source: https://youtu.be/ew4NtzkXj8U + some changes
 * N to remove point
 * LMB to add point
 */

[CustomEditor(typeof(SebastiansShapeCreator))]
public class ShapeEditor : Editor {

    SebastiansShapeCreator _sebastiansShapeCreator;
    SelectionInfo selectionInfo;
    bool needsRepaint;

    private Vector3 parentPos;


    void OnSceneGUI()
    {
        parentPos = _sebastiansShapeCreator.transform.position;
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
        {
            Draw();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
			if (needsRepaint)
			{
				HandleUtility.Repaint();
			}
        }
    }

    void HandleInput(Event guiEvent)
    {
		Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
		float drawPlaneHeight = 0;
		float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
		Vector3 localMousePosition = mouseRay.GetPoint(dstToDrawPlane) - parentPos;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None 
            /*TODO проверка чтоб не реагировал на нажатие на инструмент перемещения самого объекта*/)
		{
            HandleLeftMouseDown(localMousePosition);
		}

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseUp(localMousePosition);
        }

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDrag(localMousePosition);
        }
        
        
        if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.N && guiEvent.modifiers == EventModifiers.None)
        {
            HandleRemovePoint();
        }
        

        if (!selectionInfo.pointIsSelected)
        {
            UpdateMouseOverInfo(localMousePosition);
        }

	}

    void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (!selectionInfo.mouseIsOverPoint)
        {
            int newPointIndex = (selectionInfo.mouseIsOverLine) ? selectionInfo.lineIndex + 1 : _sebastiansShapeCreator.points.Count;
            Undo.RecordObject(_sebastiansShapeCreator, "Add point");
            _sebastiansShapeCreator.points.Insert(newPointIndex,mousePosition);
            selectionInfo.pointIndex = newPointIndex;
        }

        selectionInfo.pointIsSelected = true;
        selectionInfo.positionAtStartOfDrag = mousePosition;
		needsRepaint = true;
    }

	void HandleLeftMouseUp(Vector3 mousePosition)
	{
        if (selectionInfo.pointIsSelected)
        {
            _sebastiansShapeCreator.points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
            Undo.RecordObject(_sebastiansShapeCreator, "Move point");
            _sebastiansShapeCreator.points[selectionInfo.pointIndex] = mousePosition;

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            needsRepaint = true;
        }

	}
    
    
    void HandleRemovePoint()
    {
        if (selectionInfo.mouseIsOverPoint)
        {
            _sebastiansShapeCreator.points.RemoveAt(selectionInfo.pointIndex);
            needsRepaint = true;
        }
    }

	void HandleLeftMouseDrag(Vector3 mousePosition)
	{
        if (selectionInfo.pointIsSelected)
        {
            _sebastiansShapeCreator.points[selectionInfo.pointIndex] = mousePosition;
            needsRepaint = true;
        }

	}

    void UpdateMouseOverInfo(Vector3 mousePosition)
    {
        int mouseOverPointIndex = -1;
        for (int i = 0; i < _sebastiansShapeCreator.points.Count; i++)
        {
            if (Vector3.Distance(mousePosition, _sebastiansShapeCreator.points[i]) < _sebastiansShapeCreator.handleRadius)
            {
                mouseOverPointIndex = i;
                break;
            }
        }

        if (mouseOverPointIndex != selectionInfo.pointIndex)
        {
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

            needsRepaint = true;
        }

        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            int mouseOverLineIndex = -1;
            float closestLineDst = _sebastiansShapeCreator.handleRadius;
            for (int i = 0; i < _sebastiansShapeCreator.points.Count; i++)
            {
                Vector3 nextPointInShape = _sebastiansShapeCreator.points[(i + 1) % _sebastiansShapeCreator.points.Count];
                float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePosition.ToXZ(), _sebastiansShapeCreator.points[i].ToXZ(), nextPointInShape.ToXZ());
                if (dstFromMouseToLine < closestLineDst)
                {
                    closestLineDst = dstFromMouseToLine;
                    mouseOverLineIndex = i;
                }
            }

            if (selectionInfo.lineIndex != mouseOverLineIndex)
            {
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                needsRepaint = true;
            }
        }
    }

    void Draw()
    {
        for (int i = 0; i < _sebastiansShapeCreator.points.Count; i++)
		{
			Vector3 nextPoint = _sebastiansShapeCreator.points[(i + 1) % _sebastiansShapeCreator.points.Count];
            if (i == selectionInfo.lineIndex)
            {
                Handles.color = Color.red;
                Handles.DrawLine(_sebastiansShapeCreator.points[i] + parentPos, nextPoint + parentPos);
            }
            else
            {
                Handles.color = Color.black;
                Handles.DrawDottedLine(_sebastiansShapeCreator.points[i] + parentPos, nextPoint + parentPos, 4);
            }

            if (i == selectionInfo.pointIndex)
            {
                Handles.color = (selectionInfo.pointIsSelected) ? Color.black : Color.red;
            }
            else
            {
                Handles.color = Color.white;
            }
            Handles.DrawSolidDisc(_sebastiansShapeCreator.points[i] + parentPos, Vector3.up, _sebastiansShapeCreator.handleRadius);
		}
        needsRepaint = false;
    }

    void OnEnable()
    {
        _sebastiansShapeCreator = target as SebastiansShapeCreator;
        selectionInfo = new SelectionInfo();
    }

    public class SelectionInfo
    {
        public int pointIndex = -1;
        public bool mouseIsOverPoint;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine;
    }

}