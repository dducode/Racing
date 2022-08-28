using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System;

[CustomEditor(typeof(TrackGenerateInEditor))]
public class TrackGenerate_Editor : Editor
{
    public VisualTreeAsset m_inspectorXML;
    public GameObject trackGenerator;
    
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();

        m_inspectorXML.CloneTree(myInspector);

        VisualElement inspectorFoldout = myInspector.Q("Default_inspector");
        InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

        Button createTrack = new Button();
        createTrack.text = "Create track";
        createTrack.clicked += trackGenerator.GetComponent<TrackGenerateInEditor>().CreateTrack;

        Button removeTrack = new Button();
        removeTrack.text = "Remove all tracks";
        removeTrack.clicked += trackGenerator.GetComponent<TrackGenerateInEditor>().RemoveTrack;

        myInspector.Add(createTrack);
        myInspector.Add(removeTrack);

        return myInspector;
    }
}
