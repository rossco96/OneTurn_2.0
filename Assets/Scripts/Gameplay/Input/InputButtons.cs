// [TODO][Q] Delete this class? Can we just set m_input to null inside OTController if we've chosen buttons mode, or nah?

public class InputButtons : Input_Base
{
	public override bool Check(out EMovement movement)
	{
		movement = EMovement.Forward;
		return false;
	}
}
