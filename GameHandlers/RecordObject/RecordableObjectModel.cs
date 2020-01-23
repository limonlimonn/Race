using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCR
{

    public class RecordableObjectModel
    {
        public Vector3 RecordedPosition;
        public Quaternion RecordedRotation;
        //public int CurrentFrameIndex = 0;

        public RecordableObjectModel()
        {
              
        }

        public RecordableObjectModel(Vector3 _position, Quaternion _rotation)
        {
            RecordedPosition = _position;
            RecordedRotation = _rotation;
        }
    }

   

}
