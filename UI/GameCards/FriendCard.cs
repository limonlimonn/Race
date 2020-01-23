using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine;
namespace HCR
{
    public class FriendCard : MonoBehaviour
    {
        // ACTIONS

        public Action<string,string> OnPlayClick;

        // FIELDS

        [Header("CARD")]
        public Text FriendName;
        public Text FriendID;
        //public Text buttonStatus;
        //public GameObject playButton;

        //used on button !
        public void PlayGame()
        {
            // pre-condition
            Assert.IsTrue(FriendName.text != "");
            Assert.IsTrue(FriendID.text != "");
            if (OnPlayClick != null)
            {
                OnPlayClick(FriendID.text, FriendName.text);
            }
        }

    }
}
