using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HCR.Interfaces;

namespace HCR
{
    public class ReplyObjectController : IService
    {
        // FIELDS
        private List<Rigidbody> _objectsToReplay;

        private List<int> TotalFrameCount;

        public Dictionary<int, Dictionary<int, RecordableObjectModel>> ObjectsReplayInfo;

        public bool IsFind = false;
        public int offset;

        //METHODS
        public void Init()
        {


            if (GameObject.Find("ReplayListContainer") != null)
            {
                TotalFrameCount = new List<int>();
                ObjectsReplayInfo = new Dictionary<int, Dictionary<int, RecordableObjectModel>>();
                offset = 0;
                IsFind = true;
                _objectsToReplay = GameObject.Find("ReplayListContainer").GetComponent<ReplayList>().GetList();
            }

        }

        public void GetReplayDict(byte[] _rawData)
        {
            int localoffset = 0;
            for (int i = 0; i < _objectsToReplay.Count; i++)
            {


                //char[] chars = { BitConverter.ToChar(_rawData, localoffset), BitConverter.ToChar(_rawData, localoffset + 2),
                //BitConverter.ToChar(_rawData, localoffset+ 4), BitConverter.ToChar(_rawData, localoffset+ 6),
                //BitConverter.ToChar(_rawData, localoffset+ 8), BitConverter.ToChar(_rawData, localoffset+ 10),
                //BitConverter.ToChar(_rawData, localoffset+ 12), BitConverter.ToChar(_rawData, localoffset+ 14),
                //BitConverter.ToChar(_rawData, localoffset+ 16), BitConverter.ToChar(_rawData, localoffset + 18),
                //BitConverter.ToChar(_rawData, localoffset+ 20), BitConverter.ToChar(_rawData, localoffset+ 22),
                //BitConverter.ToChar(_rawData, localoffset+ 24), BitConverter.ToChar(_rawData, localoffset+ 26),
                //BitConverter.ToChar(_rawData, localoffset+ 28) };

                //localoffset += 30;

                ObjectsReplayInfo.Add(i, new Dictionary<int, RecordableObjectModel>());
            }


            foreach (var val in _objectsToReplay)
            {
                TotalFrameCount.Add(BitConverter.ToInt32(_rawData, localoffset));
                localoffset += 4;
            }

     
            for (int j = 0; j < ObjectsReplayInfo.Keys.Count; j++)
            {
                for (int i = 0; i < TotalFrameCount[j]; i++)
                {
                    try
                    {
                        ObjectsReplayInfo[j].Add(
                            BitConverter.ToInt32(_rawData, localoffset),
                                new RecordableObjectModel(
                                    new Vector3(
                                        BitConverter.ToSingle(_rawData, localoffset + 4),
                                        BitConverter.ToSingle(_rawData, localoffset + 8),
                                        -2f),
                                    new Quaternion(
                                        BitConverter.ToSingle(_rawData, localoffset + 12),
                                        BitConverter.ToSingle(_rawData, localoffset + 16),
                                        BitConverter.ToSingle(_rawData, localoffset + 20),
                                        BitConverter.ToSingle(_rawData, localoffset + 24)
                                        )
                                )
                        );
                       
                    }
                    catch (Exception error)
                    {
                        Debug.LogError("Exception Data : "+ error.GetType() + "  |||  " +  error.ToString());
                    }
                    localoffset += 28;

                }
            }


            //foreach (var key in ObjectsReplayInfo.Keys)
            //{
            //    for (int i = 0; i < TotalFrameCount[key + "Replay"]; i++)
            //    {
            //        ObjectsReplayInfo[key].Add(BitConverter.ToInt32(_rawData, localoffset), new RecordableObjectModel(
            //        new Vector3(BitConverter.ToSingle(_rawData, localoffset + 4), BitConverter.ToSingle(_rawData, localoffset + 8),
            //        -2f), new Quaternion(BitConverter.ToSingle(_rawData, localoffset + 12),
            //        BitConverter.ToSingle(_rawData, localoffset + 16), BitConverter.ToSingle(_rawData, localoffset + 20),
            //        BitConverter.ToSingle(_rawData, localoffset + 24))));
            //        localoffset += 28;
            //    }
            //}
            offset = BitConverter.ToInt32(_rawData, localoffset);
        }

        public void SetObjectTransformDict(List<Vector4> _replayData)
        {
            RecordableObjectModel PreviosTransform;

            for (int j = 0; j < ObjectsReplayInfo.Keys.Count; j++)
            {
                PreviosTransform = new RecordableObjectModel(_objectsToReplay[j].transform.position,
                    _objectsToReplay[j].transform.rotation);
                for (int i = 0; i < _replayData.Count; i++)
                {

                    if (ObjectsReplayInfo[j].ContainsKey(i))
                    {
                        PreviosTransform = ObjectsReplayInfo[j][i];
                        continue;
                    }

                    ObjectsReplayInfo[j].Add(i, PreviosTransform);

                }
            }
        }

        public void AplyRecord(int frame)
        {
            for (int i = 0; i < ObjectsReplayInfo.Keys.Count; i++)
            {
                _objectsToReplay[i].transform.position = ObjectsReplayInfo[i][frame].RecordedPosition;
                _objectsToReplay[i].transform.rotation = ObjectsReplayInfo[i][frame].RecordedRotation;
            }
        }
        public void DisablePhysics()
        {
            for (int i = 0; i < _objectsToReplay.Count; i++)
            {
                _objectsToReplay[i].GetComponent<CapsuleCollider>().enabled = false;
                _objectsToReplay[i].GetComponent<Rigidbody>().useGravity = false;
            }
        }

        public void EnablePhysics()
        {
            for (int i = 0; i < _objectsToReplay.Count; i++)
            {
                _objectsToReplay[i].GetComponent<CapsuleCollider>().enabled = true;
                _objectsToReplay[i].GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }
}