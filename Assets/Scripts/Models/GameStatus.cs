using System;

public class GameStatus {
	private GameTime _currentTime;

	public GameTime CurrentTime {
		get { return _currentTime; }
		set {
			var diff = value - _currentTime;
			if (diff != GameTime.zero) {
				OnGameTimeChange?.Invoke(value, diff);
			}

			_currentTime = value;
		}
	}

	public LocationType _location;

	public LocationType Location {
		get {
			return _location;
		}
		set {
			var prev = _location;
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
			if (diff != 0) {
				OnMoneyChange?.Invoke(value, diff);
			}

			_money = value;
		}
	}

	private int _energy;

	public int Energy {
		get { return _energy; }
		set {
			var diff = value - _energy;
			if (diff != 0) {
				OnEnergyChange?.Invoke(value, diff);
			}

			_energy = value;
		}
	}
	
	private int _personalHappiness;

	public int PersonalHappiness {
		get { return _personalHappiness; }
		set {
			var diff = value - _personalHappiness;
			if (diff != 0) {
				OnPersonalHappinessChange?.Invoke(value, diff);
			}

			_personalHappiness = value;
		}
	}
	
	private int _familyHappiness;

	public int FamilyHappiness {
		get { return _familyHappiness; }
		set {
			var diff = value - _familyHappiness;
			if (diff != 0) {
				OnFamilyHappinessChange?.Invoke(value, diff);
			}

			_familyHappiness = value;
		}
	}
	
	private int _career;

	public int Career {
		get { return _career; }
		set {
			var diff = value - _career;
			if (diff != 0) {
				OnCareerChange?.Invoke(value, diff);
			}

			_career = value;
		}
	}
	public event Action<LocationType, LocationType> OnLocationChange;
	public event Action<int, int> OnCareerChange;
	public event Action<GameTime, GameTime> OnGameTimeChange;
	public event Action<int, int> OnMoneyChange;
	public event Action<int, int> OnEnergyChange;
	public event Action<int, int> OnPersonalHappinessChange;
	public event Action<int, int> OnFamilyHappinessChange;

	public int TotalHour => CurrentTime.TotalHourInGame;
	public GameTime RemainingHourToday => new GameTime(0, Config.HoursInDay - CurrentTime.Hour);

	public GameStatus(
		GameTime gameTime = default(GameTime),
		int money = 100,
		int energy = 100,
		int personalHappiness = 80,
		int familyHappiness = 80,
		int career = 0
		) {
		_currentTime = gameTime;
		_money = Money = money;
		_energy = Energy = energy;
		_personalHappiness = PersonalHappiness = personalHappiness;
		_familyHappiness = FamilyHappiness = familyHappiness;
		_career = Career = career;
	}

	public void Merge(StatusChangeData changes) {
		Money += changes.Money;
		Energy += changes.Energy;
		PersonalHappiness += changes.PersonalHappiness;
		FamilyHappiness += changes.FamilyHappiness;
		Career += changes.Career;
		
		if(changes.Location != LocationType.Null) {
			Location = changes.Location;
		}
	}

	public void Replace(GameStatus status, bool triggerEvents = false) {
		if (triggerEvents) {
			CurrentTime = status.CurrentTime;
			Energy = status.Energy;
			Money = status.Money;
		} else {
			_currentTime = status.CurrentTime;
			_energy = status.Energy;
			_money = status.Money;
		}

	}
}