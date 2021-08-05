using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using COTG.Views;
using static COTG.Game.Enum;
namespace COTG.Game
{
	public enum AttackType : byte
	{
		none,
		// only valid as attack type
		assault,

		senator,
		se,

		// only valid as target type
		senatorFake,
		seFake,
		invalid,
	}
	public enum AttackCategory
	{
		se,
		senator,
		count,
		invalid=count,
	}
	//  public class Attack
	//  {
	//public const int attackClusterNone = -1;
	//public const int attackClusterSEStart = 0;
	//public const int attackClusterSennyStart = 10000;
	//public const int attackClusterSEEnd = attackClusterSennyStart;
	//[JsonIgnore] public bool isAttackClusterSenny => attackCluster >= attackClusterSennyStart;
	//[JsonIgnore] public bool isAttackClusterSE => attackCluster >= attackClusterSEStart && attackCluster < attackClusterSEEnd;
	//[JsonIgnore] public bool isAttackClusterNone => attackCluster == attackClusterNone;

	//public string player { get; set; } = string.Empty;
	//      public int cid { get; set; }
	//      [JsonIgnore]
	//      public string xy => cid.CidToString();
	//      public int attackCluster { get; set; }

	//      [JsonIgnore]
	//      public bool isSE => typeT == Type.se;

	//      // Todo: make enum
	//      public int type { get; set; }
	//      [JsonIgnore]
	//      public Type typeT { get => (Type)type; set=>type=(int)value; }
	//      public bool fake { get; set; } // not used currently
	//      public byte troopType { get; set; }
	//      [JsonIgnore]
	//      public string troopTypeString => Game.Enum.ttNameWithCaps[troopType];
	//      public int ts { get; set; } // not used currently
	//  }

	public class AttackComboType
	{
		public AttackComboType(AttackType _type, string _name) { attackType = _type; name = _name; }
		
		public AttackType attackType { get; set; }
		public string name { get; set; }
		public static AttackComboType[] targetTypes = { new AttackComboType(AttackType.none, "None"),
			new AttackComboType(AttackType.senator, "Capture"),
			new AttackComboType(AttackType.senatorFake,"Capture Fake"),
			new AttackComboType(AttackType.se, "Demo"),
			new AttackComboType(AttackType.seFake, "Demo Fake")
		 };
		public static AttackComboType[] sourceTypes = { new AttackComboType(AttackType.none, "None"),
			new AttackComboType(AttackType.senator, "Senator"),
			new AttackComboType(AttackType.se, "SE"),
			new AttackComboType(AttackType.assault, "Assault")
		 };
	}

	public class AttackPlanCity : IEquatable<AttackPlanCity>
	{
		[JsonInclude]
		public int cid; // spot that this refers to
		[JsonInclude]
		public AttackType attackType { get; set; }
		[JsonInclude]
		public byte troopType { get; set; }
		[JsonInclude]
		public bool hasAcademy { get; set;}
		[JsonInclude]
		public int fixedTarget { get; set; } // only valid if attack, if set the target is specified by the player

		[JsonInclude]
		public int attackCluster { get; set; } = AttackPlan.attackClusterNone; // for SE clusters,

		[JsonIgnore]
		public City city => City.GetOrAdd(cid);
		public Player player => city.player;
		public int pid => city.pid;

		public bool isAttack => city.IsAllyOrNap(); // true for attack, false for target, must be set on initialization
		public bool isTarget => !isAttack; // true for attack, false for target
										   //public DateTimeOffset lastAccessed { get; set; } // lass user access


		public bool isAttackTypeAssault => attackType == AttackType.assault;
		public bool isAttackTypeSenator => attackType == AttackType.senator;
		[JsonIgnore]
		public bool isAttackTypeSenatorFake => attackType == AttackType.senatorFake;
		public bool isAttackTypeSE => attackType == AttackType.se;
		public bool isAttackTypeSEFake => attackType == AttackType.seFake;
		public bool isAttackTypeReal => attackType == AttackType.assault || attackType == AttackType.se || attackType == AttackType.senator;
		public bool isAttackTypeSiege => attackType == AttackType.se || attackType == AttackType.senator;
		public bool isAttackTypeFake => attackType == AttackType.seFake || attackType == AttackType.senatorFake;
		public bool isAttackTypeNone => attackType == AttackType.none;
		public string nameMarkdown => city.nameMarkdown;

		public bool isAttackClusterNone => attackCluster == AttackPlan.attackClusterNone;

		public uint spatialIndex => cid.ZCurveEncodeCid();


		public void Set(City city, AttackType _attackType =AttackType.invalid, byte troopType = ttPending)
		{
			cid = city.cid;
			if (troopType == ttPending)
			{
				troopType = city.primaryTroopType;
			}
			else
			{
				troopType = troopType;
				City.TryConvertTroopTypeToClassification(troopType, out city.classification);
			}
			hasAcademy = city.hasAcademy.GetValueOrDefault();
			if (_attackType != AttackType.invalid)
				attackType = _attackType;
			else
			{
				if (city.IsAllyOrNap())
					attackType = troopType == ttScorpion ? AttackType.se : hasAcademy ? AttackType.senator : AttackType.assault;
				else
					attackType = AttackType.invalid; //??
			}
		}
		public AttackPlanCity(City city, AttackType _attackType = AttackType.invalid, byte troopType = ttPending) => Set(city, _attackType, troopType);
		public AttackPlanCity(int cid, AttackType _attackType = AttackType.invalid, byte troopType = ttPending) => Set(City.Get(cid), _attackType, troopType);
		public AttackPlanCity() { }

		public void CopyTo(City t)
		{
			if (!t.isMine)
			{
				Spot.TryConvertTroopTypeToClassification(troopType, out t.classification);
				t.tags = TagHelper.FromTroopType(troopType);
				t.hasAcademy = hasAcademy;
			}
			else
			{
				t.TouchClassification();
			}	
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as AttackPlanCity);
		}

		public bool Equals(AttackPlanCity other)
		{
			return other != null &&
				   cid == other.cid;
		}

		public override int GetHashCode()
		{
			return cid;
		}

		public static AttackPlanCity none = new();

		//public static bool operator ==(AttackPlanCity left, AttackPlanCity right)
		//{
		//	return left.Equals(right);
		//}

		//public static bool operator !=(AttackPlanCity left, AttackPlanCity right)
		//{
		//	return !left.Equals(right);
		//}
		public static AttackPlanCity Get(int cid) => AttackPlan.Get(cid);
		
	}
	public class AttackPlan
	{
		[JsonInclude]
		public float attackMaxTravelHoursSE = 40;
		[JsonInclude]
		public float attackMaxTravelHoursSen = 40;

		[JsonInclude]
		public int tickToCapture = 4;
		[JsonInclude]
		public int senTime;
		[JsonInclude]
		public int seTime;	
		// if there are 2 reals, then it wants 2x assaults
		[JsonInclude]
		public bool normalizeAssaultsPerSeSiege  = true;
		[JsonInclude]
		public int attackSEMaxFakes = 9;
		[JsonInclude]
		public int attackSEMinFakes = 3;
		[JsonInclude]
		public int attackSenMaxFakes  = 10;
		[JsonInclude]
		public int attackSenMinFakes = 0;
		[JsonInclude]
		public int attackSEMinAssaults = 6;
		
		[JsonInclude]
		public int attackSEMaxAssaults = 20;
	
		[JsonInclude]
		public int attackSenMinAssaults=0;
		
		[JsonInclude]
		public int attackSenMaxAssaults = 10;


		[JsonInclude]
		public ImmutableArray<AttackPlanCity> attacks  = ImmutableArray<AttackPlanCity>.Empty;
		[JsonInclude]
		public ImmutableArray<AttackPlanCity> targets = ImmutableArray<AttackPlanCity>.Empty;

		
		public static AttackPlan plan = new();
		public static AttackPlanCity GetForRead(int cid) => Get(cid) ?? AttackPlanCity.none;
		public static AttackPlanCity Get(int cid)
		{
			foreach (var atk in plan.attacks)
			{
				if (atk.cid == cid)
					return atk;
			}
			foreach (var t in plan.targets)
			{
				if (t.cid == cid)
					return t;
			}
			return null;
		}
		public static AttackPlanCity Get(City city) => Get(city.cid);
		public static void Remove(int cid)
		{
			AddOrUpdate(new AttackPlanCity(cid,AttackType.none));
		}
		public static void Remove(City city) => Remove(city.cid);
		public static bool AddOrUpdate(AttackPlanCity c)
		{
			var city = c.city;
			ref var l = ref plan.attacks;
			if(!city.IsAllyOrNap() )
				l = ref plan.targets;
			if (c.attackType == AttackType.none)
			{
				l = l.RemoveAll(a => a.cid == c.cid);
				AttackTab.SyncUIGrids();
				return false;
			}
			else
			{
				var cur = AttackPlan.Get(c.cid);
				if (cur == null)
				{
					l = l.Add( c );
					AttackTab.SyncUIGrids();
					return true;
				}
				else
				{
					cur.attackType = c.attackType;
					
					if (c.troopType != ttPending)
					{
						cur.troopType = c.troopType;
						City.TryConvertTroopTypeToClassification(cur.troopType, out city.classification);
					}
					// for UI
					App.DispatchOnUIThreadLow( ()=>cur.city.OnPropertyChanged() );
					// can this be done on a background thread?
					// Todo:  hasAcademy
					
					return false;
				}
			}
		}
		public const int attackClusterNone = -1;
	}

	public struct AttackSenderScript
	{
		public int cid { get; set; }
		public List<int> x { get; set; }
		public List<int> y { get; set; }
		public List<int> type { get; set; }
		public string command { get; set; }
		public string[] time { get; set; }
		/*	"x": [
				"282",
				"282"
			],
			"y": [
				"231",
				"230"
			],
			"type": [
				"0",
				"0"
			],
			"time": [
				"10",
				"00",
				"00",
				"00/00/0000"
			]
			}*/

	}
	
	public static class AttackPlanHelper
		{
		public static bool IsTargetSE(this AttackType a) => a == AttackType.se || a == AttackType.seFake;
		public static bool IsTargetSenator(this AttackType a) => a == AttackType.senator || a == AttackType.senatorFake;
		public static bool IsTargetFake(this AttackType a) => a == AttackType.senatorFake || a == AttackType.seFake;
		public static bool IsTargetReal(this AttackType a) => IsTargetNone(a) ? false :!IsTargetFake(a);
		public static bool IsTargetNone(this AttackType a) => a == AttackType.none||a==AttackType.invalid;
		public static AttackCategory GetCategory(this AttackType a) => IsTargetNone(a) ? AttackCategory.invalid : IsTargetSE(a) ? AttackCategory.se :  AttackCategory.senator;
		public static int Value(this AttackType a) => (int)a;
		public static int Value(this AttackCategory a) => (int)a;

		public static bool Contains(this ImmutableArray<AttackPlanCity> l, int cid) => l.Any(a => a.cid==cid);
		public static bool Contains(this ImmutableArray<AttackPlanCity> l, City city) => Contains(l, city.cid);

	};
}
