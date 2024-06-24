using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class GridButton : Button
{
	private UnityAction<GridButton> OnButtonSelected;
	public void RegisterOnButtonSelected(UnityAction<GridButton> onButtonSelected)
	{ 
		OnButtonSelected += onButtonSelected; 
	}
	
	private Color m_propertyColor = Color.white;
	public Color PropertyColor => m_propertyColor;
	public void SetPropertyColor(Color color)
	{ 
		m_propertyColor = color;
		// [TODO] Implement setting the sprite associated with said colour
		// DEBUG FOR NOW -- Just set the button colour
		image.color = color;
	}

	private SelectionState m_previousState = SelectionState.Normal;
	private bool m_buttonClicked = false;


	#region [other override methods]
	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		// [NOTE] This feels very hacky, but absolutely (finally!) gives the desried workaround to navigate the grid

		if (m_buttonClicked)
		{
			m_buttonClicked = false;
			DoStateTransition(SelectionState.Normal, true);
			return;
		}

		if (state == SelectionState.Selected)
		{
			return;
		}

		if (state == SelectionState.Pressed && m_previousState == SelectionState.Pressed)
		{
			DoStateTransition(SelectionState.Normal, true);
			return;
		}

		m_previousState = state;
		base.DoStateTransition(state, instant);
	}


	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			m_buttonClicked = true;
			OnButtonSelected.Invoke(this);
		}
	}
	#endregion
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
