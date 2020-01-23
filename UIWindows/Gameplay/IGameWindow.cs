using UnityEngine;
using UnityEngine.UI;

namespace HCR.Interfaces
{
	public interface IGameWindow
	{

		Text Get_TimerText();
		void Set_TimerText(string value);

		Button Get_PauseButton();

		int Get_TriesCount();
		void Set_TriesCount(int value);

		void Set_TriesSpriteListValue(int id, Sprite sprite);

		Sprite Get_SpriteLostTrie();
		Sprite Get_SpriteTrie();

		Text Get_TextRaceTimer();

	}
}