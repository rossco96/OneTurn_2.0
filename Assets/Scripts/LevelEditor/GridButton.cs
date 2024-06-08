using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class GridButton : Button
{
	// [TODO] Delete!
	//private int m_xCoordinate;
	//private int m_yCoordinate;
	//public void SetGridCoordinate(int x, int y) { m_xCoordinate = x; m_yCoordinate = y; }

	private UnityAction<GridButton> OnButtonSelected;
	public void RegisterOnButtonSelected(UnityAction<GridButton> onButtonSelected) { OnButtonSelected += onButtonSelected; }
	
	private Color m_propertyColor = Color.white;
	public Color PropertyColor => m_propertyColor;
	public void SetPropertyColor(Color color) { m_propertyColor = color; }


	#region [other override methods]
	/*
	public override void OnSelect(BaseEventData eventData)
	{
		if (EventSystem.current.currentSelectedGameObject != null)
		{
			if (EventSystem.current.currentSelectedGameObject.TryGetComponent<GridButton>(out GridButton gb) == false)
			{
				return;
			}
			gb.DoStateTransition(SelectionState.Normal, true);
			Debug.Log($"{EventSystem.current.currentSelectedGameObject}<statenormal>");
		}
		base.OnSelect(eventData);
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		//Debug.Log($"{eventData == null} / {eventData.selectedObject == null} / {EventSystem.current == null} / {EventSystem.current.currentSelectedGameObject == null}");
		//Debug.Log($"[{EventSystem.current.currentSelectedGameObject?.name}]---[{eventData.selectedObject?.name}]");
		//(eventData.selectedObject == null ? null : eventData.selectedObject.name)
		
		//EventSystem.current.SetSelectedGameObject(null);
		if (EventSystem.current.currentSelectedGameObject != null)
		{
			if (EventSystem.current.currentSelectedGameObject.TryGetComponent<GridButton>(out GridButton gb) == false)
			{
				return;
			}
			gb.DoStateTransition(SelectionState.Normal, true);
			Debug.Log($"{EventSystem.current.currentSelectedGameObject}<statenormal>");
		}
		base.OnPointerEnter(eventData);
	}
	//*/
	#endregion


	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			EventSystem.current.SetSelectedGameObject(gameObject);
			OnButtonSelected.Invoke(this);
			eventData.Reset();
		}
		DoStateTransition(SelectionState.Normal, true);
	}
}

// [TODO] Move the below to its own editor script!
// [Q] Can delete or nah?


#if UNITY_EDITOR
[CustomEditor(typeof(GridButton))]
public class E_GridButton : Editor
{
	public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
	{
		return base.CreateInspectorGUI();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}
#endif
