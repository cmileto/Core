#region Header
//   Vorspire    _,-'/-'/  Info.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2016  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Linq;

using Server;
#endregion

namespace VitaNex.Schedules
{
	[PropertyObject]
	public class ScheduleInfo
	{
		private static readonly TimeSpan _OneDay = TimeSpan.FromDays(1.0);

		[CommandProperty(Schedules.Access)]
		public ScheduleMonths Months { get; set; }

		[CommandProperty(Schedules.Access)]
		public ScheduleDays Days { get; set; }

		[CommandProperty(Schedules.Access)]
		public ScheduleTimes Times { get; set; }

		public ScheduleInfo(
			ScheduleMonths months = ScheduleMonths.All,
			ScheduleDays days = ScheduleDays.All,
			ScheduleTimes times = null)
		{
			Months = months;
			Days = days;
			Times = times ?? new ScheduleTimes();
		}

		public override string ToString()
		{
			return "Schedule Info";
		}

		public ScheduleInfo(GenericReader reader)
		{
			Deserialize(reader);
		}

		public virtual void Clear()
		{
			Months = ScheduleMonths.None;
			Days = ScheduleDays.None;

			Times.Clear();
		}

		public virtual bool HasMonth(ScheduleMonths month)
		{
			return Months.HasFlag(month);
		}

		public virtual bool HasDay(ScheduleDays day)
		{
			return Days.HasFlag(day);
		}

		public virtual bool HasTime(TimeSpan time)
		{
			Validate(ref time);

			return Times.Contains(time);
		}

		public virtual void Validate(ref TimeSpan time)
		{
			time = new TimeSpan(0, time.Hours, time.Minutes, 0, 0);
		}

		public virtual void Validate(ref DateTime dt, out TimeSpan time)
		{
			Validate(ref dt);

			time = new TimeSpan(0, dt.TimeOfDay.Hours, dt.TimeOfDay.Minutes, 0, 0);
		}

		public virtual void Validate(ref DateTime dt)
		{
			if (!dt.Kind.HasFlag(DateTimeKind.Utc))
			{
				dt = dt.ToUniversalTime();
			}

			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.TimeOfDay.Hours, dt.TimeOfDay.Minutes, 0, 0);
		}

		public virtual DateTime? FindBefore(DateTime dt)
		{
			if (Months == ScheduleMonths.None || Days == ScheduleDays.None || Times.Count == 0)
			{
				return null;
			}

			TimeSpan ts;
			Validate(ref dt, out ts);

			try
			{
				var past = false;

				for (var year = dt.Year; year >= dt.Year - 1; year--)
				{
					for (var month = past ? 12 : dt.Month; month >= 1; month--)
					{
						if (!HasMonth(Schedules.ConvertMonth(month)))
						{
							past = true;
							continue;
						}

						var start = new DateTime(year, month, past ? DateTime.DaysInMonth(year, month) : dt.Day);
						var end = new DateTime(year, month, 1);

						for (var date = start; date >= end; date -= _OneDay)
						{
							if (!HasDay(Schedules.ConvertDay(date.DayOfWeek)))
							{
								past = true;
								continue;
							}

							foreach (var time in Times.Where(t => past || t < ts).OrderByDescending(t => t.Ticks))
							{
								return new DateTime(year, month, date.Day, time.Hours, time.Minutes, 0, DateTimeKind.Utc);
							}

							past = true;
						}
					}

					past = true;
				}
			}
			catch (Exception e)
			{
				VitaNexCore.Catch(e);
			}

			return null;
		}

		public virtual DateTime? FindAfter(DateTime dt)
		{
			if (Months == ScheduleMonths.None || Days == ScheduleDays.None || Times.Count == 0)
			{
				return null;
			}

			TimeSpan ts;
			Validate(ref dt, out ts);

			try
			{
				var future = false;

				for (var year = dt.Year; year <= dt.Year + 1; year++)
				{
					for (var month = future ? 1 : dt.Month; month <= 12; month++)
					{
						if (!HasMonth(Schedules.ConvertMonth(month)))
						{
							future = true;
							continue;
						}

						var start = new DateTime(year, month, future ? 1 : dt.Day);
						var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));

						for (var date = start; date <= end; date += _OneDay)
						{
							if (!HasDay(Schedules.ConvertDay(date.DayOfWeek)))
							{
								future = true;
								continue;
							}

							foreach (var time in Times.Where(t => future || t > ts).OrderBy(t => t.Ticks))
							{
								return new DateTime(year, month, date.Day, time.Hours, time.Minutes, 0, DateTimeKind.Utc);
							}

							future = true;
						}
					}

					future = true;
				}
			}
			catch (Exception e)
			{
				VitaNexCore.Catch(e);
			}

			return null;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteFlag(Months);
					writer.WriteFlag(Days);
					writer.WriteType(Times, t => Times.Serialize(writer));
				}
					break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
				{
					Months = reader.ReadFlag<ScheduleMonths>();
					Days = reader.ReadFlag<ScheduleDays>();
					Times = reader.ReadTypeCreate<ScheduleTimes>(reader) ?? new ScheduleTimes();
				}
					break;
			}
		}
	}
}