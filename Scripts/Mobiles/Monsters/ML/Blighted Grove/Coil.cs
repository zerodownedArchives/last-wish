using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a Coil corpse" )]
	public class Coil : SilverSerpent
	{
		// TODO: Check faction allegiance

		[Constructable]
		public Coil()
		{
			IsParagon = true;

			Name = "Coil";
			Hue = 0x3F;

			SetStr( 205, 343 );
			SetDex( 202, 283 );
			SetInt( 88, 142 );

			SetHits( 628, 1291 );

			SetDamage( 19, 28 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 56, 62 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 25, 30 );

			SetSkill( SkillName.Wrestling, 124.5, 141.3 );
			SetSkill( SkillName.Tactics, 130.2, 142.0 );
			SetSkill( SkillName.MagicResist, 102.3, 113.0 );
			SetSkill( SkillName.Anatomy, 120.8, 138.1 );
			SetSkill( SkillName.Poisoning, 110.1, 133.4 );

			// TODO: Fame/Karma

			PackGem( 2 );
			PackItem( new Bone() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
		}

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.MortalStrike;
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			c.DropItem( new CoilsFang() );

			/*
			// TODO: uncomment once added
			if ( Utility.RandomDouble() < 0.025 )
			{
				switch ( Utility.Random( 5 ) )
				{
					case 0: c.DropItem( new AssassinChest() ); break;
					case 1: c.DropItem( new DeathGloves() ); break;
					case 2: c.DropItem( new LeafweaveLegs() ); break;
					case 3: c.DropItem( new HunterLegs() ); break;
					case 4: c.DropItem( new MyrmidonLegs() ); break;
				}
			}
			*/
		}

		public override Poison HitPoison { get { return Poison.Lethal; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override bool GivesMLMinorArtifact { get { return true; } }
		public override int Hides { get { return 48; } }
		public override int Meat { get { return 1; } }

		public Coil( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
