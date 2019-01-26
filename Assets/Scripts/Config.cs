using System.Collections.Generic;

public static class Config {
	public static readonly int WorkDayInWeek = 6;
	public static readonly int RestDayInWeek = 2;
	public static readonly int DaysInWeek = RestDayInWeek + WorkDayInWeek;

	public static readonly int HoursInDay = 10;
	public static readonly float HoursInRealSecond = 0.5f;

	public static readonly int VerticalStep = 125;
	
	public static readonly List<string> EventCrossDay = new List<string> {
		{"Rest"},
	};
}