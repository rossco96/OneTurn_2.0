public class InGameStats
{
	public int Lives = 0;                        // Init to 0 since we will likely be allowing to choose 1, 3, 5, or unlimited (= -1 ?) lives
	public int Moves = 0;
	
	public int Items = 0;
	
	public bool IsAtExit = false;
	
	public bool IsChaser = false;
	public int ChaseMovesLeft = 0;

	public bool HasBomb = false;
	public float BombTimeLeft = 0.0f;
}
