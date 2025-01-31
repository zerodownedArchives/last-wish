using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "the remains of Grim" )]
	public class Grim : Drake // Varchild's
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.CrushingBlow;
		}

		[Constructable]
		public Grim () : base()
		{
			Name = "Grim";
			Hue = 1744;

			SetStr( 527, 580 );
			SetDex( 284, 322 );
			SetInt( 249, 386 );

			SetHits( 1762, 2502 );

			SetResistance( ResistanceType.Physical, 55, 60 );
			SetResistance( ResistanceType.Fire, 62, 68 );
			SetResistance( ResistanceType.Cold, 52, 57 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 40, 44 );

			SetSkill( SkillName.MagicResist, 105.8, 115.6 );
			SetSkill( SkillName.Tactics, 102.8, 120.8 );
			SetSkill( SkillName.Wrestling, 111.7, 119.2 );
			SetSkill( SkillName.Anatomy, 105.0, 128.4 );
		}

		public Grim( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}