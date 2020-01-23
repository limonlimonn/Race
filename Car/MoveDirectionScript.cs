using UnityEngine;

namespace HCR
{
	public class MoveDirectionScript : MonoBehaviour
	{

		[HideInInspector]
		public float movement = 0f;

		public float brake = 0f;

		public bool isRB = false;

		public bool isMenu = false;

		[HideInInspector]
		public bool isRacePressed = false;
		[HideInInspector]
		public bool isbrakePressed = false;
		[HideInInspector]
		public float rot = 0;
		[HideInInspector]
		public bool isRotPressed = false;
		[HideInInspector]
		public bool isRotBPressed = false;
		// Use this for initialization
		public bool IsMenu()
		{
			return isMenu;
		}

		public void BrakeTorque()
		{
			isRB = true;
		}
		public void BrakeTorque1()
		{
			isRB = false;
		}
		public void IsRaceBPressed() // Если кнопка нажата происходит движение автомобиля вперед
		{
			isRacePressed = true;
		}
		public void IsRaceBUnPressed() // Если кнопка не нажата автомобиль двигается с прежней (набранной) скоростью
		{
			isRacePressed = false;
		}
		public void IsRaceB1Pressed() // Если кнопка нажата происходит движение автомобиля в обратную сторону
		{
			isbrakePressed = true;
		}
		public void IsRaceB1UnPressed() // Если кнопка не нажата происходит автомобиль движется в обратную сторону с прежней (набранной) скоростью
		{
			isbrakePressed = false;
		}
		public void IsRotBPressed() // Если кнопка нажата происходит движение автомобиля вперед
		{
			isRotPressed = true;
		}
		public void IsRotBUnPressed() // Если кнопка не нажата автомобиль двигается с прежней (набранной) скоростью
		{
			isRotPressed = false;
		}
		public void IsRotB1Pressed() // Если кнопка нажата происходит движение автомобиля в обратную сторону
		{
			isRotBPressed = true;
		}
		public void IsRotB1UnPressed() // Если кнопка не нажата происходит автомобиль движется в обратную сторону с прежней (набранной) скоростью
		{
			isRotBPressed = false;
		}
		void Update () {
			if (isRB)
			{
				brake = 1;
			}
			if (!isRB)
			{
				brake = 0;
			}
			if (isRacePressed)
			{
				movement = 1;
			}
			if (!isRacePressed)
			{
				movement = 0;
			}
			if (isbrakePressed)
			{
				movement = -1;
			}
			if (isRotPressed)
			{
				rot = 1;
			}
			if (!isRotPressed)
			{
				rot = 0;
			}
			if (isRotBPressed)
			{
				rot = -1;
			}
		}



	}
}