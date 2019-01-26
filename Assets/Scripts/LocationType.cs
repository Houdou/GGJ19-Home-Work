using System.ComponentModel;

public enum LocationType {
	Null = 0,
	[Description("Home")]Home = 1,
	[Description("Office")]Office = 2,
	[Description("Other")]Other = 3,
}
