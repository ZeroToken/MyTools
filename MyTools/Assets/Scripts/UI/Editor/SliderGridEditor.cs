using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SliderGrid))]
public class SliderGridEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        SliderGrid mGrid = target as SliderGrid;
        mGrid.arrangement = (SliderGrid.Arrangement)EditorGUILayout.EnumPopup("Arrangement", mGrid.arrangement);
        if (mGrid.arrangement == SliderGrid.Arrangement.Horizontal)
        {
            mGrid.cellWidth = EditorGUILayout.IntField("Cell Width", mGrid.cellWidth);
            mGrid.offsetWidthCount = EditorGUILayout.IntField("Offset Width Count", mGrid.offsetWidthCount);
        }

        else if (mGrid.arrangement == SliderGrid.Arrangement.Vertical)
        {
            mGrid.cellHeight = EditorGUILayout.IntField("Cell Height", mGrid.cellHeight);
            mGrid.offsetHeightCount = EditorGUILayout.IntField("Offset Height Count", mGrid.offsetHeightCount);
        }
        else
        {
            mGrid.cellWidth = EditorGUILayout.IntField("Cell Width", mGrid.cellWidth);
            mGrid.offsetWidthCount = EditorGUILayout.IntField("Offset Width Count", mGrid.offsetWidthCount);
            mGrid.cellHeight = EditorGUILayout.IntField("Cell Height", mGrid.cellHeight);
            mGrid.offsetHeightCount = EditorGUILayout.IntField("Offset Height Count", mGrid.offsetHeightCount);
        }
        mGrid.pivot = (UIWidget.Pivot)EditorGUILayout.EnumPopup("Pivot", mGrid.pivot);
        mGrid.springStrength = EditorGUILayout.IntField("Spring Strength", mGrid.springStrength);
        mGrid.isLoop = EditorGUILayout.Toggle("Loop", mGrid.isLoop);
        mGrid.isChildOnCenter = EditorGUILayout.Toggle("Child On Center", mGrid.isChildOnCenter);
        if (mGrid.isChildOnCenter)
        {
            mGrid.isInvalidateBounds = EditorGUILayout.Toggle("Is Invalidate Bounds", mGrid.isInvalidateBounds);
            mGrid.moveDeltaOffset = EditorGUILayout.FloatField("Move Delta Offset", mGrid.moveDeltaOffset);
        }

        EditorUtility.SetDirty(target);
    }
            
}
