using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(U2DGrid))]
public class U2DGridEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        U2DGrid mGrid = target as U2DGrid;
        mGrid.sorting = (UIGrid.Sorting)EditorGUILayout.EnumPopup("Sorting", mGrid.sorting);
        mGrid.arrangement = (U2DGrid.Arrangement)EditorGUILayout.EnumPopup("Arrangement", mGrid.arrangement);
        if (mGrid.arrangement == U2DGrid.Arrangement.Horizontal)
            mGrid.cellWidth = EditorGUILayout.IntField("Cell Width", mGrid.cellWidth);
        else if (mGrid.arrangement == U2DGrid.Arrangement.Vertical)
            mGrid.cellHeight = EditorGUILayout.IntField("Cell Height", mGrid.cellHeight);
        else
        {
            mGrid.cellWidth = EditorGUILayout.IntField("Cell Width", mGrid.cellWidth);
            mGrid.cellHeight = EditorGUILayout.IntField("Cell Height", mGrid.cellHeight);
            mGrid.maxPerLine = EditorGUILayout.IntField("Max Per Line", 0);
        }
        mGrid.pivot = (UIWidget.Pivot)EditorGUILayout.EnumPopup("Pivot", mGrid.pivot);
        mGrid.springStrength = EditorGUILayout.IntField("Spring Strength", mGrid.springStrength);
        mGrid.isOffsetCount = EditorGUILayout.Toggle("Offset Count", mGrid.isOffsetCount);
        mGrid.isLoop = EditorGUILayout.Toggle("Loop", mGrid.isLoop);
        mGrid.isChildOnCenter = EditorGUILayout.Toggle("Child On Center", mGrid.isChildOnCenter);
        if(mGrid.isChildOnCenter)
            mGrid.isInvalidateBounds = EditorGUILayout.Toggle("Is Invalidate Bounds", mGrid.isInvalidateBounds);
        EditorUtility.SetDirty(target);
    }
            
}
