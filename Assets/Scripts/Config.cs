using System.Collections.Generic;
using UnityEngine;

public static class Config {
	public static readonly int WorkDayInWeek = 6;
	public static readonly int RestDayInWeek = 2;
	public static readonly int DaysInWeek = RestDayInWeek + WorkDayInWeek;

	public static readonly int HoursInDay = 10;
	public static readonly float HoursInRealSecond = 0.5f;

	public static readonly int VerticalStep = 125;

	public static readonly Color SpriteFadeOutColor = new Color(1f, 1f, 1f, 0f);
	public static readonly Color SpriteFadeInColor = new Color(1f, 1f, 1f, 1f);
	
	public static readonly List<string> EventCrossDay = new List<string> {
		{"Rest"},
	};
}