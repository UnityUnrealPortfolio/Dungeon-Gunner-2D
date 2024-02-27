using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    public void Initialize(Rect rect, RoomNodeGraphSO currentRoomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        id = Guid.NewGuid().ToString();
        name = "RoomNode";
        roomNodeGraph = currentRoomNodeGraph;
        this.roomNodeType = roomNodeType;

        //load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle roomNodeStyle)
    {
        //Draw Node Box Using BeginArea
        GUILayout.BeginArea(rect, roomNodeStyle);

        /*
         * Display a popup using the RoomNodeType name Values that can be selected from
         * (default to the currently set roomNodeType)
         */
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

        roomNodeType = roomNodeTypeList.list[selection];

        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndArea();
    }

    private string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];   

        for(int i = 0;i < roomNodeTypeList.list.Count;i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }
#endif

    #endregion
}
