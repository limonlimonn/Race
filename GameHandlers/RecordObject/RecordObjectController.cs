using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCR.Interfaces;
using System;


namespace HCR
{
    public class RecordObjectController : IService
    {
        // FIELDS
        public Dictionary<int, Dictionary<int, RecordableObjectModel>> ObjectsRecordInfo;
        private Dictionary<int, RecordableObjectModel> ObjectsRecordInfoPreviousFrame;
        private List<Rigidbody> _objectsToRecord;

        public bool IsFind = false;
        public int CurrentFrameIndex;
        private List<int> TotalFrameCount;
        public int offset;

        //METHODS

        public void Init()
        {

            if (GameObject.Find("ReplayListContainer") != null)
            {
                CurrentFrameIndex = 0;
                offset = 4;
                ObjectsRecordInfo = new Dictionary<int, Dictionary<int, RecordableObjectModel>>();
                ObjectsRecordInfoPreviousFrame = new Dictionary<int, RecordableObjectModel>();
                TotalFrameCount = new List<int>();
                IsFind = true;
                _objectsToRecord = GameObject.Find("ReplayListContainer").GetComponent<RecordList>().GetList();

                for (int i = 0; i < _objectsToRecord.Count; i++)
                {
                    ObjectsRecordInfo.Add(i, new Dictionary<int, RecordableObjectModel>());
                    ObjectsRecordInfoPreviousFrame.Add(i, null);
                    TotalFrameCount.Add(0);
                    offset += 4;
                }
            }

        }

        public void RecordObjectInFrame()
        {
            int index = 0;
            foreach (Rigidbody val in _objectsToRecord)
            {
                RecordableObjectModel model = new RecordableObjectModel();
                model.RecordedPosition = val.transform.position;
                model.RecordedPosition.z = -2;
                model.RecordedRotation = val.transform.rotation;

                RecordableObjectModel PreviousFrameModel = ObjectsRecordInfoPreviousFrame[index];
                if (PreviousFrameModel != null)
                {
                    if (val.transform.position == PreviousFrameModel.RecordedPosition &&
                        val.transform.rotation == PreviousFrameModel.RecordedRotation)
                    {
                        index++;
                        continue;
                    }
                }
                ObjectsRecordInfo[index].Add(CurrentFrameIndex, model);
                offset += 28;
                ObjectsRecordInfoPreviousFrame[index] = model;
                index++;
            }
            CurrentFrameIndex++;
        }

        public List<int> GetTotalFrameForObjects()
        {
            if (ObjectsRecordInfo.Count != 0)
            {
                for (int i = 0; i < ObjectsRecordInfo.Keys.Count; i++)
                {
                    TotalFrameCount[i] = ObjectsRecordInfo[i].Keys.Count;
                }
            }
            return TotalFrameCount;
        }

    }
}