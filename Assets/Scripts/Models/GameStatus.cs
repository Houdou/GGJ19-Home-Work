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

	public event Action<int, int> OnEnergyChange;
	public event Action<int, int> OnMoneyChange;
	public event Action<GameTime, GameTime> OnGameTimeChange;

	public int TotalHour => CurrentTime.TotalHourInGame;
	public GameTime RemainingHourToday => new GameTime(0, Config.HoursInDay - CurrentTime.Hour);

	public GameStatus(GameTime gameTime = default(GameTime), int money = 100, int energy = 100) {
		_currentTime = gameTime;
		_money = Money = money;
		_energy = Energy = energy;
	}

	public void Merge(StatusChangeData statusChange) {
		Money += statusChange.Money;
		Energy += statusChange.Energy;
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