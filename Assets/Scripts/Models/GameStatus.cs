using System;
using TMPro;
using UnityEngine;

public class GameStatus {
	#region Properties	
	private GameTime _currentTime;

	public GameTime CurrentTime {
		get { return _currentTime; }
		set {
			var diff = value - _currentTime;
			_currentTime = value;
			if (diff != GameTime.zero) {
				OnGameTimeChange?.Invoke(value, diff);
			}

		}
	}

	public LocationType _location;

	public LocationType Location {
		get {
			return _location;
		}
		set {
			if (value == LocationType.Null)
				return;
			
			var prev = _location;
			_location = value;
			if (value != prev) {
				OnLocationChange?.Invoke(value, prev);
			}

		}
	}

	private int _money;

	public int Money {
		get { return _money; }
		set {
			var diff = value - _money;
			_money = value;
			if (diff != 0) {
				OnMoneyChange?.Invoke(value, diff);
			}
		}
	}

	private int _energy;

	public int Energy {
		get { return _energy; }
		set {
			var next = value;
			if (value > 100) {
				next = 100;
			}

			if (value < 0) {
				next = 0;
			}
			
			var diff = next - _energy;
			_energy = next;
			if (diff != 0) {
				OnEnergyChange?.Invoke(next, diff);
			}

		}
	}
	
	private int _personalHappiness;

	public int PersonalHappiness {
		get { return _personalHappiness; }
		set {
			var next = value;
			if (value > 100) {
				next = 100;
			}

			if (value < 0) {
				next = 0;
			}
			var diff = next - _personalHappiness;
			_personalHappiness = next;
			if (diff != 0) {
				OnPersonalHappinessChange?.Invoke(next, diff);
			}

		}
	}
	
	private int _familyHappiness;

	public int FamilyHappiness {
		get { return _familyHappiness; }
		set {
			var next = value;
			if (value > 100) {
				next = 100;
			}

			if (value < 0) {
				next = 0;
			}
			var diff = next - _familyHappiness;
			_familyHappiness = next;
			if (diff != 0) {
				OnFamilyHappinessChange?.Invoke(next, diff);
			}

		}
	}
	
	private int _career;

	public int Career {
		get { return _career; }
		set {
			
			var next = value;
			if (value > 100) {
				next = 100;
			}

			if (value < 0) {
				next = 0;
			}

			var diff = next - _career;
			_career = next;
			if (diff != 0) {
				OnCareerChange?.Invoke(next, diff);
			}

		}
	}
	
	private int _projectProgress;

	public int ProjectProgress {
		get { return _projectProgress; }
		set {
			var diff = value - _projectProgress;
			_projectProgress = value;
			if (diff != 0) {
				OnProjectProgressChange?.Invoke(value, diff);
			}
		}
	}
	public event Action<LocationType, LocationType> OnLocationChange;
	public event Action<GameTime, GameTime> OnGameTimeChange;
	public event Action<int, int> OnMoneyChange;
	public event Action<int, int> OnEnergyChange;
	public event Action<int, int> OnPersonalHappinessChange;
	public event Action<int, int> OnFamilyHappinessChange;
	public event Action<int, int> OnCareerChange;
	public event Action<int, int> OnProjectProgressChange;
	

	public int TotalHour => CurrentTime.TotalHourInGame;
	public GameTime RemainingHourToday => new GameTime(0, Config.HoursInDay - CurrentTime.Hour);
	
	#endregion

	public GameStatus(
		GameTime gameTime = default(GameTime),
		LocationType location = LocationType.Home,
		int money = 100,
		int energy = 100,
		int personalHappiness = 80,
		int familyHappiness = 80,
		int career = 0,
		int projectProgress = 0
		) {
		_currentTime = gameTime;
		_location = Location = location;
		_money = Money = money;
		_energy = Energy = energy;
		_personalHappiness = PersonalHappiness = personalHappiness;
		_familyHappiness = FamilyHappiness = familyHappiness;
		_career = Career = career;
		_projectProgress = ProjectProgress = projectProgress;
	}

	public void Merge(StatusChangeData changes) {
		if (changes.OverrideMoney) {
			Money = changes.Money;
		}
		else {
			Money += changes.Money;
		}

		if (changes.OverrideEnergy) {
			Energy = changes.Energy;
		}
		else {
			Energy += changes.Energy;
		}

		if (changes.OverridePersonalHappiness) {
			PersonalHappiness = changes.PersonalHappiness;
		}
		else {
			PersonalHappiness += changes.PersonalHappiness;
		}

		if (changes.OverrideFamilyHappiness) {
			FamilyHappiness = changes.FamilyHappiness;
		}
		else {
			FamilyHappiness += changes.FamilyHappiness;
		}

		if (changes.OverrideCareer) {
			Career = changes.Career;
		}
		else {
			Career += changes.Career;
		}

		if (changes.OverrideProjectProgress) {
			ProjectProgress = changes.ProjectProgress;
		}
		else {
			ProjectProgress += changes.ProjectProgress;
		}

		if (changes.OverrideLocation && changes.Location != LocationType.Null) {
			Location = changes.Location;
		}
	}

	public void Replace(GameStatus status, bool triggerEvents = false) {
		if (triggerEvents) {
			CurrentTime = status.CurrentTime;
			Location = status.Location;
			Money = status.Money;
			Energy = status.Energy;
			PersonalHappiness = status.PersonalHappiness;
			FamilyHappiness = status.FamilyHappiness;
			Career = status.Career;
			ProjectProgress = status.ProjectProgress;
		} else {
			_currentTime = status.CurrentTime;
			_location = status.Location;
			_money = status.Money;
			_energy = status.Energy;
			_personalHappiness = status.PersonalHappiness;
			_familyHappiness = status.FamilyHappiness;
			_career = status.Career;
			_projectProgress = status.ProjectProgress;
		}

	}

	public void Tick() {
		OnGameTimeChange?.Invoke(CurrentTime, GameTime.zero);
	}
}