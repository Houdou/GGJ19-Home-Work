using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameTime {
	public bool Equals(GameTime other) {
		return Day == other.Day && Hour == other.Hour;
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		return obj is GameTime && Equals((GameTime) obj);
	}

	public override int GetHashCode() {
		unchecked {
			return (Day * 397) ^ Hour;
		}
	}

	public int Day;
	public int Hour;

	public static GameTime zero => new GameTime(0, 0);
	public static GameTime oneHour => new GameTime(0, 1);
	public static GameTime oneDay => new GameTime(1, 0);

	public GameTime(int day, int hour) {
		Day = day;
		Hour = hour;

		if (Hour > Config.DaysInWeek) {
			while (Hour >= Config.HoursInDay) {
				Hour -= Config.HoursInDay;
				Day++;
			}
		}

		if (Hour >= 0) return;

		while (Hour < 0) {
			Hour += Config.HoursInDay;
			Day--;
		}
	}

	public int TotalHourInGame => Day * Config.HoursInDay + Hour;
	public bool IsWeekDay => Day % Config.DaysInWeek < Config.WorkDayInWeek;
	public bool IsNight => Day % 2 == 0;

	public static GameTime operator +(GameTime lhs, GameTime rhs) {
		return new GameTime(lhs.Day + rhs.Day, lhs.Hour + rhs.Hour);
	}

	public static GameTime operator -(GameTime lhs, GameTime rhs) {
		return new GameTime(lhs.Day - rhs.Day, lhs.Hour - rhs.Hour);
	}

	public static bool operator ==(GameTime lhs, GameTime rhs) {
		return lhs.Day == rhs.Day && lhs.Hour == rhs.Hour;
	}

	public static bool operator !=(GameTime lhs, GameTime rhs) {
		return lhs.Day != rhs.Day || lhs.Hour != rhs.Hour;
	}

	public static bool operator <(GameTime lhs, GameTime rhs) {
		return lhs.TotalHourInGame < rhs.TotalHourInGame;
	}

	public static bool operator >(GameTime lhs, GameTime rhs) {
		return lhs.TotalHourInGame > rhs.TotalHourInGame;
	}

	public static bool operator <=(GameTime lhs, GameTime rhs) {
		return lhs.TotalHourInGame <= rhs.TotalHourInGame;
	}

	public static bool operator >=(GameTime lhs, GameTime rhs) {
		return lhs.TotalHourInGame >= rhs.TotalHourInGame;
	}

	public override string ToString() {
		var day = $"Day {Day + 1}";
		var hourRaw = $"00{Hour}";
		var hour = $"{hourRaw.Substring(hourRaw.Length - 2, 2)}:00";
		
		return $"{day} - {hour}";
	}
}
