using System.Collections.Generic;

using HCR.Enums;
using UnityEngine;

namespace HCR
{
	[System.Serializable]
	public class GameData
	{
		public string gameId;
		public string track_id;
		public GameTypeEnum GameType;

        public List<Vector4> ghostData = new List<Vector4>();
        public Dictionary<int, Dictionary<int, RecordableObjectModel>> ObjectsReplayGhost = new Dictionary<int, Dictionary<int, RecordableObjectModel>>();
        public string replayData = null;
		public string recordData = null;

		public string player1_Id;
		public string player1_name;
		public PlayerCarModel player1_car;
		public int player1_score;

		public string player2_id;
		public string player2_name;
		public PlayerCarModel player2_car;
		public int player2_score;

		public string last_player;

		public string last_update;
		public string replay_id;

		public int gameResultScore;
        private int tryesCount = 3;
        public int GetTryes
        {
            get
            {
                return tryesCount;
            }
        }
        public int IsFinishedEnemy = 0;
        public int IsFinishedRecord = 0;
        public int IsFinishedReplay = 0;
        public int IsDoneReplay = 0;
        public int IsDone = 0;
        public int AppyTrack = 0; // bool 

		public List<int> smiles;



		public GameData()
		{
		}

        public void AddTriesCount()
        {
            tryesCount += 1;
        }

        public void RemoveTriesCount()
        {
            tryesCount -= 1;
            Debug.LogError("RemoveTriesCount " + tryesCount);
        }

        public override string ToString()
		{
			return string.Format("[GameData] gameId : {0} GameTypeEnum : {1} last_player : {2}", gameId, GameType, last_player);
		}



	}
}