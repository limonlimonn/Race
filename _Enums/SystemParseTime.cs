namespace HCR
{
    class parz
    {

        public string  result(float time)
        {
            string parsedString = "";
            var timerForShow = time / 100;
            parsedString = System.TimeSpan.FromSeconds(timerForShow).Minutes.ToString() + ":" +
                System.TimeSpan.FromSeconds(timerForShow).Seconds.ToString() + ":" +
                (int)(System.TimeSpan.FromSeconds(timerForShow).Milliseconds / 10);

            return parsedString;
        }
        

    }
}