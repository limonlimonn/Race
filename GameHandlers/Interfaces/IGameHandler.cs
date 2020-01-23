
namespace HCR.Interfaces
{
	public interface IGameHandler
	{


		

		/// <summary>
		/// Instantiate car, get components
		/// </summary>
		void Init(GameData data);

		/// <summary>
		/// Start timer, enable controll
		/// </summary>
		void Start();

		/// <summary>
		///From Movement controll
		/// </summary>
		void OnPlayerCrash();

		/// <summary>
		///From Pause menu
		/// </summary>
		void Restart();

		/// <summary>
		///On trigger finish line
		/// </summary>
		void OnPlayerFinish();

		/// <summary>
		///Calculate winner and score
		/// </summary>
		void CalculateResults();

		/// <summary>
		///Destroy cars, unsubscribe from actions
		/// </summary>
		void Destroy();


	}
}