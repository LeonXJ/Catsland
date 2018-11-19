using UnityEngine;
using UnityEditor;
using OSP;
using UnityEditorInternal;

[CustomEditor(typeof(OneStopParallax))]
public class OSPEditor : Editor
{
    ReorderableList _parallaxObjectList;

    void OnEnable()
    {
        var osp = (OneStopParallax)target;

        _parallaxObjectList = new ReorderableList(serializedObject, serializedObject.FindProperty("ParallaxObjects"), false, true, false, true);

        _parallaxObjectList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Parallax Objects (Double Click To Select)");
        };

        _parallaxObjectList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            rect.y += 2;
            //var element = _parallaxObjectList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 40, EditorGUIUtility.singleLineHeight), index.ToString());

            EditorGUI.ObjectField(new Rect(rect.x + 40, rect.y, rect.width - 40, EditorGUIUtility.singleLineHeight),
                osp.ParallaxObjects[index], typeof(OSPObject), true);
        };

        _parallaxObjectList.onRemoveCallback += RemoveItem;
    }

    public override void OnInspectorGUI()
    {
        var osp = (OneStopParallax)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        osp.ParallaxSmoothingX = EditorGUILayout.Slider(new GUIContent("Parallax Smoothing X", "Ranges from 0 to 1 and scales the horizontal speed of all layers."), osp.ParallaxSmoothingX, 0, 1);
        EditorGUILayout.Space();
        osp.ParallaxSmoothingY = EditorGUILayout.Slider(new GUIContent("Parallax Smoothing Y", "Ranges from 0 to 1 and scales the vertical speed of all layers."), osp.ParallaxSmoothingY, 0, 1);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        osp.MainCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "The main camera of the game. This will be the camera that follows your player."), osp.MainCamera, typeof(Camera), true);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        osp.UseSeedForRandomization = EditorGUILayout.ToggleLeft(new GUIContent("Use Seed For Randomization", "Use a seed for random layer placements. This will allow you to have the same layout each time the game loads instead of randomizing it every time."), osp.UseSeedForRandomization);
        if(osp.UseSeedForRandomization)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            osp.RandomSeed = EditorGUILayout.IntField(new GUIContent("Seed", "The number that will be used to seed the randomization of your layer placements. This must be a whole number between -2,147,483,647 and 2,147,483,647."), osp.RandomSeed);
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Generate Seed", "Generate a random seed number.")))
            {
                osp.RandomSeed = Random.Range(int.MinValue, int.MaxValue);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 70.0f, GUILayout.ExpandWidth(true));
        var style = new GUIStyle("box");
        if (EditorGUIUtility.isProSkin)
            style.normal.textColor = Color.white;
        GUI.Box(drop_area, "\nDrag & Drop Parallax Sprites Here\n\nParallax Script Will Automatically Be Added To Elements Dropped Here", style);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    if (osp.ParallaxObjects == null)
                        osp.ParallaxObjects = new System.Collections.Generic.List<OSPObject>();

                    foreach (Object dragged_object in DragAndDrop.objectReferences)
                    {
                        var spriteObject = (GameObject)dragged_object;
                        var ospObj = spriteObject.GetComponent<OSPObject>();

                        if (ospObj == null)
                        {
                            spriteObject.AddComponent<OSPObject>();
                            ospObj = spriteObject.GetComponent<OSPObject>();
                            EditorUtility.SetDirty(spriteObject);
                        }

                        if (!osp.ParallaxObjects.Contains(ospObj))
                        {
                            osp.ParallaxObjects.Add(ospObj);
                        }

                        EditorUtility.SetDirty(osp);
                    }
                }
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        _parallaxObjectList.DoLayoutList();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    private void RemoveItem(ReorderableList list)
    {
        var osp = (OneStopParallax)target;
        osp.ParallaxObjects.RemoveAt(list.index);
        EditorUtility.SetDirty(target);
    }
}

[CustomEditor(typeof(OSPObject))]
[CanEditMultipleObjects]
public class OSPObjectEditor : Editor
{
    #region Serialized Editor Properties
    SerializedProperty UseParallaxOnXAxisProp;
    SerializedProperty ParallaxSmoothingXProp;
    SerializedProperty UseParallaxOnYAxisProp;
    SerializedProperty ParallaxSmoothingYProp;
    SerializedProperty AutoTileXProp;
    SerializedProperty HorizontalDistanceProp;
    SerializedProperty RandomizeHorizontalDistanceProp;
    SerializedProperty HorizontalDistanceMinProp;
    SerializedProperty HorizontalDistanceMaxProp;
    SerializedProperty RandomizeHorizontalYAxisProp;
    SerializedProperty HorizontalYMinProp;
    SerializedProperty HorizontalYMaxProp;
    SerializedProperty AutoTileYProp;
    SerializedProperty VerticalDistanceProp;
    SerializedProperty RandomizeVerticalDistanceProp;
    SerializedProperty VerticalDistanceMinProp;
    SerializedProperty VerticalDistanceMaxProp;
    SerializedProperty RandomizeVerticalXAxisProp;
    SerializedProperty VerticalXMinProp;
    SerializedProperty VerticalXMaxProp;
    SerializedProperty AutoMoveXProp;
    SerializedProperty HorizontalSpeedProp;
    SerializedProperty AutoMoveYProp;
    SerializedProperty VerticalSpeedProp;
    SerializedProperty UseRandomHistoryProp;
    SerializedProperty MaxPlacementHistorySizeProp;
    #endregion

    void OnEnable()
    {
        UseParallaxOnXAxisProp = serializedObject.FindProperty("UseParallaxOnXAxis");
        ParallaxSmoothingXProp = serializedObject.FindProperty("ParallaxSmoothingX");
        UseParallaxOnYAxisProp = serializedObject.FindProperty("UseParallaxOnYAxis");
        ParallaxSmoothingYProp = serializedObject.FindProperty("ParallaxSmoothingY");
        AutoTileXProp = serializedObject.FindProperty("AutoTileX");
        HorizontalDistanceProp = serializedObject.FindProperty("HorizontalDistance");
        RandomizeHorizontalDistanceProp = serializedObject.FindProperty("RandomizeHorizontalDistance");
        HorizontalDistanceMinProp = serializedObject.FindProperty("HorizontalDistanceMin");
        HorizontalDistanceMaxProp = serializedObject.FindProperty("HorizontalDistanceMax");
        RandomizeHorizontalYAxisProp = serializedObject.FindProperty("RandomizeHorizontalYAxis");
        HorizontalYMinProp = serializedObject.FindProperty("HorizontalYMin");
        HorizontalYMaxProp = serializedObject.FindProperty("HorizontalYMax");
        AutoTileYProp = serializedObject.FindProperty("AutoTileY");
        VerticalDistanceProp = serializedObject.FindProperty("VerticalDistance");
        RandomizeVerticalDistanceProp = serializedObject.FindProperty("RandomizeVerticalDistance");
        VerticalDistanceMinProp = serializedObject.FindProperty("VerticalDistanceMin");
        VerticalDistanceMaxProp = serializedObject.FindProperty("VerticalDistanceMax");
        RandomizeVerticalXAxisProp = serializedObject.FindProperty("RandomizeVerticalXAxis");
        VerticalXMinProp = serializedObject.FindProperty("VerticalXMin");
        VerticalXMaxProp = serializedObject.FindProperty("VerticalXMax");
        AutoMoveXProp = serializedObject.FindProperty("AutoMoveX");
        HorizontalSpeedProp = serializedObject.FindProperty("HorizontalSpeed");
        AutoMoveYProp = serializedObject.FindProperty("AutoMoveY");
        VerticalSpeedProp = serializedObject.FindProperty("VerticalSpeed");
        UseRandomHistoryProp = serializedObject.FindProperty("UseRandomHistory");
        MaxPlacementHistorySizeProp = serializedObject.FindProperty("MaxPlacementHistorySize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.showMixedValue = true;

        #region Use X Parallax
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(UseParallaxOnXAxisProp, new GUIContent("Use X Parallax", "Check this if you want your parallax layer to move at a different rate horizontally depending on its Z distance from the camera. Leave it unchecked if you want it to move horizontally at the same rate as the camera's movement."));
        EditorGUILayout.Space();
        if (UseParallaxOnXAxisProp.boolValue)
        {
            EditorGUILayout.Slider(ParallaxSmoothingXProp, 0, 1, new GUIContent("  Parallax Smoothing X", "Ranges from 0 to 1 and scales the horizontal speed of the layer."));
            EditorGUILayout.Space();
        }
        #endregion

        #region Use Y Parallax
        EditorGUILayout.PropertyField(UseParallaxOnYAxisProp, new GUIContent("Use Y Parallax", "Check this if you want your parallax layer to move at a different rate vertically depending on its Z distance from the camera. Leave it unchecked if you want it to move vertically at the same rate as the camera's movement"));
        EditorGUILayout.Space();
        if (UseParallaxOnYAxisProp.boolValue)
        {
            EditorGUILayout.Slider(ParallaxSmoothingYProp, 0, 1, new GUIContent("  Parallax Smoothing Y", "Ranges from 0 to 1 and scales the vertical speed of the layer."));
            EditorGUILayout.Space();
        }
        #endregion

        #region Auto Tile X
        EditorGUILayout.PropertyField(AutoTileXProp, new GUIContent("Auto Tile Horizontally", "Check this if you want this layer to tile horizontally."));

        if (AutoTileXProp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(HorizontalDistanceProp, new GUIContent("  Horizontal Spacing", "The amount of X space between each tile."));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(RandomizeHorizontalDistanceProp, new GUIContent("  Randomize Distance Between Tiles", "Check this if you want the amount of X space between the tiles to be random."));
            if (RandomizeHorizontalDistanceProp.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(HorizontalDistanceMinProp, new GUIContent("    Minimum X Distance", "The minimum random X distance between tiles. This value must be positive."));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(HorizontalDistanceMaxProp, new GUIContent("    Maximum X Distance", "The maximum random X distance between tiles. This value must be positive."));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(RandomizeHorizontalYAxisProp, new GUIContent("    Offset Y Value", "Check this if you want to randomly offset the Y value of tiles between two values."));
                if (RandomizeHorizontalYAxisProp.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(HorizontalYMinProp, new GUIContent("      Minimum Y Offset", "The minimum Y offset. This value can be negative or positive."));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(HorizontalYMaxProp, new GUIContent("      Maximum Y Offset", "The maximum Y offset. This value can be negative or positive."));
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.Space();
        #endregion

        #region Auto Tile Y
        EditorGUILayout.PropertyField(AutoTileYProp, new GUIContent("Auto Tile Vertically", "Check this if you want this layer to tile vertically."));

        if (AutoTileYProp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(VerticalDistanceProp, new GUIContent("  Vertical Spacing", "The amount of Y space between each tile."));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(RandomizeVerticalDistanceProp, new GUIContent("  Randomize Distance Between Tiles", "Check this if you want the amount of Y space between the tiles to be random."));
            if (RandomizeVerticalDistanceProp.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(VerticalDistanceMinProp, new GUIContent("    Minimum Y Distance", "The minimum random Y distance between tiles. This value must be positive."));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(VerticalDistanceMaxProp, new GUIContent("    Maximum Y Distance", "The maximum random Y distance between tiles. This value must be positive."));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(RandomizeVerticalXAxisProp, new GUIContent("    Offset X Value", "Check this if you want to randomly offset the X value of tiles between two values."));
                if (RandomizeVerticalXAxisProp.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(VerticalXMinProp, new GUIContent("      Minimum X Offset", "The minimum X offset. This value can be negative or positive."));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(VerticalXMaxProp, new GUIContent("      Maximum X Offset", "The maximum X offset. This value can be negative or positive."));
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.Space();
        #endregion

        #region Auto Move X

        EditorGUILayout.PropertyField(AutoMoveXProp, new GUIContent("Has Horizontal Speed", "Check this if you want this layer to move horizontally on its own."));

        if (AutoMoveXProp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(HorizontalSpeedProp, new GUIContent("  Horizontal Speed", "The horizontal speed of this layer. Negative moves left and positive moves right."));
        }
        EditorGUILayout.Space();

        #endregion

        #region Auto Move Y

        EditorGUILayout.PropertyField(AutoMoveYProp, new GUIContent("Has Vertical Speed", "Check this if you want this layer to move vertically on its own."));
        if (AutoMoveYProp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(VerticalSpeedProp, new GUIContent("  Vertical Speed", "The vertical speed of this layer. Negative moves down and positive moves up."));
        }
        EditorGUILayout.Space();

        #endregion

        #region Random History

        if ((AutoTileXProp.boolValue && RandomizeHorizontalDistanceProp.boolValue) || (AutoTileYProp.boolValue && RandomizeHorizontalDistanceProp.boolValue))
        {
            EditorGUILayout.PropertyField(UseRandomHistoryProp, new GUIContent("Use Random History", "Check if you want to store the random positions for your layers in memory so objects always appear in the same place."));

            if (UseRandomHistoryProp.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MaxPlacementHistorySizeProp, new GUIContent("  Max Random History Elements", "The maximum number of random layer positions to keep in memory."));
            }
        }

        #endregion

        #region Number Checks and Corrections

        if (HorizontalDistanceMinProp.floatValue < 0)
            HorizontalDistanceMinProp.floatValue = 0;

        if (HorizontalDistanceMaxProp.floatValue < 0)
            HorizontalDistanceMaxProp.floatValue = 0;

        if (VerticalDistanceMinProp.floatValue < 0)
            VerticalDistanceMinProp.floatValue = 0;

        if (VerticalDistanceMaxProp.floatValue < 0)
            VerticalDistanceMaxProp.floatValue = 0;

        if (ParallaxSmoothingXProp.floatValue < 0)
            ParallaxSmoothingXProp.floatValue = 0;

        if (ParallaxSmoothingXProp.floatValue > 1)
            ParallaxSmoothingXProp.floatValue = 1;

        if (ParallaxSmoothingYProp.floatValue < 0)
            ParallaxSmoothingYProp.floatValue = 0;

        if (ParallaxSmoothingYProp.floatValue > 1)
            ParallaxSmoothingYProp.floatValue = 1;


        #endregion

        EditorGUI.showMixedValue = false;
        serializedObject.ApplyModifiedProperties();
    }
}