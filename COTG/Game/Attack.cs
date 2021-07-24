using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

	public struct AttackDataPersist // structure use to persist targets
	{
		public int cid { get; set; } // spot that this refers to
		public AttackType attackType { get; set; }
		public byte troopType { get; set; }
		public bool hasAcademy { get; set;}

		public int attackCluster { get; set; } // for SE clusters,

		[JsonIgnore]
		public Spot spot => Spot.GetOrAdd(cid);

		public void CopyFrom(City t)
		{
			attackCluster = t.attackCluster;
			cid = t.cid;
			troopType = t.primaryTroopType;
			hasAcademy = t.hasAcademy.GetValueOrDefault();
			attackType = t.attackType;
		}
		
		public void CopyTo(City t)
		{
			t.attackCluster = attackCluster;
			t.cid = cid;
			if (!t.isMine)
			{
				Spot.TryConvertTroopTypeToClassification(troopType, out t.classification);
				t.tags = TagHelper.FromTroopType(troopType);
			}

			t.hasAcademy = hasAcademy;
			t.attackType = attackType;
		}
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
	
	public static class AttackTypeHelper
		{
		public static bool IsTargetSE(this AttackType a) => a == AttackType.se || a == AttackType.seFake;
		public static bool IsTargetSenator(this AttackType a) => a == AttackType.senator || a == AttackType.senatorFake;
		public static bool IsTargetFake(this AttackType a) => a == AttackType.senatorFake || a == AttackType.seFake;
		public static bool IsTargetReal(this AttackType a) => IsTargetNone(a) ? false :!IsTargetFake(a);
		public static bool IsTargetNone(this AttackType a) => a == AttackType.none||a==AttackType.invalid;
		public static AttackCategory GetCategory(this AttackType a) => IsTargetNone(a) ? AttackCategory.invalid : IsTargetSE(a) ? AttackCategory.se :  AttackCategory.senator;
		public static int Value(this AttackType a) => (int)a;
		public static int Value(this AttackCategory a) => (int)a;
	};
}
