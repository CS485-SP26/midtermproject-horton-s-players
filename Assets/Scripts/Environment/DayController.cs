using UnityEngine;
using TMPro; // Important for TextMeshPro
using UnityEngine.Events;
using Farming;

namespace Environment 
{
    public class DayController : MonoBehaviour
    {
        public enum DayOfWeek
        {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

        public enum Season
        {
            Spring,
            Summer,
            Fall,
            Winter
        }

        [Header("Object References")]
        [SerializeField] private Light sunLight;
        [SerializeField] private TMP_Text dayLabel;
        
        [Header("Time Constraints")]
        [SerializeField] private float dayLengthSeconds = 60f;
        [SerializeField] private float dayProgressSeconds = 0f; // good for debugging from the editor
        [SerializeField] private int currentDay = 1; // Good for debugging from the editor

        [Header("Season Settings")]
        [SerializeField, Min(1)] private int weeksPerSeason = 2;
        [SerializeField, Min(1)] private int springWitherDays = 3;
        [SerializeField, Min(1)] private int summerWitherDays = 2;
        [SerializeField, Min(1)] private int fallWitherDays = 3;
        [SerializeField, Min(1)] private int winterWitherDays = 1;
        [SerializeField] private Color springSunTint = new Color(1f, 0.95f, 0.8f, 1f);
        [SerializeField] private Color summerSunTint = new Color(1f, 0.98f, 0.86f, 1f);
        [SerializeField] private Color fallSunTint = new Color(1f, 0.82f, 0.62f, 1f);
        [SerializeField] private Color winterSunTint = new Color(0.82f, 0.9f, 1f, 1f);

        // Properties
        public float DayProgressPercent => Mathf.Clamp01(dayProgressSeconds / dayLengthSeconds);
        public int CurrentDay { get { return currentDay; } } 
        public DayOfWeek CurrentDayOfWeek => (DayOfWeek)((currentDay - 1) % 7);
        public Season CurrentSeason => CalculateSeason(currentDay);
        public int CurrentWeekInSeason => CalculateWeekInSeason(currentDay);
        public int WeeksPerSeason => weeksPerSeason;

        public UnityEvent dayPassedEvent = new UnityEvent(); // Invoke() at end of day

        private string BuildDayLabelText()
        {
            return $"Day {currentDay} • {CurrentDayOfWeek} • {CurrentSeason} W{CurrentWeekInSeason}";
        }

        void Start()
        {
            currentDay = Mathf.Max(1, GameManager.Instance.GetSavedDayCount());
            GameManager.Instance.SaveDayCount(currentDay);

            if (dayLabel)
            {
                dayLabel.SetText(BuildDayLabelText());
            }

            UpdateVisuals();
        }

        public void AdvanceDay()
        {
            Debug.Assert(sunLight, "DayController requires a 'Sun'");
            if (dayLabel == null) Debug.Log("DayController does not have a label to update");

            dayProgressSeconds = 0f; // Reset to start a new day
            currentDay++;
            
            if (dayLabel)
            {
                dayLabel.SetText(BuildDayLabelText());
            }

            GameManager.Instance.SaveDayCount(currentDay);

            dayPassedEvent.Invoke(); //make announcement to all listeners
        }

        public int GetWitherDaysForCurrentSeason()
        {
            return GetWitherDaysForSeason(CurrentSeason);
        }

        public int GetWitherDaysForSeason(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return springWitherDays;
                case Season.Summer:
                    return summerWitherDays;
                case Season.Fall:
                    return fallWitherDays;
                case Season.Winter:
                    return winterWitherDays;
                default:
                    return springWitherDays;
            }
        }

        private Season CalculateSeason(int absoluteDay)
        {
            int daysPerSeason = Mathf.Max(1, weeksPerSeason) * 7;
            int seasonIndex = ((Mathf.Max(1, absoluteDay) - 1) / daysPerSeason) % 4;
            return (Season)seasonIndex;
        }

        private int CalculateWeekInSeason(int absoluteDay)
        {
            int safeWeeksPerSeason = Mathf.Max(1, weeksPerSeason);
            int seasonDayIndex = (Mathf.Max(1, absoluteDay) - 1) % (safeWeeksPerSeason * 7);
            return (seasonDayIndex / 7) + 1;
        }

        private Color GetSunTintForSeason(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return springSunTint;
                case Season.Summer:
                    return summerSunTint;
                case Season.Fall:
                    return fallSunTint;
                case Season.Winter:
                    return winterSunTint;
                default:
                    return Color.white;
            }
        }

        public void UpdateVisuals()
        {
            // Calculate sun's rotation based on time of day
            // 0 degrees for sunrise, 180 for sunset, 360 for next sunrise
            float sunRotationX = Mathf.Lerp(0f, 360f, DayProgressPercent);

            // Apply rotation to the sun light
            sunLight.transform.rotation = Quaternion.Euler(sunRotationX, 0f, 0f);
            sunLight.color = GetSunTintForSeason(CurrentSeason);

            // Optional: Adjust other elements, like skybox, light source intensity, and so on
            // sunLight.intensity = 
            // RenderSettings.fogColor = 
            // RenderSettings.skybox.SetFloat
        }

        void Update()
        {
            if (dayLengthSeconds <= 0f)
            {
                return;
            }

            dayProgressSeconds += Time.deltaTime;

            if (dayProgressSeconds >= dayLengthSeconds)
            {
                AdvanceDay();
            }

            UpdateVisuals();
        }
    }
}