// [TODO] Turn this into a struct (remove private/public GET SET layout. Make all vars public, end of!)

public class InGameStats
{
	private int m_lives = 0;                        // Init to 0 since we will likely be allowing to choose 1, 3, 5, or unlimited (= -1 ?) lives
	public int Lives => m_lives;
	private int m_moves = 0;
	public int Moves => m_moves;

	private int m_items = 0;
	public int Items => m_items;

	private bool m_isAtExit = false;
	public bool IsAtExit => m_isAtExit;

	private bool m_isChaser = false;
	private int m_chaseMovesLeft = 0;

	private bool m_hasBomb = false;
	private float m_bombTimeLeft = 0.0f;



	public void SetLives(int startingLives) { m_lives = startingLives; }
	public bool RemoveLife() { m_lives--; return m_lives > 0; }

	public void AddMove() { m_moves++; }


	public void AddItem() { m_items++; }

	public void SetAtExit() { m_isAtExit = true; }


	public void SetIsChaser(bool isChaser) { m_isChaser = isChaser; }				// [Q] Do we need to know this here? What about m_isChaseTurn?
	public void SetNumberOfChaseMoves(int moves) { m_chaseMovesLeft = moves; }
	public bool TakeChaseMove() { m_chaseMovesLeft--; return m_chaseMovesLeft > 0; }


	public void SetBombTime(int time) { m_bombTimeLeft = time; }
	public void SetHasBomb(bool hasBomb) { m_hasBomb = hasBomb; }
	public void ReduceBombTime(float deltaTime) { m_bombTimeLeft -= deltaTime; }
}
