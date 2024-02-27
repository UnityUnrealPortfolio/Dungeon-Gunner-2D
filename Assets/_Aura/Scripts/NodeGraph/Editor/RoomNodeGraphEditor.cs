
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;

    //Node layout values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private void OnEnable()
    {
        //define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Load Room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }


    [MenuItem("Room Node Graph Editor",menuItem ="Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }


    /// <summary>
    /// Open the room node graph editor window if a room node graph scriptable object asset is double
    /// clicked in the inspector
    /// </summary>
    /// <param name="instanceID"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if(roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }
 



    /// <summary>
    /// Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        //only proceed if there is a selected(double clicked) RoomNodeGraphSO
        if(currentRoomNodeGraph != null)
        {
            //process Events
            ProcessEvents(Event.current);

            //DrawRoomNodes
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    private void ProcessEvents(Event currentEvent)
    {
        //what event happened? mouse click? etc
        switch(currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvents(currentEvent);
                break;

        }
    }

    private void ProcessMouseDownEvents(Event currentEvent)
    {
        //Handle right mouse button click
        //to show context menu
       if(currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        //tell the system whic method to call when mouse is clicked
        menu.AddItem(new GUIContent("Create a Room Node"), false, CreateRoomNode, mousePosition);

        menu.ShowAsContext();
    }

    /// <summary>
    /// Create a Room Node at the position of mouse click
    /// </summary>
    /// <param name="mousePosObject">incoming mouse position as type object</param>
    private void CreateRoomNode(object mousePosObject)
    {
        CreateRoomNode(mousePosObject, roomNodeTypeList.list.Find(x => x.isNone));
    }


    /// <summary>
    /// Create a Room Node at the Position of mouse click
    /// - overloaded to also accept the room node type
    /// </summary>
    /// <param name="mousePositionObject"></param>
    /// <param name="roomNodeType"></param>
    private void CreateRoomNode(object mousePositionObject,RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePos = (Vector2)mousePositionObject;

        //create an instance of the RoomNodeSO Scriptable Object
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        //add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        //set the values in the room node
        roomNode.Initialize(new Rect(mousePos, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        //add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode,currentRoomNodeGraph);

        AssetDatabase.SaveAssets();
    }

    private void DrawRoomNodes()
    {
       //Loop through all room nodes and draw them
       foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }
        GUI.changed = true;
    }

   
}
