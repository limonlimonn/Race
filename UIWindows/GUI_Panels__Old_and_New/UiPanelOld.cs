
using UnityEngine;

namespace HCR
{
	/// <summary>
	/// Класс - старый UI для геймплея
	/// </summary>

	public class UiPanelOld : ABaseUiPanel
	{

		public void UpdateTriesView(int tries)
        {
           
            Sprite lostTrie = Get_LostTrie();
            Sprite trie = Get_Trie();
            Debug.Log("UpdateTriesView" + tries);
            switch (tries)
            {
                case 0:
                    triesSprite[0].sprite = lostTrie;
                    triesSprite[1].sprite = lostTrie;
                    triesSprite[2].sprite = lostTrie;
                    break;
                case 1:
                    triesSprite[0].sprite = lostTrie;
                    triesSprite[1].sprite = lostTrie;
                    triesSprite[2].sprite = trie;
                    break;

                case 2:
                    triesSprite[0].sprite = lostTrie;
                    triesSprite[1].sprite = trie;
                    triesSprite[2].sprite = trie;
                    break;

                case 3:
                    triesSprite[0].sprite = trie;
                    triesSprite[1].sprite = trie;
                    triesSprite[2].sprite = trie;
                    break;

                default:
                    #region DEBUG
#if UNITY_EDITOR
                    Debug.Log("[ERROR] wrong tries count = " + tries + " (must be 1 || 2 || 3)");
#endif
                    #endregion
                    break;
            }
        }

        public void BlockPauseButton()
        {
            pauseButton.interactable = false;
        }

        public void UnblockPauseButton()
        {
            pauseButton.interactable = true;
        }
    }
}